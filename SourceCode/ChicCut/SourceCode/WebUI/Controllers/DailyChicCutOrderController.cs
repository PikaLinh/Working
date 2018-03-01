using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constant;
using ViewModels;
using System.Transactions;
using Repository;
using System.Data.Entity.SqlServer;
namespace WebUI.Controllers
{
    public class DailyChicCutOrderController : BaseController
    {
        // GET: DailyChicCutOrder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _GetAllServicePartital(int Orderstatusid)
        {
            var lst = _context.Daily_ChicCut_OrderModel
                .Where(p => Orderstatusid == EnumDaily_ChicCut_OrderStatus.TatCa || p.OrderStatusId == Orderstatusid)
                .OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView(lst);
        }

        public ActionResult _AddNewCustomerPartital()
        {
            #region Quận, Huyện
            var provinces = _context.ProvinceModel.OrderBy(p => p.ProvinceName).ToList();
            ViewBag.ProvinceId = new SelectList(provinces, "ProvinceId", "ProvinceName", null);

            var districts = _context.DistrictModel
                                    .Where(p => p.ProvinceId == null)
                                    .OrderBy(p => p.DistrictName)
                                    .Select(p => new
                                    {
                                        Id = p.DistrictId,
                                        Name = p.Appellation + " " + p.DistrictName
                                    }).ToList();
            ViewBag.DistrictId = new SelectList(districts, "Id", "Name", null);
            #endregion

            return PartialView(new CustomerModel());
        }

        public ActionResult _SaveAddNewCustomerPartital(CustomerModel customermodel)
        {
            int CustomerId = 0;
            string resuilt = "", CustomerName = "";
            if (ModelState.IsValid)
            {
                customermodel.Actived = true;
                customermodel.RegDate = DateTime.Now;
                customermodel.CustomerLevelId = 1;
                _context.CustomerModel.Add(customermodel);
                _context.SaveChanges();
                CustomerId = customermodel.CustomerId;
                return Json(
                    new
                    {
                        CustomerId = CustomerId,
                        CustomerName = (customermodel.FullName + " - " + (string.IsNullOrEmpty(customermodel.Phone) ? "" : customermodel.Phone)),
                        resuilt = "success"
                    }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(
                   new
                   {
                       CustomerId = CustomerId,
                       CustomerName = CustomerName,
                       resuilt = "Vui lòng kiểm tra lại thông tin không hợp lệ!"
                   }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult _UpdateOrder(int? OrderId = null)
        {
            try
            {
                CreateViewBag();
                ViewBag.ListProduct = (from p in _context.ProductModel
                                       join price in _context.ProductPriceModel on p.ProductId equals price.ProductId
                                       where p.Actived == true && p.IsProduct == true && (p.isParentProduct == null || p.isParentProduct == false)
                                       select new ProductViewModel()
                                       {
                                           ProductId = p.ProductId,
                                           ProductName = p.ProductName,
                                           Price1 = price.Price
                                       }).ToList();
                if (OrderId.HasValue)
                {
                    var model = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
                    GetInfoCustomer(model.CustomerId.Value, model);
                    CreateViewBag(model.HairTypeId);
                    ViewBag.OrderId = OrderId;
                    TempData["BillDiscount"] = model.BillDiscount;
                    TempData["BillDiscountTypeId"] = model.BillDiscountTypeId;
                    CreateViewBag(model.HairTypeId, null, model.StaffId, model.PaymentMethodId);
                    return PartialView(model);
                }
                else
                {
                    return PartialView(new Daily_ChicCut_OrderModel());
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình thực thi!");
            }

        }

        #region Thêm mới đơn hàng trong Popup
        #region Thêm dịch vụ
        //CreateList
        public ActionResult _DailyChicCutOrderDetailInfo(List<Daily_ChicCut_OrderDetailViewModel> details = null)
        {
            if (details == null)
            {
                details = new List<Daily_ChicCut_OrderDetailViewModel>();
            }
            return PartialView(details);
        }

        //CreateListInner
        public ActionResult _DailyChicCutOrderDetailInnerInfo(List<Daily_ChicCut_OrderDetailViewModel> details = null, int? ServiceId = null, int? OrderId = null, int? ProductId = null)
        {
            if (details == null)
            {
                details = new List<Daily_ChicCut_OrderDetailViewModel>();
            }
            /*Kiểm tra xem Danh sách đã có dịch vụ đó chưa :
                        + Có rồi: Tăng Qty + Cogs + UnitPrice
                        + Chưa có: Thêm mới.
            */
            #region Dịch vụ
            if (ServiceId.HasValue)
            {
                var itemExist = details.Where(p => p.ServiceId == ServiceId).FirstOrDefault();
                if (itemExist != null)     //Đã có dịch vụ đó
                {
                    var index = details.FindIndex(c => c.ServiceId == ServiceId);
                    details[index] = new Daily_ChicCut_OrderDetailViewModel()
                    {
                        ServiceId = itemExist.ServiceId,
                        COGS = itemExist.COGS,
                        Price = itemExist.Price,
                        Qty = itemExist.Qty + 1,
                        UnitCOGS = itemExist.COGS * (itemExist.Qty + 1),
                        UnitPrice = itemExist.Price * (itemExist.Qty + 1),
                        ServiceName = itemExist.ServiceName,
                        QuantificationMasterId = itemExist.QuantificationMasterId,
                        QuantificationMasterList = itemExist.QuantificationMasterList
                    };
                }
                else //Chưa có dịch vụ đó
                {
                    #region Chưa có dịch vụ đó
                    var item = _context.Master_ChicCut_ServiceModel.Where(p => p.ServiceId == ServiceId).Select(p => new Daily_ChicCut_OrderDetailViewModel()
                    {
                        ServiceId = p.ServiceId,
                        ServiceCategoryId = p.ServiceCategoryId,
                        COGS = 0,
                        Price = p.Price,
                        Qty = 1,
                        UnitCOGS = 0,
                        UnitPrice = p.Price,
                        ServiceName = p.ServiceName,
                        QuantificationMasterList = _context.Master_ChicCut_QuantificationMasterModel.Where(Q => Q.ServiceId == p.ServiceId).Select(Q => new SelectListItem()
                        {
                            Text = Q.QuantificationName,
                            Value = SqlFunctions.StringConvert((double)Q.QuantificationMasterId).Trim()
                        })
                    }).FirstOrDefault();
                    details.Add(item);
                    #endregion
                }
                if (OrderId.HasValue && OrderId.Value > 0)//Sửa đơn hàng: + Chưa có dịch vụ đó: Thêm mới 
                {
                    //              + Có rồi: Update Qty, TT, Vốn, Sum..
                    #region //Sửa đơn hàng:
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var itemOrderDetailModel = _context.Daily_ChicCut_OrderDetailModel.Where(p => p.ServiceId == ServiceId && p.OrderId == OrderId).FirstOrDefault();
                        if (itemOrderDetailModel != null) // Update
                        {
                            itemOrderDetailModel.COGS = itemOrderDetailModel.COGS;
                            itemOrderDetailModel.UnitCOGS = itemOrderDetailModel.COGS * (itemOrderDetailModel.Qty + 1);
                            itemOrderDetailModel.UnitPrice = itemOrderDetailModel.Price * (itemOrderDetailModel.Qty + 1);
                            itemOrderDetailModel.Qty++;
                            _context.Entry(itemOrderDetailModel).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                        }
                        else // Chưa có
                        {
                            #region Thêm mới
                            /* Tính giá vốn  */
                            List<int> ProductIdLst = (from QMaster in _context.Master_ChicCut_QuantificationMasterModel
                                                      join QDetail in _context.Master_ChicCut_QuantificationDetailModel on QMaster.QuantificationMasterId equals QDetail.QuantificationMasterId
                                                      where QMaster.ServiceId == ServiceId
                                                      select QDetail.ProductId.Value).ToList();
                            var LstProductModel = _context.ProductModel.Where(p => ProductIdLst.Contains(p.ProductId)).ToList();
                            decimal COGSCal = (LstProductModel != null && LstProductModel.Count > 0) ? LstProductModel.Sum(p => p.COGS.HasValue ? p.COGS : 0).Value : 0;
                            var ServiceModel = _context.Master_ChicCut_ServiceModel.Where(p => p.ServiceId == ServiceId).FirstOrDefault();

                            var itemAdd = new Daily_ChicCut_OrderDetailModel()
                            {
                                OrderId = OrderId,
                                ServiceId = ServiceModel.ServiceId,
                                COGS = COGSCal,
                                Price = ServiceModel.Price,
                                Qty = 1,
                                UnitCOGS = COGSCal,
                                UnitPrice = ServiceModel.Price,
                            };

                            _context.Entry(itemAdd).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            #endregion
                        }
                        var model = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
                        if (model != null)// Cập nhật Master
                        {
                            var lstDetail = model.Daily_ChicCut_OrderDetailModel.ToList();
                            model.SumCOGSOfOrderDetail = lstDetail.Sum(p => p.UnitCOGS.HasValue ? p.UnitCOGS.Value : 0);
                            model.SumPriceOfOrderDetail = lstDetail.Sum(p => p.UnitPrice.HasValue ? p.UnitPrice.Value : 0);
                            model.TotalBillDiscount = model.BillDiscount.HasValue ? ((model.BillDiscountTypeId == 1 ? model.BillDiscount : (model.BillDiscount / 100) * lstDetail.Sum(p => p.UnitPrice.HasValue ? p.UnitPrice.Value : 0))) : 0;
                            model.Total = model.SumPriceOfOrderDetail - model.TotalBillDiscount;
                            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                            ts.Complete();
                        }
                    }
                    #endregion
                }

            }

            #endregion

            #region Sản phẩm
            if (ProductId.HasValue && ProductId > 0)
            {
                #region #Thêm sản phẩm vào list giao diện
                var itemExist = details.Where(p => p.ProductId == ProductId).FirstOrDefault();
                if (itemExist != null)     //Đã có sản phẩm đó
                {
                    var index = details.FindIndex(c => c.ProductId == ProductId);
                    details[index] = new Daily_ChicCut_OrderDetailViewModel()
                    {
                        ProductId = itemExist.ProductId,
                        COGS = itemExist.COGS,
                        Price = itemExist.Price,
                        Qty = itemExist.Qty + 1,
                        UnitCOGS = itemExist.COGS * (itemExist.Qty + 1),
                        UnitPrice = itemExist.Price * (itemExist.Qty + 1),
                        ProductName = itemExist.ProductName,
                        ServiceName = itemExist.ProductName
                    };
                }
                else //Chưa có sản phẩm đó
                {
                    #region Chưa có sản phẩm đó
                    var ProductModel = _context.ProductModel.Where(p => p.ProductId == ProductId).FirstOrDefault();
                    Daily_ChicCut_OrderDetailViewModel item = new Daily_ChicCut_OrderDetailViewModel();
                    var COGS = ProductModel.COGS;
                    var Price = ProductModel.ProductPriceModel.Select(price => price.Price).FirstOrDefault();
                    item.ProductId = ProductModel.ProductId;
                    item.COGS = COGS ?? 0;
                    item.Price = Price ?? 0;
                    item.Qty = 1;
                    item.UnitCOGS = COGS ?? 0;
                    item.UnitPrice = Price ?? 0;
                    item.ProductName = ProductModel.ProductName;
                    item.ServiceName = ProductModel.ProductName;
                    details.Add(item);
                    #endregion
                }
                #endregion
                #region #Thêm vào database
                if (OrderId.HasValue && OrderId.Value > 0)
                {
                    #region //Sửa đơn hàng:
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //Sửa đơn hàng: 
                        // + Có rồi: Update Qty, TT, Vốn, Sum..
                        var itemOrderDetailModel = _context.Daily_ChicCut_OrderProductDetailModel.Where(p => p.ProductId == ProductId && p.OrderId == OrderId).FirstOrDefault();
                        if (itemOrderDetailModel != null) // Update
                        {
                            itemOrderDetailModel.COGS = itemOrderDetailModel.COGS;
                            itemOrderDetailModel.UnitCOGS = itemOrderDetailModel.COGS * (itemOrderDetailModel.Qty + 1);
                            itemOrderDetailModel.UnitPrice = itemOrderDetailModel.Price * (itemOrderDetailModel.Qty + 1);
                            itemOrderDetailModel.Qty++;
                            _context.Entry(itemOrderDetailModel).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                        }
                        else
                        //+ Chưa có dịch vụ đó: Thêm mới
                        {
                            #region Thêm mới
                            var productModel = _context.ProductModel.Where(p => p.ProductId == ProductId).FirstOrDefault();
                            Daily_ChicCut_OrderProductDetailModel modelAdd = new Daily_ChicCut_OrderProductDetailModel();
                            var COGS = productModel.COGS;
                            var Price = productModel.ProductPriceModel.Select(price => price.Price).FirstOrDefault();
                            modelAdd.ProductId = ProductId;
                            modelAdd.OrderId = OrderId;
                            modelAdd.COGS = COGS ?? 0;
                            modelAdd.Price = Price ?? 0;
                            modelAdd.Qty = 1;
                            modelAdd.UnitCOGS = COGS ?? 0;
                            modelAdd.UnitPrice = Price ?? 0;
                            _context.Entry(modelAdd).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            #endregion
                        }
                        var model = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
                        if (model != null)// Cập nhật Master
                        {
                            var lstDetail = model.Daily_ChicCut_OrderProductDetailModel;
                            model.SumCOGSOfOrderDetail = lstDetail.Sum(p => p.UnitCOGS);
                            model.SumPriceOfOrderDetail = lstDetail.Sum(p => p.UnitPrice);
                            model.TotalBillDiscount = model.BillDiscount.HasValue ? ((model.BillDiscountTypeId == 1 ? model.BillDiscount : (model.BillDiscount / 100) * lstDetail.Sum(p => p.UnitPrice))) : 0;
                            model.Total = (model.SumPriceOfOrderDetail ?? 0) - (model.TotalBillDiscount ?? 0);
                            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                            ts.Complete();
                        }
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            var LstDetailsWithQMasterLst = details.Where(p => p.QuantificationMasterId != null).ToList();//lấy ghi chú cho dv Uốn/Duỗi (có định mức 2 Qdetail trở lên)
            if (LstDetailsWithQMasterLst != null && LstDetailsWithQMasterLst.Count > 0)
            {
                foreach (var item in LstDetailsWithQMasterLst)
                {
                    var index = details.FindIndex(c => c.ServiceId == item.ServiceId);
                    details[index].QuantificationMasterList = _context.Master_ChicCut_QuantificationMasterModel.Where(Q => Q.ServiceId == item.ServiceId).Select(Q => new SelectListItem()
                    {
                        Text = Q.QuantificationName,
                        Value = SqlFunctions.StringConvert((double)Q.QuantificationMasterId).Trim(),
                        Selected = (Q.QuantificationMasterId == item.QuantificationMasterId)
                    }).ToList();
                }
            }

            return PartialView(details);
        }


        //DeleteInner

        #region _DeletelistInner
        public ActionResult _DeleteDetailInnerInfo(List<Daily_ChicCut_OrderDetailViewModel> details, int? ServiceId = null, int? OrderId = null, int? ProductId = null)
        {
            if (details == null)
            {
                details = new List<Daily_ChicCut_OrderDetailViewModel>();
            }
            if (OrderId.HasValue && OrderId.Value > 0)//Sửa đơn hàng: Xoá record trong detail và update master:  Sum..
            {
                #region //Sửa đơn hàng:
                using (TransactionScope ts = new TransactionScope())
                {
                    #region Dịch vụ
                    if (ServiceId.HasValue)
                    {
                        var itemOrderDetailModel = _context.Daily_ChicCut_OrderDetailModel.Where(p => p.ServiceId == ServiceId && p.OrderId == OrderId).FirstOrDefault();
                        if (itemOrderDetailModel != null) // Update
                        {
                            _context.Entry(itemOrderDetailModel).State = System.Data.Entity.EntityState.Deleted;
                            _context.SaveChanges();
                            #region Update Master
                            var model = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
                            if (model != null)// Cập nhật Master
                            {
                                var lstDetail = model.Daily_ChicCut_OrderDetailModel.ToList();
                                model.SumCOGSOfOrderDetail = lstDetail.Sum(p => p.UnitCOGS.HasValue ? p.UnitCOGS.Value : 0);
                                model.SumPriceOfOrderDetail = lstDetail.Sum(p => p.UnitPrice.HasValue ? p.UnitPrice.Value : 0);
                                model.TotalBillDiscount = model.BillDiscount.HasValue ? ((model.BillDiscountTypeId == 1 ? model.BillDiscount : (model.BillDiscount / 100) * lstDetail.Sum(p => p.UnitPrice.HasValue ? p.UnitPrice.Value : 0))) : 0;
                                model.Total = model.SumPriceOfOrderDetail - model.TotalBillDiscount;
                                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                                _context.SaveChanges();
                            }
                            ts.Complete();
                            #endregion
                        }
                    }
                    #endregion

                    #region Sản phẩm
                    if (ProductId.HasValue && ProductId > 0)
                    {
                        var itemOrderDetailModel = _context.Daily_ChicCut_OrderProductDetailModel.Where(p => p.ProductId == ProductId && p.OrderId == OrderId).FirstOrDefault();
                        if (itemOrderDetailModel != null) // Update
                        {
                            _context.Entry(itemOrderDetailModel).State = System.Data.Entity.EntityState.Deleted;
                            _context.SaveChanges();
                            #region Update Master
                            var model = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
                            if (model != null)// Cập nhật Master
                            {
                                var lstDetail = model.Daily_ChicCut_OrderProductDetailModel.ToList();
                                model.SumCOGSOfOrderDetail = lstDetail.Sum(p => p.UnitCOGS.HasValue ? p.UnitCOGS.Value : 0);
                                model.SumPriceOfOrderDetail = lstDetail.Sum(p => p.UnitPrice.HasValue ? p.UnitPrice.Value : 0);
                                model.TotalBillDiscount = model.BillDiscount.HasValue ? ((model.BillDiscountTypeId == 1 ? model.BillDiscount : (model.BillDiscount / 100) * lstDetail.Sum(p => p.UnitPrice.HasValue ? p.UnitPrice.Value : 0))) : 0;
                                model.Total = model.SumPriceOfOrderDetail - model.TotalBillDiscount;
                                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                                _context.SaveChanges();
                            }
                            ts.Complete();
                            #endregion
                        }
                    }
                    #endregion

                }
                #endregion
            }
            var ListDetailToSend = details.Where(p => (ServiceId == null || p.ServiceId != ServiceId) && (ProductId == 0 || p.ProductId != ProductId)).ToList();
            var LstDetailsWithQMasterLst = ListDetailToSend.Where(p => p.QuantificationMasterId != null).ToList();//lấy ghi chú cho dv Uốn/Duỗi (có định mức 2 Qdetail trở lên)
            if (LstDetailsWithQMasterLst != null && LstDetailsWithQMasterLst.Count > 0)
            {
                foreach (var item in LstDetailsWithQMasterLst)
                {
                    var index = ListDetailToSend.FindIndex(c => c.ServiceId == item.ServiceId);
                    ListDetailToSend[index].QuantificationMasterList = _context.Master_ChicCut_QuantificationMasterModel.Where(Q => Q.ServiceId == item.ServiceId).Select(Q => new SelectListItem()
                    {
                        Text = Q.QuantificationName,
                        Value = SqlFunctions.StringConvert((double)Q.QuantificationMasterId).Trim(),
                        Selected = (Q.QuantificationMasterId == item.QuantificationMasterId)
                    }).ToList();
                }
            }
            return PartialView("_DailyChicCutOrderDetailInnerInfo", ListDetailToSend);
        }
        #endregion

        #region Sửa -> Update Qty
        public ActionResult _DailyChicCutOrderDetailInnerInfoUpdateQty(decimal Qty, int ServiceId, int OrderId)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var itemDetail = _context.Daily_ChicCut_OrderDetailModel.Where(p => p.ServiceId == ServiceId && p.OrderId == OrderId).FirstOrDefault();
                    if (itemDetail != null)
                    {
                        itemDetail.Qty = Qty;
                        itemDetail.COGS = itemDetail.COGS;
                        itemDetail.UnitCOGS = itemDetail.COGS * (itemDetail.Qty);
                        itemDetail.UnitPrice = itemDetail.Price * (itemDetail.Qty);
                        _context.Entry(itemDetail).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        #region Master
                        var model = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
                        var lstDetail = model.Daily_ChicCut_OrderDetailModel.ToList();
                        model.SumCOGSOfOrderDetail = lstDetail.Sum(p => p.UnitCOGS.HasValue ? p.UnitCOGS.Value : 0);
                        model.SumPriceOfOrderDetail = lstDetail.Sum(p => p.UnitPrice.HasValue ? p.UnitPrice.Value : 0);
                        model.TotalBillDiscount = model.BillDiscount.HasValue ? ((model.BillDiscountTypeId == 1 ? model.BillDiscount : (model.BillDiscount / 100) * lstDetail.Sum(p => p.UnitPrice.HasValue ? p.UnitPrice.Value : 0))) : 0;
                        model.Total = model.SumPriceOfOrderDetail - model.TotalBillDiscount;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        ts.Complete();
                        #endregion

                        return Content("success");
                    }
                    else
                    {
                        return Content("Không tìm thấy dịch vụ cần cập nhật số lượng!");
                    }
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình cập nhật số lượng!");
            }

        }
        #endregion

        #region Chuyển trạng thái Đang chờ -> Phục vụ ngay
        public ActionResult UpdateOrderServeImmediately(int OrderId)
        {
            try
            {
                var model = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
                if (model == null)
                {
                    return Content("Không tìm thấy đơn hàng yêu cầu!");
                }
                else
                {
                    model.OrderStatusId = EnumDaily_ChicCut_OrderStatus.DangPhucVu;
                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    return Content("success");
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình cập nhật trạng thái");
            }
        }
        #endregion

        #region Huỷ dịch vụ
        public ActionResult UpdateOrderCalcelOrderId(int OrderId)
        {
            try
            {
                var model = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
                model.CanceledDate = DateTime.Now;
                model.CanceledUserId = CurrentUser.UserId;
                if (model == null)
                {
                    return Content("Không tìm thấy đơn hàng yêu cầu!");
                }
                else
                {
                    model.OrderStatusId = EnumDaily_ChicCut_OrderStatus.Huy;
                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    return Content("success");
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình cập nhật trạng thái");
            }
        }
        #endregion

        #region Thanh toán
        public JsonResult UpdateOrderPay(ChicCutOrderViewModel OrderMaster)
        {
            //2018-01-22 Update:
            //-- Thêm các field: HolidayCommission: "hoa hồng ngày lễ cho NV"
            //isHoliday: cập nhật giá ngày lễ
            //Ngày thường
            //              + Commitsion:  "Hoa hồng dịch vụ"
            //                  => Dò bảng xổ số
            //              + ProductCommission: "Hoa hồng sản phẩm"
            //                  => 30.000/sản phẩm bán thêm
            //      Lưu ý: có một số trường hợp đặc biệt với phục hồi chuyên sâu
            //Nếu là ngày lễ: 
            // "Daily_ChicCut_OrderModel"
            //08-01-22.1
            //              + Tính "Phụ thu"(AddtionalPrice) cho khách hàng = 15% cho các dịch vụ (không phải sản phẩm) trừ dịch vụ "Cắt"
            //              => tổng bill sẽ tăng <= 15% giá ban đầu so với ngày thường. Do cắt tóc vs sản phẩm không tăng, chỉ tăng dịch vụ
            //              => cập nhật: sửa bên JavaScript file = /Scripts/DailyChicCutOrder/Index.js
            //08-02-01.1 Update: Không tính phụ thu nữa => set AddtionalPrice = 0
            //08-01-22.2
            //              + Không tính commission như bình thường, chỉ tính ProductCommission
            //              => set Commission = 0;
            //              + HolidayCommission: Tính hoa hồng ngày lễ cho nhân viên = 5% tổng bill 


            try
            {
                var model = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderMaster.OrderId).FirstOrDefault();
                var payment = _context.PaymentMethodModel.Where(p => p.PaymentMethodId == OrderMaster.PaymentMethodId).FirstOrDefault();

                if (model == null)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.InternalServerError,
                        Success = false,
                        Data = "Không tìm thấy đơn hàng yêu cầu!"
                    });
                }
                else if (model.OrderStatusId == EnumDaily_ChicCut_OrderStatus.DaTinhTien)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Conflict,
                        Success = false,
                        Data = "Phiếu này đã được thanh Toán, Vui lòng kiểm tra lại!"
                    });
                }
                else if (model.OrderStatusId == EnumDaily_ChicCut_OrderStatus.Huy)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Conflict,
                        Success = false,
                        Data = "Phiếu này đã bị hủy, Vui lòng kiểm tra lại!"
                    });
                }
                else if (OrderMaster.details == null)
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.InternalServerError,
                        Success = false,
                        Data = "Không có dịch vụ nào nên không thể thanh toán đơn hàng này!"
                    });
                }
                else
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var currentTime = DateTime.Now;
                        //Cập nhật thông in model
                        model.CashierUserId = CurrentUser.UserId;
                        model.CashierDate = currentTime;
                        model.Note = OrderMaster.Note;
                        model.Phone = OrderMaster.Phone;
                        model.OrderStatusId = EnumDaily_ChicCut_OrderStatus.DaTinhTien;
                        decimal Commission = 0;
                        decimal ProductCommission = 0;

                        #region #Tính hoa hồng + tiền tip
                        //Số lượng dịch vụ tính hoa hồng
                        var TinhHoaHong2List = (from od in _context.Daily_ChicCut_OrderDetailModel
                                                join s in _context.Master_ChicCut_ServiceModel on od.ServiceId equals s.ServiceId
                                                join c in _context.Master_ChicCut_ServiceCategoryModel on s.ServiceCategoryId equals c.ServiceCategoryId
                                                where od.OrderId == model.OrderId && c.Type == EnumTipAndCommission.TinhHH
                                                select new { od.ServiceId, od.Qty, od.UnitPrice }).ToList();
                        decimal Count2_TinhHoaHong = TinhHoaHong2List.Sum(p => p.Qty) ?? 0;
                        //Số lượng dịch vụ tính hoa hồng khi không dùng chung với dịch vụ khác
                        var TinhHoaHong3List = (from od in _context.Daily_ChicCut_OrderDetailModel
                                                join s in _context.Master_ChicCut_ServiceModel on od.ServiceId equals s.ServiceId
                                                join c in _context.Master_ChicCut_ServiceCategoryModel on s.ServiceCategoryId equals c.ServiceCategoryId
                                                where od.OrderId == model.OrderId && c.Type == EnumTipAndCommission.TinhHHKhiKhongDungChungDichVuKhac
                                                select new { od.ServiceId, od.Qty, od.UnitPrice }).ToList();
                        decimal Count3_TinhHoaHong = TinhHoaHong3List.Sum(p => p.Qty) ?? 0;
                        //1. Tính tiền boa
                        model.Tip = (Count2_TinhHoaHong + Count3_TinhHoaHong) * EnumTipAndCommission.TienTip;
                        //2. Tính tiền hoa hồng cho các dịch vụ có service category có type = 2
                        if (TinhHoaHong2List != null && TinhHoaHong2List.Count > 0)
                        {
                            foreach (var item in TinhHoaHong2List)
                            {
                                Commission += ComputingCommission(item.Qty, item.UnitPrice);
                            }
                        }

                        //3. Tính tiền hoa hồng cho các dịch vụ có service category có type = 3
                        if (TinhHoaHong3List != null && TinhHoaHong3List.Count > 0)
                        {
                            if (Count2_TinhHoaHong == 0)
                            {
                                foreach (var item in TinhHoaHong3List)
                                {
                                    Commission += ComputingCommission(item.Qty, item.UnitPrice);
                                }
                            }
                        }
                        #endregion
                        //Loại giảm giá(theo % hoặc tiền mặt)
                        model.BillDiscountTypeId = OrderMaster.BillDiscountTypeId;
                        //Giá trị giảm(VD: 10% hay 100.000vnđ)
                        model.BillDiscount = OrderMaster.BillDiscount;
                        //Tổng cộng tiền các dịch vụ
                        model.SumPriceOfOrderDetail = OrderMaster.SumPriceOfOrderDetail;
                        //Phụ thu
                        model.AdditionalPrice = OrderMaster.AdditionalPrice;
                        //Tổng giá giảm
                        model.TotalBillDiscount = OrderMaster.TotalBillDiscount;
                        //số tiền phải thanh toán
                        model.Total = OrderMaster.Total;
                        //Tổng giá vốn
                        model.SumCOGSOfOrderDetail = OrderMaster.SumCOGSOfOrderDetail;


                        //Cật nhật file in
                        #region
                        OrderMaster.CashierDate = currentTime;
                        OrderMaster.CashierName = currentEmployee.FullName;
                        OrderMaster.Gender = model.Gender;
                        if (model.HairTypeId.HasValue)
                        {
                            OrderMaster.HairStyle = model.Master_ChicCut_HairTypeModel.HairTypeName;
                        }
                        OrderMaster.PaymentMethod = payment.PaymentMethodName;
                        #endregion
                        List<Daily_ChicCut_OrderDetailModel> detail = model.Daily_ChicCut_OrderDetailModel.ToList();

                        //Cật nhật thông tin tồn kho
                        #region // Insert InventoryMaster
                        InventoryMasterModel InvenMaster = new InventoryMasterModel();
                        InvenMaster.WarehouseModelId = 1;//Kho 489 Hồng Bàng
                        InvenMaster.InventoryTypeId = EnumInventoryType.XB; // xuất bán hàng
                        InvenMaster.InventoryCode = string.Format("{0}-{1}", ConstantInventoryCode.MaDonHangTaoMoi, model.OrderId);
                        InvenMaster.CreatedDate = currentTime;
                        InvenMaster.CreatedAccount = currentAccount.UserName;
                        InvenMaster.CreatedEmployeeId = currentEmployee.EmployeeId;
                        InvenMaster.StoreId = 1000;//Chic Cut Salon
                        InvenMaster.Actived = true;
                        InvenMaster.BusinessId = model.OrderId; // Id nghiệp vụ 
                        InvenMaster.BusinessName = "Daily_ChicCut_OrderModel";// Tên bảng nghiệp vụ
                        InvenMaster.ActionUrl = "/Daily_ChicCut_OrderModel/Details/";// Đường dẫn ( cộng ID cho truy xuất)

                        _context.Entry(InvenMaster).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges(); // insert tạm để lấy InvenMasterID
                        #endregion

                        #region InventoryDetailModel cho Product và Tính tiền hoa hồng cho Sản phẩm: cộng dồn Qty*EnumTipAndCommission.TienProductCommission
                        //Sản phẩm
                        var LstOrderProductDetail = model.Daily_ChicCut_OrderProductDetailModel.ToList();
                        if (LstOrderProductDetail != null && LstOrderProductDetail.Count > 0)
                        {
                            EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
                            foreach (var item in LstOrderProductDetail)
                            {
                                ProductCommission += item.Qty.Value * EnumTipAndCommission.TienProductCommission;

                                var productModel = _context.ProductModel.Where(p => p.ProductId == item.ProductId).FirstOrDefault();
                                //Nếu là sản phẩm con
                                if (productModel.ParentProductId.HasValue && productModel.ParentProductId != 0)
                                {
                                    var parentProductModel = _context.ProductModel.Find(productModel.ParentProductId);
                                    decimal TonCuoiTrongHeThong = EndInventoryRepo.GetQty(productModel.ParentProductId.Value);
                                    //Tính trọng lượng thực tế
                                    decimal ShippingWeight = 0;
                                    if (productModel.ShippingWeight.HasValue && parentProductModel.ShippingWeight.HasValue)
                                    {
                                        ShippingWeight = (productModel.ShippingWeight.Value / parentProductModel.ShippingWeight.Value);
                                    }
                                    else
                                    {
                                        throw new Exception("Chưa nhập trọng lượng cho sản phẩm cha và sản phảm con");
                                    }
                                    InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                    {
                                        InventoryMasterId = InvenMaster.InventoryMasterId,
                                        ProductId = productModel.ParentProductId.Value,
                                        BeginInventoryQty = TonCuoiTrongHeThong,
                                        COGS = item.COGS,
                                        Price = item.Price,
                                        //ImportQty = 0, 
                                        ExportQty = item.Qty * ShippingWeight,//Tính lại
                                        UnitCOGS = item.COGS * item.Qty, //Tổng giá vốn = [COGS] * [ImportQty] = 0
                                        UnitPrice = item.Price * item.Qty,
                                        EndInventoryQty = TonCuoiTrongHeThong + 0 - (item.Qty * ShippingWeight) //Tồn cuối = [BeginInventoryQty] + [ImportQty] -  [ExportQty]
                                    };
                                    _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                                }
                                //Nếu không phải là sản phẩm con
                                else
                                {
                                    decimal TonCuoiTrongHeThong = EndInventoryRepo.GetQty(item.ProductId.Value);
                                    InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                    {
                                        InventoryMasterId = InvenMaster.InventoryMasterId,
                                        ProductId = item.ProductId,
                                        BeginInventoryQty = TonCuoiTrongHeThong,
                                        COGS = item.COGS,
                                        Price = item.Price,
                                        //ImportQty = 0, 
                                        ExportQty = item.Qty,
                                        UnitCOGS = item.COGS * item.Qty, //Tổng giá vốn = [COGS] * [ImportQty] = 0
                                        UnitPrice = item.Price * item.Qty,
                                        EndInventoryQty = TonCuoiTrongHeThong + 0 - item.Qty //Tồn cuối = [BeginInventoryQty] + [ImportQty] -  [ExportQty]
                                    };
                                    _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                                }
                            }
                        }
                        #endregion

                        model.Commission = Commission;
                        model.ProductCommission = ProductCommission;


                        //08-01-22.2
                        //              + Không tính commission như bình thường
                        //              => set Commission = 0; set ProductCommission = 0
                        //              + HolidayCommission: Tính hoa hồng ngày lễ cho nhân viên = 5% tổng bill 

                        if (model.isHoliday == ConstantIsHoliday.NgayLe)
                        {
                            //Cập nhật lại hoa hồng sản phẩm, hoa hồng dịch vụ đã tính ở trên
                            model.Commission = 0;
                            //model.ProductCommission = 0;
                            //Tính hoa hồng ngày lễ
                            model.HolidayCommission = model.Total * 5 / 100;
                        }


                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;

                        /* Chi tiết định lượng : Qty = QDetail.Qty * Od.Qty,  Price = pdprice.Price * Od.Qty*/
                        #region Detail Dịch vụ
                        var QDetailListTmp = (
                            from Od in detail
                            join QMaster in _context.Master_ChicCut_QuantificationMasterModel on Od.ServiceId equals QMaster.ServiceId
                            join QDetail in _context.Master_ChicCut_QuantificationDetailModel on QMaster.QuantificationMasterId equals QDetail.QuantificationMasterId
                            join pd in _context.ProductModel on QDetail.ProductId equals pd.ProductId
                            //join pdprice in _context.ProductPriceModel on pd.ProductId equals pdprice.ProductId
                            where detail.Select(p => p.ServiceId).ToList().Contains(QMaster.ServiceId)
                            select new Master_ChicCut_QuantificationDetailViewModel()
                            {
                                ProductId = QDetail.ProductId,
                                Qty = (QDetail.Qty * Od.Qty) / pd.ShippingWeight,
                                Price = pd.COGS//pdprice.Price * Od.Qty, (Lấy giá nhập để tính giá vốn trong kho)

                            }).ToList();
                        // Gom nhóm ProductId lại
                        var QDetailList = QDetailListTmp.GroupBy(p => p.ProductId).Select(p => new Master_ChicCut_QuantificationDetailViewModel()
                            {
                                ProductId = p.Key,
                                Qty = p.Sum(a => a.Qty),
                                Price = p.Sum(a => a.Price)
                            }).ToList();

                        #region //Thêm record bán sản phẩm vào QDetailList:
                        var ProductListToSale = OrderMaster.details.Where(p => p.ProductId > 0).ToList();
                        if (ProductListToSale != null && ProductListToSale.Count > 0)
                        {
                            foreach (var item in ProductListToSale)
                            {
                                if (QDetailList.Select(p => p.ProductId).Contains(item.ProductId)) // Nếu có r thì edit Qty, Price 
                                {
                                    var index = QDetailList.FindIndex(c => c.ProductId == item.ProductId);
                                    decimal? PriceOfProduct = _context.ProductPriceModel.Where(p => p.ProductId == item.ProductId).Select(p => p.Price).FirstOrDefault();
                                    QDetailList[index].Qty += item.Qty;
                                    QDetailList[index].Price += (PriceOfProduct ?? 0);
                                }
                                else // Chưa có thì thêm mới vào QDetailList
                                {
                                    Master_ChicCut_QuantificationDetailViewModel itemadd = new Master_ChicCut_QuantificationDetailViewModel()
                                    {
                                        ProductId = item.ProductId,
                                        Qty = item.Qty,
                                        Price = item.Price
                                    };
                                    QDetailList.Add(itemadd);
                                }
                            }
                        }
                        #endregion

                        if (QDetailList != null && QDetailList.Count > 0)
                        {
                            foreach (var item in QDetailList)
                            {
                                EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
                                decimal TonCuoiTrongHeThong = EndInventoryRepo.GetQty(item.ProductId.Value);
                                InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                {
                                    InventoryMasterId = InvenMaster.InventoryMasterId,
                                    ProductId = item.ProductId,
                                    BeginInventoryQty = TonCuoiTrongHeThong,
                                    COGS = item.Price,
                                    Price = 0, // Là dịch vụ nên giá bán = 0
                                    //ImportQty = 0, // số lượng nhập = 0
                                    ExportQty = item.Qty,
                                    UnitCOGS = item.Price * item.Qty, //Tổng giá vốn = [COGS] * [ImportQty] = 0
                                    UnitPrice = 0, // Là dịch vụ nên giá bán = 0
                                    EndInventoryQty = TonCuoiTrongHeThong + 0 - item.Qty //Tồn cuối = [BeginInventoryQty] + [ImportQty] -  [ExportQty]
                                };
                                _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                            }

                        }
                        #endregion

                        var staff = _context.EmployeeModel.Where(p => p.EmployeeId == model.StaffId).FirstOrDefault();
                        if (staff != null)
                        {
                            OrderMaster.StaffName = staff.FullName;
                        }
                        #region // Gửi SMS cho khách hàng
                        SendSMSRepository SMSRepo = new SendSMSRepository();
                        string name = OrderMaster.FullName.LastIndexOf(' ') > 0 ? OrderMaster.FullName.Substring(OrderMaster.FullName.LastIndexOf(' ') + 1) : OrderMaster.FullName;
                        name = Library.ConvertToNoMarkString(name);
                        if (!string.IsNullOrEmpty(name))
                        {
                            name = name.First().ToString().ToUpper() + name.Substring(1).ToLower();
                        }
                        string mes = string.Format(
                                                    "Cam on {0} {1} da su dung dich vu tai Chic Cut, so tien da thanh toan la: {2:n0}d",
                                                    OrderMaster.Gender.HasValue && OrderMaster.Gender.Value ? "anh" : "chi",
                                                    name,
                                                    OrderMaster.Total
                                                    );
                        AY_SMSCalendar smsModel = new AY_SMSCalendar()
                        {
                            EndDate = DateTime.Now,
                            isSent = true,
                            NumberOfFailed = 0,
                            SMSContent = mes,
                            SMSTo = OrderMaster.Phone
                        };
                        _context.Entry(smsModel).State = System.Data.Entity.EntityState.Added;
                        #endregion
                        _context.SaveChanges();
                        SMSRepo.SendSMSModel(smsModel);
                        ts.Complete();

                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = true,
                            Data = OrderMaster
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = "Xảy ra lỗi trong quá trình cập nhật trạng thái!" + ex.Message
                });
            }
        }

        private decimal ComputingCommission(decimal? Qty, decimal? UnitPrice)
        {
            decimal result = 0;
            if (UnitPrice.Value > 4000000)
            {
                result = 200000;
            }
            else if (UnitPrice.Value >= 3000000)
            {
                result = 150000;
            }
            else if (UnitPrice.Value >= 2000000)
            {
                result = 100000;
            }
            else
            {
                result = 50000;
            }
            return result * Qty.Value;
        }
        #endregion

        #endregion

        #region Lưu đơn hàng: Khi thêm mới
        public ActionResult SaveAddNewOrder(Daily_ChicCut_OrderModel model, List<Daily_ChicCut_OrderDetailViewModel> details = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var currentTime = DateTime.Now;

                        model.CreatedDate = currentTime;
                        model.CreatedUserId = CurrentUser.UserId;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();

                        decimal SumCOGSOfOrderDetail = 0;
                        #region Chi tiết đơn hàng
                        if (details != null)
                        {
                            #region //Dịch vụ
                            foreach (var item in details.Where(p => p.ProductId == 0).ToList())
                            {

                                var ProductIdLst = (from QMaster in _context.Master_ChicCut_QuantificationMasterModel
                                                    join QDetail in _context.Master_ChicCut_QuantificationDetailModel on QMaster.QuantificationMasterId equals QDetail.QuantificationMasterId
                                                    join p in _context.ProductModel on QDetail.ProductId equals p.ProductId
                                                    where (QMaster.ServiceId == item.ServiceId) && (item.QuantificationMasterId == null || QMaster.QuantificationMasterId == item.QuantificationMasterId)
                                                    select new { QDetail.ProductId.Value, Qty = QDetail.Qty / p.ShippingWeight, p.COGS }).ToList();
                                //Gia vốn = định lượng * Product.COGS
                                decimal COGSCal = 0;
                                foreach (var product in ProductIdLst)
                                {
                                    COGSCal += product.Qty.Value * product.COGS.Value;
                                }
                                SumCOGSOfOrderDetail += COGSCal;
                                Daily_ChicCut_OrderDetailModel itemAdd = new Daily_ChicCut_OrderDetailModel()
                                {
                                    ServiceId = item.ServiceId,
                                    COGS = COGSCal,
                                    Price = item.Price,
                                    Qty = item.Qty,
                                    UnitCOGS = COGSCal * item.Qty,
                                    UnitPrice = item.Price * item.Qty,
                                    OrderId = model.OrderId,
                                    QuantificationMasterId = item.QuantificationMasterId
                                };
                                _context.Entry(itemAdd).State = System.Data.Entity.EntityState.Added;
                            }
                            #endregion

                            #region //Sản phẩm
                            foreach (var item in details.Where(p => p.ProductId > 0))
                            {

                                SumCOGSOfOrderDetail += item.UnitCOGS.HasValue ? item.UnitCOGS.Value : 0;
                                Daily_ChicCut_OrderProductDetailModel itemAdd = new Daily_ChicCut_OrderProductDetailModel()
                                {
                                    ProductId = item.ProductId,
                                    COGS = (item.COGS ?? 0),
                                    Price = item.Price,
                                    Qty = item.Qty,
                                    UnitCOGS = (item.COGS ?? 0) * item.Qty,
                                    UnitPrice = (item.Price ?? 0) * item.Qty,
                                    OrderId = model.OrderId
                                };
                                _context.Entry(itemAdd).State = System.Data.Entity.EntityState.Added;
                            }
                            #endregion
                        }
                        #endregion

                        model.SumCOGSOfOrderDetail = SumCOGSOfOrderDetail;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        ts.Complete();
                        return Content("success");
                    }
                }
                else
                {
                    string errorMessage = "";
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            //DoSomethingWith(error);
                            string.Format("{0}, {1}", errorMessage, error.ErrorMessage);
                        }
                    }

                    return Content("Vui lòng kiểm tra lại thông tin không hợp lệ");
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình thêm mới đơn hàng");
            }
        }
        #endregion

        #region Lưu đơn hàng: Khi sửa
        public ActionResult SaveEditOrder(Daily_ChicCut_OrderModel model, int OrderId)
        {
            try
            {
                var modelUpdate = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
                if (modelUpdate != null)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        modelUpdate.StaffId = model.StaffId;
                        modelUpdate.Note = model.Note;
                        modelUpdate.BillDiscount = model.BillDiscount;
                        modelUpdate.BillDiscountTypeId = model.BillDiscountTypeId;
                        modelUpdate.TotalBillDiscount = modelUpdate.BillDiscount.HasValue ? ((modelUpdate.BillDiscountTypeId == 1 ? modelUpdate.BillDiscount : (modelUpdate.BillDiscount / 100) * modelUpdate.SumPriceOfOrderDetail)) : 0;
                        modelUpdate.Total = modelUpdate.SumPriceOfOrderDetail - modelUpdate.TotalBillDiscount;
                        modelUpdate.PaymentMethodId = model.PaymentMethodId;
                        modelUpdate.isHoliday = model.isHoliday;
                        _context.Entry(modelUpdate).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        ts.Complete();
                        return Content("success");
                    }
                }
                else
                {
                    return Content("Không tìm thấy đơn hàng!");
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình sửa đơn hàng");
            }

        }
        #endregion
        #endregion

        #region Thêm mới đơn hàng
        public ActionResult Create(int? CustomerId = null)
        {
            Daily_ChicCut_OrderModel model = new Daily_ChicCut_OrderModel();
            CreateViewBag();
            #region Lấy thông tin nhân viên khách hàng (nếu có ReturnUrl)
            if (CustomerId.HasValue)
            {
                GetInfoCustomer(CustomerId.Value, model);
            }
            #endregion

            return View(model);
        }

        public ActionResult _GetServicePartital(int HairTypeId, bool? Gender)
        {
            var lst = (
                from p in _context.Master_ChicCut_ServiceModel
                join sc in _context.Master_ChicCut_ServiceCategoryModel on p.ServiceCategoryId equals sc.ServiceCategoryId
                where p.HairTypeId == HairTypeId &&
                    //p.Gender == Gender &&
                p.Actived == true
                orderby (sc.OrderBy)
                select p
                ).ToList();
            return PartialView(lst);
        }
        #endregion

        #region Helper
        private void CreateViewBag(int? HairTypeId = null, int? ServiceCategoryId = null, int? EmployeeId = null, int? PaymentMethodId = null)
        {
            //Kiểu tóc
            var HairTypeLst = _context.Master_ChicCut_HairTypeModel.OrderBy(p => p.OrderIndex).ToList();
            ViewBag.HairTypeId = new SelectList(HairTypeLst, "HairTypeId", "HairTypeName", HairTypeId);

            var HairTypeLstWithCheckedItem = _context.Master_ChicCut_HairTypeModel.OrderBy(p => p.OrderIndex).Select(p => new Master_ChicCut_HairTypeSeachModel()
            {
                HairTypeId = p.HairTypeId,
                HairTypeName = p.HairTypeName,
                Checked = p.HairTypeId == HairTypeId ? true : false
            }).ToList();
            ViewBag.HairTypeLstWithCheckedItem = HairTypeLstWithCheckedItem;

            //Danh mục dịch vụ
            var ServiceCategoryLst = _context.Master_ChicCut_ServiceCategoryModel.OrderBy(p => p.OrderBy).Where(p => p.Actived == true).ToList();
            ViewBag.ServiceCategoryId = new SelectList(ServiceCategoryLst, "ServiceCategoryId", "ServiceName", ServiceCategoryId);

            //Phương thức thanh toán
            var PaymentMethodLst = _context.PaymentMethodModel.Where(p => p.Actived == true).ToList();
            ViewBag.PaymentMethodId = new SelectList(PaymentMethodLst, "PaymentMethodId", "PaymentMethodName", PaymentMethodId);

            //Nhân viên phục vụ
            var StaffLst = (from p in _context.EmployeeModel
                            join ac in _context.AccountModel on p.EmployeeId equals ac.EmployeeId
                            where p.Actived == true && ac.RolesId == EnumRoles.NVPV
                            orderby p.FullName ascending
                            select p
                            ).ToList();

            ViewBag.StaffId = new SelectList(StaffLst, "EmployeeId", "FullName", EmployeeId);
        }

        private void GetInfoCustomer(int CustomerId, Daily_ChicCut_OrderModel model)
        {
            var customer = _context.CustomerModel.Where(p => p.CustomerId == CustomerId).FirstOrDefault();
            if (customer != null)
            {
                model.FullName = (customer.FullName + " - " + customer.Phone);
                model.IdentityCard = customer.IdentityCard;
                model.Phone = customer.Phone;
                model.Gender = customer.Gender;

                ViewBag.IdCustomer = customer.CustomerId;
            }
        }
        #endregion

        #region Không cho chọn phục vụ 1 khách 2 lần cùng lúc
        public ActionResult CheckCustomerExist(int CustomerId)
        {
            //Lấy CustomerId thuộc đơn hàng có trạng thái đang phục vụ hoặc đang chờ
            var cusList = (from o in _context.Daily_ChicCut_OrderModel
                           join c in _context.CustomerModel on o.CustomerId equals c.CustomerId
                           //Đang chờ => OrderStatusId = 1
                           //Đang phục vụ => OrderStatusId = 2
                           where (o.OrderStatusId == 1 || o.OrderStatusId == 2) && o.CustomerId == c.CustomerId
                           select c.CustomerId).ToList();
            foreach (var item in cusList)
            {
                //Nếu CustomerId chọn từ Dropdownlist trùng với CustomerId trong đơn hàng đang phục vụ hoặc đang chờ 
                // => Hiện thông báo 
                if (CustomerId == item)
                {
                    return Json(new { Message = "Khách hàng này hiện đang được phục vụ hoặc đang chờ. Vui lòng chọn khách hàng khác!" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //Thông báo
        public ActionResult Alert(string Content)
        {
            ViewBag.Content = Content;
            return PartialView();
        }
        #endregion
    }
}
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
using System.Data.Entity;
using System.IO;
using ViewModels.BeautyBar;
using System.Data.SqlClient;
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
                //Danh mục dịch vụ
                ViewBag.ListCategory = (from spc in _context.Master_ChicCut_ServiceParentCategoryModel
                                        where spc.Actived == true
                                        orderby (spc.OrderBy)
                                        select new ServiceCategoryViewModel()
                                        {
                                            ServiceParentCategoryId = spc.ServiceParentCategoryId,
                                            ServiceParentCategoryName = spc.ServiceParentCategoryName
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
                if (OrderId.HasValue && OrderId.Value <= 0)
                {
                    if (itemExist != null)     //Đã có dịch vụ đó
                    {
                        var index = details.FindIndex(c => c.ServiceId == ServiceId);
                        details[index] = new Daily_ChicCut_OrderDetailViewModel()
                        {
                            ServiceId = itemExist.ServiceId,
                            COGS = itemExist.COGS,
                            Price = itemExist.Price,
                            Qty = itemExist.Qty + 1,
                            ServiceQty = itemExist.ServiceQty,
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
                            COGS = 0,
                            Price = p.Price,
                            //Qty = 1,
                            //UnitPrice = p.Price * 

                            //18-01-23.001: Khi chọn dịch vụ với số lần thực hiện liệu trình >= 3 => Cập nhật "Số lượng"(Qty) và "Thành tiền"(UnitPrice) của dịch vụ qua "Chi tiết hóa đơn"(Daily_ChicCut_OrderDetailViewModel)
                            //Dịch vụ có liệu trình 1 lần 
                            //      => Cập nhật Qty = 1
                            //Dịch vụ có liệu trình nhiều lần 
                            //      => Qty = số lần thực hiện liệu trình (dựa vào "Số lần sử dụng"(ServiceQty) của "Loại giá"(Master_ChicCut_HairTypeModel))
                            //      VD: Loại dịch vụ Mesotherapy có liệu trình 3 lần và 5 lần
                            //          1. 3 lần => Cập nhật Qty = 3
                            //          2. 5 lần => Cập nhật Qty = 5
                            //          => Cập nhật UnitPrice
                            //Số lần thực hiện liệu trình
                            ServiceQty = _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault(),

                            //Số lượng trong chi tiết hóa đơn
                            Qty = _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() > 1 ?
                                    _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() : 1,
                            UnitCOGS = 0,
                            //UnitPrice = p.Price,

                            //Tính thành tiền dựa vào số lần thực hiện liệu trình
                            UnitPrice = p.Price * (_context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() > 1 ?
                                                    _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() : 1),
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
                }

                //if (OrderId.HasValue && OrderId.Value > 0)//Sửa đơn hàng: + Chưa có dịch vụ đó: Thêm mới 
                else
                {
                    //              + Có rồi: Update Qty, TT, Vốn, Sum..
                    #region //Sửa đơn hàng:
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var itemOrderDetailModel = _context.Daily_ChicCut_OrderDetailModel.Where(p => p.ServiceId == ServiceId && p.OrderId == OrderId).FirstOrDefault();
                        if (itemOrderDetailModel != null) // Update
                        {
                            itemOrderDetailModel.COGS = itemOrderDetailModel.COGS;
                            //itemOrderDetailModel.UnitCOGS = itemOrderDetailModel.COGS * (itemOrderDetailModel.Qty + 1);
                            //itemOrderDetailModel.UnitPrice = itemOrderDetailModel.Price * (itemOrderDetailModel.Qty + 1);
                            //itemOrderDetailModel.Qty++;
                            //18-01-23.001

                            itemOrderDetailModel.Qty = _context.Master_ChicCut_HairTypeModel
                                                            .Where(h => h.HairTypeId == itemOrderDetailModel.Master_ChicCut_ServiceModel.HairTypeId)
                                                            .Select(h => h.ServiceQty).FirstOrDefault() > 1 ?
                                                        itemOrderDetailModel.Qty + _context.Master_ChicCut_HairTypeModel
                                                            .Where(h => h.HairTypeId == itemOrderDetailModel.Master_ChicCut_ServiceModel.HairTypeId)
                                                            .Select(h => h.ServiceQty).FirstOrDefault() :
                                                        itemOrderDetailModel.Qty + 1;
                            itemOrderDetailModel.UnitCOGS = itemOrderDetailModel.COGS * itemOrderDetailModel.Qty;
                            itemOrderDetailModel.UnitPrice = itemOrderDetailModel.Price * itemOrderDetailModel.Qty;

                            //Hiển thị ra chi tiết hóa đơn
                            itemExist.ServiceQty = _context.Master_ChicCut_HairTypeModel
                                                            .Where(h => h.HairTypeId == itemOrderDetailModel.Master_ChicCut_ServiceModel.HairTypeId)
                                                            .Select(h => h.ServiceQty).FirstOrDefault();

                            var index = details.FindIndex(c => c.ServiceId == ServiceId);
                            details[index] = new Daily_ChicCut_OrderDetailViewModel()
                            {
                                ServiceId = itemExist.ServiceId,
                                COGS = itemExist.COGS,
                                Price = itemExist.Price,
                                ServiceQty = itemExist.ServiceQty,
                                Qty = itemExist.Qty + 1,
                                UnitCOGS = itemExist.COGS * (itemExist.Qty + 1),
                                UnitPrice = itemExist.Price * (itemExist.Qty + 1),
                                ServiceName = itemExist.ServiceName,
                                QuantificationMasterId = itemExist.QuantificationMasterId,
                                QuantificationMasterList = itemExist.QuantificationMasterList,
                            };


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
                                //TreatmentId = 
                                COGS = COGSCal,
                                Price = ServiceModel.Price,
                                //Qty = 1,
                                //18-01-23.001
                                Qty = _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == ServiceModel.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() > 1 ?
                                _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == ServiceModel.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() : 1,
                                UnitCOGS = COGSCal,
                                //UnitPrice = ServiceModel.Price,
                                UnitPrice = ServiceModel.Price * (_context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == ServiceModel.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() > 1 ?
                                _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == ServiceModel.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() : 1)
                            };

                            //Hiển thị ra chi tiết hóa đơn
                            var item = _context.Master_ChicCut_ServiceModel.Where(p => p.ServiceId == ServiceId).Select(p => new Daily_ChicCut_OrderDetailViewModel()
                            {
                                ServiceId = p.ServiceId,
                                COGS = 0,
                                Price = p.Price,
                                ServiceQty = _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault(),
                                Qty = _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() > 1 ?
                                        _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() : 1,
                                UnitCOGS = 0,
                                UnitPrice = p.Price * (_context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() > 1 ?
                                                        _context.Master_ChicCut_HairTypeModel.Where(h => h.HairTypeId == p.HairTypeId).Select(h => h.ServiceQty).FirstOrDefault() : 1),
                                ServiceName = p.ServiceName,
                                QuantificationMasterList = _context.Master_ChicCut_QuantificationMasterModel.Where(Q => Q.ServiceId == p.ServiceId).Select(Q => new SelectListItem()
                                {
                                    Text = Q.QuantificationName,
                                    Value = SqlFunctions.StringConvert((double)Q.QuantificationMasterId).Trim()
                                })
                            }).FirstOrDefault();
                            details.Add(item);

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
                        //Cập nhật thông tin model
                        model.CashierUserId = CurrentUser.UserId;
                        model.CashierDate = currentTime;
                        model.Note = OrderMaster.Note;
                        model.Phone = OrderMaster.Phone;
                        model.OrderStatusId = EnumDaily_ChicCut_OrderStatus.DaTinhTien;
                        decimal Commission = 0;

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
                                Commission += item.Qty.Value * EnumTipAndCommission.TienProductCommission;

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

                        //Cật nhật thông tin liệu trình có số lần >=3
                        #region // Insert TreatmentModel
                        TreatmentModel TreatMaster = new TreatmentModel();
                        //Mã liệu trình: 
                        //  VD: 
                        //      + CreatedDate: 25/01/2018 
                        //          =>Lấy 6 số: 20150818
                        //          =>TH1: Trong ngày chưa có đơn hàng chứa liệu trình với số lần sử dụng >= 3 => Cộng thêm ".001": 20150818.001
                        //          =>TH2: Ngược lại => Cộng thêm 1 đơn vị: 
                        //                  VD: Trong ngày đã có 2 đơn hàng chứa liệu trình với số lần sử dụng >= 3 với 2 mã liệu trình đã tồn tại
                        //                      1. 20150818.001
                        //                      2. 20150818.002
                        //                  => Mã liệu trình hiện tại: 20150818.003

                        //Lấy dữ liệu từ "Chi tiết Đơn hàng"
                        var treatment = (from od in _context.Daily_ChicCut_OrderDetailModel
                                         //Lấy dữ liệu từ "Đơn hàng"
                                         join om in _context.Daily_ChicCut_OrderModel on od.OrderId equals om.OrderId
                                         //Lấy dữ liệu từ "Dịch vụ"
                                         join s in _context.Master_ChicCut_ServiceModel on od.ServiceId equals s.ServiceId
                                         //Lấy dữ liệu từ "Liệu trình"
                                         join t in _context.TreatmentModel on s.ServiceId equals t.ServiceId
                                         //Lấy dữ liệu từ "Loại giá"
                                         join h in _context.Master_ChicCut_HairTypeModel on s.HairTypeId equals h.HairTypeId
                                         //Lấy những đơn hàng trong ngày hiện tại với:
                                         //         + Số lần sử dụng của liệu trình >= 3 => Loại giá có "Số lần sử dụng"(ServiceQty) >= 3
                                         //         + Những đơn hàng đã thanh toán => OrderStatusId = 3
                                         //     => Dùng để tạo "Mã liệu trình"(TreatmentCode)
                                         where h.ServiceQty >= 3 
                                                && DbFunctions.TruncateTime(om.CashierDate) == DbFunctions.TruncateTime(DateTime.Now) 
                                                && om.OrderStatusId == 3 
                                                && t.ServiceId == od.ServiceId
                                         group t by t.TreatmentId into g
                                         select new
                                         {
                                             TreatmentId = g.Key,
                                         }).ToList();
                        if (OrderMaster.details != null && OrderMaster.details.Count > 0)
                        {
                            int count = treatment.Count();
                            int TreatmentCodeInt = 1;
                            foreach (var item1 in OrderMaster.details)
                            {
                                if (treatment != null && treatment.Count > 0)
                                {

                                    //Liệu trình chưa hoàn thành => Thêm mới vào "Chi tiết liệu trình đã sử dụng" (TreatmentDetailModel)
                                    if (item1.Price == 0 && item1.UnitPrice == 0)
                                    {
                                        TreatmentModel TreatMasterHasValue = _context.TreatmentModel.Where(p => p.TreatmentId == item1.TreatmentId)
                                         .Include("Daily_ChicCut_OrderModel").FirstOrDefault();

                                        if (TreatMasterHasValue != null)
                                        {
                                            //Thêm dữ liệu về chi tiết liệu trình mà khách hàng sử dụng vào bảng TreatmentDetailModel
                                            var order = _context.Daily_ChicCut_OrderModel.Find(OrderMaster.OrderId);
                                            if (order != null)
                                            {
                                                TreatMasterHasValue.Daily_ChicCut_OrderModel.Add(order);
                                            }

                                            _context.Entry(TreatMasterHasValue).State = System.Data.Entity.EntityState.Modified;
                                            _context.SaveChanges();
                                        }
                                    }

                                    //Liệu trình mới => Thêm mới vào "Liệu trình" (TreatmentModel)
                                    else
                                    {
                                        string TreatmentCode = model.CashierDate.Value.Date.ToString("yyyyMMdd") + ".";
                                        string countString = "";
                                        count++;

                                        //Mã liệu trình
                                        if (count < 10)
                                        {
                                            countString = "00";
                                            TreatMaster.TreatmentCode = TreatmentCode + countString + count;
                                        }
                                        else if (count >= 10 && count < 100)
                                        {
                                            countString = "0";
                                            TreatMaster.TreatmentCode = TreatmentCode + countString + count;
                                        }
                                        else
                                        {
                                            TreatMaster.TreatmentCode = TreatmentCode + count;
                                        }

                                        //Khách hàng
                                        TreatMaster.CustomerId = model.CustomerId;

                                        //Dịch vụ
                                        TreatMaster.ServiceId = item1.ServiceId;

                                        //Số lần
                                        TreatMaster.Qty = (int)item1.Qty;

                                        //Lấy đơn hàng từ db giống với đơn hàng được truyền vào, nếu có thì add
                                        var order = _context.Daily_ChicCut_OrderModel.Find(OrderMaster.OrderId);
                                        if (order != null)
                                        {
                                            TreatMaster.Daily_ChicCut_OrderModel.Add(order);
                                        }

                                        _context.Entry(TreatMaster).State = System.Data.Entity.EntityState.Added;
                                        _context.SaveChanges();

                                    }

                                }
                                else
                                {
                                    string TreatmentCode = model.CashierDate.Value.Date.ToString("yyyyMMdd") + ".00";
                                    TreatMaster.TreatmentCode = TreatmentCode + TreatmentCodeInt;
                                    TreatMaster.CustomerId = model.CustomerId;
                                    TreatMaster.ServiceId = item1.ServiceId;
                                    TreatMaster.Qty = (int)item1.Qty;
                                    var order = _context.Daily_ChicCut_OrderModel.Find(OrderMaster.OrderId);
                                    if (order != null)
                                    {
                                        TreatMaster.Daily_ChicCut_OrderModel.Add(order);
                                    }

                                    _context.Entry(TreatMaster).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();

                                    TreatmentCodeInt++;
                                }
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
                                    TreatmentId = item.TreatmentId,
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

        public ActionResult _GetServicePartital(int HairTypeId)
        {
            //Danh sách dịch vụ
            List<ServiceViewModel> lst = (
               from p in _context.Master_ChicCut_ServiceModel
               join sc in _context.Master_ChicCut_ServiceCategoryModel on p.ServiceCategoryId equals sc.ServiceCategoryId
               //Loại giá
               where p.HairTypeId == HairTypeId &&
                   //Dịch vụ đang sử dụng
                     sc.Actived == true &&
                   //Đang sử dụng
                     p.Actived == true

               orderby (sc.OrderBy)
               select new ServiceViewModel()
               {
                   Price = p.Price,
                   ServiceId = p.ServiceId,
                   ServiceName = p.ServiceName,
                   ServiceParentCategoryId = sc.ServiceParentCategoryId
               }).ToList();

            //Danh sách các danh mục
            List<int> parentList = _context.Master_ChicCut_ServiceParentCategoryModel
                                                 .Select(p => p.ServiceParentCategoryId).ToList();
            ViewBag.ParentCategoryList = parentList;
            return PartialView(lst);

        }
        #endregion

        #region Helper
        private void CreateViewBag(int? HairTypeId = null, int? ServiceCategoryId = null, int? EmployeeId = null, int? ServiceParentCategoryId = null, int? PaymentMethodId = null)
        {
            //Loại giá
            var HairTypeLst = _context.Master_ChicCut_HairTypeModel.OrderBy(p => p.OrderIndex).Where(p => p.Actived == true).ToList();
            ViewBag.HairTypeId = new SelectList(HairTypeLst, "HairTypeId", "HairTypeName", HairTypeId);

            var HairTypeLstWithCheckedItem = _context.Master_ChicCut_HairTypeModel.OrderBy(p => p.OrderIndex).Select(p => new Master_ChicCut_HairTypeSeachModel()
            {
                HairTypeId = p.HairTypeId,
                HairTypeName = p.HairTypeName,
                Checked = p.HairTypeId == HairTypeId ? true : false
            }).ToList();
            ViewBag.HairTypeLstWithCheckedItem = HairTypeLstWithCheckedItem;

            //Dịch vụ
            var ServiceCategoryLst = _context.Master_ChicCut_ServiceCategoryModel.OrderBy(p => p.OrderBy).Where(p => p.Actived == true).ToList();
            ViewBag.ServiceCategoryId = new SelectList(ServiceCategoryLst, "ServiceCategoryId", "ServiceName", ServiceCategoryId);

            //Danh mục dịch vụ
            var ServiceParentCategoryLst = _context.Master_ChicCut_ServiceParentCategoryModel.OrderBy(p => p.OrderBy).Where(p => p.Actived == true).ToList();
            ViewBag.ServiceParentCategoryId = new SelectList(ServiceParentCategoryLst, "ServiceParentCategoryId", "ServiceParentCategoryName", ServiceParentCategoryId);

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

        #region //Chi tiết những lần sử dụng liệu trình

        //Hiển thị tự động liệu trình chưa hoàn thành trong "Chi tiết hóa đơn"(Daily_ChicCut_OrderDetailModel): 
        //      + VD: 
        //              - Số lần thực hiện liệu trình: 3
        //              - Khách hàng đã sử dụng: 1
        //              => Lần sử dụng liệu trình tiếp theo: 
        //                                                  -- set Price = 0
        //                                                  -- set UnitPrice = 0    
        public ActionResult GetTreatment(int CustomerId)
        {
            //Nếu Số lần đã sử dụng < Số lần liệu trình => Hiển thị lần kế tiếp
            //Ngược lại => Không hiển thị

            //Lọc thông tin từ DB dùng stored procedure "usp_TreatmentDetails1"
            var lst = _context.Database.SqlQuery<TreatmentDetailViewModel>("usp_TreatmentDetails1 @CustomerId", new SqlParameter("CustomerId", CustomerId)).ToList();
            if (lst != null && lst.Count > 0)
            {
                var treatmentDetails = lst.Select(p => new Daily_ChicCut_OrderDetailViewModel()
                {
                    TreatmentId = p.TreatmentId,
                    ServiceId = p.ServiceId,
                    COGS = 0,
                    Price = 0,
                    Qty = 1,
                    UnitCOGS = 0,
                    UnitPrice = 0,
                    ServiceName = p.ServiceName,
                    QuantificationMasterList = _context.Master_ChicCut_QuantificationMasterModel.Where(Q => Q.ServiceId == p.ServiceId).Select(Q => new SelectListItem()
                    {
                        Text = Q.QuantificationName,
                        Value = SqlFunctions.StringConvert((double)Q.QuantificationMasterId).Trim()
                    })
                }).ToList();


                return Json(new
                {
                    Details = RenderPartialViewToString("_DailyChicCutOrderDetailInnerInfo", treatmentDetails)
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        //Xem chi tiết những liệu trình khách hàng đã sử dụng
        //  VD:
        //      + Khách sử dụng dịch vụ với liệu trình 3 lần và đã sử dụng 2 lần => Hiển thị chi tiết 2 lần đã sử dụng.
        //      + Khách sử dụng dịch vụ với liệu trình 5 lần và đã sử dụng 3 lần => Hiển thị chi tiết 3 lần đã sử dụng.
        public ActionResult GetTreatmentDetails(int CustomerId, int? TreatmentId)
        {
            //Hiển thị header modal popup chi tiết liệu trình của khách hàng dựa vào CustomerId
            var customer = _context.CustomerModel.Where(p => p.CustomerId == CustomerId).FirstOrDefault();
            ViewBag.TitlePopup = string.Format("Chi tiết sử dụng liệu trình của khách hàng: {0}", customer.FullName);

            //Lọc thông tin từ DB dùng stored procedure "usp_TreatmentDetails2"
            var treatmentList = _context.Database.SqlQuery<TreatmentDetailViewModel>("usp_TreatmentDetails2 @CustomerId, @TreatmentId",
                                        new SqlParameter("CustomerId", CustomerId),
                                         new SqlParameter("TreatmentId", TreatmentId)
                                        ).ToList();
            if (treatmentList != null && treatmentList.Count > 0)
            {
                return Json(new
                {
                    Treatment = RenderPartialViewToString(null, treatmentList),
                    CusName = ViewBag.TitlePopup,
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        //Hàm chuyển PartialView sang dạng String => Return Json data
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion
    }
}
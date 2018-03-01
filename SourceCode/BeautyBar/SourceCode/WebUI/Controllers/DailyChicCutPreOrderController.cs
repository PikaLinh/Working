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
namespace WebUI.Controllers
{
    public class DailyChicCutPreOrderController : BaseController
    {
        // GET: DailyChicCutPreOrder
        public ActionResult Index()
        {
            var Model = new ChicCutPreOrderSeachViewModel();
            Model.SearchAppointmentTime = DateTime.Now.Date;
            CreateViewBag();
            return View(Model);
        }
        #region Danh sách đơn hàng đặt trước
        public ActionResult _ReservationLst(ChicCutPreOrderSeachViewModel modelSearch)
        {
            DateTime? FromDateCreated = null;
            DateTime? ToDateCreated = null;
            if (modelSearch.SearchCreatedDate.HasValue)
            {
                FromDateCreated = modelSearch.SearchCreatedDate.Value.Date;
                ToDateCreated = modelSearch.SearchCreatedDate.Value.Date.AddDays(1).AddMilliseconds(-1);
            }


            DateTime? FromDate = null;
            DateTime? ToDate = null;
            if (modelSearch.SearchAppointmentTime.HasValue)
            {
                FromDate = modelSearch.SearchAppointmentTime.Value.Date;
                ToDate = modelSearch.SearchAppointmentTime.Value.Date.AddDays(1).AddMilliseconds(-1);
            }

            var lst = _context.Daily_ChicCut_Pre_OrderModel.Where(p =>
                (modelSearch.SearchCreatedDate == null || (FromDateCreated <= p.CreatedDate && p.CreatedDate <= ToDateCreated)) &&
                (modelSearch.SearchAppointmentTime == null || (FromDate <= p.AppointmentTime && p.AppointmentTime <= ToDate)) &&
                (modelSearch.SearchPreOrderCode == null || p.PreOrderCode.Contains(modelSearch.SearchPreOrderCode)) &&
                (modelSearch.SearchFullName == null || p.FullName.Contains(modelSearch.SearchFullName)) &&
                (modelSearch.SearchPhone == null || p.Phone.Contains(modelSearch.SearchPhone)) &&
                (p.OrderStatusId == EnumDaily_ChicCut_OrderStatus.DatTruoc)
                ).OrderByDescending(p => p.CreatedDate).ToList();
            return PartialView("_Reservation", lst);
        }

        public ActionResult GetPreOrderModelFromPreOrderId(int PreOrderId)
        {
            var model = _context.Daily_ChicCut_Pre_OrderModel.Where(p => p.PreOrderId == PreOrderId).Select(p => new ChicCutOrderViewModel()
                {
                    FullName = p.FullName,
                    Phone = p.Phone,
                    Gender = p.Gender,
                    PreOrderId = PreOrderId,
                    MinSumPriceOfOrderDetail = p.MinSumPriceOfOrderDetail,
                    MaxSumPriceOfOrderDetail = p.MaxSumPriceOfOrderDetail,
                    details = _context.Daily_ChicCut_Pre_OrderDetailModel.Where(pp => pp.PreOrderId == PreOrderId).Select(pp => new ChicCutOrderDetailViewModel()
                    {
                        ServiceName = _context.Master_ChicCut_ServiceCategoryModel.Where(m => m.ServiceCategoryId == pp.ServiceCategoryId).Select(m => m.ServiceName).FirstOrDefault(),
                        MinPrice = pp.MinPrice,
                        MaxPrice = pp.MaxPrice,
                        Qty = pp.Qty,
                        MinUnitPrice = pp.MinUnitPrice,
                        MaxUnitPrice = pp.MaxUnitPrice
                    }).ToList()
                }).FirstOrDefault();
            //return Json(model);
            if (model != null)
            {
                return Json(new
                      {
                          Success = true,
                          Data = model
                      });
            }
            else
            {
                return Json(new
                {
                    Success = true,
                    Data = "Không tìm thấy đơn đặt hàng yêu cầu!"
                });
            }
        }
        #endregion

        #region Xác nhận chuyển trạng thái Chờ
        public ActionResult ConfirmToPreOrder(int PreOrderId, int HairTypeId, int OrderStatusId)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    #region #B1. Tạo OrderMaster và OrderDetail
                    var PreOrderMaster = _context.Daily_ChicCut_Pre_OrderModel.Find(PreOrderId);
                    List<Daily_ChicCut_OrderDetailViewModel> LstOrderDetail = GetListOrderDetail(HairTypeId, PreOrderId);
                    Daily_ChicCut_OrderModel modelOrder = GetOrderMaster(PreOrderMaster, LstOrderDetail, HairTypeId, OrderStatusId);
                    _context.Entry(modelOrder).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();

                    //Duyệt danh sách LstOrderDetail để Add vào OrderDetail: Tính SumCOGSOfOrderDetail (CashierUserId, CashierDate, Tip, Commission khi Thanh toán sẽ tính) 
                    decimal SumCOGSOfOrderDetail = 0;
                    if (LstOrderDetail != null && LstOrderDetail.Count > 0)
                    {
                        DuyetDanhSachChiTietDonHang(ref SumCOGSOfOrderDetail, LstOrderDetail, modelOrder.OrderId);
                    }
                    #endregion

                    #region #B2.Cập nhật PreOder: OrderId, OrderStatusId. Cập nhật OrderMaster: SumCOGSOfOrderDetail
                    // PreOder
                    PreOrderMaster.OrderId = modelOrder.OrderId;
                    PreOrderMaster.OrderStatusId = OrderStatusId; // 1.Đang chờ || 2.Đang phục vụ
                    _context.Entry(PreOrderMaster).State = System.Data.Entity.EntityState.Modified;
                    //OrderMaster
                    modelOrder.SumCOGSOfOrderDetail = SumCOGSOfOrderDetail;
                    _context.Entry(modelOrder).State = System.Data.Entity.EntityState.Modified;                                                                                                 
                    _context.SaveChanges();

                    #endregion
                    ts.Complete();
                    return Content("success");
                    //Gửi tin nhắn

                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình xác nhận đơn hàng!");
            }
        }

        private Daily_ChicCut_OrderModel GetOrderMaster(Daily_ChicCut_Pre_OrderModel PreOrderMaster, List<Daily_ChicCut_OrderDetailViewModel> LstOrderDetail, int HairTypeId, int OrderStatusId)
        {
            // Tạo Order Master
            var currentTime = DateTime.Now;
            var modelOrder = new Daily_ChicCut_OrderModel()
            {
                CustomerId = PreOrderMaster.CustomerId,
                FullName = PreOrderMaster.FullName,
                IdentityCard = PreOrderMaster.IdentityCard,
                HairTypeId = HairTypeId,
                Gender = PreOrderMaster.Gender,
                Phone = PreOrderMaster.Phone,
                BillDiscountTypeId = 1,
                OrderStatusId = OrderStatusId,
                CreatedDate = currentTime,
                CreatedUserId = CurrentUser.UserId,
                Note = PreOrderMaster.Note,
                SumPriceOfOrderDetail = (LstOrderDetail.Sum(p => p.UnitPrice) ?? 0),
                Total = (LstOrderDetail.Sum(p => p.UnitPrice) ?? 0)
            };
            return modelOrder;
        }

        private List<Daily_ChicCut_OrderDetailViewModel> GetListOrderDetail(int HairTypeId, int PreOrderId)
        {
            return (
                             from p in _context.Master_ChicCut_ServiceModel
                             join PreDe in _context.Daily_ChicCut_Pre_OrderDetailModel on p.ServiceCategoryId equals PreDe.ServiceCategoryId // Không thể join dữ liệu từ db với 1 dữ liệu khác trong bộ nhớ
                             where p.HairTypeId == HairTypeId
                             && PreDe.PreOrderId == PreOrderId
                             select new Daily_ChicCut_OrderDetailViewModel()
                             {
                                 ServiceId = p.ServiceId,
                                 COGS = 0,
                                 Price = p.Price,
                                 Qty = PreDe.Qty,
                                 UnitCOGS = 0,
                                 UnitPrice = p.Price * PreDe.Qty
                             }
                         ).ToList();
        }

        private void DuyetDanhSachChiTietDonHang(ref decimal SumCOGSOfOrderDetail, List<Daily_ChicCut_OrderDetailViewModel> LstOrderDetail, int OrderId)
        {
            #region //Dịch vụ
            foreach (var item in LstOrderDetail)
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
                    OrderId = OrderId,
                    QuantificationMasterId = item.QuantificationMasterId
                };
                _context.Entry(itemAdd).State = System.Data.Entity.EntityState.Added;
            }
            #endregion
        }
        #endregion

        public ActionResult _GetAllServicePartital(int Orderstatusid)
        {
            var lst = _context.Daily_ChicCut_Pre_OrderModel.Where(p => (Orderstatusid == EnumDaily_ChicCut_OrderStatus.TatCa) || (p.OrderStatusId == Orderstatusid)).OrderByDescending(p => p.CreatedDate).ToList();
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
        public ActionResult _UpdateOrder(int? PreOrderId = null)
        {
            try
            {
                CreateViewBag();
                if (PreOrderId.HasValue)
                {
                    var model = _context.Daily_ChicCut_Pre_OrderModel.Where(p => p.PreOrderId == PreOrderId).FirstOrDefault();
                    GetInfoCustomer(model.CustomerId.Value, model);
                    CreateViewBag(model.HairTypeId);
                    ViewBag.PreOrderId = PreOrderId;
                    //TempData["BillDiscount"] = model.BillDiscount;
                    //TempData["BillDiscountTypeId"] = model.BillDiscountTypeId;
                    return PartialView(model);
                }
                else
                {
                    return PartialView(new Daily_ChicCut_Pre_OrderModel());
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
        public ActionResult _DailyChicCutOrderDetailInfo(List<Daily_ChicCut_Pre_OrderDetailViewModel> details = null)
        {
            if (details == null)
            {
                details = new List<Daily_ChicCut_Pre_OrderDetailViewModel>();
            }
            return PartialView(details);
        }

        //CreateListInner
        public ActionResult _DailyChicCutOrderDetailInnerInfo(List<Daily_ChicCut_Pre_OrderDetailViewModel> details = null, int? ServiceCategoryId = null, int? PreOrderId = null)
        {
            if (details == null)
            {
                details = new List<Daily_ChicCut_Pre_OrderDetailViewModel>();
            }
            /*Kiểm tra xem Danh sách đã có dịch vụ đó chưa :
                        + Có rồi: Tăng Qty + Cogs + UnitPrice
                        + Chưa có: Thêm mới.
            */
            if (ServiceCategoryId.HasValue)
            {
                var itemExist = details.Where(p => p.ServiceCategoryId == ServiceCategoryId).FirstOrDefault();
                if (itemExist != null)     //Đã có dịch vụ đó
                {
                    var index = details.FindIndex(c => c.ServiceCategoryId == ServiceCategoryId);
                    details[index] = new Daily_ChicCut_Pre_OrderDetailViewModel()
                    {
                        ServiceCategoryId = itemExist.ServiceCategoryId,
                        // COGS = itemExist.COGS,
                        MinPrice = itemExist.MinPrice,
                        MaxPrice = itemExist.MaxPrice,
                        Qty = itemExist.Qty + 1,
                        // UnitCOGS = itemExist.COGS * (itemExist.Qty + 1),
                        MinUnitPrice = itemExist.MinPrice * (itemExist.Qty + 1),
                        MaxUnitPrice = itemExist.MaxUnitPrice * (itemExist.Qty + 1),
                        ServiceName = itemExist.ServiceName
                    };
                }
                else //Chưa có dịch vụ đó
                {
                    #region Chưa có dịch vụ đó
                    /* Tính giá vốn 
                    List<int> ProductIdLst = (from QMaster in _context.Master_ChicCut_QuantificationMasterModel
                                              join QDetail in _context.Master_ChicCut_QuantificationDetailModel on QMaster.QuantificationMasterId equals QDetail.QuantificationMasterId
                                              where QMaster.ServiceId == ServiceId
                                              select QDetail.ProductId.Value).ToList();
                    var LstProductModel = _context.ProductModel.Where(p => ProductIdLst.Contains(p.ProductId)).ToList();
                    decimal COGSCal = (LstProductModel != null && LstProductModel.Count > 0) ? LstProductModel.Sum(p => p.COGS.HasValue ? p.COGS : 0).Value : 0;
                    */

                    //    from p in _context.Master_ChicCut_ServiceModel
                    //join sc in _context.Master_ChicCut_ServiceCategoryModel on p.ServiceCategoryId equals sc.ServiceCategoryId
                    //where p.Gender == Gender && p.Actived == true && p.Price > 0
                    //group p by p.ServiceCategoryId into P_Groupby // nhóm bảng nào  + theo cột nào + ra bảng mới
                    //orderby (P_Groupby.Key)
                    //select new Master_ChicCut_ServiceViewModel()
                    //{
                    //    ServiceCategoryId = P_Groupby.Key,
                    //    MinPrice = P_Groupby.Min(p => p.Price.Value),
                    //    MaxPrice = P_Groupby.Max(p => p.Price.Value),
                    //    ServiceName = _context.Master_ChicCut_ServiceCategoryModel.Where(p => p.ServiceCategoryId == P_Groupby.Key).Select(p => p.ServiceName).FirstOrDefault()
                    //}

                    var item = (
                        from p in _context.Master_ChicCut_ServiceModel
                        join sc in _context.Master_ChicCut_ServiceCategoryModel on p.ServiceCategoryId equals sc.ServiceCategoryId
                        where p.Actived == true && p.Price > 0 && p.ServiceCategoryId == ServiceCategoryId
                        group p by p.ServiceCategoryId into P_Groupby // nhóm bảng nào  + theo cột nào + ra bảng mới
                        select new Daily_ChicCut_Pre_OrderDetailViewModel()
                            {
                                ServiceCategoryId = P_Groupby.Key,
                                //COGS = 0,
                                MinPrice = P_Groupby.Min(p => p.Price.Value),
                                MaxPrice = P_Groupby.Max(p => p.Price.Value),
                                Qty = 1,
                                ServiceName = _context.Master_ChicCut_ServiceCategoryModel.Where(p => p.ServiceCategoryId == P_Groupby.Key).Select(p => p.ServiceName).FirstOrDefault(),
                                MinUnitPrice = P_Groupby.Min(p => p.Price.Value),
                                MaxUnitPrice = P_Groupby.Max(p => p.Price.Value)
                            }).FirstOrDefault();
                    details.Add(item);
                    #endregion
                }
                if (PreOrderId.HasValue && PreOrderId.Value > 0)//Sửa đơn hàng: + Chưa có dịch vụ đó: Thêm mới 
                {
                    //              + Có rồi: Update Qty, TT, Vốn, Sum..
                    #region //Sửa đơn hàng:
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var itemOrderDetailModel = _context.Daily_ChicCut_Pre_OrderDetailModel.Where(p => p.ServiceCategoryId == ServiceCategoryId && p.PreOrderId == PreOrderId).FirstOrDefault();
                        if (itemOrderDetailModel != null) // Update
                        {
                            //itemOrderDetailModel.COGS = itemOrderDetailModel.COGS;
                            //itemOrderDetailModel.UnitCOGS = itemOrderDetailModel.COGS * (itemOrderDetailModel.Qty + 1);
                            itemOrderDetailModel.MinUnitPrice = itemExist.MinPrice * (itemExist.Qty + 1);
                            itemOrderDetailModel.MaxUnitPrice = itemExist.MaxUnitPrice * (itemExist.Qty + 1);
                            itemOrderDetailModel.Qty++;
                            _context.Entry(itemOrderDetailModel).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                        }
                        else // Chưa có
                        {
                            #region Thêm mới
                            /* Tính giá vốn  */
                            //List<int> ProductIdLst = (from QMaster in _context.Master_ChicCut_QuantificationMasterModel
                            //                          join QDetail in _context.Master_ChicCut_QuantificationDetailModel on QMaster.QuantificationMasterId equals QDetail.QuantificationMasterId
                            //                          where QMaster.ServiceId == ServiceId
                            //                          select QDetail.ProductId.Value).ToList();
                            //var LstProductModel = _context.ProductModel.Where(p => ProductIdLst.Contains(p.ProductId)).ToList();
                            //decimal COGSCal = (LstProductModel != null && LstProductModel.Count > 0) ? LstProductModel.Sum(p => p.COGS.HasValue ? p.COGS : 0).Value : 0;
                            var ServiceLstModel = _context.Master_ChicCut_ServiceModel.Where(p => p.ServiceCategoryId == ServiceCategoryId).ToList();

                            var itemAdd = new Daily_ChicCut_Pre_OrderDetailModel()
                            {
                                PreOrderId = PreOrderId,
                                ServiceCategoryId = ServiceLstModel.Select(p => p.ServiceCategoryId).FirstOrDefault(),
                                // COGS = itemExist.COGS,
                                MinPrice = ServiceLstModel.Where(p => p.Price > 0).Min(p => p.Price),
                                MaxPrice = ServiceLstModel.Where(p => p.Price > 0).Max(p => p.Price),
                                Qty = 1,
                                // UnitCOGS = itemExist.COGS * (itemExist.Qty + 1),
                                MinUnitPrice = ServiceLstModel.Where(p => p.Price > 0).Min(p => p.Price),
                                MaxUnitPrice = ServiceLstModel.Where(p => p.Price > 0).Max(p => p.Price),
                            };

                            _context.Entry(itemAdd).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            #endregion
                        }
                        var model = _context.Daily_ChicCut_Pre_OrderModel.Where(p => p.PreOrderId == PreOrderId).FirstOrDefault();
                        if (model != null)// Cập nhật Master
                        {
                            var lstDetail = model.Daily_ChicCut_Pre_OrderDetailModel.ToList();
                            //model.SumCOGSOfOrderDetail = lstDetail.Sum(p => p.UnitCOGS.HasValue ? p.UnitCOGS.Value : 0);
                            model.MinSumPriceOfOrderDetail = lstDetail.Sum(p => p.MinUnitPrice.HasValue ? p.MinUnitPrice.Value : 0);
                            model.MaxSumPriceOfOrderDetail = lstDetail.Sum(p => p.MaxUnitPrice.HasValue ? p.MaxUnitPrice.Value : 0);
                            model.MinTotalBillDiscount = model.BillDiscount.HasValue ? ((model.BillDiscountTypeId == 1 ? model.BillDiscount : (model.BillDiscount / 100) * lstDetail.Sum(p => p.MinUnitPrice.HasValue ? p.MinUnitPrice.Value : 0))) : 0;
                            model.MaxTotalBillDiscount = model.BillDiscount.HasValue ? ((model.BillDiscountTypeId == 1 ? model.BillDiscount : (model.BillDiscount / 100) * lstDetail.Sum(p => p.MaxUnitPrice.HasValue ? p.MaxUnitPrice.Value : 0))) : 0;
                            model.MinTotal = model.MinSumPriceOfOrderDetail - model.MinTotalBillDiscount;
                            model.MaxTotal = model.MaxSumPriceOfOrderDetail - model.MaxTotalBillDiscount;
                            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                            ts.Complete();
                        }
                    }
                    #endregion
                }
            }


            return PartialView(details);
        }

        //DeleteInner

        #region _DeletelistInner
        public ActionResult _DeleteDetailInnerInfo(List<Daily_ChicCut_Pre_OrderDetailViewModel> details, int? ServiceCategoryId = null, int? PreOrderId = null)
        {
            if (details == null)
            {
                details = new List<Daily_ChicCut_Pre_OrderDetailViewModel>();
            }
            if (PreOrderId.HasValue && PreOrderId.Value > 0)//Sửa đơn hàng: Xoá record trong detail và update master:  Sum..
            {
                #region //Sửa đơn hàng:
                using (TransactionScope ts = new TransactionScope())
                {
                    var itemOrderDetailModel = _context.Daily_ChicCut_Pre_OrderDetailModel.Where(p => p.ServiceCategoryId == ServiceCategoryId && p.PreOrderId == PreOrderId).FirstOrDefault();
                    if (itemOrderDetailModel != null) // Update
                    {
                        _context.Entry(itemOrderDetailModel).State = System.Data.Entity.EntityState.Deleted;
                        _context.SaveChanges();
                        #region Update Master
                        var model = _context.Daily_ChicCut_Pre_OrderModel.Where(p => p.PreOrderId == PreOrderId).FirstOrDefault();
                        if (model != null)// Cập nhật Master
                        {
                            var lstDetail = model.Daily_ChicCut_Pre_OrderDetailModel.ToList();
                            //model.SumCOGSOfOrderDetail = lstDetail.Sum(p => p.UnitCOGS.HasValue ? p.UnitCOGS.Value : 0);
                            model.MinSumPriceOfOrderDetail = lstDetail.Sum(p => p.MinUnitPrice.HasValue ? p.MinUnitPrice.Value : 0);
                            model.MaxSumPriceOfOrderDetail = lstDetail.Sum(p => p.MaxUnitPrice.HasValue ? p.MaxUnitPrice.Value : 0);
                            model.MinTotalBillDiscount = model.BillDiscount.HasValue ? ((model.BillDiscountTypeId == 1 ? model.BillDiscount : (model.BillDiscount / 100) * lstDetail.Sum(p => p.MinUnitPrice.HasValue ? p.MinUnitPrice.Value : 0))) : 0;
                            model.MaxTotalBillDiscount = model.BillDiscount.HasValue ? ((model.BillDiscountTypeId == 1 ? model.BillDiscount : (model.BillDiscount / 100) * lstDetail.Sum(p => p.MaxUnitPrice.HasValue ? p.MaxUnitPrice.Value : 0))) : 0;
                            model.MinTotal = model.MinSumPriceOfOrderDetail - model.MinTotalBillDiscount;
                            model.MaxTotal = model.MaxSumPriceOfOrderDetail - model.MaxTotalBillDiscount;
                            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                        }
                        ts.Complete();
                        #endregion
                    }

                }
                #endregion
            }
            return PartialView("_DailyChicCutOrderDetailInnerInfo", details.Where(p => p.ServiceCategoryId != ServiceCategoryId).ToList());
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
        public ActionResult UpdateOrderCalcelOrderId(int PreOrderId)
        {
            try
            {
                var model = _context.Daily_ChicCut_Pre_OrderModel.Where(p => p.PreOrderId == PreOrderId).FirstOrDefault();
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
        public ActionResult UpdateOrderPay(int OrderId)
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
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var currentTime = DateTime.Now;
                        List<Daily_ChicCut_OrderDetailModel> detail = model.Daily_ChicCut_OrderDetailModel.ToList();

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

                        #region
                        /* Chi tiết định lượng : Qty = QDetail.Qty * Od.Qty,  Price = pdprice.Price * Od.Qty*/
                        var QDetailList = (
                            from Od in detail
                            join QMaster in _context.Master_ChicCut_QuantificationMasterModel on Od.ServiceId equals QMaster.ServiceId
                            join QDetail in _context.Master_ChicCut_QuantificationDetailModel on QMaster.QuantificationMasterId equals QDetail.QuantificationMasterId
                            join pd in _context.ProductModel on QDetail.ProductId equals pd.ProductId
                            //join pdprice in _context.ProductPriceModel on pd.ProductId equals pdprice.ProductId
                            where detail.Select(p => p.ServiceId).ToList().Contains(QMaster.ServiceId)
                            select new Master_ChicCut_QuantificationDetailViewModel()
                            {
                                ProductId = QDetail.ProductId,
                                Qty = QDetail.Qty * Od.Qty,
                                Price = 0//pdprice.Price * Od.Qty,
                            }).ToList();
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
                                    //COGS = 0,
                                    Price = item.Price,
                                    //ImportQty = 0, // số lượng nhập = 0
                                    ExportQty = item.Qty,
                                    //UnitCOGS = 0, //Tổng giá vốn = [COGS] * [ImportQty] = 0
                                    UnitPrice = item.Qty * item.Price, //[ExportQty] *[Price]
                                    EndInventoryQty = TonCuoiTrongHeThong + 0 - item.Qty //Tồn cuối = [BeginInventoryQty] + [ImportQty] -  [ExportQty]
                                };
                                _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                            }

                        }
                        #endregion

                        model.OrderStatusId = EnumDaily_ChicCut_OrderStatus.DaTinhTien;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        ts.Complete();
                        return Content("success");
                    }
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình cập nhật trạng thái");
            }
        }
        #endregion

        #endregion

        #region Lưu đơn hàng: Khi thêm mới
        public ActionResult SaveAddNewOrder(Daily_ChicCut_Pre_OrderModel model, List<Daily_ChicCut_Pre_OrderDetailViewModel> details = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var currentTime = DateTime.Now;
                        model.CreatedDate = currentTime;
                        // model.CreatedUserId = currentAccount.UserName;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();

                        //decimal SumCOGSOfOrderDetail = 0;
                        string ServicesName = "";
                        int count = 0;
                        #region Chi tiết đơn hàng
                        if (details != null)
                        {
                            foreach (var item in details)
                            {
                                count++;
                                ServicesName += item.ServiceName;
                                if (count < details.Count)
                                {
                                    ServicesName += " ; ";
                                }
                                Daily_ChicCut_Pre_OrderDetailModel itemAdd = new Daily_ChicCut_Pre_OrderDetailModel()
                                {
                                    ServiceCategoryId = item.ServiceCategoryId,
                                    // COGS = COGSCal,
                                    MinPrice = item.MinPrice,
                                    MaxPrice = item.MaxPrice,
                                    Qty = item.Qty,
                                    //UnitCOGS = COGSCal * item.Qty,
                                    MinUnitPrice = item.MinPrice * item.Qty,
                                    MaxUnitPrice = item.MaxPrice * item.Qty,
                                    PreOrderId = model.PreOrderId
                                };
                                _context.Entry(itemAdd).State = System.Data.Entity.EntityState.Added;
                            }
                        }
                        #endregion
                        // model.SumCOGSOfOrderDetail = SumCOGSOfOrderDetail;
                        model.ServicesName = ServicesName;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        model.PreOrderCode = (model.PreOrderId % 1000).ToString("D3");



                        #region // Gửi SMS cho khách hàng
                        SendSMSRepository SMSRepo = new SendSMSRepository();
                        string name = model.FullName.LastIndexOf(' ') > 0 ? model.FullName.Substring(model.FullName.LastIndexOf(' ') + 1) : model.FullName;
                        name = Library.ConvertToNoMarkString(name);
                        if (!string.IsNullOrEmpty(name))
                        {
                            name = name.First().ToString().ToUpper() + name.Substring(1).ToLower();
                        }
                        string mes = string.Format(
                                                    "Cam on {0} {1} dat hen tai Chic Cut, thoi gian hen la: {2:HH:mm dd/MM/yyyy}. {0} vui long den dung gio da hen.",
                                                    model.Gender.HasValue && model.Gender.Value ? "anh" : "chi",
                                                    name,
                                                    model.AppointmentTime
                                                    );
                        AY_SMSCalendar smsModel = new AY_SMSCalendar()
                        {
                            EndDate = DateTime.Now,
                            isSent = true,
                            NumberOfFailed = 0,
                            SMSContent = mes,
                            SMSTo = model.Phone
                        };
                        _context.Entry(smsModel).State = System.Data.Entity.EntityState.Added;
                        #endregion

                        _context.SaveChanges();

                        SMSRepo.SendSMSModel(smsModel);

                        _context.SaveChanges();
                        ts.Complete();
                        return Content("success");
                    }
                }
                else
                {
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
        public ActionResult SaveEditOrder(Daily_ChicCut_Pre_OrderModel model, int PreOrderId)
        {
            try
            {
                var modelUpdate = _context.Daily_ChicCut_Pre_OrderModel.Where(p => p.PreOrderId == PreOrderId).FirstOrDefault();
                if (modelUpdate != null)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        modelUpdate.Note = model.Note;
                        modelUpdate.AppointmentTime = model.AppointmentTime;
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
        public ActionResult _GetServicePartital(bool Gender)
        {
            var lst = (
                from p in _context.Master_ChicCut_ServiceModel
                join sc in _context.Master_ChicCut_ServiceCategoryModel on p.ServiceCategoryId equals sc.ServiceCategoryId
                where // p.Gender == Gender && 
                p.Actived == true &&
                p.Price > 0
                group p by p.ServiceCategoryId into P_Groupby // nhóm bảng nào  + theo cột nào + ra bảng mới
                orderby (P_Groupby.Key)
                select new Master_ChicCut_ServiceViewModel()
                {
                    ServiceCategoryId = P_Groupby.Key,
                    MinPrice = P_Groupby.Min(p => p.Price.Value),
                    MaxPrice = P_Groupby.Max(p => p.Price.Value),
                    ServiceName = _context.Master_ChicCut_ServiceCategoryModel.Where(p => p.ServiceCategoryId == P_Groupby.Key).Select(p => p.ServiceName).FirstOrDefault()
                }
                ).ToList();
            return PartialView(lst);
        }
        #endregion

        #region Helper
        private void CreateViewBag(int? HairTypeId = null, int? ServiceCategoryId = null)
        {
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
        }

        private void GetInfoCustomer(int CustomerId, Daily_ChicCut_Pre_OrderModel model)
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using Repository;
using ViewModels;

namespace WebUI.Controllers
{
    public class OrderController : BaseController
    {
        public ActionResult _Search()
        {
            //OrderSearchViewModel model = new OrderSearchViewModel();
            //model.CustomerNumber;
            //model.StatusId;
            var customer = from a in _context.AccountModel
                           join c in _context.CustomerModel on a.CustomerId equals c.CustomerId
                           where a.RolesId != (int)RolesEnum.Admin
                           orderby c.FullName
                           select new { FullName = c.FullName, CustomerId = c.CustomerId };
            ViewBag.CustomerId = new SelectList(customer.ToList(), "CustomerId", "FullName");
            var status = _context.StatusModel
                        .Where(p => p.StatusId != (int)Status.Huy && p.StatusId != (int)Status.TaoMoi)
                        .Select(p => new { StatusName = p.StatusName, StatusId = p.StatusId });
            ViewBag.StatusId = new SelectList(status.ToList(), "StatusId", "StatusName");
            return PartialView();
        }
        public ActionResult _List(OrderSearchViewModel model = null, bool isFirst = false)
        {
            ViewBag.isFirst = isFirst;
            ViewBag.CurrentRoleid = currentAccount.RolesId;
            if (currentAccount.RolesId == (int)RolesEnum.Admin)
            {
                var ord = _context.OrderModel.Where(p => p.StatusId != (int)Status.TaoMoi && p.StatusId != (int)Status.Huy);
                if (model != null)
                {
                    if (model.CustomerId.HasValue)
                    {
                        ord = ord.Where(p => p.CustomerId == model.CustomerId.Value);
                    }
                    if (model.StatusId.HasValue)
                    {
                        ord = ord.Where(p => p.StatusId == model.StatusId.Value);
                    }
                    if (model.FromDate.HasValue)
                    {
                        ord = ord.Where(p => model.FromDate.Value.CompareTo((p.OrderDate ?? DateTime.Now)) <= 0);
                    }
                    if (model.ToDate.HasValue)
                    {
                        ord = ord.Where(p => model.ToDate.Value.CompareTo((p.OrderDate ?? DateTime.Now)) >= 0);
                    }
                }
                return PartialView(ord.Select(p => new OrderViewModel()
                {
                    OrderId = p.OrderId,
                    TONTotal = p.TONTotal,
                    OrderDate = p.OrderDate,
                    StatusId = p.StatusId,
                    StatusName = p.StatusModel.StatusName,
                    ConfirmTONTotal = p.ConfirmTONTotal
                }).OrderByDescending(p => p.OrderId).ToList());
            }
            else
            {
                List<OrderViewModel> ord = new List<OrderViewModel>();
                ord = _context.OrderModel.Where(p => p.CustomerId == currentCustomer.CustomerId)
                                        .Select(p => new OrderViewModel()
                                        {
                                            OrderId = p.OrderId,
                                            TONTotal = p.TONTotal,
                                            OrderDate = p.OrderDate,
                                            StatusId = p.StatusId,
                                            StatusName = p.StatusModel.StatusName,
                                            ConfirmTONTotal = p.ConfirmTONTotal
                                        })
                .OrderByDescending(p => p.OrderId)
                .ToList();
                return PartialView(ord);
            }
        }

        #region Create
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Create()
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }

            if (checkSession() == false)
            {
                return null;
            }
            OrderViewModel orderModel = new OrderViewModel();

            orderModel.CustomerId = currentCustomer.CustomerId;
            orderModel.Phone = currentCustomer.Phone;
            orderModel.OrderDate = DateTime.Now.Date;

            CreateViewBag();
            ViewBag.isFirst = true;
            return View(orderModel);
        }

        private void CreateViewBag(OrderViewModel orderModel = null)
        {
            //orderModel.PaymentMethodId
            ViewBag.SelectPaymentMethodId = _context.PaymentMethodModel.Where(p => p.Actived == true).ToList();
        }

        public ActionResult _CreateList(List<OrderDetailViewModel> detail = null, bool isFirst = false)
        {
            if (isFirst)
            {
                detail = new List<OrderDetailViewModel>();
                OrderDetailViewModel newModel = new OrderDetailViewModel();
                newModel.DeliveryDate = DateTime.Now.AddDays(7);
                newModel.SteelMarkId = 1;
                newModel.SteelFIId = 1;
                detail.Add(newModel);
                //OrderDetailViewModel newModel2 = new OrderDetailViewModel();
                //newModel2.DeliveryDate = DateTime.Now.AddDays(7);
                //newModel2.SteelMarkId = 1;
                //newModel2.SteelFIId = 2;
                //detail.Add(newModel2);
                //OrderDetailViewModel newModel3 = new OrderDetailViewModel();
                //newModel3.DeliveryDate = DateTime.Now.AddDays(7);
                //newModel3.SteelMarkId = 2;
                //newModel3.SteelFIId = 3;
                //detail.Add(newModel3);
                //OrderDetailViewModel newModel4 = new OrderDetailViewModel();
                //newModel4.DeliveryDate = DateTime.Now.AddDays(7);
                //newModel4.SteelMarkId = 2;
                //newModel4.SteelFIId = 4;
                //detail.Add(newModel4);
                //OrderDetailViewModel newModel5 = new OrderDetailViewModel();
                //newModel5.DeliveryDate = DateTime.Now.AddDays(7);
                //newModel5.SteelMarkId = 3;
                //newModel5.SteelFIId = 5;
                //detail.Add(newModel5);
                //OrderDetailViewModel newModel6 = new OrderDetailViewModel();
                //newModel6.DeliveryDate = DateTime.Now.AddDays(7);
                //newModel6.SteelMarkId = 3;
                //newModel6.SteelFIId = 6;
                //detail.Add(newModel6);
            }
            CreateDetailViewBag();
            return PartialView(detail);
        }

        public ActionResult _CreatelistInner(List<OrderDetailViewModel> detail)
        {
            if (detail == null)
            {
                detail = new List<OrderDetailViewModel>();
            }
            OrderDetailViewModel newModel = new OrderDetailViewModel();
            newModel.DeliveryDate = DateTime.Now.AddDays(7);
            detail.Add(newModel);
            //Tính tổng
            double SumToal = 0;
            foreach (var item in detail)
            {
                SumToal += item.TONQty ?? 0;
            }
            ViewBag.SumTotal = SumToal;
            CreateDetailViewBag();
            return PartialView(detail);
        }
        public ActionResult _Delete(int id, List<OrderDetailViewModel> detail)
        {
            if (detail == null)
            {
                detail = new List<OrderDetailViewModel>();
            }
            detail = detail.Where(p => p.STT != id).ToList();
            //Tính tổng
            double SumToal = 0;
            foreach (var item in detail)
            {
                SumToal += item.TONQty ?? 0;
            }
            ViewBag.SumTotal = SumToal;
            CreateDetailViewBag();
            return PartialView("_CreatelistInner", detail);
        }

        private void CreateDetailViewBag()
        {
            //Danh sách Mark
            ViewBag.SelectSteelMark = _context.SteelMarkModel.Where(p => p.Actived == true).ToList();
            //Danh sách Pi Theo Mark
            SteelFIReposytory _repository = new SteelFIReposytory(_context);
            ViewBag.SelectSteelFI = _repository.GetALL();
        }

        public ActionResult Save(OrderViewModel orderModel, List<OrderDetailViewModel> detail, bool isSend = false)
        {
            if (detail == null || detail.Count == 0)
            {
                return Content("Vui lòng nhập thông tin sản phẩm!");
            }
            double TONTotal = 0;
            foreach (var item in detail)
            {
                TONTotal += item.TONQty ?? 0;
            }
            ViewBag.SumTotal = TONTotal;
            if (detail != null && detail.Count > 0)
            {
                try
                {
                    OrderModel model = new OrderModel()
                    {
                        TONTotal = TONTotal,
                        OrderDate = orderModel.OrderDate,
                        CustomerName = currentCustomer.FullName,
                        EnterpriseName = orderModel.EnterpriseName,
                        Phone = orderModel.Phone,
                        Fax = orderModel.Fax,
                        ReceivedPerson = orderModel.ReceivedPerson,
                        NumberOfTrucks = orderModel.NumberOfTrucks,
                        ReceivedAddress = orderModel.ReceivedAddress,
                        PaymentMethodId = orderModel.PaymentMethodId,
                        RejectReason = "",
                        Note = orderModel.Note,
                        StatusId = (int)Repository.Status.TaoMoi
                    };
                    //20160407-2.khi lưu, dòng nào không có thông tin liên quan đến cuộn/bó, Tấn thì không Insert vào database vì sẽ tạo rác
                    var list = detail.Where(p => !((p.RollQty == null || p.RollQty == 0) && (p.TONQty == null || p.TONQty == 0))).ToList();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            //Nam: xử lý vụ trùng mác thép, phi thép, ngày giao thì không cho lưu (khác ngày giao thì OK)    
                            #region
                            if (model.OrderDetailModel.Where(p => p.DeliveryDate == item.DeliveryDate && p.SteelMarkId == item.SteelMarkId && p.SteelFIId == item.SteelFIId).FirstOrDefault() != null)
                            {
                                return Content("Vui lòng không nhập chi tiết đơn hàng có Mác thép, Phi thép và ngày giao trùng nhau!");    
                            }
                            #endregion
                            OrderDetailModel a = new OrderDetailModel()
                            {
                                SteelMarkId = item.SteelMarkId,
                                SteelFIId = item.SteelFIId,
                                RollQty = item.RollQty,
                                TONQty = item.TONQty,
                                DeliveryDate = item.DeliveryDate,
                                Note = item.Note
                            };
                            model.OrderDetailModel.Add(a);
                        }
                        _context.Entry(model).State = System.Data.EntityState.Added;
                        _context.SaveChanges();
                    }
                    else
                    {
                        return Content("Không thành công! Vui lòng nhập danh sách sản phẩm và không để trống số lượng cuộn/bó hoặc tấn!");
                    }
                    if (isSend)
                    {
                        model.StatusId = (int)Repository.Status.DaGui;
                        _context.Entry(model).State = System.Data.EntityState.Modified;
                        _context.SaveChanges();
                    }
                    return Content("success");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            return View();
        }

        #endregion

        #region View
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult View(int id)
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }

            if (checkSession() == false)
            {
                return null;
            }

            OrderRepository _repository = new OrderRepository(_context);
            OrderModel order = _repository.Find(id, currentCustomer.CustomerId, currentAccount.RolesId);

            //status khac send => co confirm
            //#region 
            //if (order != null && order.StatusId != (int)Repository.Status.TaoMoi)
            //{
            //    OrderViewModel modelConfirm;
            //    List<OrderDetailViewModel> detailConfirm;
            //    _repository.GetInfo(order.OrderId, out modelConfirm, out detailConfirm);

            //    if (order.StatusId != modelConfirm.StatusId)
            //    {
            //        var updateStatus = _context.StatusModel.Find(modelConfirm.StatusId);
            //        if (updateStatus != null)
            //        {
            //            order.StatusId = modelConfirm.StatusId;
            //            order.RejectReason = modelConfirm.RejectReason;
            //            order.ConfirmTONTotal = modelConfirm.ConfirmTONTotal;
            //            order.SalesDocumentNumber = modelConfirm.SalesDocumentNumber;
            //            order.ConfirmDate = modelConfirm.ConfirmDate;
            //            _context.Entry(order).State = System.Data.EntityState.Modified;
            //            _context.SaveChanges();
            //        }
            //    }
            //    var checkHasModify = false;
            //    foreach (var item in detailConfirm)
            //    {
            //        if ((item.ConfirmDate != null || item.ConfirmRollQty != null || item.ConfirmTONQty != null))
            //        {
            //            var detail = _context.OrderDetailModel.Find(item.OrderDetailId);
            //            detail.ConfirmDate = item.ConfirmDate;
            //            detail.ConfirmRollQty = item.ConfirmRollQty;
            //            detail.ConfirmTONQty = item.ConfirmTONQty;
            //            _context.Entry(detail).State = System.Data.EntityState.Modified;
            //            checkHasModify = true;
            //        }
            //    }
            //    if (checkHasModify)
            //    {
            //        _context.SaveChanges();
            //    }
            //}
            //#endregion
            if (order == null)
            {
                return View("Error");
            }

            CreateViewBagForView(order);
            return View(order);
        }
        private void CreateViewBagForView(OrderModel order = null)
        {
            if (order == null)
            {
                order = new OrderModel();
            }
            //orderModel.PaymentMethodId
            ViewBag.PaymentMethodId = new SelectList(_context.PaymentMethodModel.Where(p => p.Actived == true).ToList(), "PaymentMethodId", "PaymentMethodName", order.PaymentMethodId);
            ViewBag.SumTotal = order.TONTotal;
        }

        public ActionResult _ViewList(List<OrderDetailModel> model, double? SumTotal)
        {
            ViewBag.SumTotal = SumTotal;
            return PartialView(model);
        }
        #endregion

        #region Edit
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Edit(int id)
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }

            if (checkSession() == false)
            {
                return null;
            }
            OrderRepository _repository = new OrderRepository(_context);
            OrderViewModel order = _repository.FindView(id, currentCustomer.CustomerId);
            
            if (order == null)
            {
                return View("Error");
            }
            else
            {
                List<OrderDetailViewModel> detail = new List<OrderDetailViewModel>();
                detail = _repository.FindOrderDetailViewModel(id, currentCustomer.CustomerId);
                ViewBag.Detail = detail;
            }

            CreateViewBag(); 
            return View(order);
        }

        public ActionResult Update(OrderViewModel orderModel, List<OrderDetailViewModel> detail, bool isSend = false)
        {
            if (detail == null || detail.Count == 0)
            {
                return Content("Vui lòng nhập thông tin sản phẩm!");
            }
            double TONTotal = 0;
            foreach (var item in detail)
            {
                TONTotal += item.TONQty ?? 0;
            }
            ViewBag.SumTotal = TONTotal;
            if (detail != null && detail.Count > 0)
            {
                try
                {
                    OrderModel model = _context.OrderModel.Include("OrderDetailModel").Where(p => p.OrderId == orderModel.OrderId).FirstOrDefault();
                    if ( model.OrderDetailModel != null &&  model.OrderDetailModel.Count > 0)
                    {
                        while (model.OrderDetailModel.Count > 0)
                        {
                            _context.Entry(model.OrderDetailModel.First()).State = System.Data.EntityState.Deleted;
                        }
                    }

                    model.TONTotal = TONTotal;
                    model.OrderDate = orderModel.OrderDate;
                    model.CustomerName = currentCustomer.FullName;
                    model.CustomerId = currentCustomer.CustomerId;
                    model.EnterpriseName = orderModel.EnterpriseName;
                    model.Phone = orderModel.Phone;
                    model.Fax = orderModel.Fax;
                    model.ReceivedPerson = orderModel.ReceivedPerson;
                    model.NumberOfTrucks = orderModel.NumberOfTrucks;
                    model.ReceivedAddress = orderModel.ReceivedAddress;
                    model.PaymentMethodId = orderModel.PaymentMethodId;
                    model.Note = orderModel.Note;
                    //model.RejectReason = "";
                    model.StatusId = (int)Repository.Status.TaoMoi;
                    _context.Entry(model).State = System.Data.EntityState.Modified;
                    //_context.SaveChanges();

                    //20160407-2.khi lưu, dòng nào không có thông tin liên quan đến cuộn/bó, Tấn thì không Insert vào database vì sẽ tạo rác
                    var list = detail.Where(p => !((p.RollQty == null || p.RollQty == 0) && (p.TONQty == null || p.TONQty == 0))).ToList();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            //Nam: xử lý vụ trùng mác thép, phi thép, ngày giao thì không cho lưu (khác ngày giao thì OK)    
                            #region
                            if (model.OrderDetailModel.Where(p => p.DeliveryDate == item.DeliveryDate && p.SteelMarkId == item.SteelMarkId && p.SteelFIId == item.SteelFIId).FirstOrDefault() != null)
                            {
                                return Content("Vui lòng không nhập chi tiết đơn hàng có Mác thép, Phi thép và ngày giao trùng nhau!");
                            }
                            #endregion

                            OrderDetailModel a = new OrderDetailModel()
                            {
                                OrderId = orderModel.OrderId,
                                SteelMarkId = item.SteelMarkId,
                                SteelFIId = item.SteelFIId,
                                RollQty = item.RollQty,
                                TONQty = item.TONQty,
                                DeliveryDate = item.DeliveryDate,
                                Note = item.Note
                            };
                            _context.Entry(a).State = System.Data.EntityState.Added;
                        }
                        _context.SaveChanges();
                    }
                    else
                    {
                        return Content("Không thành công! Vui lòng nhập danh sách sản phẩm và không để trống số lượng cuộn/bó hoặc tấn!");
                    }

                    if (isSend)
                    {
                        _context.Entry(model).State = System.Data.EntityState.Modified;
                        _context.SaveChanges();
                    }
                    return Content("success");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            return View();
        }
        #endregion

        #region Cancel order
        [HttpPost]
        public ActionResult CancelOrder(int id)
        {
            if (checkSession() == false)
            {
                return Content("Error! Session has expired");
            }
            OrderRepository _repository = new OrderRepository(_context);
            OrderModel order = _repository.Find(id, currentCustomer.CustomerId,currentAccount.RolesId);

            if (order == null)
            {
                return Content("Error! Could not found this order!");
            }
            else
            {
                try
                {
                    if (order.StatusId == (int)Repository.Status.TaoMoi)
                    {
                        order.StatusId = (int)Repository.Status.Huy;
                        _context.Entry(order).State = System.Data.EntityState.Modified;
                        _context.SaveChanges();
                        return Content("success");
                    }
                    else
                    {
                        return Content("Chỉ có thể hủy đơn hàng ở trạng thái Tạo Mới!");
                    }
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
        }
        #endregion

        public ActionResult Send(int id)
        {
            try
            {
                try
                {
                    var model = _context.OrderModel.Include("OrderDetailModel").Where(p => p.OrderId == id && p.CustomerId == currentCustomer.CustomerId).FirstOrDefault();
                    if (model != null && model.StatusId == (int)Repository.Status.TaoMoi)
                    {
                        if (model.OrderDetailModel == null || model.OrderDetailModel.Count == 0)
                        {
                            return Content("Vui lòng nhập thông tin sản phẩm!");
                        }

                        model.StatusId = (int)Repository.Status.DaGui;
                        _context.Entry(model).State = System.Data.EntityState.Modified;
                        _context.SaveChanges();

                        return Content("success");

                    }
                    return Content("Không tìm thấy");
                }
                catch (Exception ex)
                {

                    return Content(ex.Message);
                }
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }
        }
                       
    }
}

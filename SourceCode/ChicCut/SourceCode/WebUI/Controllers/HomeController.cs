using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;
using System.Data.SqlClient;
using System.Data.Entity;

namespace WebUI.Controllers
{
    public class HomeController : BaseController
    {
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            CustomerModel customer = (CustomerModel)Session["customer"];
            ViewBag.currentAccount = currentAccount;

            #region Cánh báo tồn kho
            if (Session["PopupQtyAlert"] == null || (string)Session["PopupQtyAlert"] == "Show")
            {
                SqlParameter paramRolesId = new SqlParameter("@RolesId", CurrentUser.RolesId);
                var lst = _context.Database.
                                           SqlQuery<ProductAlertViewModel>("dbo.usp_TonKhoCanhBaoHienThiTrangChu @RolesId", paramRolesId)
                                           .ToList();
                ViewBag.lstQtyAlert = lst;
                ViewBag.PopupQtyAlert = (lst != null && lst.Count > 0) ? "Show" : "Hide"; // không/có sản phẩm cần cảnh báo
            }
            else
            {
                ViewBag.PopupQtyAlert = "Hide";
            }
            #endregion

            return View(customer);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Tooltip()
        {
            return PartialView(_context.StatusModel.ToList());
        }
        public ActionResult Alert(string Content)
        {
            ViewBag.Content = Content;
            return PartialView();
        }
        #region Notifi
        public ActionResult _GetNewNotification()
        {
            var curenttime = DateTime.Now.Date;
            var lst = (from p in _context.NotificationModel
                       join emp in _context.EmployeeModel on p.AccountId equals emp.EmployeeId
                       where ((p.EffectDate == curenttime || p.EffectDate == null) && (p.Actived == true))
                       orderby p.NotificationId descending
                       select new NotificationViewModel()
                       {
                           UserName = emp.FullName,
                           Note = p.Note,
                           CreateDate = p.CreateDate,
                           NotificationId = p.NotificationId
                       }).ToList();

            //_context.NotificationModel.Where(p => p.Actived == true).OrderByDescending(p => p.NotificationId).ToList();
            //Danh sách các đơn hàng mới
            if (currentAccount.RolesId == Constant.EnumRoles.DEV
                || currentAccount.RolesId == Constant.EnumRoles.NQL
                || currentAccount.RolesId == Constant.EnumRoles.NVTN)
            {
                var lst2 = (from po in _context.Daily_ChicCut_Pre_OrderModel
                            where po.OrderStatusId == 5 && DbFunctions.TruncateTime(po.AppointmentTime) == curenttime
                            select new NotificationViewModel()
                            {
                                NotificationId = po.PreOrderId,
                                Note = "Có đơn hàng mới " + po.PreOrderCode,
                                Url = "",
                                CreateDate = po.CreatedDate
                            }).ToList();
                lst.AddRange(lst2);
            }
            return PartialView(lst);
        }
        public ActionResult _NotifiDetail(int id)
        {
            //var model = (from p in _context.NotificationModel
            //             join emp in _context.EmployeeModel on p.AccountId equals emp.EmployeeId
            //             where (p.Actived == true && p.NotificationId == id)
            //             orderby p.NotificationId descending
            //             select new NotificationViewModel()
            //             {
            //                 UserName = emp.FullName,
            //                 Note = p.Note,
            //                 CreateDate = p.CreateDate,
            //                 NotificationId = p.NotificationId
            //             }).FirstOrDefault();
            var model = _context.Daily_ChicCut_Pre_OrderModel.Where(p => p.OrderStatusId == 5 && p.PreOrderId == id).FirstOrDefault();

            return Json(new { SearchPreOrderCode = model.PreOrderCode }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NotifiSeen(int id)
        {
            try
            {
                var model = _context.NotificationModel.Where(p => p.Actived == true && p.NotificationId == id).FirstOrDefault();
                if (model != null)
                {
                    model.Actived = false;
                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                }
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 
        public ActionResult SetNoDisplayQtyAlert()
        {
            Session["PopupQtyAlert"] = "Hide";
            return Json(true,JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}

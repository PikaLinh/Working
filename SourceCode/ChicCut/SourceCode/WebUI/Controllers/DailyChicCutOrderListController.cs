using Constant;
using EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace WebUI.Controllers
{
    public class DailyChicCutOrderListController : BaseController
    {
        // GET: DailyChicCutOrderList
        public ActionResult Index(ChicCutOrderSeachViewModel model, int PageIndex = 1, int PageSize = 10)
        {
            //Tạo câu querry
            //Nếu dùng ajax
            //fillter theo điều kiện
            if (Request.IsAjaxRequest())
            {
                var result = from o in _context.Daily_ChicCut_OrderModel
                             join ca in _context.AccountModel on o.CashierUserId equals ca.UserId
                             join cashier in _context.EmployeeModel on ca.EmployeeId equals cashier.EmployeeId
                             join staffTmp in _context.EmployeeModel on o.StaffId equals staffTmp.EmployeeId into staffList
                             from staff in staffList.DefaultIfEmpty()
                             where (model.StaffId == null || model.StaffId == o.StaffId) &&
                                (model.CashierUserId == null || model.CashierUserId == o.CashierUserId) &&
                                (model.FromDate == null || model.FromDate <= o.CashierDate) &&
                                (model.ToDate == null || model.ToDate >= o.CashierDate) &&
                                o.OrderStatusId == EnumDaily_ChicCut_OrderStatus.DaTinhTien
                                orderby o.OrderId descending
                             select new ChicCutOrderViewModel()
                             {
                                 OrderId = o.OrderId,
                                 FullName = o.FullName,
                                 Phone = o.Phone,
                                 CashierName = cashier.FullName,
                                 StaffName = staff.FullName,
                                 CreatedDate = o.CreatedDate,
                                 CashierDate = o.CashierDate,
                                 SumPriceOfOrderDetail = o.SumPriceOfOrderDetail,
                                 TotalBillDiscount = o.TotalBillDiscount,
                                 Total = o.Total
                             };
                //return PartialView("_Index", result.ToList());
                result = result.OrderByDescending(p => p.CashierDate);
                int rows = result.Count();
                ViewBag.TotalRow = rows;
                ViewBag.RowIndex = (PageIndex - 1) * PageSize;
                return PartialView("_Index", result.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
            }
            else
            {
                var result = from o in _context.Daily_ChicCut_OrderModel
                             join ca in _context.AccountModel on o.CashierUserId equals ca.UserId
                             join cashier in _context.EmployeeModel on ca.EmployeeId equals cashier.EmployeeId
                             join staffTmp in _context.EmployeeModel on o.StaffId equals staffTmp.EmployeeId into staffList
                             from staff in staffList.DefaultIfEmpty()
                             where o.OrderStatusId == EnumDaily_ChicCut_OrderStatus.DaTinhTien
                             orderby o.OrderId descending
                             select new ChicCutOrderViewModel()
                             {
                                 OrderId = o.OrderId,
                                 FullName = o.FullName,
                                 Phone = o.Phone,
                                 CashierName = cashier.FullName,
                                 StaffName = staff.FullName,
                                 CreatedDate = o.CreatedDate,
                                 CashierDate = o.CashierDate,
                                 SumPriceOfOrderDetail = o.SumPriceOfOrderDetail,
                                 TotalBillDiscount = o.TotalBillDiscount,
                                 Total = o.Total
                             };
                CreateViewBag();
                //return View(result.ToList());
                result = result.OrderByDescending(p => p.CashierDate);

                ViewBag.TotalRow = result.Count();
                ViewBag.RowIndex = (PageIndex - 1) * PageSize;
                return View(result.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
            } 
        }
        public ActionResult detail(int OrderId)
        {
            ChicCutOrderViewModel result = (from o in _context.Daily_ChicCut_OrderModel
                                            join ca in _context.AccountModel on o.CashierUserId equals ca.UserId
                                            join cashier in _context.EmployeeModel on ca.EmployeeId equals cashier.EmployeeId
                                            join pay in _context.PaymentMethodModel on o.PaymentMethodId equals pay.PaymentMethodId
                                            join staffTmp in _context.EmployeeModel on o.StaffId equals staffTmp.EmployeeId into staffList
                                            from staff in staffList.DefaultIfEmpty()
                                            where o.OrderId == OrderId
                                            select new ChicCutOrderViewModel()
                                            {
                                                OrderId = o.OrderId,
                                                FullName = o.FullName,
                                                Phone = o.Phone,
                                                CashierName = cashier.FullName,
                                                StaffName = staff.FullName,
                                                CreatedDate = o.CreatedDate,
                                                CashierDate = o.CashierDate,
                                                SumPriceOfOrderDetail = o.SumPriceOfOrderDetail,
                                                TotalBillDiscount = o.TotalBillDiscount,
                                                Total = o.Total,
                                                HairStyle = o.Master_ChicCut_HairTypeModel.HairTypeName,
                                                Gender = o.Gender,
                                                PaymentMethod = pay.PaymentMethodName
                                            }).FirstOrDefault();
            if (result != null)
            {
                result.details = (from d in _context.Daily_ChicCut_OrderDetailModel
                                  where d.OrderId == result.OrderId
                                  select new ChicCutOrderDetailViewModel()
                                  {
                                      COGS = d.COGS,
                                      Qty = d.Qty,
                                      ServiceName = d.Master_ChicCut_ServiceModel.ServiceName,
                                      Price = d.Price,
                                      UnitPrice = d.UnitPrice,

                                  }).ToList();
                #region #Add List Product vào
                var LstProductDetail =
               _context.Daily_ChicCut_OrderProductDetailModel.Where(p => p.OrderId == result.OrderId).Select(p => new ChicCutOrderDetailViewModel()
               {
                   COGS = p.COGS,
                   Price = p.Price,
                   Qty = p.Qty,
                   UnitPrice = p.UnitPrice,
                   ServiceName = _context.ProductModel.Where(pd => pd.ProductId == p.ProductId).Select(pd => pd.ProductName).FirstOrDefault()
               }).ToList();
                result.details.AddRange(LstProductDetail); // Add Lst Product vào ListService
            }
                #endregion
            return Json(new
            {
                Code = System.Net.HttpStatusCode.Created,
                Success = true,
                Data = result
            }); ;
        }

        #region//Sửa phương thức thanh toán
        public ActionResult Edit(int OrderId)
        {
            ChicCutOrderViewModel result = (from o in _context.Daily_ChicCut_OrderModel
                                            where o.OrderId == OrderId
                                            select new ChicCutOrderViewModel()
                                            {
                                                OrderId = o.OrderId,
                                                PaymentMethodId = o.PaymentMethodId
                                            }).FirstOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(int OrderId, int PaymentMethodId)
        {
            Daily_ChicCut_OrderModel orderModel = _context.Daily_ChicCut_OrderModel.Where(p => p.OrderId == OrderId).FirstOrDefault();
            if (orderModel != null)
            {
                orderModel.PaymentMethodId = PaymentMethodId;
                _context.Entry(orderModel).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        //PartialView chứa modal sửa phương thức thanh toán
        public ActionResult EditPaymentMethod(int OrderId, int PaymentMethodId)
        {
            //Phương thức thanh toán
            var PaymentMethodLst = _context.PaymentMethodModel.Where(p => p.Actived == true).ToList();
            ViewBag.PaymentMethodId = new SelectList(PaymentMethodLst, "PaymentMethodId", "PaymentMethodName", PaymentMethodId);
            ViewBag.OrderId = OrderId;
            return PartialView();
        }
        #endregion
        private void CreateViewBag(int? StaffId = null, int? CashierUserId = null)
        {
            ViewBag.FromDate = DateTime.Now.Date;
            ViewBag.ToDate = DateTime.Now.Date.AddDays(1).AddMinutes(-1);

            var StaffList = from a in _context.AccountModel
                            join e in _context.EmployeeModel on a.EmployeeId equals e.EmployeeId
                            where a.RolesId == Constant.EnumRoles.NVPV
                            select new { e.EmployeeId, e.FullName };

            var CashUserList = from a in _context.AccountModel
                               join e in _context.EmployeeModel on a.EmployeeId equals e.EmployeeId
                               where a.RolesId == Constant.EnumRoles.NVTN || a.RolesId == Constant.EnumRoles.NQL
                               select new { a.UserId, e.FullName };

            ViewBag.StaffId = new SelectList(StaffList, "EmployeeId", "FullName", StaffId);
            ViewBag.CashierUserId = new SelectList(CashUserList, "UserId", "FullName", CashierUserId);
        }

        public ActionResult CancelDailyChicCutOrder(int OrderId)
        {
            Daily_ChicCut_OrderModel model = _context.Daily_ChicCut_OrderModel
                                             .Where(p => p.OrderId == OrderId)
                                             .FirstOrDefault();
            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy đơn hàng yêu cầu !";
            }
            else if (model.OrderStatusId == EnumDaily_ChicCut_OrderStatus.DaTinhTien)
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_Daily_ChicCut_Order_Canceled";
                        cmd.Parameters.AddWithValue("@OrderId", model.OrderId);
                        cmd.Parameters.AddWithValue("@CanceledDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CanceledUserId", CurrentUser.UserId);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                Resuilt = "success";
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
    }
}
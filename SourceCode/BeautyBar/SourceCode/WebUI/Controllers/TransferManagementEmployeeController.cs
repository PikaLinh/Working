using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using EntityModels;
using System.Transactions;

namespace WebUI.Controllers
{
    public class TransferManagementEmployeeController : BaseController
    {
        // GET: TransferManagementEmployee
        public ActionResult Index(int? EmployeeCurrentId = 0)
        {
            EmployeeCurrentId = EmployeeCurrentId != 0 ? EmployeeCurrentId : currentEmployee.EmployeeId;
            ViewBag.EmployeeCurrentId = EmployeeCurrentId;
            ViewBag.EmployeeCurrentName = _context.EmployeeModel.Where(p => p.EmployeeId == EmployeeCurrentId).Select(p => (p.FullName + (string.IsNullOrEmpty(p.Phone) ? "" : " - " + p.Phone))).FirstOrDefault();
            return View();
        }
        public ActionResult Save(TransferManagermentEmployeeViewModel model, List<int> ListCustomerId = null)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        CustomerModel cusModel;
                        foreach (var item in ListCustomerId)
                        {
                            cusModel = _context.CustomerModel.Where(p => p.EmployeeId == model.EmployeeCurrentId && p.CustomerId == item).FirstOrDefault();
                            cusModel.EmployeeId = model.EmployeeNewId;
                            _context.Entry(cusModel).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                        }

                        ts.Complete();
                        return Content("success");
                    }
                    catch
                    {
                        return Content("Xảy ra lỗi trong quá trình thêm mới đơn hàng");
                    }
                }
            }
            else
            {
                return Content("Vui lòng kiểm tra lại thông tin không hợp lệ");
            }
        }

        public ActionResult _CustomerList(int EmployeeId = 0)
        {
            var lst = _context.CustomerModel.Where(p => p.EmployeeId == EmployeeId).ToList();
            return PartialView(lst);
        } 

        public ActionResult GetEmployeeId(string q)
        {
            var data = _context
                       .EmployeeModel
                       .Where(p => q == null || (p.FullName + (string.IsNullOrEmpty(p.Phone) ? "" :  " - " + p.Phone)).Contains(q))
                       .Select(p => new
                       {
                           value = p.EmployeeId,
                           text = (p.FullName + (string.IsNullOrEmpty(p.Phone) ? "" :  " - " + p.Phone))
                       })
                       .Take(10)
                       .ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
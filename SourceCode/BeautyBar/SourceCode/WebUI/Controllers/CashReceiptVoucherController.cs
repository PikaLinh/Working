using Constant;
using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class CashReceiptVoucherController : BaseController
    {
        //
        // GET: /CashReceiptVoucher/
        #region Danh sách phiếu thu - chi tiền mặt

        
        public ActionResult Index()
        {
            //checkSession();
            CreateViewSearchBag();
            var lst = _context.AM_TransactionModel
                              .Where(p => p.AMAccountId == EnumAM_Account.TIENMAT)
                              .OrderByDescending(p => p.TransactionId)
                              .ToList();
            return View(lst);
        }

        public ActionResult _SearchImportMaster(AM_TransactionSearchViewModel model)
        {
            //Danh sách
            List<AM_TransactionInfoViewModel> list = new List<AM_TransactionInfoViewModel>();

            if (model.ToDate.HasValue)
            {
                model.ToDate.Value.AddDays(1);
            }
            //Tim kiếm
            list = (from p in _context.AM_TransactionModel
                    join sm in _context.StoreModel on p.StoreId equals sm.StoreId 
                    join tm in _context.AM_TransactionTypeModel on p.TransactionTypeCode equals tm.TransactionTypeCode
                    join cm in _context.AM_ContactItemTypeModel on p.ContactItemTypeCode equals cm.ContactItemTypeCode
                    join em in _context.EmployeeModel on p.CreateEmpId equals em.EmployeeId
                    join am in _context.AM_AccountModel on p.AMAccountId equals am.AMAccountId
                    where (model.StoreId == null || p.StoreId == model.StoreId) &&
                    //(model.TransactionTypeCode == null || tm.TransactionTypeCode == model.TransactionTypeCode) &&
                    (model.ContactItemTypeCode == null || cm.ContactItemTypeCode == model.ContactItemTypeCode) &&
                    (model.FromDate == null || p.CreateDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
                    (model.ToDate == null || p.CreateDate.Value.CompareTo(model.ToDate.Value) <= 0) &&
                    (model.FromTotalPrice == null || p.Amount >= model.FromTotalPrice) &&
                    (model.ToTotalPrice == null || p.Amount <= model.ToTotalPrice) &&
                    (am.AMAccountTypeCode == EnumAM_AccountType.TIENMAT) &&
                    (model.Isimport == null || model.Isimport == tm.isImport)

                    select new AM_TransactionInfoViewModel()
                    {
                        StoreName = sm.ShortName,
                        CreateDate = p.CreateDate,
                        TransactionTypeName = tm.TransactionTypeName,
                        ContactItemTypeName = cm.ContactItemTypeName,
                        Amount = p.Amount,
                        Note = p.Note,
                        IsImport = tm.isImport.Value,
                        TransactionId = p.TransactionId,
                        EmployeeName = em.FullName
                    })
                    .OrderByDescending(p => p.CreateDate)
                    .ToList();
            return PartialView(list);
        }

        private void CreateViewSearchBag(int? StoreId = null, string TransactionTypeCode = null, string ContactItemTypeCode = null)
        {
            //1. StoreId
            var StoreIdList = _context.StoreModel.OrderBy(p => p.StoreName).ToList();
            ViewBag.StoreId = new SelectList(StoreIdList, "StoreId", "StoreName", StoreId);

            //2. TransactionTypeCode
            var TransactionTypeCodeList = _context.AM_TransactionTypeModel
                                                 .OrderBy(p => p.TransactionTypeName)
                                                 .Where(p => p.TransactionTypeCode == EnumTransactionType.TCCHI || p.TransactionTypeCode == EnumTransactionType.TCTHU)
                                                 .ToList();
            ViewBag.TransactionTypeCode = new SelectList(TransactionTypeCodeList, "TransactionTypeCode", "TransactionTypeName", TransactionTypeCode);

            //3. ContactItemTypeCode
            var ContactItemTypeCodeList = _context.AM_ContactItemTypeModel.OrderBy(p => p.OrderBy).ToList();
            ViewBag.ContactItemTypeCode = new SelectList(ContactItemTypeCodeList, "ContactItemTypeCode", "ContactItemTypeName", ContactItemTypeCode);

        }

        #endregion

        #region Thêm mới phiếu thu chi
        [HttpGet]
        
        public ActionResult Create()
        {
         
            CreateViewSearchBag();
            AM_TransactionModel model = new AM_TransactionModel();
            return View(model);
        }

        
        public ActionResult Save(AM_TransactionModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var currentTime = DateTime.Now;
                        decimal? DebtOld = null;// nợ cũ
                        int? CustomerIdCreate = null, SupplierIdCreate = null;
                        model.AMAccountId = (_context.AM_AccountModel.Where(p => p.AMAccountTypeCode == EnumAM_AccountType.TIENMAT && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault();
                        model.CreateDate = currentTime;
                        model.CreateEmpId = currentEmployee.EmployeeId;
                        
                        
                        int dau; // Xét dấu (Thu hay chi tiền )
                        if (model.TransactionTypeCode.Equals(EnumTransactionType.TCCHI))
                        {
                            dau = model.ContactItemTypeCode == "NCC" ? -1 : 1;
                        }
                        else
                        {
                            dau = model.ContactItemTypeCode == "NCC" ? 1 : -1;
                        }

                        #region Thêm vào bảng AMDebModel  
                        if (model.ContactItemTypeCode.Equals("KH") || model.ContactItemTypeCode.Equals("NCC"))
                        {
                            #region Đối tượng là khách hàng
                            //B1. Tính nợ cũ còn lại
                            if (model.ContactItemTypeCode.Equals("KH") && model.CustomerId.HasValue)
                            {
                                DebtOld = _context.AM_DebtModel
                                                             .Where(p => p.CustomerId == model.CustomerId)
                                                             .OrderByDescending(p => p.TimeOfDebt)
                                                             .Select(p => p.RemainingAmountAccrued)
                                                             .FirstOrDefault();
                                CustomerIdCreate = model.CustomerId;
                            }
                            #endregion

                            #region Đối tượng là NCC
                            else if (model.ContactItemTypeCode.Equals("NCC") && model.SupplierId.HasValue)
                            {
                                DebtOld = _context.AM_DebtModel
                                                             .Where(p => p.SupplierId == model.SupplierId)
                                                             .OrderByDescending(p => p.TimeOfDebt)
                                                             .Select(p => p.RemainingAmountAccrued)
                                                             .FirstOrDefault();
                                SupplierIdCreate = model.SupplierId;
                            }
                            #endregion

                            //B2 : Đưa nợ cũ còn lại vào  AM_DebtModel
                            DebtOld = (DebtOld == null) ? 0 : DebtOld.Value;
                            model.RemainingAmountAccrued = DebtOld + (model.Amount * dau);

                            _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            var AMDebModel = new AM_DebtModel()
                            {
                                SupplierId = SupplierIdCreate,
                                CustomerId = CustomerIdCreate,
                                TimeOfDebt = currentTime,
                                RemainingAmountAccrued = model.RemainingAmountAccrued,
                                TransactionId = model.TransactionId,
                                TransactionTypeCode = model.TransactionTypeCode
                            };
                            _context.Entry(AMDebModel).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }
                        else
                        {
                            _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }

                        #endregion

                        ts.Complete();
                        return Json("success", JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                    return Json("Xảy ra lỗi trong quá trình thêm mới !", JsonRequestBehavior.AllowGet);
                }
            }

            return Json("Vui lòng kiểm tra các ô được điền đầy đủ thông tin !", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Chi tiết phiếu thu - chi
        
        public ActionResult Details(int id)
        {
            //AM_TransactionModel model = _context.AM_TransactionModel.Find(id);
            var model = (from p in _context.AM_TransactionModel
                         join cus in _context.CustomerModel on p.CustomerId equals cus.CustomerId into custemp
                         from cus2 in custemp.DefaultIfEmpty()

                         join sup in _context.SupplierModel on p.SupplierId equals sup.SupplierId into suptemp
                         from sup2 in suptemp.DefaultIfEmpty()

                         join emp in _context.EmployeeModel on p.EmployeeId equals emp.EmployeeId into emptemp
                         from emp2 in emptemp.DefaultIfEmpty()

                         join sm in _context.StoreModel on p.StoreId equals sm.StoreId
                         join tm in _context.AM_TransactionTypeModel on p.TransactionTypeCode equals tm.TransactionTypeCode
                         join cm in _context.AM_ContactItemTypeModel on p.ContactItemTypeCode equals cm.ContactItemTypeCode
                         where (p.TransactionId == id)
                         select new AM_TransactionInfoViewModel()
                         {
                             StoreName = sm.StoreName,
                             TransactionTypeName = tm.TransactionTypeName,
                             ContactItemTypeName = cm.ContactItemTypeName ,
                             CustomerName =  !string.IsNullOrEmpty(cus2.FullName) ? ( cus2.FullName  + " - " + cus2.Phone): null,
                             SupplierName = !string.IsNullOrEmpty(sup2.SupplierName) ? (sup2.SupplierName  + " - " + sup2.Phone) : null,
                             EmployeeName = !string.IsNullOrEmpty(emp2.FullName) ? (emp2.FullName  + " - " + emp2.Phone) : null,
                             Amount = p.Amount,
                             Note = p.Note
                         }).FirstOrDefault();
                        
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        #endregion

        #region CustomerModel
        public ActionResult GetCustomerID(string q)
        {
            var data2 = _context
                       .CustomerModel
                       .Where(p => q == null || (p.FullName + " | " + p.Phone).Contains(q))
                       .Select(p => new
                       {
                           value = p.CustomerId,
                           text = p.FullName + " | " + p.Phone
                       })
                       .Take(10)
                       .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region EmployeeModel
        public ActionResult GetEmployeeId(string q, int StoreId)
        {
            var data2 = _context
                       .EmployeeModel
                       .Where(p => (q == null || (p.FullName + " | " + p.Phone).Contains(q) ) && p.StoreId == StoreId)
                       .Select(p => new
                       {
                           value = p.EmployeeId,
                           text = p.FullName + " | " + p.Phone
                       })
                       .Take(10)
                       .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetDebtOld
        public ActionResult GetDebOld(int CustomerId)
        {
            decimal? DebtOld = _context.AM_TransactionModel
                                    .Where(p => p.CustomerId == CustomerId)
                                    .OrderByDescending(p => p.TransactionId)
                                    .Select(p => p.RemainingAmountAccrued)
                                    .FirstOrDefault();
            DebtOld = DebtOld ?? 0;
            return Json(DebtOld, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDebOldSupplier(int SupplierId)
        {
            decimal? DebtOld = _context.AM_TransactionModel
                                    .Where(p => p.SupplierId == SupplierId)
                                    .OrderByDescending(p => p.TransactionId)
                                    .Select(p => p.RemainingAmountAccrued)
                                    .FirstOrDefault();
            DebtOld = DebtOld ?? 0;
            return Json(DebtOld, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}

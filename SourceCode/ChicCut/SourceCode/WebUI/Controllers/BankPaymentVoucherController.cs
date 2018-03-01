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
    public class BankPaymentVoucherController : BaseController
    {
        //
        // GET: /BankPaymentVoucher/

        #region Danh sách phiếu thu - chi ngân hàng
        public ActionResult Index()
        {
            CreateViewSearchBag();
            return View();
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
                    (am.AMAccountTypeCode == EnumAM_AccountType.NGANHANG) &&
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
                                                 .Where(p => p.TransactionTypeCode == EnumTransactionType.NHNOP || p.TransactionTypeCode == EnumTransactionType.NHRUT)
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
                        model.AMAccountId = (_context.AM_AccountModel.Where(p => p.AMAccountTypeCode == EnumAM_AccountType.NGANHANG && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault();
                        model.CreateDate = currentTime;
                        model.CreateEmpId = currentEmployee.EmployeeId;
                        int dau = model.TransactionTypeCode.Equals(EnumTransactionType.NHRUT) ? 1 : -1; // Xét dấu (Thu hay chi tiền )

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

    }
}

using Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers
{
    public class ReturnReportViewController : BaseController
    {
        //
        // GET: /ReturnReportView/
         
        public ActionResult Index(int id)
        {
            ViewBag.ReturnMasterId = id;
           // int ImportMasterId = _context.ReturnMasterModel.Where(p => p.ReturnMasterId == id).Select(p => p.ImportMasterId).FirstOrDefault();
           // int TransactionId = _context.AM_TransactionModel.Where(p => p.ImportMasterId == ImportMasterId && p.TransactionTypeCode == EnumTransactionType.NXXUAT).Select(p => p.TransactionId).FirstOrDefault();
            //ViewBag.TransactionId = TransactionId;
            int TransactionId = (from p in _context.ReturnMasterModel
                                 join tm in _context.AM_TransactionModel on p.ImportMasterId equals tm.ImportMasterId
                                 orderby (tm.TransactionId) ascending
                                 where (p.CreatedDate <= tm.CreateDate && tm.ImportMasterId == p.ImportMasterId && p.ReturnMasterId == id && tm.TransactionTypeCode == EnumTransactionType.NXXUAT && tm.Amount != 0)
                                 select tm.TransactionId
                                   ).FirstOrDefault();
            ViewBag.TransactionId = TransactionId;
            return View();
        }

    }
}

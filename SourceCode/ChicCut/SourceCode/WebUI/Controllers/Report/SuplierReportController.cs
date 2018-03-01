using Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers
{
    public class SuplierReportController : BaseController
    {
        //
        // GET: /SuplierReport/
         
        public ActionResult Index(int id)
        {
            ViewBag.ImportMasterId = id;
            int TransactionId = _context.AM_TransactionModel.Where(p => p.ImportMasterId == id && p.TransactionTypeCode == EnumTransactionType.NXNHAP && p.Amount != 0).Select(p => p.TransactionId).FirstOrDefault();
            ViewBag.TransactionId = TransactionId;
            return View();
        }

    }
}

using Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers.Report
{
    public class CustomerReportController : BaseController
    {
        //
        // GET: /CustomerReport/
         
        public ActionResult Index(int id)
        {
            ViewBag.OrderId = id;
            int TransactionId = _context.AM_TransactionModel.Where(p => p.OrderId == id && p.TransactionTypeCode == EnumTransactionType.BHBAN && p.Amount != 0).Select(p => p.TransactionId).FirstOrDefault();
            ViewBag.TransactionId = TransactionId;
            return View();
        }

    }
}

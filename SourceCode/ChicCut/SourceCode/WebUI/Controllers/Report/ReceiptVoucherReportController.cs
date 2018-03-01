using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers.Report
{
    public class ReceiptVoucherReportController : Controller
    {
        //
        // GET: /ReceiptVoucherReport/
         
        public ActionResult Index(int id)
        {
            ViewBag.TransactionId = id;
            return View();
        }

    }
}

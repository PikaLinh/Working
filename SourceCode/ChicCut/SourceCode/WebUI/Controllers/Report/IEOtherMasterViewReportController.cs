using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class IEOtherMasterViewReportController : Controller
    {
        //
        // GET: /IEOtherMasterViewReport/
         //
        public ActionResult Index(int id)
        {
            ViewBag.IEOtherMasterId = id;
            return View();
        }

    }
}

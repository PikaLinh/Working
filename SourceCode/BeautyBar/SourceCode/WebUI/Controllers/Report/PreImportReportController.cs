using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers
{
    public class PreImportReportController : BaseController
    {
        //
        // GET: /PreImportReport/

        
        public ActionResult Index(int id)
        {
            ViewBag.PreImportMasterId = id;
            return View();
        }

    }
}

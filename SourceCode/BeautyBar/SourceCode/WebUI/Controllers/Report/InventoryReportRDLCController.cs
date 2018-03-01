using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers.Report
{
    public class InventoryReportRDLCController : Controller
    {
        // GET: InventoryReportRDLC
        public ActionResult Index()
        {
            //ViewBag.CandidateID = id;
            return View();
        }
    }
}
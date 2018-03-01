using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;


namespace WebUI.Controllers
{
    public class PageController : Controller
    {
        //
        // GET: /Page/
        private EntityDataContext _context = new EntityDataContext();
         
        public ActionResult Index()
        {
            return View(_context.PageModel.OrderBy(p => p.PageId).ToList());
        }
         
        public ActionResult Create()
        {
            PageModel rolesmodel = new PageModel() { 
                Actived = true,
                Visiable = true
            };
            return View(rolesmodel);
        }

    }
}

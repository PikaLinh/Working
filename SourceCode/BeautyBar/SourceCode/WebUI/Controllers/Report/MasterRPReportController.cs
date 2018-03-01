using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace WebUI.Controllers.Report
{
    public class MasterRPReportController : BaseController
    {
        //
        // GET: /MasterRPReport/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _ReportCategory()
        {
            var result = _context.Database.SqlQuery<ReportCategory1ViewModel>
                          ("dbo.usp_CategoryReportChicCut").ToList();
            return PartialView(result);
        }
        public ActionResult _ReportProductChicCut(int? rdCategoryId)
        {
            ViewBag.CategoryName = _context.CategoryModel.Where(p => p.CategoryId == rdCategoryId).Select(p => p.CategoryName).FirstOrDefault();
            var result = new List<ReportCategory1ViewModel>();
            if (rdCategoryId == -1)
            {
                ViewBag.CategoryName = "Tất cả sản phẩm";

                result = _context.Database.SqlQuery<ReportCategory1ViewModel>
                        ("usp_CategoryProductAll")
                        .ToList();
            }
            else
            {
                result = _context.Database.SqlQuery<ReportCategory1ViewModel>
                        ("usp_CategoryProductReport @CategoryId",
                          new SqlParameter("@CategoryId", rdCategoryId))
                        .ToList();
            }

            return PartialView(result);
        }



        public ActionResult _ReportProductType(int? rdCategoryId)
        {
            var result = new List<ReportCategory1ViewModel>();
            if (rdCategoryId == -1)
            {
                result = _context.Database.SqlQuery<ReportCategory1ViewModel>
                         ("usp_CategoryProductTypeAll")
                         .ToList();
            }
            else
            {
                result = _context.Database.SqlQuery<ReportCategory1ViewModel>
                              ("usp_CategoryProductTypeReport @CategoryId",
                                new SqlParameter("@CategoryId", rdCategoryId))
                              .ToList();
            }
            return PartialView(result);
        }
        public ActionResult _ReportProduct(int? rdCategoryId, int? rdCategoryParent, bool isWithChild = false)
        {
            var result = new List<ReportCategory1ViewModel>();

            if (rdCategoryId == -1)
            {
                if (rdCategoryParent == -1)
                {
                    result = _context.Database.SqlQuery<ReportCategory1ViewModel>
                            ("usp_CategoryProductAll")
                            .ToList();
                }
                else
                {
                    if (rdCategoryId == - 1)
                    {
                        isWithChild = true;
                    }
                    result = _context.Database.SqlQuery<ReportCategory1ViewModel>
                            ("usp_CategoryProductAllPa @CategoryId,@isWithChild",
                              new SqlParameter("@CategoryId", rdCategoryParent),
                              new SqlParameter("@isWithChild", isWithChild))
                            .ToList();
                }
            }
            else
            {
                result = _context.Database.SqlQuery<ReportCategory1ViewModel>
                        ("usp_CategoryProductReport @CategoryId",
                          new SqlParameter("@CategoryId", rdCategoryId))
                        .ToList();
            }
           
            return PartialView(result);
        }

    }
}

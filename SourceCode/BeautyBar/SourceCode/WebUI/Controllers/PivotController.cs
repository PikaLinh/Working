using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;
using DevExpress.Web.Mvc;
using DevExpress.Utils;
using DevExpress.Web.ASPxPivotGrid;
namespace WebUI.Controllers
{
    public class PivotController : BaseController
    {
        //
        // GET: /Pivot/

        public ActionResult Index()
        {
            return View(GetList());
        }
        public ActionResult _PivotPartialView()
        {
            return PartialView("_PivotPartialView", GetList());
        }
        public IEnumerable GetList()
        {
            var lst = (from p in _context.ProductModel
                       join cat2 in _context.CategoryModel on p.CategoryId equals cat2.CategoryId into lst2

                       from cat in lst2.DefaultIfEmpty()

                       join ordetail in _context.OrderDetailModel on p.ProductId equals ordetail.ProductId 

                       join ordmaster in _context.OrderMasterModel on ordetail.OrderId equals ordmaster.OrderId 

                       join cus in _context.CustomerModel on ordmaster.CustomerId equals cus.CustomerId 

                       orderby p.ProductId descending
                       select new ProductInfoViewModel()
                       {
                           CategoryName = cat.CategoryName,
                           ProductName = p.ProductName,
                           COGS = ordetail.UnitPrice,
                           UnitName = cus.FullName,
                           CreatedDate = ordmaster.CreatedDate
                       }).Take(100).ToList();

            return (lst);
        }

        public ActionResult ExportToXLS(string ExportType)
        {
            switch (ExportType)
            {
                case "Excel": return PivotGridExtension.ExportToXlsx(Settings, GetList()); //break;
                case "PDF": return PivotGridExtension.ExportToPdf(Settings, GetList()); //break;
                default: return PivotGridExtension.ExportToImage(Settings, GetList()); //break;
            }
        }

        static PivotGridSettings _settings;
        public static PivotGridSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = CreatePivotGridSettings();
                }
                return _settings;
            }
        }

        public static PivotGridSettings CreatePivotGridSettings()
        {
            PivotGridSettings settings = new PivotGridSettings();
            settings.Name = "BaoCao_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            settings.CallbackRouteValues = new { Controller = "Pivot", Action = "_PivotPartialView" };
            settings.OptionsView.ShowHorizontalScrollBar = true;
            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);

            #region // Xác định các field
            //Fillter
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.FieldName = "UnitName";
                filed.Caption = "Tên khách hàng"; // Tên hiển thị fillter
            });

            //Giá trị vùng dữ liệu
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.FieldName = "COGS";
                filed.Caption = "Giá bán";
                //filed.ValueFormat.FormatType = FormatType.Custom;
                //filed.ValueFormat.FormatString = "c";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0} VNĐ";
            });




            // Dòng dữ liệu
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
                filed.AreaIndex = 0;
                filed.FieldName = "CategoryName";
                filed.Caption = "Loại sản phẩm";
            });

            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
                filed.AreaIndex = 1;
                filed.FieldName = "ProductName";
                filed.Caption = "Tên sản phẩm";
            });

            //cột dữ liệu
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                filed.FieldName = "CreatedDate";
                filed.Caption = "Năm";
                filed.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateYear;
            });

            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                filed.FieldName = "CreatedDate";
                filed.Caption = "Quý";
                filed.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateQuarter;
                filed.ValueFormat.FormatType = FormatType.Numeric;
                filed.ValueFormat.FormatString = "Quý {0}";
            });
            #endregion

            settings.PreRender = (sender, e) =>
            {
                MVCxPivotGrid PivotGrid = sender as MVCxPivotGrid;
                if (System.Web.HttpContext.Current.Session["Layout"] != null)
                    PivotGrid.LoadLayoutFromString((string)System.Web.HttpContext.Current.Session["Layout"], PivotGridWebOptionsLayout.DefaultLayout);
                PivotGrid.CollapseAll();
            };
            settings.GridLayout = (sender, e) =>
            {
                MVCxPivotGrid PivotGrid = sender as MVCxPivotGrid;
                System.Web.HttpContext.Current.Session["Layout"] = PivotGrid.SaveLayoutToString(PivotGridWebOptionsLayout.DefaultLayout);
            };

            return settings;
        }

    }
}



using DevExpress.Utils;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace WebUI.Controllers
{
    public class StockMovementSummaryController : BaseController
    {
        //
        // GET: /StockMovementSummary/

        public ActionResult Index()
        {
            return View();
        }

        static DateTime _Fromdate, _Todate;
        static bool IsDrop = true; //Không kéo thả => Settings

        static int _FromQuater, _FromYearQuater, _ToQuater, _ToYearQuater;
        static int _FromMonth, _FromYearMonth, _ToMonth, _ToYearMonth;
        static decimal? _WarehouseId = null;
        static int PageSize = 20;

        public ActionResult ChangedPasize(int NumberRecordPerPage)
        {
            IsDrop = false;
            PageSize = NumberRecordPerPage;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #region -- Ngày
        public ActionResult MarkIsDropFillter()
        {
            IsDrop = false;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _PivotPartialView(DateTime Fromdate, DateTime Todate, decimal? WarehouseId)
        {
            _Fromdate = Fromdate;
            _Todate = Todate;
            _WarehouseId = WarehouseId;
            return PartialView("_PivotPartialView", GetList(_Fromdate, _Todate, WarehouseId));
        }

        #region Export
        public ActionResult ExportD()
        {
            return PivotGridExtension.ExportToXlsx(SettingsD, GetList(_Fromdate, _Todate, _WarehouseId));
        }
        #endregion

        public IEnumerable GetList(DateTime Fromdate, DateTime Todate, decimal? WarehouseId)
        {
            // var lst = _context.Database.SqlQuery<ProductInfoViewModel>(
            //"usp_BaoCaoXuatNhapTon").ToList();
            var List = new List<ProductInfoViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_BaoCaoXuatNhapTonDay";
                        cmd.Parameters.AddWithValue("@FromDate", Fromdate);
                        cmd.Parameters.AddWithValue("@ToDate", Todate);
                        cmd.Parameters.AddWithValue("@WarehouseId", WarehouseId);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        // do
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var v = ListViewDay(dr);
                                List.Add(v);
                            }
                        }
                        conn.Close();
                    }
                }
                return List;
            }
            catch
            {
                return List;
            }

        }

        public ProductInfoViewModel ListView(SqlDataReader s)
        {
            ProductInfoViewModel ret = new ProductInfoViewModel();
            try
            {
                if (s["CategoryName"] != null)
                {
                    ret.CategoryName = s["CategoryName"].ToString();
                }

                if (s["ProductName"] != null)
                {
                    ret.ProductName = s["ProductName"].ToString();
                }

                if (s["CreatedDateString"] != null)
                {
                    ret.CreatedDateString = s["CreatedDateString"].ToString();
                }
               

                if (s["BeginInventoryQty"] != null)
                {
                    ret.BeginInventoryQty = decimal.Parse(s["BeginInventoryQty"].ToString());
                }

                if (s["ImportQty"] != null)
                {
                    ret.ImportQty = decimal.Parse(s["ImportQty"].ToString());
                }

                if (s["ExportQty"] != null)
                {
                    ret.ExportQty = decimal.Parse(s["ExportQty"].ToString());
                }

                if (s["EndInventoryQty"] != null)
                {
                    ret.EndInventoryQty = decimal.Parse(s["EndInventoryQty"].ToString());
                }

                if (s["ShortName"] != null)
                {
                    ret.ShortName = s["ShortName"].ToString();
                }

                //if (s["InventoryCode"] != null)
                //{
                //    ret.InventoryCode = s["InventoryCode"].ToString();
                //}

                //if (s["CreatedAccount"] != null)
                //{
                //    ret.CreatedAccount = s["CreatedAccount"].ToString();
                //}

                if (s["WarehouseName"] != null)
                {
                    ret.WarehouseName = s["WarehouseName"].ToString();
                }

                if (s["CreatedDate"] != null)
                {
                    ret.CreatedDate = Convert.ToDateTime(s["CreatedDate"]);
                }
            }
            catch { }
            return ret;
        }

        public ProductInfoViewModel ListViewDay(SqlDataReader s)
        {
            ProductInfoViewModel ret = new ProductInfoViewModel();
            try
            {
                if (s["CategoryName"] != null)
                {
                    ret.CategoryName = s["CategoryName"].ToString();
                }

                if (s["ProductName"] != null)
                {
                    ret.ProductName = s["ProductName"].ToString();
                }
                if (s["CreatedDate"] != null)
                {
                    ret.CreatedDate = Convert.ToDateTime(s["CreatedDate"]);
                }

                if (s["BeginInventoryQty"] != null)
                {
                    ret.BeginInventoryQty = decimal.Parse(s["BeginInventoryQty"].ToString());
                }

                if (s["ImportQty"] != null)
                {
                    ret.ImportQty = decimal.Parse(s["ImportQty"].ToString());
                }

                if (s["ExportQty"] != null)
                {
                    ret.ExportQty = decimal.Parse(s["ExportQty"].ToString());
                }

                if (s["EndInventoryQty"] != null)
                {
                    ret.EndInventoryQty = decimal.Parse(s["EndInventoryQty"].ToString());
                }

                if (s["ShortName"] != null)
                {
                    ret.ShortName = s["ShortName"].ToString();
                }

                //if (s["InventoryCode"] != null)
                //{
                //    ret.InventoryCode = s["InventoryCode"].ToString();
                //}

                //if (s["CreatedAccount"] != null)
                //{
                //    ret.CreatedAccount = s["CreatedAccount"].ToString();
                //}

                if (s["WarehouseName"] != null)
                {
                    ret.WarehouseName = s["WarehouseName"].ToString();
                }

            
            }
            catch { }
            return ret;
        }

        #region Settings Day
        static PivotGridSettings _settingsD;
        public static PivotGridSettings SettingsD
        {
            get
            {
                if (!IsDrop) // không kéo thả => settings
                {
                    _settingsD = null;
                    _settingsD = CreatePivotGridSettingsD();
                    IsDrop = true; // settings xong thì cho là kéo thả 
                }
                return _settingsD;
            }
        }

        public static PivotGridSettings CreatePivotGridSettingsD()
        {
            PivotGridSettings settings = new PivotGridSettings();
            settings.Name = "BaoCaoTheoNgay_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            settings.CallbackRouteValues = new
            {
                Controller = "StockMovementSummary",
                Action = "_PivotPartialView",
                Fromdate = _Fromdate,
                Todate = _Todate ,
                WarehouseId = _WarehouseId
            };
            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.Theme = "Metropolis";
            settings.OptionsView.ShowHorizontalScrollBar = true;
            settings.OptionsView.ShowColumnGrandTotals = false;
            settings.OptionsView.ShowRowTotals = false;
            settings.OptionsView.ShowRowGrandTotals = false; // dòng tổng cuối
            settings.OptionsPager.RowsPerPage = PageSize;
            // Xác định các field

            //Fillter
          
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.AreaIndex = 0;
                filed.FieldName = "ShortName";
                filed.Caption = "Cửa hàng"; // Tên hiển thị fillter
            });

            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.AreaIndex = 1;
                filed.FieldName = "WarehouseName";
                filed.Caption = "Kho";
            });

            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.AreaIndex = 2;
                filed.FieldName = "CategoryName";
                filed.Caption = "Loại sản phẩm";
            });

            //Giá trị vùng dữ liệu
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 0;
                filed.FieldName = "BeginInventoryQty";
                filed.Caption = "Tồn đầu";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 1;
                filed.FieldName = "ImportQty";
                filed.Caption = "Nhập";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 2;
                filed.FieldName = "ExportQty";
                filed.Caption = "Xuất";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 3;
                filed.FieldName = "EndInventoryQty";
                filed.Caption = "Tồn cuối";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });




            // Dòng dữ liệu
           
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
                filed.AreaIndex = 0;
                filed.FieldName = "ProductName";
                filed.Caption = "Sản phẩm";
            });

            //cột dữ liệu
            //settings.Fields.Add(filed =>
            //{
            //    filed.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            //    filed.FieldName = "CreatedDate";
            //    filed.AreaIndex = 0;
            //    filed.Caption = "Năm";
            //    filed.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateYear;
            //    filed.Options.ShowTotals = false;
            //});

            //settings.Fields.Add(filed =>
            //{
            //    filed.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            //    filed.FieldName = "CreatedDate";
            //    filed.AreaIndex = 1;
            //    filed.Caption = "Quý";
            //    filed.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateQuarter;
            //    filed.ValueFormat.FormatType = FormatType.Numeric;
            //    filed.ValueFormat.FormatString = "Quý {0}";
            //    filed.Options.ShowTotals = false;
            //});

            //settings.Fields.Add(filed =>
            //{
            //    filed.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            //    filed.FieldName = "CreatedDate";
            //    filed.AreaIndex = 2;
            //    filed.Caption = "Tháng";
            //    filed.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateMonth;
            //    filed.ValueFormat.FormatType = FormatType.Custom;
            //    filed.ValueFormat.FormatString = "Tháng {0:MM}";
            //    filed.Options.ShowTotals = false;
            //});

            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                filed.AreaIndex = 0;
                filed.FieldName = "CreatedDate";
                filed.Caption = "Ngày";
               // filed.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateDay;
                filed.ValueFormat.FormatType = FormatType.DateTime;
                filed.ValueFormat.FormatString = "dd/MM/yyyy";
                //filed.Options.ShowTotals = false;
            });

            settings.PreRender = (sender, e) =>
            {
                MVCxPivotGrid PivotGrid = sender as MVCxPivotGrid;
                if (System.Web.HttpContext.Current.Session["LayoutD"] != null)
                    PivotGrid.LoadLayoutFromString((string)System.Web.HttpContext.Current.Session["LayoutD"], PivotGridWebOptionsLayout.DefaultLayout);
                PivotGrid.CollapseAll();
            };
            settings.GridLayout = (sender, e) =>
            {
                MVCxPivotGrid PivotGrid = sender as MVCxPivotGrid;
                System.Web.HttpContext.Current.Session["LayoutD"] = PivotGrid.SaveLayoutToString(PivotGridWebOptionsLayout.DefaultLayout);
            };
            return settings;
        }
        #endregion

        #endregion

        #region -- Quý
        public ActionResult _PivotPartialViewQ(int FromQuater, int FromYearQuater, int ToQuater, int ToYearQuater, decimal? WarehouseId)
        {
            _FromQuater = FromQuater;
            _FromYearQuater = FromYearQuater;
            _ToQuater = ToQuater;
            _ToYearQuater = ToYearQuater;
            _WarehouseId = WarehouseId;

            return PartialView("_PivotPartialViewQ", GetListQ(FromQuater, FromYearQuater, ToQuater, ToYearQuater, WarehouseId));
        }

        public IEnumerable GetListQ(int FromQuater, int FromYearQuater, int ToQuater, int ToYearQuater, decimal? WarehouseId)
        {
           // var lst = _context.Database.SqlQuery<ProductInfoViewModel>(
           //"usp_BaoCaoXuatNhapTonQuater").ToList();
            var List = new List<ProductInfoViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_BaoCaoXuatNhapTonQuater";
                        cmd.Parameters.AddWithValue("@FromQuater", FromQuater);
                        cmd.Parameters.AddWithValue("@FromYearQuater", FromYearQuater);
                        cmd.Parameters.AddWithValue("@ToQuater", ToQuater);
                        cmd.Parameters.AddWithValue("@ToYearQuater", ToYearQuater);
                        cmd.Parameters.AddWithValue("@WarehouseId", WarehouseId);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        // do
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var v = ListView(dr);
                                List.Add(v);
                            }
                        }
                        conn.Close();
                    }
                }
                return List;
            }
            catch
            {
                return List;
            }
        }

        #region Settings Quater
        static PivotGridSettings _settingsQ;
        public static PivotGridSettings SettingsQ
        {
            get
            {
                if (!IsDrop) // không kéo thả => settings
                {
                    _settingsQ = null;
                    _settingsQ = CreatePivotGridSettingsQ();
                    IsDrop = true; // settings xong thì cho là kéo thả 
                }
                return _settingsQ;
            }
        }

        public static PivotGridSettings CreatePivotGridSettingsQ()
        {
            PivotGridSettings settings = new PivotGridSettings();
            settings.Name = "BaoCaoTheoQuy_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            settings.CallbackRouteValues = new {
                                                  Controller = "StockMovementSummary",
                                                  Action = "_PivotPartialViewQ",
                                                  FromQuater = _FromQuater,
                                                  FromYearQuater = _FromYearQuater,
                                                  ToQuater = _ToQuater,
                                                  ToYearQuater = _ToYearQuater,
                                                  WarehouseId = _WarehouseId
                                               };
            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.Theme = "Metropolis";
            settings.OptionsView.ShowHorizontalScrollBar = true;
            settings.OptionsView.ShowColumnGrandTotals = false; // cột
            settings.OptionsView.ShowRowTotals = false; // nhóm từng dòng
            settings.OptionsView.ShowRowGrandTotals = false; // dòng tổng cuối
            settings.OptionsPager.RowsPerPage = PageSize;
            // Xác định các field

            //Fillter
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.AreaIndex = 0;
                filed.FieldName = "ShortName";
                filed.Caption = "Cửa hàng"; // Tên hiển thị fillter
            });

            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.AreaIndex = 1;
                filed.FieldName = "WarehouseName";
                filed.Caption = "Kho";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.AreaIndex = 2;
                filed.FieldName = "CategoryName";
                filed.Caption = "Loại sản phẩm";
            });

            //Giá trị vùng dữ liệu
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 0;
                filed.FieldName = "BeginInventoryQty";
                filed.Caption = "Tồn đầu";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 1;
                filed.FieldName = "ImportQty";
                filed.Caption = "Nhập";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 2;
                filed.FieldName = "ExportQty";
                filed.Caption = "Xuất";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 3;
                filed.FieldName = "EndInventoryQty";
                filed.Caption = "Tồn cuối";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });

            // Dòng dữ liệu
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
                filed.AreaIndex = 0;
                filed.FieldName = "ProductName";
                filed.Caption = "Sản phẩm";
            });

            //cột dữ liệu

           

            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                filed.FieldName = "CreatedDateString";
                filed.AreaIndex = 0;
                filed.Caption = "Quý";
                //filed.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.DateQuarter;
                //filed.ValueFormat.FormatType = FormatType.DateTime;
                //filed.ValueFormat.FormatString = "Quý {0} Năm {1:yyyy}";
               // filed.Options.ShowTotals = false;
            });

            settings.PreRender = (sender, e) =>
            {
                MVCxPivotGrid PivotGrid = sender as MVCxPivotGrid;
                if (System.Web.HttpContext.Current.Session["LayoutQ"] != null)
                    PivotGrid.LoadLayoutFromString((string)System.Web.HttpContext.Current.Session["LayoutQ"], PivotGridWebOptionsLayout.DefaultLayout);
                PivotGrid.CollapseAll();
            };
            settings.GridLayout = (sender, e) =>
            {
                MVCxPivotGrid PivotGrid = sender as MVCxPivotGrid;
                System.Web.HttpContext.Current.Session["LayoutQ"] = PivotGrid.SaveLayoutToString(PivotGridWebOptionsLayout.DefaultLayout);
            };
            return settings;
        }
        #endregion

        #region Export
        public ActionResult ExportQ()
        {
            return PivotGridExtension.ExportToXlsx(SettingsQ, GetListQ(_FromQuater,_FromYearQuater,_ToQuater,_ToYearQuater, _WarehouseId));
        }
        #endregion

        #endregion

        #region -- Tháng
        public ActionResult _PivotPartialViewM(int FromMonth, int FromYearMonth, int ToMonth, int ToYearMonth, decimal? WarehouseId)
        {
            _FromMonth = FromMonth;
            _FromYearMonth = FromYearMonth;
            _ToMonth = ToMonth;
            _ToYearMonth = ToYearMonth;
            _WarehouseId = WarehouseId;
            return PartialView("_PivotPartialViewM", GetListM(FromMonth, FromYearMonth, ToMonth, ToYearMonth, WarehouseId));
        }

        public IEnumerable GetListM(int FromMonth, int FromYearMonth, int ToMonth, int ToYearMonth, decimal? WarehouseId)
        {
           // var lst = _context.Database.SqlQuery<ProductInfoViewModel>(
           //"usp_BaoCaoXuatNhapTonMonth").ToList();
           // return lst;
            var List = new List<ProductInfoViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_BaoCaoXuatNhapTonMonth";
                        cmd.Parameters.AddWithValue("@FromMonth", FromMonth);
                        cmd.Parameters.AddWithValue("@FromYearMonth", FromYearMonth);
                        cmd.Parameters.AddWithValue("@ToMonth", ToMonth);
                        cmd.Parameters.AddWithValue("@ToYearMonth", ToYearMonth);
                        cmd.Parameters.AddWithValue("@WarehouseId", WarehouseId);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        // do
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var v = ListView(dr);
                                List.Add(v);
                            }
                        }
                        conn.Close();
                    }
                }
                return List;
            }
            catch
            {
                return List;
            }

        }

        #region Settings Month
        static PivotGridSettings _settingsM;
        public static PivotGridSettings SettingsM
        {
            get
            {
                if (!IsDrop) // không kéo thả => settings
                {
                    _settingsM = null;
                    _settingsM = CreatePivotGridSettingsM();
                    IsDrop = true; // settings xong thì cho là kéo thả 
                }
                return _settingsM;
            }
        }

        public static PivotGridSettings CreatePivotGridSettingsM()
        {
            PivotGridSettings settings = new PivotGridSettings();
            settings.Name = "BaoCaoTheoThang_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            settings.CallbackRouteValues = new { 
                                                    Controller = "StockMovementSummary",
                                                    Action = "_PivotPartialViewM",
                                                    FromMonth = _FromMonth,
                                                    FromYearMonth = _FromYearMonth,
                                                    ToMonth = _ToMonth,
                                                    ToYearMonth = _ToYearMonth,
                                                    WarehouseId = _WarehouseId
                                               };
            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.Theme = "Metropolis";
            settings.OptionsView.ShowHorizontalScrollBar = true;
            settings.OptionsView.ShowColumnGrandTotals = false;
            settings.OptionsView.ShowRowTotals = false;
            settings.OptionsView.ShowRowGrandTotals = false; // dòng tổng cuối
            settings.OptionsPager.RowsPerPage = PageSize;
            // Xác định các field

            //Fillter
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.AreaIndex = 0;
                filed.FieldName = "ShortName";
                filed.Caption = "Cửa hàng"; // Tên hiển thị fillter
            });

            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.AreaIndex = 1;
                filed.FieldName = "WarehouseName";
                filed.Caption = "Kho";
            });

            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.FilterArea;
                filed.AreaIndex = 2;
                filed.FieldName = "CategoryName";
                filed.Caption = "Loại sản phẩm";
            });


            //Giá trị vùng dữ liệu
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 0;
                filed.FieldName = "BeginInventoryQty";
                filed.Caption = "Tồn đầu";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 1;
                filed.FieldName = "ImportQty";
                filed.Caption = "Nhập";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 2;
                filed.FieldName = "ExportQty";
                filed.Caption = "Xuất";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                filed.AreaIndex = 3;
                filed.FieldName = "EndInventoryQty";
                filed.Caption = "Tồn cuối";
                filed.CellFormat.FormatType = FormatType.Numeric;
                filed.CellFormat.FormatString = "{0:n0}";
            });




            // Dòng dữ liệu
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
                filed.AreaIndex = 0;
                filed.FieldName = "ProductName";
                filed.Caption = "Sản phẩm";
            });

            //cột dữ liệu
            settings.Fields.Add(filed =>
            {
                filed.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                filed.FieldName = "CreatedDateString";
                filed.AreaIndex = 0;
                filed.Caption = "Tháng";
            });

            settings.PreRender = (sender, e) =>
            {
                MVCxPivotGrid PivotGrid = sender as MVCxPivotGrid;
                if (System.Web.HttpContext.Current.Session["LayoutM"] != null)
                    PivotGrid.LoadLayoutFromString((string)System.Web.HttpContext.Current.Session["LayoutM"], PivotGridWebOptionsLayout.DefaultLayout);
                PivotGrid.CollapseAll();
            };
            settings.GridLayout = (sender, e) =>
            {
                MVCxPivotGrid PivotGrid = sender as MVCxPivotGrid;
                System.Web.HttpContext.Current.Session["LayoutM"] = PivotGrid.SaveLayoutToString(PivotGridWebOptionsLayout.DefaultLayout);
            };
            return settings;
        }
        #endregion

        #region Export
        public ActionResult ExportM()
        {
            return PivotGridExtension.ExportToXlsx(SettingsM, GetListM(_FromMonth,_FromYearMonth,_ToMonth, _ToYearMonth , _WarehouseId));
        }
        #endregion

        #endregion


    }
}

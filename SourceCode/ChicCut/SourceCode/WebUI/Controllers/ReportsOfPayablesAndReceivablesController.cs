using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace WebUI.Controllers
{
    public class ReportsOfPayablesAndReceivablesController : BaseController
    {
        //
        // GET: /ReportsOfPayablesAndReceivables/

        public ActionResult Index()
        {
            return View();
        }

        #region Công nợ khách hàng
        public ActionResult _CustomerInforPartial(ReportSearchViewModel model)
        {
            var CusInfo = new ReportCusInforViewModel();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_ThongTinCongNoKhachHang";
                        cmd.Parameters.AddWithValue("@CustomerId", model.CustomerId);
                        cmd.Parameters.AddWithValue("@FromDate", model.FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", model.ToDate);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            CusInfo = SingleView(dr);
                            
                        }
                        conn.Close();
                        return PartialView(CusInfo);
                    }
                }
            }
            catch
            {
                return PartialView(CusInfo);
            }
        }

        public ActionResult _CustomerInforTransaction(ReportSearchViewModel model)
        {
            var List = new List<ReportCusTransactionViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_ThongTinChiTietGiaoDichCongNoKhachHang";
                        cmd.Parameters.AddWithValue("@CustomerId", model.CustomerId);
                        cmd.Parameters.AddWithValue("@FromDate", model.FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", model.ToDate);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var v = ListView(dr);
                                List.Add(v);
                            }
                        }
                        conn.Close();
                        return PartialView(List);
                    }
                }
            }
            catch
            {
                return PartialView(List);
            }
        }

        public ReportCusInforViewModel SingleView(SqlDataReader s)
        {
            ReportCusInforViewModel ret = new ReportCusInforViewModel();
            try
            {
                if (s["FullName"] != null)
                {
                    ret.FullName = s["FullName"].ToString();
                }

                if (s["Phone"] != null)
                {
                    ret.Phone = s["Phone"].ToString();
                }

                if (s["Email"] != null)
                {
                    ret.Email = s["Email"].ToString();
                }

                if (s["Address"] != null)
                {
                    ret.Address = s["Address"].ToString();
                }

                if (s["BirthDay"] != DBNull.Value)
                {
                    ret.BirthDay = Convert.ToDateTime(s["BirthDay"].ToString());
                }

                if (s["GenderString"] != null)
                {
                    ret.GenderString = s["GenderString"].ToString();
                }

                if (s["ProvinceName"] != null)
                {
                    ret.ProvinceName = s["ProvinceName"].ToString();
                }

                if (s["DistrictName"] != null)
                {
                    ret.DistrictName = s["DistrictName"].ToString();
                }

                if (s["CustomerLevelName"] != null)
                {
                    ret.CustomerLevelName = s["CustomerLevelName"].ToString();
                }

                if (s["NoPhaiThu"] != DBNull.Value)
                {
                    ret.NoPhaiThu = decimal.Parse(s["NoPhaiThu"].ToString());
                }

                if (s["SoDuNoDauKy"] != DBNull.Value)
                {
                    ret.SoDuNoDauKy = decimal.Parse(s["SoDuNoDauKy"].ToString());
                }

                if (s["SoDuNoCuoiKy"] != DBNull.Value)
                {
                    ret.SoDuNoCuoiKy = decimal.Parse(s["SoDuNoCuoiKy"].ToString());
                }

            }
            catch { }
            return ret;
        }

        public ReportCusTransactionViewModel ListView(SqlDataReader s)
        {
            ReportCusTransactionViewModel ret = new ReportCusTransactionViewModel();
            try
            {
                if (s["DateTransaction"] != DBNull.Value)
                {
                    ret.DateTransaction = Convert.ToDateTime(s["DateTransaction"].ToString());
                }

                if (s["TransactionCode"] != null)
                {
                    ret.TransactionCode = s["TransactionCode"].ToString();
                }

                if (s["Change"] != null)
                {
                    ret.Change = s["Change"].ToString();
                }

                if (s["Amout"] != DBNull.Value) 
                {
                    ret.Amout = decimal.Parse(s["Amout"].ToString());
                }

                if (s["RemainingAmountAccrued"] != DBNull.Value) 
                {
                    ret.RemainingAmountAccrued = decimal.Parse(s["RemainingAmountAccrued"].ToString());
                }

                if (s["Description"] != null)
                {
                    ret.Description = s["Description"].ToString();
                }
            }
            catch { }
            return ret;
        }
        #endregion

        #region Công nợ nhà cung cấp
        public ActionResult _SupplierInforPartial(ReportSearchViewModel model)
        {
            var SupInfo = new ReportSupInforViewModel();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_ThongTinCongNoNhaCungCap";
                        cmd.Parameters.AddWithValue("@SupplierId", model.SupplierId);
                        cmd.Parameters.AddWithValue("@FromDateSup", model.FromDateSup);
                        cmd.Parameters.AddWithValue("@ToDateSup", model.ToDateSup);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            SupInfo = SingleViewSup(dr);
                        }
                        conn.Close();
                        return PartialView(SupInfo);
                    }
                }
            }
            catch
            {
                return PartialView(SupInfo);
            }
        }

        public ActionResult _SupplierInforTransaction(ReportSearchViewModel model)
        {
            var List = new List<ReportCusTransactionViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_ThongTinChiTietGiaoDichCongNoNCC";
                        cmd.Parameters.AddWithValue("@SupplierId", model.SupplierId);
                        cmd.Parameters.AddWithValue("@FromDateSup", model.FromDateSup);
                        cmd.Parameters.AddWithValue("@ToDateSup", model.ToDateSup);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var v = ListView(dr);
                                List.Add(v);
                            }
                        }
                        conn.Close();
                        return PartialView(List);
                    }
                }
            }
            catch
            {
                return PartialView(List);
            }
        }

        public ReportSupInforViewModel SingleViewSup(SqlDataReader s)
        {
            ReportSupInforViewModel ret = new ReportSupInforViewModel();
            try
            {
                if (s["SupplierName"] != null)
                {
                    ret.SupplierName = s["SupplierName"].ToString();
                }

                if (s["Phone"] != null)
                {
                    ret.Phone = s["Phone"].ToString();
                }

                if (s["Email"] != null)
                {
                    ret.Email = s["Email"].ToString();
                }

                if (s["Address"] != null)
                {
                    ret.Address = s["Address"].ToString();
                }

                if (s["ProvinceName"] != null)
                {
                    ret.ProvinceName = s["ProvinceName"].ToString();
                }

                if (s["DistrictName"] != null)
                {
                    ret.DistrictName = s["DistrictName"].ToString();
                }

                if (s["BankName"] != null)
                {
                    ret.BankName = s["BankName"].ToString();
                }

                if (s["BankAccountNumber"] != null)
                {
                    ret.BankAccountNumber = s["BankAccountNumber"].ToString();
                }

                if (s["BankOwner"] != null)
                {
                    ret.BankOwner = s["BankOwner"].ToString();
                }

                if (s["NoPhaiTra"] != DBNull.Value)
                {
                    ret.NoPhaiTra = decimal.Parse(s["NoPhaiTra"].ToString());
                }

                if (s["SoDuNoDauKy"] != DBNull.Value)
                {
                    ret.SoDuNoDauKy = decimal.Parse(s["SoDuNoDauKy"].ToString());
                }

                if (s["SoDuNoCuoiKy"] != DBNull.Value)
                {
                    ret.SoDuNoCuoiKy = decimal.Parse(s["SoDuNoCuoiKy"].ToString());
                }

            }
            catch { }
            return ret;
        }

        #endregion
    }
}

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
    public class ReceivablesFromCustomersController : BaseController
    {
        //
        // GET: /ReceivablesFromCustomers /

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _CustomerInforPartial(DateTime? FromDate = null,DateTime? ToDate = null)
        {
            var List = new List<ReportCusInforViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_DanhSachNoPhaiThuKhachHang";
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
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
                        ViewBag.FromDate = FromDate;
                        ViewBag.ToDate = ToDate;
                        return PartialView(List);
                    }
                }
            }
            catch
            {
                return PartialView(List);
            }
        }

        public ReportCusInforViewModel ListView(SqlDataReader s)
        {
            ReportCusInforViewModel ret = new ReportCusInforViewModel();
            try
            {
                if (s["CustomerId"] != null)
                {
                    ret.CustomerId = Int32.Parse(s["CustomerId"].ToString());
                }

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

                if (s["CustomerLevelName"] != null)
                {
                    ret.CustomerLevelName = s["CustomerLevelName"].ToString();
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
    }
}

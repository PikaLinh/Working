using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Drawing.Printing;


namespace WebUI.Report
{
    public partial class DanhSachXuatNhapTonSP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderReport();

        }
        public void RenderReport()
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_DanhSachXuatNhapTonSp2";
                    //cmd.Parameters.AddWithValue("@CandidateID", CandidateID);
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    conn.Close();
                }
            }
            ds.Tables[0].TableName = "HeaderInfomation";
            ds.Tables[1].TableName = "Detail";

            ReportViewer1.Reset();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            //ReportViewer1.LocalReport.EnableHyperlinks = true;
            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("HeaderInfomation", ds.Tables[0]));
            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Detail", ds.Tables[1]));
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/DanhSachXuatNhapTonSP.rdlc");
            //ReportViewer1.LocalReport.DisplayName = "Phieu-dang-ki-tim-viec-KT-" + CreateDate + "-" + code;
            ReportViewer1.ServerReport.Refresh();
        }

    }
}
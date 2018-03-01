using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tkwz.DAL
{
    public  class Connect
    {
         CommandType ct = CommandType.StoredProcedure;
         public string strconn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //không có tham số 
        public  DataSet Get_Data_DataSet(ref string error, string strSQL)
        {
            using (SqlConnection Conn = new SqlConnection(strconn))
            {
                using (SqlCommand Comm = new SqlCommand(strSQL, Conn) { CommandType = ct })
                {
                    using (SqlDataAdapter adp = new SqlDataAdapter(Comm))
                    {
                        DataSet ds = new DataSet();
                        try
                        {
                            adp.Fill(ds);
                        }
                        catch (SqlException ex)
                        {
                            error = ex.Message;
                        }
                        return ds;
                    }
                }
            }
        }
        //trả ra 1 dataset có tham số
        public  DataSet Get_Data_DataSet(ref string error, string strSQL, params SqlParameter[] param)
        {
            CommandType ct = CommandType.StoredProcedure;
            using (SqlConnection Conn = new SqlConnection(strconn))
            {
                using (SqlCommand Comm = new SqlCommand(strSQL, Conn) { CommandType = ct })
                {
                    Array.ForEach(param, p => Comm.Parameters.Add(p));
                    using (SqlDataAdapter adp = new SqlDataAdapter(Comm))
                    {
                        DataSet ds = new DataSet();
                        try
                        {
                            adp.Fill(ds);
                        }
                        catch (SqlException ex)
                        {
                            error = ex.Message;
                        }
                        return ds;
                    }
                }
            }
        }
        //trả ra 1 datatable không có tham số
        public  DataTable Get_Data_DataTable(ref string error, string strSQL)
        {
            using (SqlConnection Conn = new SqlConnection(strconn))
            {
                using (SqlCommand Comm = new SqlCommand(strSQL, Conn) { CommandType = ct })
                {

                    using (SqlDataAdapter adp = new SqlDataAdapter(Comm))
                    {
                        DataTable dt = new DataTable();
                        try
                        {
                            adp.Fill(dt);
                        }
                        catch (SqlException ex)
                        {
                            error = ex.Message;
                        }
                        return dt;
                    }
                }
            }
        }
        //trả ra 1 datatable có tham số
        public  DataTable Get_Data_DataTable(ref string error, string strSQL, params SqlParameter[] param)
        {
            using (SqlConnection Conn = new SqlConnection(strconn))
            {
                using (SqlCommand Comm = new SqlCommand(strSQL, Conn) { CommandType = ct })
                {
                    Array.ForEach(param, p => Comm.Parameters.Add(p));
                    using (SqlDataAdapter adp = new SqlDataAdapter(Comm))
                    {
                        DataTable dt = new DataTable();
                        try
                        {
                            adp.Fill(dt);
                        }
                        catch (SqlException ex)
                        {
                            error = ex.Message;
                        }
                        return dt;
                    }
                }
            }
        }
        //xử lý 1 câu lệnh sql trả ra giá trị true hoặc false có tham số
        public  bool ExecuteSQl_bool(ref string error, string strSQL, params SqlParameter[] param)
        {
            using (SqlConnection Conn = new SqlConnection(strconn))
            {
                using (SqlCommand Comm = new SqlCommand(strSQL, Conn))
                {
                    bool f = false;
                    Comm.Parameters.Clear();
                    Comm.CommandText = strSQL;
                    Comm.CommandType = ct;
                    Array.ForEach(param, paramItem => Comm.Parameters.Add(paramItem));
                    try
                    {
                        if (Conn.State == ConnectionState.Open)
                        {
                            Conn.Close();
                        }
                        Conn.Open();
                        Comm.ExecuteNonQuery();
                        f = true;

                    }
                    catch (SqlException ex)
                    {
                        error = ex.Message;
                    }
                    finally
                    {
                        Conn.Close();
                    }
                    return f;
                }
            }

        }
        //xử lý 1 câu lệnh sql trả ra giá trị(dạng int) của store procedure trong câu lệnh sql có tham số
        public  int ExecuteSQl_int(ref string error, string strSQL, params SqlParameter[] param)
        {
            using (SqlConnection Conn = new SqlConnection(strconn))
            {
                using (SqlCommand Comm = new SqlCommand(strSQL, Conn))
                {
                    int kq = 0;
                    Comm.Parameters.Clear();
                    Comm.CommandText = strSQL;
                    Comm.CommandType = ct;
                    Array.ForEach(param, paramItem => Comm.Parameters.Add(paramItem));
                    try
                    {
                        if (Conn.State == ConnectionState.Open)
                        {
                            Conn.Close();
                        }
                        Conn.Open();
                        SqlParameter res = new SqlParameter("@res", SqlDbType.Int);
                        res.Direction = ParameterDirection.ReturnValue;
                        Comm.Parameters.Add(res);
                        Comm.ExecuteNonQuery();
                        kq = Convert.ToInt32(res.Value);

                    }
                    catch (SqlException ex)
                    {
                        error = ex.Message;
                    }
                    finally
                    {
                        Conn.Close();
                    }
                    return kq;
                }
            }

        }
        //lấy ra 1 giá trị duy nhất từ câu lệnh sql bằng đối tượng SqlDataReader có tham số
        public  string GetDataDepend(ref string error, string strSQL, params SqlParameter[] param)
        {
            using (SqlConnection Conn = new SqlConnection(strconn))
            {
                using (SqlCommand Comm = new SqlCommand(strSQL, Conn))
                {
                    Comm.Parameters.Clear();
                    Comm.CommandText = strSQL;
                    Comm.CommandType = ct;
                    string data = "";
                    Array.ForEach(param, paramItem => Comm.Parameters.Add(paramItem));
                    try
                    {
                        if (Conn.State == ConnectionState.Open)
                        {
                            Conn.Close();
                        }
                        Conn.Open();
                        SqlDataReader dr = Comm.ExecuteReader(CommandBehavior.CloseConnection);
                        dr.Read();
                        data = dr[0].ToString();
                        dr.Close();
                    }
                    catch (SqlException ex)
                    {
                        error = ex.Message;
                    }
                    finally
                    {
                        Conn.Close();
                    }
                    return data;
                }
            }
        }
        //lấy ra 1 giá trị duy nhất từ câu lệnh sql bằng đối tượng SqlDataReader không có tham số
        public  string GetDataDepend(ref string error, string strSQL)
        {
            using (SqlConnection Conn = new SqlConnection(strconn))
            {
                using (SqlCommand Comm = new SqlCommand(strSQL, Conn))
                {
                    Comm.Parameters.Clear();
                    Comm.CommandText = strSQL;
                    Comm.CommandType = ct;
                    string data = "";
                    try
                    {
                        if (Conn.State == ConnectionState.Open)
                        {
                            Conn.Close();
                        }
                        Conn.Open();
                        SqlDataReader dr = Comm.ExecuteReader(CommandBehavior.CloseConnection);
                        dr.Read();
                        data = dr[0].ToString();
                        dr.Close();
                    }
                    catch (SqlException ex)
                    {
                        error = ex.Message;
                    }
                    finally
                    {
                        Conn.Close();
                    }
                    return data;
                }
            }
        }
        //so sánh từ ngày đến ngày 
        public  bool CompareDate(string frmdate, string todate)
        {
            DateTime dateTest;
            DateTime dateTest1;
            System.Globalization.DateTimeFormatInfo sFormat = new System.Globalization.DateTimeFormatInfo();
            sFormat.ShortDatePattern = "dd/MM/yyyy";

            dateTest = Convert.ToDateTime(frmdate, sFormat);
            dateTest1 = Convert.ToDateTime(todate, sFormat);
            string frDate = String.Format("{0:yyyy/MM/dd}", dateTest);
            string toDate = String.Format("{0:yyyy/MM/dd}", dateTest1);

            int FromMonth = Convert.ToInt32(frmdate.Substring(3, 2));
            int FromYear = Convert.ToInt32(frmdate.Substring(6, 4));
            int ToMonth = Convert.ToInt32(todate.Substring(3, 2));
            int ToYear = Convert.ToInt32(todate.Substring(6, 4));
            double max = ToYear * 12 + ToMonth;
            double i = FromYear * 12 + FromMonth;
            if (max < i)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //Get MD5
        public string GetMd5Sum(string str)
        {
            // First we need to convert the string into bytes, which
            // means using a text encoder.

            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
            // Create a buffer large enough to hold the string

            byte[] unicodeText = new byte[str.Length * 2];

            enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

            // Now that we have a byte array we can ask the CSP to hash it

            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] result = md5.ComputeHash(unicodeText);

            // Build the final string by converting each byte

            // into hex and appending it to a StringBuilder

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {

                sb.Append(result[i].ToString("X2"));

            }
            // And return it
            return sb.ToString();
        }
        //check numeric
        public  bool IsNumeric(string anyString)
        {
            if (anyString == null)
            {
                anyString = "";
            }
            if (anyString.Length > 0)
            {
                double dummyOut = new double();
                System.Globalization.CultureInfo cultureInfo =
                    new System.Globalization.CultureInfo("en-US", true);

                return Double.TryParse(anyString, System.Globalization.NumberStyles.Any,
                    cultureInfo.NumberFormat, out dummyOut);
            }
            else
            {
                return false;
            }
        }
        //write log error
        public void WriteErrorLog(string path, string sErrMsg, string method)
        {
            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            string sErrorTime = sYear + sMonth + sDay;


            string sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
            string sRet = oInfo.FullName;

            StreamWriter sw = new StreamWriter(path + sErrorTime, true);
            sw.WriteLine(sLogFormat + "\t 1.Page: " + sRet + "\n\t" + "2.Event: " + method + "\n\t" + "3.Detail: " + sErrMsg);
            sw.Flush();
            sw.Close();
        }
        //write login
        public  void LoginLog(string path, string User, string IP)
        {
            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            string sErrorTime = sYear + sMonth + sDay;

            StreamWriter sw = new StreamWriter(path + sErrorTime, true);
            sw.WriteLine(sLogFormat + "\t 1.User: " + User + "\n\t" + "2.IP Address: " + IP);
            sw.Flush();
            sw.Close();
        }
        //bind to drop down
        public  void BindDropDownList(DataTable db, DropDownList NameDropDownList, string DataValue, string DataText)
        {
            NameDropDownList.DataSource = db;
            NameDropDownList.DataTextField = DataText;
            NameDropDownList.DataValueField = DataValue;
            NameDropDownList.DataBind();
        }
        //download file
        public  void saveFile(System.Web.HttpResponse Response, string PathfileName)
        {
            string filename = PathfileName;
            if (filename != "")
            {
                string path = filename;
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                if (file.Exists)
                {
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.WriteFile(file.FullName);
                    Response.End();

                }
            }

        }
        //chạy vòng lặp GridView ,nối chuỗi và trả ra chuỗi đó ,lưu ý : dùng cho CheckBox và Label
        public  string GenerateStringInGridViewLoop(GridView grv, string NameControlCheckBox, string NameControlLabel)
        {
            string Array = "";

            for (int i = 0; i < grv.Rows.Count; i++)
            {
                CheckBox ch = (CheckBox)grv.Rows[i].FindControl(NameControlCheckBox);
                if (ch.Checked)
                {

                    Label lbl = (Label)grv.Rows[i].FindControl(NameControlLabel);
                    Array = Array + lbl.Text + ",";
                }
            }
            if (Array != "")
            {
                return Array.Substring(0, Array.Length - 1);
            }
            else
            {
                return Array;
            }
        }
        //chạy vòng lặp GridView ,nối chuỗi và trả ra chuỗi đó ,lưu ý : dùng cho CheckBox và Label
        public  string GetLabelInGridViewByRowcommand(string commandName, GridViewCommandEventArgs e, string NameControlLabel)
        {

            if (e.CommandName == commandName)
            {

                GridViewRow grv = ((Control)e.CommandSource).NamingContainer as GridViewRow;
                Label lbl = (Label)grv.FindControl(NameControlLabel);
                return lbl.Text;
            }
            else
            {
                return "";
            }

        }
        //Tạo ngẫu nhiên password, code khôi phục mật khẩu
        public  string GenerateRandomPassword(int length)
        {
            string allowedLetterChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
            string allowedNumberChars = "0123456789";
            char[] chars = new char[length];
            Random rd = new Random();
            bool useLetter = true;
            for (int i = 0; i < length; i++)
            {
                if (useLetter)
                {
                    chars[i] = allowedLetterChars[rd.Next(0, allowedLetterChars.Length)];
                    useLetter = false;
                }
                else
                {
                    chars[i] = allowedNumberChars[rd.Next(0, allowedNumberChars.Length)];
                    useLetter = true;
                }
            }
            return new string(chars);
        }
    }
}

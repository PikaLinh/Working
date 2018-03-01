using EntityModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Repository
{
    public class SendSMSRepository
    {
        const string APIKey = "9526B130F96AB0DE840763935AA0E6";//Dang ky tai khoan tai esms.vn de lay key//Register account at esms.vn to get key
        const string SecretKey = "01002B6F633315BDD3EF93E7A46612";
        bool isTestSMS
        {
            get
            {
                return bool.Parse(WebConfigurationManager.AppSettings["isTestSMS"].ToString());
            }
        }

        public bool SendBrandname(string phone, string message)
        {
            try
            {
                    string url = "http://api.esms.vn/MainService.svc/xml/SendMultipleSMSBrandname/";
                    // declare ascii encoding
                    UTF8Encoding encoding = new UTF8Encoding();

                    string strResult = string.Empty;
                    // sample xml sent to Service & this data is sent in POST

                    string customers = "";

                    string[] lstPhone = phone.Split(',');

                    for (int i = 0; i < lstPhone.Count(); i++)
                    {
                        customers = customers + @"<CUSTOMER>"
                                        + "<PHONE>" + lstPhone[i] + "</PHONE>"
                                        + "</CUSTOMER>";
                    }


                    string SampleXml = @"<RQST>"
                                       + "<APIKEY>" + APIKey + "</APIKEY>"
                                       + "<SECRETKEY>" + SecretKey + "</SECRETKEY>"
                                       + "<SMSTYPE>2</SMSTYPE>"
                                       + "<BRANDNAME>Chic_Cut</BRANDNAME>"
                                       + "<CONTENT>" + message + "</CONTENT>"
                                       + "<CONTACTS>" + customers + "</CONTACTS>";
                    if (isTestSMS)
                    {
                        //Sandbox Thử nghiệm 1 || Thật 0
                        SampleXml +=  "<SANDBOX>1</SANDBOX>";
                    }

                    SampleXml += "</RQST>";
                    string postData = SampleXml.Trim().ToString();
                    // convert xmlstring to byte using ascii encoding
                    byte[] data = encoding.GetBytes(postData);
                    // declare httpwebrequet wrt url defined above
                    HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
                    // set method as post
                    webrequest.Method = "POST";
                    webrequest.Timeout = 500000;
                    // set content type
                    webrequest.ContentType = "application/x-www-form-urlencoded";
                    // set content length
                    webrequest.ContentLength = data.Length;
                    // get stream data out of webrequest object
                    Stream newStream = webrequest.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                    // declare & read response from service
                    HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();

                    // set utf8 encoding
                    Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                    // read response stream from response object
                    StreamReader loResponseStream =
                        new StreamReader(webresponse.GetResponseStream(), enc);
                    // read string from stream data
                    strResult = loResponseStream.ReadToEnd();
                    // close the stream object
                    loResponseStream.Close();
                    // close the response object
                    webresponse.Close();
                    // below steps remove unwanted data from response string
                    strResult = strResult.Replace("</string>", "");
                    return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void SendSMSModel(AY_SMSCalendar sms)
        {
            Thread thread = new Thread(() => SendBrandname(sms.SMSTo, sms.SMSContent));
            thread.Start();
        }

    }
}

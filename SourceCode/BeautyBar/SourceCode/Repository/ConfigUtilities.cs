using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Repository
{
    public class ConfigUtilities
    {
        /// <summary>
        /// The the value by the key from web.config file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSysConfigAppSetting(string key)
        {
            try
            {
                string value = string.Empty;
                // if (HttpContext.Current == null)
                value = ConfigurationManager.AppSettings[key];
                //  else
                //      value = WebConfigurationManager.AppSettings[key];
                if (value.EndsWith("/")) // Tien fix here for ip that end with / will remove it on web.config
                    value = value.TrimEnd('/'); 
                return value;

            }
            catch
            {
                // process exception
                return string.Empty;
            }
        }

        /// <summary>
        /// The the value by the key from web.config file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetSysConfigAppSettingInt(string key)
        {
            try
            {
                string value = GetSysConfigAppSetting(key);
                return Int32.Parse(value);
            }
            catch
            {
                // process exception
                return -1;
            }
        }

        /// <summary>
        /// Get the Data path root
        /// </summary>
        /// <returns></returns>
        public static string GetDataPath()
        {
            return GetSysConfigAppSetting("DataPath");
        }

        public static string GetDataTemp()
        {
            return GetSysConfigAppSetting("DataTemp");
        }

        /// <summary>
        /// Get the allowed image extension.
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllowedImageExtension()
        {
            string extensions = GetSysConfigAppSetting("AllowedImageExtension");
            if (!string.IsNullOrEmpty(extensions))
                return extensions.Split(',');
            return new string[] { };
        }

        /// <summary>
        /// Get Max File Length
        /// </summary>
        /// <returns></returns>
        public static int GetMaxFileLength()
        {
            int maxFileLength;
            int.TryParse(GetSysConfigAppSetting("MaxFileLength"), out maxFileLength);
            return maxFileLength;
        }

        /// <summary>
        /// Get NoImage link
        /// </summary>
        /// <returns></returns>
        /// 
        
            
        public static string GetEmail()
        {
            return GetSysConfigAppSetting("Email");
        }
        
        public static string GetEmailPassword()
        {
            return GetSysConfigAppSetting("Password");
        }

        public static string GetNoImageLink()
        {
            return GetSysConfigAppSetting("NoImage");
        }

        public static string GetNoImageProfileLink()
        {
            return GetSysConfigAppSetting("NoImageProfile");
        }

        public static string GetNoImageCompanyLink()
        {
            return GetSysConfigAppSetting("NoImageCompany");
        }

        public static string GetAPIKEY()
        {
            return GetSysConfigAppSetting("API_KEY");
        }

        public static string GetAPISECRET()
        {
            return GetSysConfigAppSetting("API_SECRET");
        }

        public static string GetIpUrl()
        {
            return GetSysConfigAppSetting("IPUrl");
        }

        public static string GetPayPalUrl()
        {
            return GetSysConfigAppSetting("PayPalUrl");
        }

        public static string GetPaymentSuccess()
        {
            return GetSysConfigAppSetting("PaymentSuccess");
        }

        public static string GetPaymentFail()
        {
            return GetSysConfigAppSetting("PaymentFail");
        }

        public static string GetBarCodePrefix()
        {
            return GetSysConfigAppSetting("BarCodePrefix");
        }

        public static string SmtpServer()
        {
            return GetSysConfigAppSetting("ErrorEmailSmtpServer");
        }
        public static string EmailLogin()
        {
            return GetSysConfigAppSetting("ErrorEmailSmtpAccount");
        }
        public static string EmailPassword()
        {
            return GetSysConfigAppSetting("ErrorEmailSmtpPassword");
        }
        public static int EmailPort()
        {
            return GetSysConfigAppSettingInt("ErrorEmailSmtpPort");
        }
        public static string EmailTo()
        {
            return GetSysConfigAppSetting("ErrorEmailReceiver");
        }
        public static string EmailFrom()
        {
            return GetSysConfigAppSetting("ErrorEmailSender");
        }

        public static int NbDaysBeforeEnding()
        {
            return GetSysConfigAppSettingInt("NbDaysBeforeEnding");
        }

        public static int TimeToStartSendingEmailThread()
        {
            return GetSysConfigAppSettingInt("TimeToStartSendingEmailThread");
        }

        public static int TimeToResetSendingEmailThread()
        {
            return GetSysConfigAppSettingInt("TimeToResetSendingEmailThread");
        }

        public static string DayOfWeekToStartSendingEmailThread()
        {
            return GetSysConfigAppSetting("DayOfWeekToStartSendingEmailThread");
        }

        // Return domain name NO include  => http://wizhuntlocals.com
        public static string GetDomainName()
        {
            return GetSysConfigAppSetting("DomainName").TrimEnd('/'); 
        }
    }
}

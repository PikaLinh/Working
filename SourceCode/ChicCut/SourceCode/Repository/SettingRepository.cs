using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModels;

namespace Repository
{

    public class SettingRepository 
    {
        public EntityDataContext context = new EntityDataContext();
        public string GetDetailByKey(string SettingName, string LanguageId = "vi-vn")
        {
            Website_SettingModel set = new Website_SettingModel();
            if (LanguageId.ToLower() == "vi-vn")
            {
                set = context.Website_SettingModel.SingleOrDefault(p => p.SettingName == SettingName);
            }else
            {
                set = context.Website_SettingModel.SingleOrDefault(p => p.SettingName == (SettingName + "En"));
            }
            
            if (set != null)
            {
                return set.Details;
            }else
            {
                return "";
            }
        }
    }
}

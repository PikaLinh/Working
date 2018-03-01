using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository
{
    public enum Status
    {
        TaoMoi = 1,
        DaGui = 2,
        DaTiepNhan = 3,
        TuChoi = 4,
        Huy = 5
    }
    public enum RolesEnum
    {
        Admin = 1,
        User = 2
    }

    //public static class SAPResStatus
    //{
    //    public static string SUCCESS  = "SUCCESS";
    //    public static string FAIL = "FAIL";
    //}
    public enum LanguageEnum
    {
        VietNamese = 10001,
        English = 10002
    }
}
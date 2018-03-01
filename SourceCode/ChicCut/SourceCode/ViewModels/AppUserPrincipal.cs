using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AppUserPrincipal : ClaimsPrincipal
    {
        public AppUserPrincipal(ClaimsPrincipal principal)
            : base(principal)
        {
        }

        public string Name
        {
            get
            {
                return this.FindFirst(ClaimTypes.Name).Value;
            }
        }
        public int UserId 
        {
            get
            {
                return Convert.ToInt32(this.FindFirst(ClaimTypes.Sid).Value);
            }
        }
        public bool isAdmin
        {
            get
            {
                return RolesId == 1;
            }
        }
        public int RolesId 
        {
            get
            {
                return Convert.ToInt32(this.FindFirst(ClaimTypes.Role).Value);
            }
        }

        public int? EmpId
        {
            get {
                string emp = this.FindFirst(ClaimTypes.PrimarySid).Value;
                if (string.IsNullOrEmpty(emp))
                {
                    return null;
                }else
                {
                    return Convert.ToInt32(emp);
                }
            }
        }
    }
}

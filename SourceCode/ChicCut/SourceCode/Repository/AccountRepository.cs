using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Web;
using EntityModels;
using ViewModels;

namespace Repository
{
    public class AccountRepository
    {
        EntityDataContext _context;
        public AccountRepository(EntityDataContext ct)
        {
            _context = ct;
        }
        
        public bool UserExists(string UserName)
        {
            var usr = _context.AccountModel.FirstOrDefault(d => d.UserName == UserName);
            return (usr != null);
        }

        public AccountModel Find(int UserId)
        {
            return _context.AccountModel.Find(UserId);
        }
        public bool UpdateNormal(AccountModel model, out string errorMessage)
        {
            // làm ơn đừng code gì trong này nữa nha
            // update bình thường thôi
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    errorMessage = "";
                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();

                    scope.Complete();
                    return true;
                }
                catch
                {
                    errorMessage = Resources.LanguageResource.SystemError;
                    return false;
                }
            }
        }
    }
}
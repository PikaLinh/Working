using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModels;

namespace Repository
{
    public class AM_AccountRepository
    {
        EntityDataContext _context;
        public AM_AccountRepository(EntityDataContext ct)
        {
            _context = ct;
        }

        public bool CodeAccountExists(string Code, int StoreId)
        {
            Code = Code.ToUpper();
            var amc = _context.AM_AccountModel.FirstOrDefault(p => p.Code == Code && p.StoreId == StoreId && p.Actived == true);
            return (amc != null);
        }
    }
}

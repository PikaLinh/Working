using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerRepository
    {
        EntityDataContext _context;
        public CustomerRepository(EntityDataContext ct)
        {
            _context = ct;
        }

        public bool PhoneExists(string Phone)
        {
            var usr = _context.CustomerModel.FirstOrDefault(d => d.Phone == Phone);
            return (usr != null);
        }
    }
}

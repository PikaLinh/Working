using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ServiceViewModel
    {
        //Mã dịch vụ
        public int ServiceId { get; set; }
        public Nullable<int> ServiceCategoryId { get; set; }
        //Tên dịch vụ
        public string ServiceName { get; set; }
        //Giá
        public Nullable<decimal> Price { get; set; }
        //Danh mục dịch vụ
        public Nullable<int> ServiceParentCategoryId { get; set; }
    }
}

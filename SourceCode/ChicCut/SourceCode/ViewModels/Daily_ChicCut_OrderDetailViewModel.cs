using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;
using System.Web.Mvc;

namespace ViewModels
{
    public class Daily_ChicCut_OrderDetailViewModel : Daily_ChicCut_OrderDetailModel
    {
        public int STT { get; set; }
        public string ServiceName { get; set; }

        public IEnumerable<SelectListItem> QuantificationMasterList { get; set; }

        //Bán sản phẩm
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        //public int EndInventoryQty { get; set; }
       // public string ProductCode { get; set; }
        //Loại dịch vụ
        public Nullable<int> ServiceCategoryId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebUI.Controllers
{
    public class ImportProductView
    {
        public HttpPostedFileBase excelfile { get; set; }
        public int? StoreId { get; set; }
    }
}

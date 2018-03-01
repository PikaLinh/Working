using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ProductSearchViewModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int ? ProductId { get; set; }
        public int ? CategoryId { get; set; }
        public int  CustomerLevelId { get; set; }
        public int ? OriginOfProductId { get; set; }
        public int? ProductStatusId { get; set; }
        public bool Actived { get; set; }
        public bool Valid { get; set; }
        public decimal? txtkhoanggiatren { get; set; }
        public decimal? txtkhoanggiaduoi { get; set; }
        public string Specifications { get; set; }
        public int? ProductStatust { get; set; }
        // Mã vạch
        // 1. null => tất cả
        // 2. true => có mã vạch
        // 3. false => chưa có mã vạch
        public bool? isHasBarcode { get; set; }
        public bool? isParentProduct { get; set; }
        public int? ParentProductId { get; set; }

    }
}

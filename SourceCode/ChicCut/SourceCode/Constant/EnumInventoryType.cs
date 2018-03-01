using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Constant
{
    public class EnumInventoryType
    {
        public const int NC = 1;//Nhập - Nhập hàng từ nhà cung cấp
        public const int XC = 2;// Xuất - Trả hàng cho nhà cung cấp
        public const int NK = 3; //Nhập - XNK
        public const int XK = 4;// Xuất - XNK
        public const int XB = 5;// Xuất - Bán hàng
        public const int NB = 6;// Nhập - Khách trả hàng
        public const int ĐK = 7;// Đầu kỳ (Thêm mới sản phẩm)
        public const int KK = 8;// Kiểm kho
    }
}

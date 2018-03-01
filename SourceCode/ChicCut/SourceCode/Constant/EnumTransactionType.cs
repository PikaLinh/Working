using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Constant
{
    public class EnumTransactionType
    {
        public const string BHBAN = "BHBAN";//Phiếu bán hàng : Bán hàng
        public const string BHTRA = "BHTRA";// Phiếu trả hàng : Bán hàng

        public const string NHNOP = "NHNOP"; //Báo có (nộp tiền) : Ngân hàng
        public const string NHRUT = "NHRUT";// Báo nợ (rút tiền) : Ngân hàng

        public const string NXNHAP ="NXNHAP";// Phiếu nhập : NHAPXUAT
        public const string NXXUAT ="NXXUAT";// Phiếu xuất : NHAPXUAT
                                    
        public const string TCCHI = "TCCHI";// Phiếu chi tiền mặt : TIENMAT
        public const string TCTHU = "TCTHU";// Phiếu thu tiền mặt : TIENMAT
    }
}

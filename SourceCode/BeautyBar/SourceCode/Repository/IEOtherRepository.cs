using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class IEOtherRepository
    {
        public EntityDataContext _context { get; set; }
        public IEOtherRepository(EntityDataContext db)
        {
            this._context = db;
        }

        public string GetIEOtherCode(bool IsImport)
        {
            // Tìm giá trị STT order code
            string OrderCodeToFind = "";
            if (IsImport)
            {
                OrderCodeToFind = string.Format("{0}-{1}{2}", "PNO", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            }
            else
            {
                OrderCodeToFind = string.Format("{0}-{1}{2}", "PXO", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            }
            var Resuilt = _context.IEOtherMasterModel.OrderByDescending(p => p.IEOtherMasterId).Where(p => p.IEOtherMasterCode.Contains(OrderCodeToFind)).Select(p => p.IEOtherMasterCode).FirstOrDefault();
            string OrderCode = "";
            if (Resuilt != null)
            {
                int LastNumber = Convert.ToInt32(Resuilt.Substring(9)) + 1;
                string STT = "";
                switch (LastNumber.ToString().Length)
                {
                    case 1: STT = "000" + LastNumber.ToString(); break;
                    case 2: STT = "00" + LastNumber.ToString(); break;
                    case 3: STT = "0" + LastNumber.ToString(); break;
                    default: STT = LastNumber.ToString(); break;
                }
                OrderCode = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT);
            }
            else
            {
                OrderCode = string.Format("{0}-{1}", OrderCodeToFind, "0001");
            }
            return OrderCode;
        }

        public string GetIEOtherCodePKK()
        {
            // Tìm giá trị STT order code
            string OrderCodeToFind = "";
            OrderCodeToFind = string.Format("{0}-{1}{2}", "PKK", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var Resuilt = _context.IEOtherMasterModel.OrderByDescending(p => p.IEOtherMasterId).Where(p => p.IEOtherMasterCode.Contains(OrderCodeToFind)).Select(p => p.IEOtherMasterCode).FirstOrDefault();
            string OrderCode = "";
            if (Resuilt != null)
            {
                int LastNumber = Convert.ToInt32(Resuilt.Substring(9)) + 1;
                string STT = "";
                switch (LastNumber.ToString().Length)
                {
                    case 1: STT = "000" + LastNumber.ToString(); break;
                    case 2: STT = "00" + LastNumber.ToString(); break;
                    case 3: STT = "0" + LastNumber.ToString(); break;
                    default: STT = LastNumber.ToString(); break;
                }
                OrderCode = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT);
            }
            else
            {
                OrderCode = string.Format("{0}-{1}", OrderCodeToFind, "0001");
            }
            return OrderCode;
        }

        public string GetIEOtherCode()
        {
            // Tìm giá trị STT order code
            string OrderCodeToFind = string.Format("{0}-{1}{2}", "PTD", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var Resuilt = _context.IEOtherMasterModel.OrderByDescending(p => p.IEOtherMasterId).Where(p => p.IEOtherMasterCode.Contains(OrderCodeToFind)).Select(p => p.IEOtherMasterCode).FirstOrDefault();
            string OrderCode = "";
            if (Resuilt != null)
            {
                int LastNumber = Convert.ToInt32(Resuilt.Substring(9)) + 1;
                string STT = "";
                switch (LastNumber.ToString().Length)
                {
                    case 1: STT = "000" + LastNumber.ToString(); break;
                    case 2: STT = "00" + LastNumber.ToString(); break;
                    case 3: STT = "0" + LastNumber.ToString(); break;
                    default: STT = LastNumber.ToString(); break;
                }
                OrderCode = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT);
            }
            else
            {
                OrderCode = string.Format("{0}-{1}", OrderCodeToFind, "0001");
            }
            return OrderCode;
        }
    }
}

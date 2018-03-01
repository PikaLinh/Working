using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class ProductRepository
    {
        public EntityDataContext _context { get; set; }
        public ProductRepository(EntityDataContext db)
        {
            this._context = db;
        }
        public bool CodeProductExists(string ProductCode)
        {
            ProductCode = ProductCode.ToUpper();
            var amc = _context.ProductModel.FirstOrDefault(p => p.ProductCode == ProductCode && p.Actived == true);
            return (amc != null);
        }

        public string GetProdcutStoreCode(int StoreId, int ProductTypeId, int? CategoryId)
        {
            // Lấy StoreCode
            var StoreCode = _context.StoreModel.Where(p => p.StoreId == StoreId).Select(p => p.StoreCode).FirstOrDefault();
            // Lấy ProductTypeCode
            var ProductTypeCode = _context.ProductTypeModel.Where(p => p.ProductTypeId == ProductTypeId).Select(p => p.ProductTypeCode).FirstOrDefault();
            // Lấy CategoryCode
            var CategoryCode = _context.CategoryModel.Where(p => p.CategoryId == CategoryId).Select(p => p.CategoryNameEn).FirstOrDefault();
            // Tìm giá trị STT ProductStoreCode
            string ProductStoreCodeToFind = string.Format("{0}-{1}{2}", StoreCode, ProductTypeCode, CategoryCode);
            var Resuilt = _context.ProductModel.OrderByDescending(p => p.ProductStoreCode)
                                               .Where(p => p.ProductStoreCode.Contains(ProductStoreCodeToFind))
                                               .Select(p => p.ProductStoreCode).FirstOrDefault();
            string ProductStoreCode = "";
            if (Resuilt != null)
            {
                //int LastNumber = Convert.ToInt32(Resuilt.Substring(9)) + 1;
                int DauGachNgangThu2 = Resuilt.IndexOf("-", Resuilt.IndexOf("-") + 1);//ví dụ : XB-GM-0001 => kq : 6
                int LastNumber = Convert.ToInt32(Resuilt.Substring(DauGachNgangThu2 + 1)) + 1;//kq : 2
                string STT = "";
                switch (LastNumber.ToString().Length)
                {
                    case 1: STT = "000" + LastNumber.ToString(); break;
                    case 2: STT = "00" + LastNumber.ToString(); break;
                    case 3: STT = "0" + LastNumber.ToString(); break;
                    default: STT = LastNumber.ToString(); break;
                }
                ProductStoreCode = string.Format("{0}{1}", Resuilt.Substring(0, DauGachNgangThu2 + 1), STT);
            }
            else
            {
                ProductStoreCode = string.Format("{0}-{1}", ProductStoreCodeToFind, "0001");
            }
            return ProductStoreCode;
        }

        public string GetDynamicProductStoreCode(int StoreId, int? CategoryId)
        {
            // Lấy StoreCode
            var StoreCode = _context.StoreModel.Where(p => p.StoreId == StoreId).Select(p => p.StoreCode).FirstOrDefault();
            // Lấy CategoryCode
            var CategoryCode = _context.CategoryModel.Where(p => p.CategoryId == CategoryId).Select(p => p.CategoryNameEn).FirstOrDefault();
            // Tìm giá trị STT ProductStoreCode
            string ProductStoreCodeToFind = string.Format("{0}-{1}", StoreCode, CategoryCode);
            var Resuilt = _context.ProductModel.OrderByDescending(p => p.ProductStoreCode)
                                               .Where(p => p.ProductStoreCode.Contains(ProductStoreCodeToFind))
                                               .Select(p => p.ProductStoreCode).FirstOrDefault();
            string ProductStoreCode = "";
            if (Resuilt != null)
            {
                //int LastNumber = Convert.ToInt32(Resuilt.Substring(9)) + 1;
                int DauGachNgangThu2 = Resuilt.IndexOf("-", Resuilt.IndexOf("-") + 1);//ví dụ : XB-GM-0001 => kq : 6
                int LastNumber = Convert.ToInt32(Resuilt.Substring(DauGachNgangThu2 + 1)) + 1;//kq : 2
                string STT = "";
                switch (LastNumber.ToString().Length)
                {
                    case 1: STT = "000" + LastNumber.ToString(); break;
                    case 2: STT = "00" + LastNumber.ToString(); break;
                    case 3: STT = "00" + LastNumber.ToString(); break;
                    default: STT = LastNumber.ToString(); break;
                }
                ProductStoreCode = string.Format("{0}-{1}", Resuilt.Substring(0, DauGachNgangThu2), STT);
            }
            else
            {
                ProductStoreCode = string.Format("{0}-{1}", ProductStoreCodeToFind, "0001");
            }
            return ProductStoreCode;
        }

        public string GetProdcutStoreCode(string StoreCode, string ProductTypeCode)
        {
            // Tìm giá trị STT ProductStoreCode
            string ProductStoreCodeToFind = string.Format("{0}-{1}", StoreCode, ProductTypeCode);
            var Resuilt = _context.ProductModel.OrderByDescending(p => p.ProductStoreCode)
                                               .Where(p => p.ProductStoreCode.Contains(ProductStoreCodeToFind))
                                               .Select(p => p.ProductStoreCode).FirstOrDefault();
            string ProductStoreCode = "";
            if (Resuilt != null)
            {
                //int LastNumber = Convert.ToInt32(Resuilt.Substring(9)) + 1;
                int DauGachNgangThu2 = Resuilt.IndexOf("-", Resuilt.IndexOf("-") + 1);//ví dụ : XB-GM-0001 => kq : 6
                int LastNumber = Convert.ToInt32(Resuilt.Substring(DauGachNgangThu2 + 1)) + 1;//kq : 2
                string STT = "";
                switch (LastNumber.ToString().Length)
                {
                    case 1: STT = "000" + LastNumber.ToString(); break;
                    case 2: STT = "00" + LastNumber.ToString(); break;
                    case 3: STT = "0" + LastNumber.ToString(); break;
                    default: STT = LastNumber.ToString(); break;
                }
                ProductStoreCode = string.Format("{0}{1}", Resuilt.Substring(0, DauGachNgangThu2 + 1), STT);
            }
            else
            {
                ProductStoreCode = string.Format("{0}-{1}", ProductStoreCodeToFind, "0001");
            }
            return ProductStoreCode;
        }

        public string GetProdcutStoreCodeDuplicate(int StoreId, int ProductTypeId, int? CategoryId, string ProductStoreCodeMark)
        {
            string ProductStoreCode = "";
            try
            {
                // Bước 1 : kiểm tra count ProductStoreCodeMark >= 2 thì thêm mã mới
                int CountProductStoreCode = _context.ProductModel.Count(p => p.ProductStoreCode == ProductStoreCodeMark);
                if (CountProductStoreCode >= 2 || ProductStoreCodeMark == "")
                {
                    // Lấy StoreCode
                    var StoreCode = _context.StoreModel.Where(p => p.StoreId == StoreId).Select(p => p.StoreCode).FirstOrDefault();
                    // Lấy ProductTypeCode
                    var ProductTypeCode = _context.ProductTypeModel.Where(p => p.ProductTypeId == ProductTypeId).Select(p => p.ProductTypeCode).FirstOrDefault();
                    // Lấy CategoryCode
                    var CategoryCode = _context.CategoryModel.Where(p => p.CategoryId == CategoryId).Select(p => p.CategoryNameEn).FirstOrDefault();
                    // Tìm giá trị STT ProductStoreCode
                    string ProductStoreCodeToFind = string.Format("{0}-{1}{2}", StoreCode, ProductTypeCode, CategoryCode);
                    var Resuilt = _context.ProductModel.OrderByDescending(p => p.ProductStoreCode)
                                                       .Where(p => p.ProductStoreCode.Contains(ProductStoreCodeToFind))
                                                       .Select(p => p.ProductStoreCode).FirstOrDefault();
                    if (Resuilt != null)
                    {
                        //int LastNumber = Convert.ToInt32(Resuilt.Substring(9)) + 1;
                        int DauGachNgangThu2 = Resuilt.IndexOf("-", Resuilt.IndexOf("-") + 1);//ví dụ : XB-GM-0001 => kq : 6
                        int LastNumber = Convert.ToInt32(Resuilt.Substring(DauGachNgangThu2 + 1)) + 1;//kq : 2
                        string STT = "";
                        switch (LastNumber.ToString().Length)
                        {
                            case 1: STT = "000" + LastNumber.ToString(); break;
                            case 2: STT = "00" + LastNumber.ToString(); break;
                            case 3: STT = "0" + LastNumber.ToString(); break;
                            default: STT = LastNumber.ToString(); break;
                        }
                        ProductStoreCode = string.Format("{0}{1}", Resuilt.Substring(0, DauGachNgangThu2 + 1), STT);
                    }
                    return ProductStoreCode;
                }
                else
                {
                    return ProductStoreCodeMark;
                }
            }
            catch
            {
                return ProductStoreCodeMark;
            }

        }
    }
}

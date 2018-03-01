using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModels;
using ViewModels;
using System.Transactions;
using Constant;

namespace Repository
{
   public class ImportMasterRepository
    {
       EntityDataContext _context ;
       public ImportMasterRepository(EntityDataContext ct)
       {
           _context = ct;
       }

        #region Save 
       public string Save(ImportMasterModel model, List<ImportDetailViewModel> detail, decimal TotalShippingWeight, decimal? GuestAmountPaid, DateTime ExchangeDate, int CreateReceipt, string UserName, int EmployeeId)
       {
           try
           {
               using (TransactionScope ts = new TransactionScope())
               {
                   var currentTime = DateTime.Now;

                   #region ImportMaster
                   model.CreatedDate = currentTime;
                   model.CreatedAccount = UserName;
                   model.CreatedEmployeeId = EmployeeId;
                   model.InventoryTypeId = EnumInventoryType.NC;
                   model.Paid = GuestAmountPaid.HasValue ? GuestAmountPaid : 0;
                   model.ImportMasterCode = GetImportMasterCode();
                   //Thêm tổng công nợ cộng dồn = nợ cũ + nợ mới 
                   //decimal? SuplierOldDebt = _context.ImportMasterModel
                   //                                  .Where(p => p.SupplierId == model.SupplierId)
                   //                                  .OrderByDescending(p => p.ImportMasterId)
                   //                                  .Select(p => p.RemainingAmountAccrued)
                   //                                  .FirstOrDefault();
                   decimal? SuplierOldDebt = _context.AM_DebtModel
                                                       .Where(p => p.SupplierId == model.SupplierId)
                                                       .OrderByDescending(p => p.TimeOfDebt)
                                                       .Select(p => p.RemainingAmountAccrued)
                                                       .FirstOrDefault();
                   SuplierOldDebt = (SuplierOldDebt == null) ? 0 : SuplierOldDebt.Value;
                   model.RemainingAmount = (model.RemainingAmount == null) ? 0 : model.RemainingAmount.Value;
                   model.RemainingAmountAccrued = SuplierOldDebt.Value + model.RemainingAmount.Value;

                   _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                   _context.SaveChanges(); // LƯU TẠM ĐỂ LẤY IMPORTMASTERID (SẼ BỊ SCROLLBACK KHI XẢY RA LỖI)
                   #endregion

                   #region Kế toán
                   if (CreateReceipt == 1)
                   {
                       #region Thêm vào giao dịch kế toán
                       AM_TransactionModel AMmodel;

                       #region TH1 : Trả đủ

                       if (model.TotalPrice == GuestAmountPaid)
                       {
                           AMmodel = new AM_TransactionModel()
                           {
                               StoreId = model.StoreId,
                               AMAccountId = (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.TM && p.AMAccountTypeCode == EnumAM_AccountType.TIENMAT && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                               TransactionTypeCode = EnumTransactionType.NXNHAP,
                               ContactItemTypeCode = EnumContactType.NCC,
                               CustomerId = null,
                               SupplierId = model.SupplierId,
                               EmployeeId = null,
                               OtherId = null,
                               Amount = GuestAmountPaid,
                               OrderId = null,
                               ImportMasterId = model.ImportMasterId,
                               IEOtherMasterId = null,
                               Note = model.Note,
                               CreateDate = currentTime,
                               CreateEmpId = EmployeeId,
                               RemainingAmountAccrued = model.RemainingAmountAccrued
                           };
                           _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                           _context.SaveChanges();
                       }
                       #endregion

                       #region TH2 : Không trả lưu vào công nợ
                       else if (GuestAmountPaid == 0 || GuestAmountPaid == null)
                       {
                           AMmodel = new AM_TransactionModel()
                           {
                               StoreId = model.StoreId,
                               AMAccountId = (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.PTNCC && p.AMAccountTypeCode == EnumAM_AccountType.CONGNO && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                               TransactionTypeCode = EnumTransactionType.NXNHAP,
                               ContactItemTypeCode = EnumContactType.NCC,
                               CustomerId = null,
                               SupplierId = model.SupplierId,
                               EmployeeId = null,
                               OtherId = null,
                               Amount = model.TotalPrice,
                               OrderId = null,
                               ImportMasterId = model.ImportMasterId,
                               IEOtherMasterId = null,
                               Note = model.Note,
                               CreateDate = currentTime,
                               CreateEmpId = EmployeeId,
                               RemainingAmountAccrued = model.RemainingAmountAccrued
                           };
                           _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                           _context.SaveChanges();
                       }
                       #endregion

                       #region TH3 : Trả 1 phần
                       else
                       {
                           #region 1 phần (Tiền mặt hoặc chuyển khoản)
                           AMmodel = new AM_TransactionModel()
                           {
                               StoreId = model.StoreId,
                               AMAccountId = (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.TM && p.AMAccountTypeCode == EnumAM_AccountType.TIENMAT && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                               TransactionTypeCode = EnumTransactionType.NXNHAP,
                               ContactItemTypeCode = EnumContactType.NCC,
                               CustomerId = null,
                               SupplierId = model.SupplierId,
                               EmployeeId = null,
                               OtherId = null,
                               Amount = GuestAmountPaid,
                               OrderId = null,
                               ImportMasterId = model.ImportMasterId,
                               IEOtherMasterId = null,
                               Note = model.Note,
                               CreateDate = currentTime,
                               CreateEmpId = EmployeeId,
                               RemainingAmountAccrued = model.RemainingAmountAccrued
                           };
                           _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                           _context.SaveChanges();
                           #endregion

                           #region 1 phần đưa vào công nợ
                           AMmodel = new AM_TransactionModel()
                           {
                               StoreId = model.StoreId,
                               AMAccountId = (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.PTNCC && p.AMAccountTypeCode == EnumAM_AccountType.CONGNO && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                               TransactionTypeCode = EnumTransactionType.NXNHAP,
                               ContactItemTypeCode = EnumContactType.NCC,
                               CustomerId = null,
                               SupplierId = model.SupplierId,
                               EmployeeId = null,
                               OtherId = null,
                               Amount = model.TotalPrice - GuestAmountPaid,
                               OrderId = null,
                               ImportMasterId = model.ImportMasterId,
                               IEOtherMasterId = null,
                               Note = model.Note,
                               CreateDate = currentTime,
                               CreateEmpId = EmployeeId,
                               RemainingAmountAccrued = model.RemainingAmountAccrued
                           };
                           _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                           _context.SaveChanges();
                           #endregion
                       }
                       #endregion

                       #endregion
                   }
                   #endregion

                   #region Thêm AM_DebtModel (Số nợ còn lại)
                   if (model.RemainingAmount > 0)
                   {
                       var AMDebModel = new AM_DebtModel()
                       {
                           SupplierId = model.SupplierId,
                           TimeOfDebt = currentTime,
                           RemainingAmountAccrued = model.RemainingAmountAccrued,
                           ImportId = model.ImportMasterId,
                           TransactionTypeCode = EnumTransactionType.NXNHAP
                       };
                       _context.Entry(AMDebModel).State = System.Data.Entity.EntityState.Added;
                       _context.SaveChanges();
                   }
                   #endregion

                   #region InventoryMaster
                   InventoryMasterModel InvenMaster = new InventoryMasterModel();
                   InvenMaster.WarehouseModelId = model.WarehouseId;
                   InvenMaster.InventoryCode = model.ImportMasterCode;
                   InvenMaster.InventoryTypeId = EnumInventoryType.NC;
                   InvenMaster.CreatedDate = model.CreatedDate;
                   InvenMaster.CreatedAccount = model.CreatedAccount;
                   InvenMaster.CreatedEmployeeId = model.CreatedEmployeeId;
                   InvenMaster.Actived = true;
                   InvenMaster.BusinessId = model.ImportMasterId; // Id nghiệp vụ 
                   InvenMaster.BusinessName = "ImportMasterModel";// Tên bảng nghiệp vụ
                   InvenMaster.ActionUrl = "/ImportMaster/Details/";// Đường dẫn ( cộng ID cho truy xuất)
                   InvenMaster.StoreId = model.StoreId;
                   _context.Entry(InvenMaster).State = System.Data.Entity.EntityState.Added;
                   _context.SaveChanges(); // insert tạm để lấy InvenMasterID
                   #endregion

                   #region ExchangeRate
                   // Update ExchangeRate
                   var Exchangerate = _context.ExchangeRateModel
                                              .OrderByDescending(p => p.ExchangeDate)
                                              .Where(p => p.CurrencyId == model.CurrencyId &&
                                                           p.ExchangeDate.Value.CompareTo(DateTime.Now) <= 0
                                                     )
                                              .FirstOrDefault();
                   string DateDB = string.Format("{0}-{1}-{2}", Exchangerate.ExchangeDate.Value.Year, Exchangerate.ExchangeDate.Value.Month, Exchangerate.ExchangeDate.Value.Day);
                   string DateNow = string.Format("{0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                   if (DateDB == DateNow)
                   {
                       // update
                       Exchangerate.ExchangeRate = (float)model.ExchangeRate.Value;
                       Exchangerate.ExchangeDate = DateTime.Now;
                       _context.Entry(Exchangerate).State = System.Data.Entity.EntityState.Modified;
                   }
                   else
                   {
                       ExchangeRateModel Exchangeratemodel = new ExchangeRateModel()
                       {
                           CurrencyId = model.CurrencyId,
                           ExchangeRate = (float)model.ExchangeRate.Value,
                           ExchangeDate = DateTime.Now,
                       };
                       // add
                       _context.Entry(Exchangeratemodel).State = System.Data.Entity.EntityState.Added;
                   }
                   _context.SaveChanges();

                   #endregion

                   #region Lst Product
                    if (detail != null)
                    {
                        //if (detail.GroupBy(p => p.ProductId).ToList().Count < detail.Count)
                        //{
                        //    //khong duoc trung san pham
                        //    return "Vui lòng không chọn thông tin sản phẩm trùng nhau !";
                        //}
                        foreach (var item in detail)
                        {
                            item.UnitCOGS = (item.Price * model.ExchangeRate) + item.ShippingFee;

                            #region Import Detail
                            ImportDetailModel detailmodel = new ImportDetailModel()
                            {
                                ImportMasterId = model.ImportMasterId,
                                ProductId = item.ProductId,
                                Qty = item.Qty,
                                Price = item.Price,
                                UnitShippingWeight = item.UnitShippingWeight,
                                UnitPrice = item.UnitPrice,
                                ShippingFee = item.ShippingFee,
                                UnitCOGS = item.UnitCOGS,
                                Note = item.Note
                            };
                            _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            #endregion

                            #region  update bảng Product
                            var productmodel = _context.ProductModel.Where(p => p.ProductId == item.ProductId).FirstOrDefault();
                            //productmodel.ImportPrice = item.Price;
                            //productmodel.ShippingFee = item.ShippingFee;
                            //productmodel.COGS = item.UnitCOGS;
                            //productmodel.CurrencyId = model.CurrencyId;
                            //productmodel.ExchangeRate = model.ExchangeRate;
                            productmodel.ImportDate = DateTime.Now;
                            _context.Entry(productmodel).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                            #endregion
                        }

                        #region Insert InventoryDetail

                        #region groupby Importdetail
                        var detailgruoppd =
                                            (from c in detail
                                             group c by new
                                             {
                                                 c.ProductId
                                             } into gcs
                                             select new ImportDetailViewModel()
                                             {
                                                 ProductId = gcs.Key.ProductId,
                                                 Qty = gcs.Sum(p => p.Qty),
                                                 Price = gcs.Sum(p => p.Price),
                                                 UnitShippingWeight = gcs.Sum(p => p.UnitShippingWeight),
                                                 UnitPrice = gcs.Sum(p => p.UnitPrice),
                                                 ShippingFee = gcs.Sum(p => p.ShippingFee),
                                                 UnitCOGS = gcs.Sum(p => p.UnitCOGS)
                                             }).ToList();
                        #endregion

                        foreach (var item in detailgruoppd)
                        {
                            item.UnitCOGS = (item.Price * model.ExchangeRate) + item.ShippingFee;
                            #region Insert
                            // Insert InventoryDetail
                            EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
                            decimal tondau = EndInventoryRepo.GetQty(item.ProductId.Value);
                            var tempt2 = _context.ProductModel.Where(p => p.ProductId == item.ProductId).FirstOrDefault();
                            decimal GiaVon = tempt2.COGS.HasValue ? tempt2.COGS.Value : 0;
                            InventoryDetailModel InvenDetail = new InventoryDetailModel()
                            {
                                InventoryMasterId = InvenMaster.InventoryMasterId,
                                ProductId = item.ProductId,
                                BeginInventoryQty = tondau,
                                COGS = GiaVon,
                                //Price = item.Price,
                                ImportQty = item.Qty,
                                //ExportQty = 0,
                                UnitCOGS = GiaVon * item.Qty,
                                //UnitPrice = 0,
                                EndInventoryQty = tondau + item.Qty
                            };
                            _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            #endregion
                        }

                        #endregion
                        // Cập nhật lại Tổng giá vốn 
                        model.SumCOGSOfOrderDetail = detail.Sum(p => p.UnitCOGS * p.Qty);
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;

                        _context.SaveChanges();
                        // đánh dấu Transaction hoàn tất
                        ts.Complete();

                        #region Cập nhật Tỉ giá và giá nhập,phí vận chuyển,giá vốn trên StoreProcedure

                        #endregion
                        return "success";
                    }
                    else
                    {
                        //chua nhap tt san pham
                        return "Vui lòng chọn thông tin sản phẩm";
                    }
                   #endregion
               }
            }
           catch 
           { 
               return "Xảy ra lỗi trong quá trình thêm mới sản phẩm từ nhà cung cấp";
           }

       }
        #endregion

       private string GetImportMasterCode()
       {
           // Tìm giá trị STT order code
           string kq = "";
           string ImportCodeToFind = string.Format("{0}-{1}{2}", "PNK", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
           var Resuilt = _context.ImportMasterModel.OrderByDescending(p => p.ImportMasterId).Where(p => p.ImportMasterCode.Contains(ImportCodeToFind)).Select(p => p.ImportMasterCode).FirstOrDefault();
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
               kq = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT); // ViewBag.ImportCode
           }
           else
           {
               kq = string.Format("{0}-{1}", ImportCodeToFind, "0001");
           }
           return kq;
       }
    }
}

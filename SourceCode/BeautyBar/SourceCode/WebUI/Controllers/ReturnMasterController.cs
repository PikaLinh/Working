using Constant;
using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
	public class ReturnMasterController : BaseController
	{
		//
		// GET: /ReturnMaster/

		#region Danh sách ReturnMaster
        
		public ActionResult Index()
		{
			CreateViewSearchBag();
			return View(_context.ReturnMasterModel.OrderByDescending(p => p.ReturnMasterId).Where(p => p.Actived == true).ToList());
		}
		#endregion
		public ActionResult _SearchImportMaster(ImportMasterSearchViewModel model)
		{
			//Danh sách
			List<ReturnMasterInfoViewModel> list = new List<ReturnMasterInfoViewModel>();

			if (model.ToDate.HasValue)
			{
				model.ToDate.Value.AddDays(1);
			}
			//Tim kiếm
			list = (from p in _context.ReturnMasterModel

					join sm in _context.SupplierModel on p.SupplierId equals sm.SupplierId into smList
					from pp in smList.DefaultIfEmpty()

					join ppp in _context.ImportMasterModel on p.ImportMasterId equals ppp.ImportMasterId
					join wh in _context.WarehouseModel on p.WarehouseId equals wh.WarehouseId
					join id in _context.ImportDetailModel on p.ImportMasterId equals id.ImportMasterId
					join pd in _context.ProductModel on id.ProductId equals pd.ProductId
					where (model.SupplierId == null || p.SupplierId == model.SupplierId) &&
					(model.WarehouseId == null || p.WarehouseId == model.WarehouseId) &&
					(model.ProductId == null || pd.ProductId == model.ProductId) &&
					(model.ImportMasterId == null || p.ImportMasterId == model.ImportMasterId) &&
					(model.FromDate == null || p.CreatedDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
					(model.ToDate == null || p.CreatedDate.Value.CompareTo(model.ToDate.Value) <= 0) &&
					(model.FromTotalPrice == null || p.TotalPrice >= model.FromTotalPrice) &&
					(model.ToTotalPrice == null || p.TotalPrice <= model.ToTotalPrice) &&
					p.Actived == true
					select new ReturnMasterInfoViewModel()
					{
						CreatedDate = p.CreatedDate,
						ReturnMasterId = p.ReturnMasterId,
						ImportMasterCode = ppp.ImportMasterCode,
						ReturnMasterCode = p.ReturnMasterCode,
						SupplierName = pp.SupplierName,
						WarehouseName = wh.WarehouseName,
						SalemanName = p.SalemanName,
						TotalQty = p.TotalQty,
						TotalPrice = p.TotalPrice
					}).Distinct()
					.OrderByDescending(p => p.CreatedDate)
					.ToList();
			return PartialView(list);
		}
		#region CreateViewSearchBag
		private void CreateViewSearchBag(int? WarehouseId = null, int? SupplierId = null)
		{
			//1. WarehouseId
			var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
			ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

			//2. SupplierId
			var SupplierList = _context.SupplierModel.OrderBy(p => p.SupplierName).ToList();
			ViewBag.SupplierId = new SelectList(SupplierList, "SupplierId", "SupplierName", SupplierId);

		}

		#endregion

		#region Thêm mới ReturnMaster
		public ActionResult Create()
		{
			ReturnMasterModel model = new ReturnMasterModel() { Actived = true };

			//model.SalemanName =
			model.CreatedAccount = currentAccount.UserName;
			AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
			model.SalemanName = Account.EmployeeModel.FullName;
			ViewBag.SalemanName = model.SalemanName;
			CreateViewBag(null, null, null, null, 1);
			// Tìm giá trị STT ReturnMaster code
			ViewBag.ReturnCode = GetReturnCode();
			return View(model);
		}
		#endregion

		#region CreateViewBag
		private void CreateViewBag(int? WarehouseId = null, int? SupplierId = null, string VATType = "", string ManualDiscountType = "", int? CurrencyId = null, int? StoreId = null)
		{
			//0. StoreId : Load theo EmployeeId
			var StoreList = _context.StoreModel.OrderBy(p => p.StoreName)
				.Where(p =>
					p.Actived == true &&
					(currentEmployee.StoreId == null || p.StoreId == currentEmployee.StoreId)
				).ToList();
			ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);
			//1. WarehouseId : Load theo cửa hàng
			var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName)
				.Where(p => p.Actived == true && (StoreId == null || p.StoreId == StoreId))
				.ToList();
			ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

			//2. SupplierId
			var SupplierList = _context.SupplierModel.OrderBy(p => p.SupplierName).ToList();
			ViewBag.SupplierId = new SelectList(SupplierList, "SupplierId", "SupplierName", SupplierId);

			//3. VATType
			var VATTypeList = _context.VATTypeModel.OrderBy(p => p.VATTypeName).ToList();
			ViewBag.VATType = new SelectList(VATTypeList, "VATType", "VATTypeName", VATType);

			//4. ManualDiscountType
			var ManualDiscountList = _context.ManualDiscountTypeModel.OrderBy(p => p.manualDiscountTypeName).ToList();
			ViewBag.ManualDiscountType = new SelectList(ManualDiscountList, "ManualDiscountType", "manualDiscountTypeName", ManualDiscountType);

			//5. CurrencyId
			var CurrencyList = _context.CurrencyModel.OrderBy(p => p.CurrencyName).ToList();
			ViewBag.CurrencyId = new SelectList(CurrencyList, "CurrencyId", "CurrencyName", CurrencyId);
		}
		#endregion

		#region GetImportMasterID
		public ActionResult GetImportMasterID(string q, int? StoreId, int? WarehouseId, int? SupplierId)
		{
			var data2 = _context.ImportMasterModel
					   .Where(p => (q == null || (p.ImportMasterCode.Contains(q))) &&
					   (StoreId == null || p.StoreId == StoreId) &&
					   (WarehouseId == null || p.WarehouseId == WarehouseId) &&
					   (SupplierId == null || p.SupplierId == SupplierId) &&
						p.Actived == true)
					   .Select(p => new
					   {
						   value = p.ImportMasterId,
						   text = p.ImportMasterCode,
						   StoreId = p.StoreId,
						   WarehouseId = p.WarehouseId,
						   SupplierId = p.SupplierId
					   }).Take(10).ToList();
			return Json(data2, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region GetInforByImportMasterID
		public ActionResult GetListDetailByImportMasterID(int? ImportMasterID)
		{
			try
			{
				// Lấy danh sách ImportDetailList theo ImportMasterID
				var idParam = new SqlParameter
				{
					ParameterName = "ImportMasterId",
					Value = ImportMasterID,
					SqlDbType = System.Data.SqlDbType.Int
				};
				var detail = _context.Database.
											   SqlQuery<ReturnDetailViewModel>("dbo.usp_ReturnDetailList @ImportMasterId", idParam)
											   .ToList();
				return PartialView("_CreatelistInner", detail);
			}
			catch
			{
				var detail = new List<ReturnDetailViewModel>();
				return PartialView("_CreatelistInner", detail);
			}

		}
		#endregion

		#region Get3FieldByImportMasterID
		public ActionResult Get3FieldByImportMasterID(int? ImportMasterID)
		{
			var data = _context.ImportMasterModel
												.Where(p => p.ImportMasterId == ImportMasterID)
												.Select(p => new
												{
													StoreId = p.StoreId,
													WarehouseId = p.WarehouseId,
													SupplierId = p.SupplierId,
													CurencyId = p.CurrencyId,
													CurencyName = p.CurrencyModel.CurrencyName,
													ExchangeRate = p.ExchangeRate,
													SalemanName = p.SalemanName,
                                                    // set giảm giá và VAT
                                                    BillDiscount = p.ManualDiscount,
                                                    BillDiscountTypeId = p.ManualDiscountType,
                                                    BillVAT = p.VATValue

												}).FirstOrDefault();
			return Json(data, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region GetWarehouseIdByStoreId
		public ActionResult GetWarehouseIdByStoreId(int? StoreId)
		{
			var data = _context.WarehouseModel
												.Where(p => (StoreId == null || p.StoreId == StoreId) && p.Actived == true)
												.Select(p => new
												{
													Id = p.WarehouseId,
													Name = p.WarehouseName
												}).ToList();
			var res = data;
			return Json(res, JsonRequestBehavior.AllowGet);
		}
		#endregion

		public ActionResult _CreateList(List<ReturnDetailViewModel> detail = null)
		{

			if (detail == null)
			{
				detail = new List<ReturnDetailViewModel>();
			}

			return PartialView(detail);
		}
		public ActionResult _CreatelistInner(List<ReturnDetailViewModel> detail = null)
		{
			if (detail == null)
			{
				detail = new List<ReturnDetailViewModel>();
			}
			ReturnDetailViewModel item = new ReturnDetailViewModel();
			//CreateDetailViewBag();
			return PartialView(detail);
		}
         
        public ActionResult Save(ReturnMasterModel model, List<ReturnDetailViewModel> detail, decimal? GuestAmountPaid, int CreateReceipt = 1)
		{
			if (ModelState.IsValid)
			{
				try
				{
					using (TransactionScope ts = new TransactionScope())
					{
                        var currentTime = DateTime.Now;

						#region Lưu ReturnMaster
                        model.CreatedDate = currentTime;
						model.CreatedAccount = currentAccount.UserName;
						AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
						model.CreatedEmployeeId = Account.EmployeeId;
						model.InventoryTypeId = EnumInventoryType.XC;//Xuất - Trả hàng cho nhà cung cấp
						model.Paid = GuestAmountPaid.HasValue ? GuestAmountPaid : 0;
						model.ReturnMasterCode = GetReturnCode();
						model.Actived = true;
					
						#endregion

                        var importModel = _context.ImportMasterModel.Where(p => p.ImportMasterId == model.ImportMasterId).FirstOrDefault();

                        #region Tính số dư còn lại
                        decimal? SuplierOldDebt = _context.AM_DebtModel
                                                        .Where(p => p.SupplierId == importModel.SupplierId)
                                                        .OrderByDescending(p => p.TimeOfDebt)
                                                        .Select(p => p.RemainingAmountAccrued)
                                                        .FirstOrDefault();
                        SuplierOldDebt = (SuplierOldDebt == null) ? 0 : SuplierOldDebt.Value;
                        model.RemainingAmount = (model.RemainingAmount == null) ? 0 : model.RemainingAmount.Value;
                        model.RemainingAmountAccrued = SuplierOldDebt.Value - model.RemainingAmount.Value;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges(); // LƯU TẠM ĐỂ LẤY ReturnMASTERID (SẼ BỊ SCROLLBACK KHI XẢY RA LỖI)
                        #endregion

                        if (CreateReceipt == 1)
                        {
                            #region Thêm vào giao dịch kế toán
                            AM_TransactionModel AMmodel;

                            #region TH1 : nhận đủ

                            if (model.TotalPrice == GuestAmountPaid)
                            {
                                AMmodel = new AM_TransactionModel()
                                {
                                    StoreId = model.StoreId,
                                    AMAccountId = (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.TM && p.AMAccountTypeCode == EnumAM_AccountType.TIENMAT && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                                    TransactionTypeCode = EnumTransactionType.NXXUAT,
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
                                    CreateEmpId = currentEmployee.EmployeeId,
                                    RemainingAmountAccrued = model.RemainingAmountAccrued
                                };
                                _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                            }
                            #endregion

                            #region TH2 : Không nhận lưu vào công nợ
                            else if (GuestAmountPaid == 0 || GuestAmountPaid == null)
                            {
                                AMmodel = new AM_TransactionModel()
                                {
                                    StoreId = model.StoreId,
                                    AMAccountId = (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.PTNCC && p.AMAccountTypeCode == EnumAM_AccountType.CONGNO && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                                    TransactionTypeCode = EnumTransactionType.NXXUAT,
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
                                    CreateEmpId = currentEmployee.EmployeeId,
                                    RemainingAmountAccrued = model.RemainingAmountAccrued
                                };
                                _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                            }
                            #endregion

                            #region TH3 : nhận 1 phần
                            else
                            {
                                #region 1 phần (Tiền mặt hoặc chuyển khoản)
                                AMmodel = new AM_TransactionModel()
                                {
                                    StoreId = model.StoreId,
                                    AMAccountId = (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.TM && p.AMAccountTypeCode == EnumAM_AccountType.TIENMAT && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                                    TransactionTypeCode = EnumTransactionType.NXXUAT,
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
                                    CreateEmpId = currentEmployee.EmployeeId,
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
                                    TransactionTypeCode = EnumTransactionType.NXXUAT,
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
                                    CreateEmpId = currentEmployee.EmployeeId,
                                    RemainingAmountAccrued = model.RemainingAmountAccrued
                                };
                                _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                                #endregion
                            }
                            #endregion

                            #endregion
                        }

                        #region Thêm AM_DebtModel (Số nợ còn lại)
                        if (model.RemainingAmount > 0)
                        {
                            var AMDebModel = new AM_DebtModel()
                            {
                                SupplierId = importModel.SupplierId,
                                TimeOfDebt = currentTime,
                                RemainingAmountAccrued = model.RemainingAmountAccrued,
                                ReturnMasterId = model.ReturnMasterId,
                                TransactionTypeCode = EnumTransactionType.NXXUAT
                            };
                            _context.Entry(AMDebModel).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }
                        #endregion

						#region Lưu InventoryMaster
						InventoryMasterModel InvenMaster = new InventoryMasterModel();
						InvenMaster.WarehouseModelId = model.WarehouseId;
						InvenMaster.InventoryCode = model.ReturnMasterCode;
						InvenMaster.InventoryTypeId = EnumInventoryType.XC;//Xuất - Trả hàng cho nhà cung cấp
						InvenMaster.CreatedDate = model.CreatedDate;
						InvenMaster.CreatedAccount = model.CreatedAccount;
						InvenMaster.CreatedEmployeeId = model.CreatedEmployeeId;
						InvenMaster.Actived = true;
						InvenMaster.BusinessId = model.ReturnMasterId; // Id nghiệp vụ 
						InvenMaster.BusinessName = "ReturnMasterModel";// Tên bảng nghiệp vụ
						InvenMaster.ActionUrl = "/ReturnMaster/Details/";// Đường dẫn ( cộng ID cho truy xuất)
						InvenMaster.StoreId = model.StoreId;
						_context.Entry(InvenMaster).State = System.Data.Entity.EntityState.Added;
						_context.SaveChanges(); // insert tạm để lấy InvenMasterID
						#endregion

						#region duyệt list lưu ReturnDetail và InvenrotyDetail
                        decimal TotalQty = 0;
                        foreach (var item in detail)
                        {
                            if (item.ReturnQty > 0) // Chỉ tính Số lượng trả > 0
                            {
                                TotalQty += item.ReturnQty.Value;
                                #region Lưu ReturnDetailModel
                                //// Lấy ReturnedQty mới nhất
                                //var tempLanDaTraCuoi = (from detal in _context.ReturnDetailModel
                                //            join master in _context.ReturnMasterModel on detal.ReturnMasterId equals master.ReturnMasterId
                                //            orderby detal.ReturnDetailId descending
                                //            where master.Actived == true && detal.ProductId == item.ProductId
                                //            select new
                                //            {
                                //                LanDaTraCuoi = detal.ReturnedQty.Value
                                //            }).FirstOrDefault();
                                decimal CogsInOd = _context.ImportDetailModel.Where(p => p.ImportMasterId == model.ImportMasterId && p.ProductId == item.ProductId).Select(p => p.UnitCOGS.Value).FirstOrDefault();
                                item.UnitCOGS = CogsInOd;
                                ReturnDetailModel detailmodel = new ReturnDetailModel()
                                {
                                    ReturnMasterId = model.ReturnMasterId,
                                    ProductId = item.ProductId,
                                    ImportQty = item.ImportQty,
                                    //ReturnedQty = (tempLanDaTraCuoi == null ? 0 + (item.ReturnQty.HasValue ? item.ReturnQty : 0) : tempLanDaTraCuoi.LanDaTraCuoi + (item.ReturnQty.HasValue ? item.ReturnQty : 0)),
                                    ReturnedQty = item.ReturnedQty,
                                    InventoryQty = item.InventoryQty,
                                    ReturnQty = item.ReturnQty.HasValue ? item.ReturnQty : 0,
                                    Price = item.Price,
                                    UnitShippingWeight = item.UnitShippingWeight,
                                    UnitPrice = item.UnitPrice,
                                    Note = item.Note,
                                    ShippingFee = item.ShippingFee,
                                    UnitCOGS = item.UnitCOGS

                                };
                                //_context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                                model.ReturnDetailModel.Add(detailmodel);
                                _context.SaveChanges();
                                #endregion

                                #region Lưu InventoryDetail
                                //var temp = _context.InventoryDetailModel.OrderByDescending(p => p.InventoryDetailId).Where(p => p.ProductId == item.ProductId).Select(p => p.EndInventoryQty).FirstOrDefault();
                                //var temp = (from detal in _context.InventoryDetailModel
                                //            join master in _context.InventoryMasterModel on detal.InventoryMasterId equals master.InventoryMasterId
                                //            orderby detal.InventoryDetailId descending
                                //            where master.Actived == true && detal.ProductId == item.ProductId
                                //            select new
                                //            {
                                //                TonCuoi = detal.EndInventoryQty.Value
                                //            }).FirstOrDefault();

                                //decimal tondau;
                                //if (temp != null)
                                //{
                                //    tondau = Convert.ToInt32(temp.TonCuoi);
                                //}
                                //else
                                //{
                                //    tondau = 0;
                                //}
                                EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
                                decimal tondau = EndInventoryRepo.GetQty(item.ProductId.Value);

                                var tempt2 = _context.ProductModel.Where(p => p.ProductId == item.ProductId).FirstOrDefault();
                                decimal GiaVon = tempt2.COGS.HasValue ? tempt2.COGS.Value : 0;
                                InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                {
                                    InventoryMasterId = InvenMaster.InventoryMasterId,
                                    ProductId = item.ProductId,
                                    BeginInventoryQty = tondau,
                                    //COGS = GiaVon,
                                    Price = item.Price,
                                    //ImportQty = 0,
                                    ExportQty = item.ReturnQty.HasValue ? item.ReturnQty : 0,// Xuất
                                    //UnitCOGS = GiaVon * (item.ReturnQty.HasValue ? item.ReturnQty : 0),
                                    UnitPrice = item.UnitPrice.HasValue ? item.UnitPrice : 0,
                                    EndInventoryQty = tondau - (item.ReturnQty.HasValue ? item.ReturnQty : 0)
                                };
                                _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                               // _context.SaveChanges();
                            }
                                #endregion
                            }

						#endregion
                        // Cập nhật lại Tổng giá vốn 
                        model.SumCOGSOfOrderDetail = detail.Where(p => p.ReturnQty > 0).Sum(p => p.UnitCOGS * p.ReturnQty);
                        model.TotalQty = TotalQty;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();

						ts.Complete();// hoàn tất và thực sự lưu vào db
						return Json("success", JsonRequestBehavior.AllowGet);
					}
				}
				catch
				{
					return Json(Resources.LanguageResource.AddErrorMessage, JsonRequestBehavior.AllowGet);
				}
			}
			else
			{
				return Json(Resources.LanguageResource.AddErrorMessage, JsonRequestBehavior.AllowGet);
			}
		}

		#region Huỷ đơn hàng
        
		public ActionResult Cancel(int id)
		{
			try
			{
				ReturnMasterModel model = _context.ReturnMasterModel
													.Where(p => p.ReturnMasterId == id)
													.FirstOrDefault();
				var Resuilt = "";
				if (model == null)
				{
					Resuilt = "Không tìm thấy đơn hàng yêu cầu !";
				}
				else
				{

					using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
					{
						using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
						{
							cmd.CommandText = "usp_ReturnCanceled";
							cmd.Parameters.AddWithValue("@ReturnMasterId", model.ReturnMasterId);
							cmd.Parameters.AddWithValue("@DeletedDate", DateTime.Now);
							AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
							model.DeletedEmployeeId = Account.EmployeeId;

							cmd.Parameters.AddWithValue("@DeletedAccount", currentAccount.UserName);
							cmd.Parameters.AddWithValue("@DeletedEmployeeId", model.DeletedEmployeeId);
							cmd.Connection = conn;
							cmd.CommandType = CommandType.StoredProcedure;
							conn.Open();
							cmd.ExecuteNonQuery();
							conn.Close();
						}
					}
					//_context.Entry(model).State = System.Data.Entity.EntityState.Modified;
					//_context.SaveChanges();
					Resuilt = "success";
				}
				return Json(Resuilt, JsonRequestBehavior.AllowGet);
			}
			catch
			{
				return Json(string.Format(Resources.LanguageResource.CancelErrorMessenge, "Đơn trả hàng nhà cung cấp"), JsonRequestBehavior.AllowGet);
			}
		}
		#endregion

		#region Detail
        
		public ActionResult Details(int id)
		{
			//ReturnMasterModel model = _context.ReturnMasterModel.Find(id);
			var model = (from p in _context.ReturnMasterModel
						 join im in _context.ImportMasterModel on p.ImportMasterId equals im.ImportMasterId
						 join sm in _context.StoreModel on p.StoreId equals sm.StoreId
						 join wm in _context.WarehouseModel on p.WarehouseId equals wm.WarehouseId
						 join supm in _context.SupplierModel on p.SupplierId equals supm.SupplierId
						 join cur in _context.CurrencyModel on p.CurrencyId equals cur.CurrencyId
						 where (p.ReturnMasterId == id)
						 select new ReturnMasterViewModel()
						 {
							 ReturnMasterId = p.ReturnMasterId,
							 ReturnMasterCode = p.ReturnMasterCode,
							 StoreName = sm.StoreName,
							 WarehouseName = wm.WarehouseName,
							 SupplierName = supm.SupplierName,
							 ImportMasterCode = im.ImportMasterCode,
							 SenderName = p.SenderName,
							 ReceiverName = p.ReceiverName,
							 Note = p.Note,
							 CurrencyName = cur.CurrencyName,
							 ExchangeRate = p.ExchangeRate,
							 SalemanName = p.SalemanName,
							 TAXBillCode = p.TAXBillCode,
							 TAXBillDate = p.TAXBillDate,
							 DebtDueDate = p.DebtDueDate,
							 SumPrice = p.ReturnDetailModel.Sum(s => s.UnitPrice),
							 ManualDiscount = p.ManualDiscount,
							 ManualDiscountType = p.ManualDiscountType,
							 VATValue = p.VATValue,
							 TotalPrice = p.TotalPrice,
							 Paid = p.Paid.HasValue ? p.Paid.Value : 0,
							 RemainingAmount = p.RemainingAmount
						 }).FirstOrDefault();
			if (model == null)
			{
				return HttpNotFound();
			}
			return View(model);
		}
		public ActionResult _DetailList(int ReturnMasterId)
		{
			List<ReturnDetailModel> model = _context.ReturnDetailModel.Where(p => p.ReturnMasterId == ReturnMasterId).ToList();
			if (model == null)
			{
				model = new List<ReturnDetailModel>();
			}
			return PartialView(model);
		}
		#endregion

		#region Healper
		private string GetReturnCode()
		{
			// Tìm giá trị STT ReturnMaster code
			string ReturnCodeToFind = string.Format("{0}-{1}{2}", "PTH", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
			var ResuiltFound = _context.ReturnMasterModel.OrderByDescending(p => p.ReturnMasterId).Where(p => p.ReturnMasterCode.Contains(ReturnCodeToFind)).Select(p => p.ReturnMasterCode).FirstOrDefault();
			string ResuiltFinal = "";
			if (ResuiltFound != null)
			{
				int LastNumber = Convert.ToInt32(ResuiltFound.Substring(9)) + 1;
				string STT = "";
				switch (LastNumber.ToString().Length)
				{
					case 1: STT = "000" + LastNumber.ToString(); break;
					case 2: STT = "00" + LastNumber.ToString(); break;
					case 3: STT = "0" + LastNumber.ToString(); break;
					default: STT = LastNumber.ToString(); break;
				}
				//ViewBag.ReturnCode = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT);
				ResuiltFinal = string.Format("{0}{1}", ResuiltFound.Substring(0, 9), STT);
			}
			else
			{
				ResuiltFinal = string.Format("{0}-{1}", ReturnCodeToFind, "0001");
			}

			return ResuiltFinal;
		}
		#endregion
	}
}


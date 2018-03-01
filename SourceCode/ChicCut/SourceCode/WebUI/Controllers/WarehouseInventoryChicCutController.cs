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
    public class WarehouseInventoryChicCutController : BaseController
    {
        // GET:  WarehouseInventoryChicCut
        
        public ActionResult Index()
        {
            var List = (from p in _context.WarehouseInventoryMasterModel
                        join wm in _context.WarehouseModel on p.WarehouseId equals wm.WarehouseId
                        orderby p.WarehouseInventoryMasterId descending
                        where (p.Actived == true && p.TotalQty > 0)
                        select new WarehouseInventoryMasterViewModel()
                        {
                            WarehouseInventoryMasterId = p.WarehouseInventoryMasterId,
                            CreatedIEOther = p.CreatedIEOther,
                            WarehouseInventoryMasterCode = p.WarehouseInventoryMasterCode,
                            WarehouseName = wm.WarehouseName,
                            CreatedDate = p.CreatedDate,
                            CreatedAccount = p.CreatedAccount,
                            TotalQty = p.TotalQty,
                            Actived = p.Actived
                        }).ToList();

            if (List == null)
            {
                List.Add(new WarehouseInventoryMasterViewModel());
            }
            return View(List);
        }
        

        #region Thêm mới phiếu kiểm kho
        public ActionResult Create()
        {
            WarehouseInventoryMasterModel master = new WarehouseInventoryMasterModel()
            {
                WarehouseInventoryMasterCode = GetWarehouseInventoryMasterCode(),
                Actived = true 
            };
            CreateViewBag();
            return View(master);
        }

        #region _CreateList
        
        public ActionResult _CreateDetailList(List<WarehouseInventoryDetailViewModel> detail = null)
        {
            if (detail == null)
            {
                detail = new List<WarehouseInventoryDetailViewModel>();
            }
            return PartialView(detail);
        }
        #endregion

        #region _CreatelistInner
        
        //public ActionResult _CreateDetailListInner(List<WarehouseInventoryDetailViewModel> detail = null)
        //{
        //    if (detail == null)
        //    {
        //        detail = new List<WarehouseInventoryDetailViewModel>();
        //    }
        //    WarehouseInventoryDetailViewModel item = new WarehouseInventoryDetailViewModel();
        //    detail.Add(item);
        //    return PartialView(detail);
        //}
        public ActionResult _CreateDetailListInner(List<WarehouseInventoryDetailViewModel> detail = null, string scanBarcode = null)
        {
            //Nếu detail == null thì khởi tạo List mới
            if (detail == null)
            {
                detail = new List<WarehouseInventoryDetailViewModel>();
                //Nếu barcode == null thì thêm mới sản phẩm
                if (string.IsNullOrEmpty(scanBarcode))
                {
                    WarehouseInventoryDetailViewModel item = new WarehouseInventoryDetailViewModel();
                    detail.Add(item);
                }
                else  // Quét mã vạch
                {
                    #region Get thông tin cho warehouse

                    //Lấy sản phẩm dựa vào barcode
                    var product = _context.ProductModel.Where(p => p.Barcode == scanBarcode).FirstOrDefault();
                    if (product != null)
                    {
                        WarehouseInventoryDetailViewModel item = new WarehouseInventoryDetailViewModel()
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductCode + " | " + product.ProductName + " | " + product.Specifications,
                        };
                        detail.Add(item);
                    }
                    else
                    {
                        return Json("Mã vạch này chưa tồn tại!", JsonRequestBehavior.AllowGet);
                    }
                    //}
                    #endregion
                }
            }
            //Nếu detail != null thì add thêm vào không cần khởi tạo List mới
            else
            {
                if (string.IsNullOrEmpty(scanBarcode))
                {
                    WarehouseInventoryDetailViewModel item = new WarehouseInventoryDetailViewModel();
                    detail.Add(item);
                }
                else  // Quét mã vạch
                {
                    #region Get thông tin cho warehouse

                    //Lấy sản phẩm dựa vào barcode
                    var product = _context.ProductModel.Where(p => p.Barcode == scanBarcode).FirstOrDefault();
                    if (product != null)
                    {
                        #region // Nếu Sản phẩm trùng
                        foreach (var itemm in detail)
                        {
                            if (itemm.ProductId == product.ProductId)
                            {
                                return Json("Mã vạch của sản phẩm này đã tồn tại trong danh sách kiểm kho!", JsonRequestBehavior.AllowGet);
                            }
                        }
                        #endregion

                        WarehouseInventoryDetailViewModel item = new WarehouseInventoryDetailViewModel()
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductCode + " | " + product.ProductName + " | " + product.Specifications,
                        };
                        detail.Add(item);
                    }
                    else
                    {
                        return Json("Mã vạch này chưa tồn tại!", JsonRequestBehavior.AllowGet);
                    }
                    //}
                    #endregion
                }
            }

            return PartialView(detail);
        }
        #endregion

        #region _CreatelistInnerCategory
        
        public ActionResult _CreateDetailListInnerCategory(int id)
        {

            EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
            var detail = (from p in _context.ProductModel
                          where p.CategoryId == id
                          select new WarehouseInventoryDetailViewModel()
                          {
                              ProductId = p.ProductId,
                              ProductName = p.ProductCode +" | "+ p.ProductName+" | "+ p.Specifications,
                              ProductCode = p.ProductCode,
                              Specifications = p.Specifications,
                              AmountDifference = 0
                          }).ToList();

            //Cập nhật tồn hệ thống
            foreach (var item in detail)
            {
                int ProductId = item.ProductId ?? default(int);
                item.EndInventoryQty = EndInventoryRepo.GetQty(ProductId);
                item.ActualInventory = item.EndInventoryQty;
            }

            return PartialView(detail);
        }
        #endregion


        #region Save
        public ActionResult Save(WarehouseInventoryMasterModel model, List<WarehouseInventoryDetailViewModel> detail)
        {
            string resuilt = "Lưu phiếu kiểm kho không thành công !";
            if (ModelState.IsValid)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    int TotalQty = 0;

                    #region lưu WarehouseInventoryMasterModel

                    WarehouseInventoryMasterModel master = new WarehouseInventoryMasterModel()
                    {
                        WarehouseInventoryMasterCode = model.WarehouseInventoryMasterCode,
                        CreatedDate = DateTime.Now,
                        CreatedAccount = currentAccount.UserName,
                        CreatedEmployeeId = currentEmployee.EmployeeId,
                        Note = model.Note,
                        WarehouseId = model.WarehouseId,
                        //TotalQty = detail.Count(),
                        CreatedIEOther = false,
                        Actived = true // TotalQty : Tổng số sản phẩm kiểm kho update sau.
                    };
                    _context.Entry(master).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();// Lưu để lấy masterID
                    #endregion

                    #region lưu WarehouseInventoryDetailModel
                    if (detail != null)
                    {
                        if (detail.GroupBy(p => p.ProductId).ToList().Count < detail.Count)
                        {
                            //khong duoc trung san pham
                            return Json("Vui lòng không chọn thông tin sản phẩm trùng nhau !", JsonRequestBehavior.AllowGet);
                        }

                        foreach (var item in detail)
                        {
                            if (item.ProductId == null)
                            {
                                return Json("Vui lòng nhập thông tin sản phẩm !", JsonRequestBehavior.AllowGet);
                            }
                            if (item.ActualInventory == null)
                            {
                                return Json("Vui lòng nhập thông tin tồn thực tế sản phẩm (" + item.ProductName +") !", JsonRequestBehavior.AllowGet);
                            }
                            if(item.EndInventoryQty != item.ActualInventory)
                            {
                                WarehouseInventoryDetailModel p = new WarehouseInventoryDetailModel()
                                {
                                    WarehouseInventoryMasterId = master.WarehouseInventoryMasterId,
                                    ProductId = item.ProductId,
                                    Inventory = item.EndInventoryQty,
                                    ActualInventory = item.ActualInventory,
                                    AmountDifference = item.AmountDifference,
                                    Specifications = item.Specifications,
                                    Actived = true
                                };
                                _context.Entry(p).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                                TotalQty++;
                            }
                            
                        }
                    }
                    else
                    {
                        return Json("Giá trị tồn hiện tại chưa được sửa đổi !", JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                    #region  Update TotalQty Master
                    master.TotalQty = TotalQty;
                    _context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    #endregion

                    ts.Complete();
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region Xem phiếu kiểm kho
        //
        public ActionResult WarehouseInventoryProducts(int? id)
        {
            #region //B1: Lấy ra danh sách sản phẩm kiểm kho theo phiếu kiểm kho
            var lsttmp = (from p in _context.WarehouseInventoryDetailModel
                          join master in _context.WarehouseInventoryMasterModel on p.WarehouseInventoryMasterId equals master.WarehouseInventoryMasterId
                          where (p.Actived == true && master.Actived && (id == null || p.WarehouseInventoryMasterId == id))
                          orderby p.WarehouseInventoryDetailId descending
                          select new WarehouseInventoryDetailViewModel()
                          {
                              MasterCode = master.WarehouseInventoryMasterCode,
                              WarehouseInventoryMasterId = p.WarehouseInventoryMasterId,
                              Inventory = p.Inventory,
                              ActualInventory = p.ActualInventory,
                              AmountDifference = p.AmountDifference,
                              WarehouseInventoryDetailId = p.WarehouseInventoryDetailId,
                              ProductId = p.ProductId
                          }).ToList();
            #endregion

            var lst = new List<WarehouseInventoryDetailViewModel>();
            #region // Nếu List có giá trị thì mới lấy tên hiển thị phụ và TonTrongDatabase
            if (lsttmp != null && lsttmp.Count > 0)
            {
                int WarehouseId = _context.WarehouseInventoryMasterModel.Where(p => p.WarehouseInventoryMasterId == id).Select(p => p.WarehouseId.Value).FirstOrDefault();

                SqlParameter paramWarehouseId = new SqlParameter("@WarehouseId", WarehouseId);

                #region Para ProductList
                var ProductIdLst = new DataTable();
                ProductIdLst.Columns.Add("ProductId", typeof(int));
                foreach (var item in lsttmp)
                {
                    ProductIdLst.Rows.Add(item.ProductId);
                }
                var pList = new SqlParameter("@ProductLst", SqlDbType.Structured);
                pList.TypeName = "dbo.ProductList";
                pList.Value = ProductIdLst;
                #endregion

                var lstEndInventoryQty = _context.Database.
                                        SqlQuery<WarehouseInventoryDetailViewModel>("dbo.usp_TonCuoiSPTuongUngVoiKho @WarehouseId, @ProductLst", paramWarehouseId, pList).ToList();
                var WarehouseName = _context.WarehouseModel.Where(p => p.WarehouseId == WarehouseId).Select(p => p.WarehouseCode).FirstOrDefault();
                lst = (from p in lsttmp // danh sách sp cần kiểm kho
                       join pm in _context.ProductModel on p.ProductId equals pm.ProductId // lấy thông tin hiển thị
                       join pEndtmp in lstEndInventoryQty on p.ProductId equals pEndtmp.ProductId into pEndLst
                       from pEnd in pEndLst.DefaultIfEmpty(new WarehouseInventoryDetailViewModel()) // lấy tồn cuối: nếu ko có trong database thì sẽ là 0
                       select new WarehouseInventoryDetailViewModel()
                       {
                           MasterCode = p.MasterCode
                           ,
                           WarehouseInventoryMasterId = p.WarehouseInventoryMasterId
                           ,
                           WarehouseName = WarehouseName
                          ,
                           ProductName = pm.ProductName
                           ,
                           ProductCode = pm.ProductCode
                           ,
                           Inventory = p.Inventory
                           ,
                           ActualInventory = p.ActualInventory
                           ,
                           AmountDifference = p.AmountDifference
                           ,
                           WarehouseInventoryDetailId = p.WarehouseInventoryDetailId
                           ,
                           ProductId = p.ProductId
                           ,
                           TonTrongDatabase = pEnd.TonTrongDatabase
                       })
                        .OrderByDescending(p => p.ProductId).ToList();
                ViewBag.isLast = 0;
                #region Disable nút xoá
                var MaxMaster = _context.WarehouseInventoryMasterModel
                                          .OrderByDescending(p => p.WarehouseInventoryMasterId)
                                          .FirstOrDefault();
                if (MaxMaster != null) // Nếu là phiếu kiểm kho mới nhất
                {
                    if (MaxMaster.Actived && MaxMaster.TotalQty > 0 && !MaxMaster.CreatedIEOther && MaxMaster.WarehouseInventoryMasterId == id)
                    {
                        ViewBag.isLast = 1;
                    }
                }
                #endregion

            }
            #endregion
            return View(lst);
        }
        #endregion

        #region Tạo xuất nhập kho
        
        public ActionResult Autoimex(int id)
        {
            WarehouseInventoryMasterModel model = _context.WarehouseInventoryMasterModel
                                             .Where(p => p.WarehouseInventoryMasterId == id && p.Actived == true && p.CreatedIEOther == false)
                                             .FirstOrDefault();
            // Bước 1 : insert IEOtherMaster 
            // Bước 2 : insert IEOtherDetail : dựa vào AmountDifference để nhập hay xuất kho

            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy đơn hàng yêu cầu !";
            }
            else
            {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        #region insert IEOtherMaster
                        IEOtherRepository IEOtherRepository = new IEOtherRepository(_context);
                        IEOtherMasterModel IEOtherMasterModel = new IEOtherMasterModel()
                        {
                            IEOtherMasterCode = IEOtherRepository.GetIEOtherCodePKK(),
                            WarehouseId = model.WarehouseId.Value,
                            InventoryTypeId = EnumInventoryType.KK,
                            Note = "Bù trừ kiểm kho khớp với hệ thống ",
                            CreatedDate = DateTime.Now,
                            CreatedAccount = currentAccount.UserName,
                            CreatedEmployeeId = currentEmployee.EmployeeId,
                            Actived = true
                        };
                        _context.Entry(IEOtherMasterModel).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        #endregion

                        #region InventoryMasterModel
                        int idStore = _context.WarehouseModel.Where(p => p.WarehouseId == model.WarehouseId).Select(p => p.StoreId.Value).FirstOrDefault();
                        InventoryMasterModel InvenMaster = new InventoryMasterModel()
                        {
                            StoreId = idStore,
                            WarehouseModelId = model.WarehouseId,
                            InventoryTypeId = EnumInventoryType.KK,
                            InventoryCode = IEOtherMasterModel.IEOtherMasterCode,
                            CreatedDate = IEOtherMasterModel.CreatedDate,
                            CreatedAccount = IEOtherMasterModel.CreatedAccount,
                            CreatedEmployeeId = IEOtherMasterModel.CreatedEmployeeId,
                            Actived = true,
                            BusinessId = IEOtherMasterModel.IEOtherMasterId, // Id nghiệp vụ 
                            BusinessName = "IEOtherMasterModel",// Tên bảng nghiệp vụ
                            ActionUrl = "/IEOtherMaster/Details/"// Đường dẫn ( cộng ID cho truy xuất)
                        };
                        _context.Entry(InvenMaster).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges(); // insert tạm để lấy InvenMasterID
                        #endregion

                        #region Duyệt WarehouseInventoryDetailModel insert vào InventoryDetailModel
                        var LstWarehouseInventoryDetailModel = _context.WarehouseInventoryDetailModel
                                                              .Where(p => p.WarehouseInventoryMasterId == model.WarehouseInventoryMasterId)
                                                              .OrderByDescending(p => p.WarehouseInventoryDetailId)
                                                              .ToList();
                        foreach (var item in LstWarehouseInventoryDetailModel)
                        {
                            if (item.AmountDifference != 0)
                            {
                                #region Kiểm tra tồn # database
                                var temp = (from detal in _context.InventoryDetailModel
                                            join master in _context.InventoryMasterModel on detal.InventoryMasterId equals master.InventoryMasterId
                                            orderby detal.InventoryDetailId descending
                                            where master.Actived == true && detal.ProductId == item.ProductId
                                            select new
                                            {
                                                TonCuoi = detal.EndInventoryQty.Value
                                            }).FirstOrDefault();
                                decimal TonTrongDatabase = temp != null ? temp.TonCuoi : 0;
                                if (item.Inventory != TonTrongDatabase)
                                {
                                    return Json("Số lượng tồn không chính xác, vui lòng nhấn nút 'Xem' để cập nhật lại", JsonRequestBehavior.AllowGet);
                                }
                                #endregion

                                #region IEOtherDetailModel
                                IEOtherDetailModel detailmodel = new IEOtherDetailModel()
                                {
                                    IEOtherMasterId = IEOtherMasterModel.IEOtherMasterId,
                                    ProductId = item.ProductId,
                                    ImportQty = item.AmountDifference > 0 ? item.AmountDifference : 0, // Nhập kho sp thừa , xuất kho sp thiếu
                                    ExportQty = item.AmountDifference < 0 ? Math.Abs(item.AmountDifference.Value) : 0,
                                    Price = 0,
                                    UnitShippingWeight = 0,
                                    UnitPrice = 0
                                };
                                _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                                #endregion

                                #region Insert InventoryDetail

                                decimal tondau;
                                tondau = TonTrongDatabase;
                                if (item.AmountDifference > 0) // Nhập
                                {
                                    InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                    {
                                        InventoryMasterId = InvenMaster.InventoryMasterId,
                                        ProductId = item.ProductId,
                                        BeginInventoryQty = tondau,
                                        COGS = 0,// nhập
                                        Price = 0, // => Xuất
                                        ImportQty = item.AmountDifference,
                                        ExportQty = 0,
                                        UnitCOGS = 0, // nhập
                                        UnitPrice = 0, // => Xuất
                                        EndInventoryQty = tondau + item.AmountDifference
                                    };
                                    _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                    {
                                        InventoryMasterId = InvenMaster.InventoryMasterId,
                                        ProductId = item.ProductId,
                                        BeginInventoryQty = tondau,
                                        COGS = 0,// nhập
                                        Price = 0, // => Xuất
                                        ImportQty = 0,
                                        ExportQty = Math.Abs(item.AmountDifference.Value),
                                        UnitCOGS = 0, // nhập
                                        UnitPrice = 0, // => Xuất
                                        EndInventoryQty = tondau - Math.Abs(item.AmountDifference.Value)
                                    };
                                    _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();
                                }
                                #endregion

                            }
                        }
                        #endregion

                        model.CreatedIEOther = true;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        ts.Complete();
                        Resuilt = "success";
                    }
                }
                catch
                {
                    Resuilt = "Xảy ra lỗi trong quá trình tạo xuất nhập kho!";
                }
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Huỷ phiếu kiểm kho
        
        public ActionResult Cancel(int id)
        {
            WarehouseInventoryMasterModel model = _context.WarehouseInventoryMasterModel
                                             .Where(p => p.WarehouseInventoryMasterId == id)
                                             .FirstOrDefault();
            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy đơn hàng yêu cầu !";
            }
            else
            {
                model.Actived = false;
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                Resuilt = "success";
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Xoá sản phẩm
        
        public ActionResult DeleteProduct(int id)
        {
            WarehouseInventoryDetailModel model = _context.WarehouseInventoryDetailModel
                                             .Where(p => p.WarehouseInventoryDetailId == id)
                                             .FirstOrDefault();
            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy sản phẩm yêu cầu !";
            }
            else
            {
                model.Actived = false;
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                var master = _context.WarehouseInventoryMasterModel.Where(p => p.WarehouseInventoryMasterId == model.WarehouseInventoryMasterId).FirstOrDefault();
                master.TotalQty--;
                _context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                Resuilt = "success";
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Cập nhật tồn kho

        public ActionResult UpdateInventoryProduct(decimal tontrongdatabase, int id)
        {
            try
            {
                var model = _context.WarehouseInventoryDetailModel.Where(p => p.WarehouseInventoryDetailId == id).FirstOrDefault();
                if (model == null)
                {
                    return Json("Sản phẩm kiểm kho cần cập nhật không tồn tại trong hệ thống !", JsonRequestBehavior.AllowGet);
                }
                model.Inventory = tontrongdatabase;
                model.AmountDifference = model.ActualInventory - model.Inventory;
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Xảy ra lỗi trong quá trình cập nhật, vui lòng liên hệ bộ phận kỹ thuật để nhận được sự giúp đỡ !", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
        public ActionResult _DeletelistInner(List<WarehouseInventoryDetailViewModel> detail, int RemoveId)
        {
            if (detail == null)
            {
                detail = new List<WarehouseInventoryDetailViewModel>();
            }


            return PartialView("_CreateDetailListInner", detail.Where(p => p.STT != RemoveId).ToList());
        }

        private void CreateViewBag(int? CategoryId = null,int? WarehouseId = null)
        {
            //1. WarehouseId
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

            //2. CategoryId
            CategoryRepository repository = new CategoryRepository(_context);
            var CategoryList = repository.GetCategoryByParentWithFormat(2).Select(p => new { CategoryId = p.CategoryId, CategoryName = p.CategoryName.Substring(4) }).ToList();
            CategoryList.RemoveAt(0);
            CategoryList.Insert(0, new { CategoryId = 2, CategoryName = "Tất cả sản phẩm" });
            ViewBag.CategoryId = new SelectList(CategoryList, "CategoryId", "CategoryName", CategoryId);


        }

        #region Hàm WarehouseInventoryMasterCode
        public string GetWarehouseInventoryMasterCode()
        {
            // Tìm giá trị STT order code
            string OrderCodeToFind = string.Format("{0}-{1}{2}", "PKK", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var Resuilt = _context.WarehouseInventoryMasterModel.OrderByDescending(p => p.WarehouseInventoryMasterId).Where(p => p.WarehouseInventoryMasterCode.Contains(OrderCodeToFind)).Select(p => p.WarehouseInventoryMasterCode).FirstOrDefault();
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
        #endregion

        #region GetValueSpecifications
        public ActionResult GetValueSpecifications(int SelectedProductid)
        {
          
            EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
            decimal tempt = EndInventoryRepo.GetQty(SelectedProductid);

            var Specifications = _context.ProductModel
                                                  .Where(p => p.ProductId == SelectedProductid)
                                                  .Select(p => p.Specifications)
                                                  .FirstOrDefault();
            var result = new
            {
                EndInventoryQty = tempt,
                Specifications = Specifications
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}
using AutoMapper;
using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class MasterChicCutServiceController : BaseController
    {
        // GET: MasterChicCutService
        #region Danh sách
        public ActionResult Index()
        {
            CreateViewBag(null);
            return View();
        }
        public ActionResult _SearchService(Master_ChicCut_ServiceViewModel model)
        {
            var list = (from p in _context.Master_ChicCut_ServiceModel
                        join h in _context.Master_ChicCut_HairTypeModel on p.HairTypeId equals h.HairTypeId
                        where (model.ServiceName == null || p.ServiceName.Contains(model.ServiceName))
                               && (model.HairTypeId == null || h.HairTypeId == model.HairTypeId)
                              // && p.Actived == true
                        select new Master_ChicCut_ServiceViewModel
                        {
                            ServiceId = p.ServiceId,
                            ServiceName = p.ServiceName,
                            Gender = p.Gender,
                            HairTypeName = h.HairTypeName,
                            Price = p.Price
                        }).OrderByDescending(p => p.ServiceId)
                      .ToList();
            return PartialView(list);
        }

        //Xem bảng giá
        public ActionResult APriceList()
        {
            var lst = (_context.Master_ChicCut_ServiceCategoryModel.Where(p => p.Actived == true).Select(p => new Master_ChicCut_ServiceCategoryViewModel()
                {
                    ServiceCategoryId = p.ServiceCategoryId,
                    ServiceName = p.ServiceName,
                    OrderBy = p.OrderBy,
                    Services = _context.Master_ChicCut_ServiceModel.Where(m => m.ServiceCategoryId == p.ServiceCategoryId && m.Gender == false).ToList()
                }))
                .OrderBy(p => p.OrderBy).ToList();

            ViewBag.lstMail = (_context.Master_ChicCut_ServiceCategoryModel.Where(p => p.Actived == true).Select(p => new Master_ChicCut_ServiceCategoryViewModel()
            {
                ServiceCategoryId = p.ServiceCategoryId,
                ServiceName = p.ServiceName,
                OrderBy = p.OrderBy,
                Services = _context.Master_ChicCut_ServiceModel.Where(m => m.ServiceCategoryId == p.ServiceCategoryId && m.Gender == true).ToList()
            }))
                .OrderBy(p => p.OrderBy).ToList();
            return View(lst);
        }

        public ActionResult Tooltip(int ServiceId)
        {
            return PartialView(ServiceId);
        }
        #endregion

        #region Thêm mới dịch vụ
        
        public ActionResult Create()
        {
            Master_ChicCut_ServiceModel model = new Master_ChicCut_ServiceModel() { Actived = true };

            CreateViewBag(null, null);
            return View(model);
        }

        public ActionResult _CreateList(List<Master_ChicCut_QuantificationDetailViewModel> detail = null, List<Master_ChicCut_QuantificationMasterViewModel> QMaster = null)
        {
            if (detail == null)
            {
                detail = new List<Master_ChicCut_QuantificationDetailViewModel>();
            }
            return PartialView(detail);
        }

        public ActionResult _CreatelistInner(List<Master_ChicCut_QuantificationDetailViewModel> detail = null, List<Master_ChicCut_QuantificationMasterViewModel> QMaster = null)
        {
            if (QMaster == null)
            {
                QMaster = new List<Master_ChicCut_QuantificationMasterViewModel>();
            }
            if (detail == null)
            {
                detail = new List<Master_ChicCut_QuantificationDetailViewModel>();
            }

            Master_ChicCut_QuantificationDetailViewModel item = new Master_ChicCut_QuantificationDetailViewModel();
            detail.Add(item);
            return PartialView(detail);
        }
        #region Thêm mới 1 định lượng
        public ActionResult _AddNewListQuantification(List<Master_ChicCut_QuantificationDetailViewModel> detailInner = null, Master_ChicCut_QuantificationMasterViewModel QMasterInner = null)
        {
            if (QMasterInner == null)
            {
                QMasterInner = new Master_ChicCut_QuantificationMasterViewModel();
            }
            if (detailInner == null)
            {
                 detailInner = new List<Master_ChicCut_QuantificationDetailViewModel>();
            }
            ViewBag.ListdetailInner = detailInner;
            return PartialView(QMasterInner);
        }
        public ActionResult _AddNewListInnerQuantification(List<Master_ChicCut_QuantificationDetailViewModel> detailInner = null)
        {
            if (detailInner == null)
            {
                detailInner = new List<Master_ChicCut_QuantificationDetailViewModel>();
            }

            Master_ChicCut_QuantificationDetailViewModel item = new Master_ChicCut_QuantificationDetailViewModel();
            detailInner.Add(item);
            return PartialView(detailInner);
        }

        public ActionResult _DeleteAddNewlistInner(List<Master_ChicCut_QuantificationDetailViewModel> detailInner, int RemoveId)
        {
            if (detailInner == null)
            {
                detailInner = new List<Master_ChicCut_QuantificationDetailViewModel>();
            }
            return PartialView("_AddNewListInnerQuantification", detailInner.Where(p => p.STT != RemoveId).ToList());
        }

        public ActionResult SaveAddNewQuantification(List<Master_ChicCut_QuantificationDetailViewModel> detailInner, string QuantificationName , int ServiceId )
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    Master_ChicCut_QuantificationMasterModel model = new Master_ChicCut_QuantificationMasterModel()
                    {
                        ServiceId = ServiceId,
                        QuantificationName = QuantificationName,
                        Actived = true

                    };
                    _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                    string DetailQuantiMaster = "";
                    if (detailInner != null && detailInner.Count > 0)
                    {
                        int i = 0;
                        foreach (var item in detailInner)
                        {
                            i++;
                            Master_ChicCut_QuantificationDetailModel detail = new Master_ChicCut_QuantificationDetailModel()
                            {
                                QuantificationMasterId = model.QuantificationMasterId,
                                ProductId = item.ProductId,
                                Qty = item.Qty
                            };
                            _context.Entry(detail).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            DetailQuantiMaster += (_context.ProductModel.Find(item.ProductId).ProductCode) + string.Format("({0})", item.Qty);
                            if (i < detailInner.Count)
                            {
                                DetailQuantiMaster += " ; ";
                            }

                        }
                        model.Detail = DetailQuantiMaster;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                    }
                    ts.Complete();
                    return Content("success");
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình thêm mới định lượng!");
            }

        }

        public ActionResult UpdateAddNewQuantification(List<Master_ChicCut_QuantificationDetailViewModel> detailInner, string QuantificationName, int ServiceId, int QuantificationMasterId)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var model = _context.Master_ChicCut_QuantificationMasterModel.Where(p => p.ServiceId == ServiceId && p.QuantificationMasterId == QuantificationMasterId).First();
                    model.QuantificationName = QuantificationName;
                    string DetailQuantiMaster = "";

                    #region //Bước 1: Xoá danh sách cũ.
                    var ListdetailInnerOld = _context.Master_ChicCut_QuantificationDetailModel.Where(p => p.QuantificationMasterId == QuantificationMasterId).ToList();
                    if (ListdetailInnerOld != null && ListdetailInnerOld.Count > 0)
                    {
                        foreach (var item in ListdetailInnerOld)
                        {
                            _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                            _context.SaveChanges();
                        }
                    }
                    #endregion

                    #region
                    if (detailInner != null && detailInner.Count > 0)
                    {
                        int i = 0;
                        foreach (var item in detailInner)
                        {
                            i++;
                            Master_ChicCut_QuantificationDetailModel detail = new Master_ChicCut_QuantificationDetailModel()
                            {
                                QuantificationMasterId = model.QuantificationMasterId,
                                ProductId = item.ProductId,
                                Qty = item.Qty
                            };
                            _context.Entry(detail).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            DetailQuantiMaster += (_context.ProductModel.Find(item.ProductId).ProductCode) + string.Format("({0})", item.Qty);
                            if (i < detailInner.Count)
                            {
                                DetailQuantiMaster += " ; ";
                            }

                        }
                        model.Detail = DetailQuantiMaster;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                    }
                    #endregion

                    ts.Complete();
                    return Content("success");
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình cập nhật định lượng!");
            }

        }
        #endregion
        public ActionResult _DeletelistInner(List<Master_ChicCut_QuantificationDetailViewModel> detail, int RemoveId)
        {
            if (detail == null)
            {
                detail = new List<Master_ChicCut_QuantificationDetailViewModel>();
            }
            return PartialView("_CreatelistInner", detail.Where(p => p.STT != RemoveId).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Master_ChicCut_ServiceModel model, string returnURL = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                        #region Dịch vụ
                        model.ServiceName = _context.Master_ChicCut_ServiceCategoryModel.Find(model.ServiceCategoryId).ServiceName;
                        _context.Master_ChicCut_ServiceModel.Add(model);
                        _context.SaveChanges();
                        int ServiceId = model.ServiceId;
                        #endregion
                        if (returnURL.Equals("None"))
                        {
                            return Content("success");
                        }
                        else
                        {
                            return Content(string.Format("/MasterChicCutService/Edit/{0}", ServiceId));
                        }
                }
                else
                {
                    return Content("ErrorInfo"); 
                }
            }
            catch
            {
                return Content("ErrorOccur");
            }
        }
        #endregion


        #region Sửa dịch vụ
        
        public ActionResult Edit(int id)
        {
            var model = _context.Master_ChicCut_ServiceModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var QuantificationList = model.Master_ChicCut_QuantificationMasterModel.ToList();

            ViewBag.QuantificationList = QuantificationList;
            CreateViewBag(model.HairTypeId, model.ServiceCategoryId);
            return View(model);
        }

        public ActionResult _QuantificationDeleteInner(int QuantificationMasterId)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var ListQuantificationDetail = _context.Master_ChicCut_QuantificationDetailModel.Where(p => p.QuantificationMasterId == QuantificationMasterId).ToList();
                    if (ListQuantificationDetail != null && ListQuantificationDetail.Count > 0)
                    {
                        foreach (var item in ListQuantificationDetail)
                        {
                            _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                            _context.SaveChanges();
                        }
                    }
                    var model = _context.Master_ChicCut_QuantificationMasterModel.Find(QuantificationMasterId);
                    _context.Entry(model).State = System.Data.Entity.EntityState.Deleted;
                    _context.SaveChanges();
                    ts.Complete();
                    return Content("success");
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình Xoá định lượng!");
            }
        }

        public ActionResult _QuantificationUpdateInner(int QuantificationMasterId)
        {
            var List = (from p in _context.Master_ChicCut_QuantificationDetailModel
                        join pro in _context.ProductModel on p.ProductId equals pro.ProductId
                        where p.QuantificationMasterId == QuantificationMasterId
                        select new Master_ChicCut_QuantificationDetailViewModel()
                        {
                            ProductId = p.ProductId,
                            Qty = p.Qty,
                            ProductName = (pro.ProductCode + " | " + pro.ProductName + " | " + pro.Specifications)
                        }).ToList();
            //ViewBag.QuantificationName = _context.Master_ChicCut_QuantificationMasterModel.Find(QuantificationMasterId).QuantificationName;
            ViewBag.ListdetailInner = List;
            var QMaster = _context.Master_ChicCut_QuantificationMasterModel.Where(p => p.QuantificationMasterId == QuantificationMasterId).Select(p => new Master_ChicCut_QuantificationMasterViewModel() { 
                QuantificationMasterId = p.QuantificationMasterId,
                QuantificationName = p.QuantificationName,
                ServiceId = p.ServiceId
            }).FirstOrDefault();
            return PartialView("_AddNewListQuantification", QMaster);
        }
        public ActionResult _QuantificationInnerPartital(List<Master_ChicCut_QuantificationMasterModel> QMaster = null)
        {
            if (QMaster == null)
            {
                QMaster = new List<Master_ChicCut_QuantificationMasterModel>();
            }
            return PartialView(QMaster);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Master_ChicCut_ServiceModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        #region Dịch vụ
                        model.ServiceName = _context.Master_ChicCut_ServiceCategoryModel.Find(model.ServiceCategoryId).ServiceName;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        #endregion
                        ts.Complete();
                        return Content("success");
                    }
                }
                else
                {
                    return Content("Vui lòng kiểm tra lại thông tin không hợp lệ !");
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình thêm mới dịch vụ !");
            }
        }

        #endregion

        #region Helper
        private void CreateViewBag(int? HairTypeId = null, int? ServiceCategoryId = null)
        {
            var HairTypeLst = _context.Master_ChicCut_HairTypeModel.OrderBy(p => p.OrderIndex).ToList();
            ViewBag.HairTypeId = new SelectList(HairTypeLst, "HairTypeId", "HairTypeName", HairTypeId);
           
            //Danh mục dịch vụ
            var ServiceCategoryLst = _context.Master_ChicCut_ServiceCategoryModel.OrderBy(p => p.OrderBy).Where(p => p.Actived == true).ToList();
            ViewBag.ServiceCategoryId = new SelectList(ServiceCategoryLst, "ServiceCategoryId", "ServiceName", ServiceCategoryId);
        }
        #endregion
    }
}
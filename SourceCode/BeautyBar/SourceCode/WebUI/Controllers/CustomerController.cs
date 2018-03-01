using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using System.IO;

using ViewModels;
//using Excel = Microsoft.Office.Interop.Excel;
//using System.Runtime.InteropServices;

namespace WebUI.Controllers
{
    public class CustomerController : BaseController
    {
        protected bool isUseCustomerLevelSearchId = false;
        protected bool isUseEmployeeSearchId = false;
        //
        // GET: /Customer/
        #region danh sách khách hàng
        
        public ActionResult Index()
        {
            CreateViewBag(null, null, null, null);
            return View();
        }
        public ActionResult _SearchCustomer(CustomerInfoModel model)
        {
            var list = (from c in _context.CustomerModel
                       // join e in _context.EmployeeModel on c.EmployeeId equals e.EmployeeId
                      //  join cs in _context.CustomerLevelModel on c.CustomerLevelId equals cs.CustomerLevelId
                        where (model.FullName == null || c.FullName.Contains(model.FullName))
                               //&& (model.EmployeeId == null || e.EmployeeId == model.EmployeeId)
                               //&& (model.CustomerLevelId == null || cs.CustomerLevelId == model.CustomerLevelId)
                               && (model.Phone == null || c.Phone.Contains(model.Phone))
                               && c.Actived == true
                               && (model.Gender == null || model.Gender == c.Gender)
                               && (model.BirthDay == null || model.BirthDay == c.BirthDay)
                        select new CustomerInfoModel
                        {
                            CustomerId = c.CustomerId,
                            FullName = c.FullName,
                            //CustomerLevelName = cs.CustomerLevelName,
                            Gender = c.Gender,
                            BirthDay = c.BirthDay,
                            Email = c.Email,
                            Phone = c.Phone,
                            //EmployeeName = e.FullName,
                            RegDate = c.RegDate
                        }).OrderByDescending(p => p.CustomerId)
                      .ToList();
            return PartialView(list);
        }
        #endregion

        #region Thêm mới khách hàng
        
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Create(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            // list customlevelmodel
            CustomerModel customModel = new CustomerModel() { Actived = true, RegDate = DateTime.Now };
            List<CustomerLevelModel> listcustomlevel = _context.CustomerLevelModel.Where(p => p.Actived == true).ToList();
            ViewBag.listcustomlevel = listcustomlevel;
            //// list Tỉnh thành
            //List<ProvinceModel> listProvince = _context.ProvinceModel.OrderByDescending( p => p.ProvinceId).ToList();
            //ViewBag.listProvince = listProvince;
            //// list Quận huyện
            //List<DistrictModel> listdistrict = _context.DistrictModel.Where(p => p.ProvinceId == 89).ToList();
            //ViewBag.listdistrict = listdistrict;
            CreateViewBag();
            return View(customModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerModel customermodel, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                _context.CustomerModel.Add(customermodel);
                _context.SaveChanges();
                if (string.IsNullOrEmpty(ReturnUrl))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    if (ReturnUrl.Equals("/DailyChicCutOrder/Create"))
                    {
                        ReturnUrl = string.Format("{0}?CustomerId={1}", ReturnUrl, customermodel.CustomerId);
                    }
                    Response.Redirect(ReturnUrl);
                    return null;
                }
            }

            return View(customermodel);
        }
        #endregion

        #region Chi tiết khách hàng
        
        public ActionResult Details(int id = 0)
        {
            CustomerModel customermodel = _context.CustomerModel.Find(id);
            ViewBag.EmployeeIdDetails = _context.EmployeeModel
                                        .Where(p => p.EmployeeId == customermodel.EmployeeId)
                                        .Select(p => p.FullName)
                                        .FirstOrDefault();
            if (customermodel == null)
            {
                return HttpNotFound();
            }
            return View(customermodel);
        }
        #endregion

        #region helper
        public ActionResult GetDistrictBy(int ProvinceId)
        {
            var districts = _context.DistrictModel
                                    .Where(p => p.ProvinceId == ProvinceId)
                                    .OrderBy(p => p.DistrictName)
                                    .Select(p => new
                                    {
                                        Id = p.DistrictId,
                                        Name = p.Appellation + " " + p.DistrictName
                                    }).ToList();

            //return Content("success");
            return Json(districts, JsonRequestBehavior.AllowGet);
        }

        private void CreateViewBag(int? ProvinceId = null, int? DistrictId = null, int? EmployeeId = null, int? CustomerLevelId = null)
        {
            ViewBag.isUseCustomerLevelSearchId = isUseCustomerLevelSearchId;

            ViewBag.isUseEmployeeSearchId = isUseEmployeeSearchId;

            var provinces = _context.ProvinceModel.OrderBy(p => p.ProvinceName).ToList();
            ViewBag.ProvinceId = new SelectList(provinces, "ProvinceId", "ProvinceName", ProvinceId);

            var districts = _context.DistrictModel
                                    .Where(p => p.ProvinceId == ProvinceId)
                                    .OrderBy(p => p.DistrictName)
                                    .Select(p => new
                                    {
                                        Id = p.DistrictId,
                                        Name = p.Appellation + " " +  p.DistrictName
                                    }).ToList();
            ViewBag.DistrictId = new SelectList(districts, "Id", "Name", DistrictId);

            // EmployeeId
            var EmployeeIdList = _context.EmployeeModel.Where(p => p.Actived == true).OrderBy(p => p.FullName).ToList();
            ViewBag.EmployeeId = new SelectList(EmployeeIdList, "EmployeeId", "FullName", EmployeeId);

            //CustomerLevelModel
            List<CustomerLevelModel> listcustomlevel = _context.CustomerLevelModel.Where(p => p.Actived == true).ToList();
            ViewBag.CustomerLevelId = new SelectList(listcustomlevel, "CustomerLevelId", "CustomerLevelName", CustomerLevelId);
        }
        #endregion


        #region Sửa thông tin khách hàng
        
        public ActionResult Edit(int id = 0)
        {
            CustomerModel model = _context.CustomerModel
                                .Where(p => p.CustomerId == id)
                                .FirstOrDefault();

            CreateViewBag(model.ProvinceId, model.DistrictId, model.EmployeeId, model.CustomerLevelId);
            return View(model);

        }
        [HttpPost]
        public ActionResult Edit(CustomerModel model)
        {
            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        //#region Export danh sách khách hàng
        //public ActionResult Export()
        //{
        //    try
        //    {
        //        Excel.Application application = new Excel.Application();
        //        Excel.Workbook workbook = application.Workbooks.Add(System.Reflection.Missing.Value);
        //        Excel.Worksheet worksheet = workbook.ActiveSheet;

        //        worksheet.Cells[1, 1] = "CustomerID";
        //        worksheet.Cells[1, 2] = "CustomerLevelId";
        //        worksheet.Cells[1, 3] = "FullName";
        //        worksheet.Cells[1, 4] = "IdentityCard";
        //        worksheet.Cells[1, 5] = "Gender";
        //        worksheet.Cells[1, 6] = "BirthDay";
        //        worksheet.Cells[1, 7] = "EnterpriseName";
        //        worksheet.Cells[1, 8] = "TaxCode";
        //        worksheet.Cells[1, 9] = "Phone";
        //        worksheet.Cells[1, 10] = "Fax";
        //        worksheet.Cells[1, 11] = "Email";
        //        worksheet.Cells[1, 12] = "Note";
        //        worksheet.Cells[1, 13] = "ProvinceId";
        //        worksheet.Cells[1, 14] = "DistrictId";
        //        worksheet.Cells[1, 15] = "Address";
        //        worksheet.Cells[1, 16] = "AdditionalPurchase";
        //        worksheet.Cells[1, 17] = "Actived";
        //        int row = 2;
        //        List<CustomerModel> listCustommodel = _context.CustomerModel.ToList();
        //        foreach (CustomerModel c in listCustommodel)
        //        {
        //            worksheet.Cells[row, 1] = c.CustomerId;
        //            worksheet.Cells[row, 2] = c.CustomerLevelId;
        //            worksheet.Cells[row, 3] = c.FullName;
        //            worksheet.Cells[row, 4] = c.IdentityCard;
        //            worksheet.Cells[row, 5] = c.Gender;
        //            worksheet.Cells[row, 6] = c.BirthDay;
        //            worksheet.Cells[row, 7] = c.EnterpriseName;
        //            worksheet.Cells[row, 8] = c.TaxCode;
        //            worksheet.Cells[row, 9] = c.Phone;
        //            worksheet.Cells[row, 10] = c.Fax;
        //            worksheet.Cells[row, 11] = c.Email;
        //            worksheet.Cells[row, 12] = c.Note;
        //            worksheet.Cells[row, 13] = c.ProvinceId;
        //            worksheet.Cells[row, 14] = c.DistrictId;
        //            worksheet.Cells[row, 15] = c.Address;
        //            worksheet.Cells[row, 16] = c.AdditionalPurchase;
        //            worksheet.Cells[row, 17] = c.Actived;
        //            row++;
        //        }
        //        string strName = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
        //        workbook.SaveAs("D:\\Test\\"+strName+"mycustomer.xlsx");
        //        workbook.Close();
        //        Marshal.ReleaseComObject(workbook);

        //        application.Quit();
        //        Marshal.FinalReleaseComObject(application);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Resuilt = ex.Message;
        //    }
        //    return RedirectToAction("Index");
        //}
        //#endregion
        //#region Import danh sách khách hàng
        //[HttpPost]
        //public ActionResult Import(HttpPostedFileBase excelfile)
        //{
        //    if (excelfile == null || excelfile.ContentLength == 0)
        //    {
        //        ViewBag.Error = "Bạn vui lòng chọn 1 file excel";
        //        return View("Index");
        //    }
        //    else
        //    {
        //        if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
        //        {
        //            // lấy đường dẫn file
        //            string strName = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + excelfile.FileName;
        //            string path = Server.MapPath("~/Content/" + strName);
        //            // kiểm tra nếu tồn tại file thì xoá
        //            if (System.IO.File.Exists(path))
        //                System.IO.File.Delete(path);
        //            // Lưu file
        //            excelfile.SaveAs(path);

        //            // Tiến hành đọc data từ file excel
        //            Excel.Application application = new Excel.Application();
        //            Excel.Workbook workbook = application.Workbooks.Open(path);
        //            Excel.Worksheet worksheet = workbook.ActiveSheet;
        //            Excel.Range range = worksheet.UsedRange;
        //            for (int row = 3; row <= range.Rows.Count; row++)
        //            {
        //                CustomerModel c = new CustomerModel();
        //                c.CustomerLevelId = int.Parse(((Excel.Range)range.Cells[row, 2]).Text);
        //                c.FullName = ((Excel.Range)range.Cells[row, 3]).Text;
        //                c.IdentityCard = ((Excel.Range)range.Cells[row, 4]).Text;
        //                c.Gender = bool.Parse((((Excel.Range)range.Cells[row, 5]).Text));
        //                c.BirthDay = DateTime.Parse(((Excel.Range)range.Cells[row, 6]).Text);
        //                c.EnterpriseName = ((Excel.Range)range.Cells[row, 7]).Text;
        //                c.TaxCode = ((Excel.Range)range.Cells[row, 8]).Text;
        //                c.Phone = ((Excel.Range)range.Cells[row, 9]).Text;
        //                c.Fax = ((Excel.Range)range.Cells[row, 10]).Text;
        //                c.Email = ((Excel.Range)range.Cells[row, 11]).Text;
        //                c.ImageUrl = ((Excel.Range)range.Cells[row, 12]).Text;
        //                c.Note = ((Excel.Range)range.Cells[row, 13]).Text;
        //                c.ProvinceId = int.Parse(((Excel.Range)range.Cells[row, 14]).Text);
        //                c.DistrictId = int.Parse(((Excel.Range)range.Cells[row, 15]).Text);
        //                c.Address = ((Excel.Range)range.Cells[row, 16]).Text;
        //                c.AdditionalPurchase = decimal.Parse(((Excel.Range)range.Cells[row, 17]).Text);
        //                c.Actived = bool.Parse(((Excel.Range)range.Cells[row, 18]).Text);
        //                _context.CustomerModel.Add(c);
        //                _context.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.Error = "Kiểu file không hợp lệ";
        //        }
        //        return RedirectToAction("Index");
        //    }
        //}
        //#endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;

//using Excel = Microsoft.Office.Interop.Excel;
//using System.Runtime.InteropServices;

namespace WebUI.Controllers
{
    public class SupplierController : BaseController
    {
        //
        // GET: /Customer/
        #region danh sách nhà cung cấp
        
        public ActionResult Index()
        {
            return View(_context.SupplierModel.OrderBy(p =>p.SupplierName).ToList());
        }
        #endregion

        #region Thêm mới nhà cung cấp
        
        public ActionResult Create()
        {
            //checkSession();

            // list customlevelmodel
            SupplierModel Supplier = new SupplierModel() { Actived = true };
            CreateViewBag();
            return View(Supplier);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SupplierModel Supplier)
        {
            if (ModelState.IsValid)
            {
                _context.SupplierModel.Add(Supplier);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Supplier);
        }
        #endregion

        #region Chi tiết nhà cung cấp
        
        public ActionResult Details(int id = 0)
        {
            SupplierModel Supplier = _context.SupplierModel.Find(id);
            if (Supplier == null)
            {
                return HttpNotFound();
            }
            return View(Supplier);
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

        private void CreateViewBag(int? ProvinceId = null, int? DistrictId = null)
        {
            var provinces = _context.ProvinceModel.OrderBy(p => p.ProvinceName).ToList();
            ViewBag.ProvinceId = new SelectList(provinces, "ProvinceId", "ProvinceName", ProvinceId);

            var districts = _context.DistrictModel
                                    .Where(p => p.ProvinceId == ProvinceId)
                                    .OrderBy(p => p.DistrictName)
                                    .Select(p => new
                                    {
                                        Id = p.DistrictId,
                                        Name = p.Appellation + " " + p.DistrictName
                                    }).ToList();
            ViewBag.DistrictId = new SelectList(districts, "Id", "Name", DistrictId);

        }
        #endregion

        #region Sửa thông tin nhà cung cấp
        
        public ActionResult Edit(int id = 0)
        {
            SupplierModel Supplier = _context.SupplierModel
                                .Where(p => p.SupplierId == id)
                                .FirstOrDefault();
            CreateViewBag(Supplier.ProvinceId, Supplier.DistrictId);
            return View(Supplier);

        }
        [HttpPost]
        public ActionResult Edit(SupplierModel Supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(Supplier).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            CreateViewBag(Supplier.ProvinceId, Supplier.DistrictId);
            return View(Supplier);
        }
        #endregion

        //#region Export danh sách nhà cung cấp
        //public ActionResult Export()
        //{
        //    try
        //    {
        //        Excel.Application application = new Excel.Application();
        //        Excel.Workbook workbook = application.Workbooks.Add(System.Reflection.Missing.Value);
        //        Excel.Worksheet worksheet = workbook.ActiveSheet;

        //        worksheet.Cells[1, 1] = "SupplierId";
        //        worksheet.Cells[1, 2] = "SupplierCode";
        //        worksheet.Cells[1, 3] = "SupplierName";
        //        worksheet.Cells[1, 4] = "Phone";
        //        worksheet.Cells[1, 5] = "Email";
        //        worksheet.Cells[1, 6] = "TaxCode";
        //        worksheet.Cells[1, 7] = "hasInvoice";
        //        worksheet.Cells[1, 8] = "isPersonal";
        //        worksheet.Cells[1, 9] = "IdentityCard";
        //        worksheet.Cells[1, 10] = "ProvinceId";
        //        worksheet.Cells[1, 11] = "DistrictId";
        //        worksheet.Cells[1, 12] = "Address";
        //        worksheet.Cells[1, 13] = "BankName";
        //        worksheet.Cells[1, 14] = "BankBranch";
        //        worksheet.Cells[1, 15] = "BankAccountNumber";
        //        worksheet.Cells[1, 16] = "BankOwner";
        //        worksheet.Cells[1, 17] = "Note";
        //        worksheet.Cells[1, 18] = "Actived";
        //        int row = 2;
        //        List<SupplierModel> listCustommodel = _context.SupplierModel.ToList();
        //        foreach (SupplierModel c in listCustommodel)
        //        {
        //            worksheet.Cells[row, 1] = c.SupplierId;
        //            worksheet.Cells[row, 2] = c.SupplierCode;
        //            worksheet.Cells[row, 3] = c.SupplierName;
        //            worksheet.Cells[row, 4] = c.Phone;
        //            worksheet.Cells[row, 5] = c.Email;
        //            worksheet.Cells[row, 6] = c.TaxCode;
        //            worksheet.Cells[row, 7] = c.hasInvoice;
        //            worksheet.Cells[row, 8] = c.isPersonal;
        //            worksheet.Cells[row, 9] = c.IdentityCard;
        //            worksheet.Cells[row, 10] = c.ProvinceId;
        //            worksheet.Cells[row, 11] = c.DistrictId;
        //            worksheet.Cells[row, 12] = c.Address;
        //            worksheet.Cells[row, 13] = c.BankName;
        //            worksheet.Cells[row, 14] = c.BankBranch;
        //            worksheet.Cells[row, 15] = c.BankAccountNumber;
        //            worksheet.Cells[row, 16] = c.BankOwner;
        //            worksheet.Cells[row, 17] = c.Note;
        //            worksheet.Cells[row, 18] = c.Actived;
        //            row++;
        //        }
        //        string strName = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
        //        workbook.SaveAs("E:\\Test\\" + strName + "mySupplier.xlsx");
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
        //#region Import thêm nhà cung cấp
        //public ActionResult import(HttpPostedFileBase file)
        //{
        //    if (file.ContentLength == 0)
        //    {
        //        ViewBag.message = "Vui lòng chọn lại file";
        //    }
        //    else
        //    {
        //        if(file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
        //        {
        //            string path= Server.MapPath("~/Content/"+file.FileName);
        //            if (System.IO.File.Exists(path))
        //            {
        //                System.IO.File.Delete(path);
        //            }
        //            file.SaveAs(path);
        //            return RedirectToAction("Index");
        //            // Tiến hành đọc data từ file excel
        //            Excel.Application application = new Excel.Application();
        //            Excel.Workbook workbook = application.Workbooks.Open(path);
        //            Excel.Worksheet worksheet = workbook.ActiveSheet;
        //            Excel.Range range = worksheet.UsedRange;
        //            for (int row = 1; row <= range.Rows.Count; row++)
        //            {
        //                SupplierModel c = new SupplierModel();
        //                c.SupplierId = int.Parse(((Excel.Range)range.Cells[row, 2]).Text);
        //                c.SupplierCode = int.Parse(((Excel.Range)range.Cells[row, 3]).Text);
        //                c.SupplierName = ((Excel.Range)range.Cells[row, 4]).Text;
        //                c.Phone = ((Excel.Range)range.Cells[row, 5]).Text;
        //                c.Email = ((Excel.Range)range.Cells[row, 6]).Text;
        //                c.TaxCode = ((Excel.Range)range.Cells[row, 7]).Text;
        //                c.hasInvoice = bool.Parse((((Excel.Range)range.Cells[row, 8]).Text));
        //                c.isPersonal = bool.Parse((((Excel.Range)range.Cells[row, 9]).Text));
        //                c.IdentityCard =((Excel.Range)range.Cells[row, 10]).Text;
        //                c.ProvinceId = int.Parse(((Excel.Range)range.Cells[row, 11]).Text);
        //                c.DistrictId = int.Parse(((Excel.Range)range.Cells[row, 12]).Text);
        //                c.Address = ((Excel.Range)range.Cells[row, 13]).Text;
        //                c.BankName = ((Excel.Range)range.Cells[row, 14]).Text;
        //                c.BankBranch = ((Excel.Range)range.Cells[row, 15]).Text;
        //                c.BankAccountNumber = ((Excel.Range)range.Cells[row, 16]).Text;
        //                c.BankOwner = ((Excel.Range)range.Cells[row, 17]).Text;
        //                c.Note = ((Excel.Range)range.Cells[row, 18]).Text;
        //                c.Actived = bool.Parse((((Excel.Range)range.Cells[row, 19]).Text));
        //                _context.SupplierModel.Add(c);
        //                _context.SaveChanges();
        //            }

        //        }
        //    }
        //    return RedirectToAction("Index");
        //}
        //#endregion

    }
}

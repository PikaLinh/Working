using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ViewModels;
using System.Transactions;
using System.IO;
using System.Drawing;

namespace WebUI.Controllers
{
    public class NewsController : BaseController
    {
        // GET: New
        
        public ActionResult Index()
        {
            var list = (from n in _context.Website_NewsModel
                        join c in _context.CategoryModel on n.CategoryId equals c.CategoryId
                        where n.Actived == true
                        select new Website_NewsInfoModel()
                        {
                            NewsID = n.NewsID,
                            CategoryName = c.CategoryName,
                            Title = n.Title,
                            PostDate = n.PostDate,
                            Views = n.Views,
                        })
                        .OrderByDescending(p =>p.NewsID)
                        .ToList();
            //var list = _context.Website_NewsModel.Where(p=>p.Actived == true).ToList();
            return View(list);
        }

        public ActionResult Create()
        {
            CreateViewBag(null);
            Website_NewsModel model = new Website_NewsModel()
            {
                Visible = true,
            };
            return View(model);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Website_NewsModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    // Lưu Website_NewsModel
                    model.UserId = currentAccount.EmployeeId;
                    model.PostDate = DateTime.Now;
                    model.Views = 0;
                    model.SEOTitle = Library.ConvertToNoMarkString(model.Title);
                    if(file != null)
                    {
                        string filename = file.FileName;
                        int index = filename.IndexOf(".");
                        string type = filename.Substring(index);

                        int size = file.ContentLength;
                        string ContentType = file.ContentType;

                        byte[] FileContent = imageToByteArray(file.InputStream);
                        model.ImageUrl = Upload(file, "Website_News");

                        //lưu
                        SYS_tblFile FileSave = new SYS_tblFile()
                        {
                            FileTitle = filename,
                            FileName = model.ImageUrl,
                            Extension = type,
                            ContentType = ContentType,
                            FileContent = FileContent,
                            FolderId = 2,
                            Size = size,
                            CreatedByUserId = model.UserId,
                            CreatedOnDate = model.PostDate
                        };
                        _context.Entry(FileSave).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        model.FileId = FileSave.FileId;
                    }
                    model.Actived = true;
                    _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                    ts.Complete();
                }
                return RedirectToAction("Index");
            }
            else
            {
                CreateViewBag(null);
                return View(model);
            }
        }
        public void CreateViewBag(int? CategoryId = null)
        {
            //1. CategoryId
            CategoryRepository repository = new CategoryRepository(_context);
            var CategoryList = repository.GetCategoryByParentWithFormat(7).Select(p => new { CategoryId = p.CategoryId, CategoryName = p.CategoryName.Substring(4) }).ToList();
            CategoryList.RemoveAt(0);
            //CategoryList.Insert(0, new { CategoryId = 7, CategoryName = "Tất cả tin tức" });
            ViewBag.CategoryId = new SelectList(CategoryList, "CategoryId", "CategoryName", CategoryId);

            //// 2. OriginOfProductId
            //var OriginOfProductList = _context.OriginOfProductModel.OrderBy(p => p.OriginOfProductName).ToList();
            //ViewBag.OriginOfProductId = new SelectList(OriginOfProductList, "OriginOfProductId", "OriginOfProductName", OriginOfProductId);

            //// 3.
            //var ProductStatusNameList = _context.ProductStatusModel.OrderBy(p => p.ProductStatusName).ToList();
            //ViewBag.ProductStatusId = new SelectList(ProductStatusNameList, "ProductStatusId", "ProductStatusName", ProductStatusId);

        }

        public ActionResult Details(int id)
        {
            var model = _context.Website_NewsModel.Find(id);
            if (model == null)
            {
                model = new Website_NewsModel();
            }
            else
            {
                if (model.FileId != null)
                {
                    ViewBag.pathImage = GetPath(model.FileId);
                }
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            CreateViewBag(null);
            var model = _context.Website_NewsModel.Find(id);

            if(model == null)
            {
                model = new Website_NewsModel(); 
            }
            else 
            {
                if (model.FileId != null)
                {
                    ViewBag.pathImage = GetPath(model.FileId);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Website_NewsModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    if (file != null)
                    {
                        string filename = file.FileName;
                        int index = filename.IndexOf(".");
                        string type = filename.Substring(index);

                        int size = file.ContentLength;
                        string ContentType = file.ContentType;

                        byte[] FileContent = imageToByteArray(file.InputStream);
                        model.ImageUrl = Upload(file, "Website_News");

                        //lưu
                        SYS_tblFile FileSave = new SYS_tblFile()
                        {
                            FileTitle = filename,
                            FileName = model.ImageUrl,
                            Extension = type,
                            ContentType = ContentType,
                            FileContent = FileContent,
                            FolderId = 2,
                            Size = size,
                            CreatedByUserId = model.UserId,
                            CreatedOnDate = model.PostDate
                        };
                        _context.Entry(FileSave).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        model.FileId = FileSave.FileId;
                    }
                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    ts.Complete();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                CreateViewBag(null);
                return View(model);
            }
        }

        public byte[] imageToByteArray(Stream inputStream)
        {
            byte[] data;
            MemoryStream memoryStream = inputStream as MemoryStream;
            if (memoryStream == null)
            {
                memoryStream = new MemoryStream();
                inputStream.CopyTo(memoryStream);
            }
            data = memoryStream.ToArray();
            return data;
        }

        public string GetPath(int? FileId)
        {
            var FileSave = _context.SYS_tblFile.Find(FileId);

            string pathFolder = _context.SYS_tblFolder
                        .Where(p => p.FolderId == FileSave.FolderId)
                        .Select(p => p.FolderPath)
                        .FirstOrDefault();

            int index = FileSave.Extension.IndexOf(".") + 1;
            string type = FileSave.Extension.Substring(index);

            string pathFile =  Server.MapPath("~" + pathFolder+ "/" + FileSave.FileName);

            string pathReturn = pathFolder + "/Thum/" + FileSave.FileName;
            string pathFileThum = Server.MapPath("~" + pathReturn);

            // Nếu Folder ko tồn tại thì tạo Folder
            if (!Directory.Exists(pathFolder))
            {
                //Tạo Folder
                Directory.CreateDirectory(Server.MapPath(pathFolder));

                //Lưu file
                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                //pathFile
                MemoryStream ms = new MemoryStream(FileSave.FileContent);
                Image Img = Image.FromStream(ms);
                SaveImage(Img, type, pathFile);

                //pathFileThum
                MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                Image Img2 = Image.FromStream(ms2);
                SaveImageThum(Img2, type, pathFileThum);

                return pathReturn;
            }
            else
            {

                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                if (!System.IO.File.Exists(pathFile))
                {
                    MemoryStream ms = new MemoryStream(FileSave.FileContent);
                    Image Img = Image.FromStream(ms);
                    SaveImage(Img, type, pathFile);
                }

                if (!System.IO.File.Exists(pathFileThum))
                {
                    MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                    Image Img2 = Image.FromStream(ms2);
                    SaveImageThum(Img2, type, pathFileThum);
                }

                return pathReturn;

            }
        }

        public string GetPathWidth(int? FileId, int Width)
        {
            var FileSave = _context.SYS_tblFile.Find(FileId);

            string pathFolder = _context.SYS_tblFolder
                        .Where(p => p.FolderId == FileSave.FolderId)
                        .Select(p => p.FolderPath)
                        .FirstOrDefault();

            int index = FileSave.Extension.IndexOf(".") + 1;
            string type = FileSave.Extension.Substring(index);

            string pathFile = Server.MapPath("~" + pathFolder + "/" + FileSave.FileName);

            string pathReturn = pathFolder + "/Thum/" + FileSave.FileName;
            string pathFileThum = Server.MapPath("~" + pathReturn);

            // Nếu Folder ko tồn tại thì tạo Folder
            if (!Directory.Exists(pathFolder))
            {
                //Tạo Folder
                Directory.CreateDirectory(Server.MapPath(pathFolder));

                //Lưu file
                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                //pathFile
                MemoryStream ms = new MemoryStream(FileSave.FileContent);
                Image Img = Image.FromStream(ms);
                SaveImage(Img, type, pathFile);

                //pathFileThum
                MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                Image Img2 = Image.FromStream(ms2);
                SaveImageWidth(Img2, type, pathFileThum, Width);

                return pathReturn;
            }
            else
            {

                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                if (!System.IO.File.Exists(pathFile))
                {
                    MemoryStream ms = new MemoryStream(FileSave.FileContent);
                    Image Img = Image.FromStream(ms);
                    SaveImage(Img, type, pathFile);
                }

                if (!System.IO.File.Exists(pathFileThum))
                {
                    MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                    Image Img2 = Image.FromStream(ms2);
                    SaveImageWidth(Img2, type, pathFileThum,Width);
                }

                return pathReturn;

            }
        }

        public string GetPathWidthHeight(int? FileId, int Width,int Height)
        {
            var FileSave = _context.SYS_tblFile.Find(FileId);

            string pathFolder = _context.SYS_tblFolder
                        .Where(p => p.FolderId == FileSave.FolderId)
                        .Select(p => p.FolderPath)
                        .FirstOrDefault();

            int index = FileSave.Extension.IndexOf(".") + 1;
            string type = FileSave.Extension.Substring(index);

            string pathFile = Server.MapPath("~" + pathFolder + "/" + FileSave.FileName);

            string pathReturn = pathFolder + "/Thum/" + FileSave.FileName;
            string pathFileThum = Server.MapPath("~" + pathReturn);

            // Nếu Folder ko tồn tại thì tạo Folder
            if (!Directory.Exists(pathFolder))
            {
                //Tạo Folder
                Directory.CreateDirectory(Server.MapPath(pathFolder));

                //Lưu file
                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                //pathFile
                MemoryStream ms = new MemoryStream(FileSave.FileContent);
                Image Img = Image.FromStream(ms);
                SaveImage(Img, type, pathFile);

                //pathFileThum
                MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                Image Img2 = Image.FromStream(ms2);
                SaveImageWidthHeight(Img2, type, pathFileThum, Width, Height);

                return pathReturn;
            }
            else
            {

                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                if (!System.IO.File.Exists(pathFile))
                {
                    MemoryStream ms = new MemoryStream(FileSave.FileContent);
                    Image Img = Image.FromStream(ms);
                    SaveImageWidthHeight(Img, type, pathFile, Width,Height);
                }

                if (!System.IO.File.Exists(pathFileThum))
                {
                    MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                    Image Img2 = Image.FromStream(ms2);
                    SaveImageWidthHeight(Img2, type, pathFileThum, Width, Height);
                }

                return pathReturn;

            }
        }

        public void SaveImageWidth(Image img, string type, string pathFileThum, int Width)
        {

            //int maxWidth = 1600, maxHeight = 1600;
            int w = img.Width;
            int h = img.Height;

            if (w > Width)
            {
                double ratio = (double)h / w;
                int Height = Convert.ToInt32(ratio * Width);
                var newImage = new Bitmap(Width, Height);

                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(img, 0, 0, Width, Height);
                newImage.Save(pathFileThum);
            }
            else
            {
                var newImage = new Bitmap(w, h);

                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(img, 0, 0, w, h);

                newImage.Save(pathFileThum);
            }
        }

        public void SaveImageWidthHeight(Image img, string type, string pathFileThum, int Width, int Height)
        {

            //int maxWidth = 1600, maxHeight = 1600;
            int w = img.Width;
            int h = img.Height;

            if (w > Width || h > Height)
            {
                var newImage = new Bitmap(Width, Height);
                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(img, 0, 0, Width, Height);
                newImage.Save(pathFileThum);
            }
            else
            {
                var newImage = new Bitmap(w, h);

                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(img, 0, 0, w, h);

                newImage.Save(pathFileThum);
            }
        }

        public void SaveImage(Image img, string type, string pathFile)
        {

            int maxWidth = 1600,  maxHeight = 1600;
            if (type.ToLower() != "gif" && type.ToLower() != "png")
            {
                int w = img.Width;
                int h = img.Height;

                //save to root folder
                if (w >= maxWidth || h >= maxHeight)
                {
                    double ratio = (double)h / w;
                    int  Height = Convert.ToInt32(ratio * maxWidth);
                    var newImage = new Bitmap(maxWidth, Height);

                    using (var graphics = Graphics.FromImage(newImage))
                        graphics.DrawImage(img, 0, 0, maxWidth, Height);
                    newImage.Save(pathFile);
                }
                else
                {
                    var newImage = new Bitmap(w, h);

                    using (var graphics = Graphics.FromImage(newImage))
                        graphics.DrawImage(img, 0, 0, w, h);

                    newImage.Save(pathFile);
                }
            }
            else
            {
                img.Save(pathFile);
            }
        }

        public void SaveImageThum(Image img, string type, string pathFileThum)
        {

            int minWidth = 250; 
            int minHeight = 500;
            //int maxWidth = 1600;

            if (type.ToLower() != "gif" && type.ToLower() != "png")
            {
                int w = img.Width;
                int h = img.Height;
                //save to root folder
                if (w >= minWidth || h >= minHeight)
                {
                    double ratio = (double)h / w;
                    int Height = Convert.ToInt32(ratio * minWidth);

                    var newImage = new Bitmap(minWidth, Height);
                    using (var graphics = Graphics.FromImage(newImage))
                        graphics.DrawImage(img, 0, 0, minWidth, Height);
                    newImage.Save(pathFileThum);
                }
                else
                {
                    var newImage = new Bitmap(w, h);

                    using (var graphics = Graphics.FromImage(newImage))
                        graphics.DrawImage(img, 0, 0, w, h);
                    newImage.Save(pathFileThum);
                }
            }
            else
            {
                img.Save(pathFileThum);
            }
        }

    }
}



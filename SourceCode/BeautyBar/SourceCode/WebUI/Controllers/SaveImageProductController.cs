using EntityModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class SaveImageProductController : BaseController
    {
        // GET: SaveImageProduct
        public ActionResult Index()
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    int count = 0;
                    var list = _context.ProductModel.Where(p => p.FileId == null && p.ImageUrl != "noimage.jpg").ToList();
                    foreach (var item in list)
                    {
                        string filepath = Server.MapPath("/Upload/Product/Thum/" + item.ImageUrl);
                        if (System.IO.File.Exists(filepath) && item.ImageUrl != "noimage.jpg")
                        {
                            string filename = item.ImageUrl;
                            int index = filename.IndexOf(".");
                            string type = filename.Substring(index);
                            string type2 = filename.Substring(index + 1);
                            string ContentType = "Image/" + type2;

                            Image myImg = Image.FromFile(filepath);
                            //System.IO.FileInfo info = new System.IO.FileInfo(filepath);

                            byte[] FileContent = imageToByteArray(myImg);
                            int size = FileContent.Length;

                            //addmodel.ImageUrl = Upload(file, "Product");

                            SYS_tblFile FileSave = new SYS_tblFile()
                            {
                                FileTitle = filename,
                                FileName = item.ImageUrl,
                                Extension = type,
                                ContentType = ContentType,
                                FileContent = FileContent,
                                FolderId = 1,
                                Size = size,
                                CreatedByUserId = currentAccount.EmployeeId,
                                CreatedOnDate = DateTime.Now
                            };
                            _context.Entry(FileSave).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            item.FileId = FileSave.FileId;
                            _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                            count++;
                        }
                    }
                    ViewBag.Message = "Sao lưu  thành công "+ count +" file hình ảnh !";
                    ts.Complete();
                }
            }
            catch
            {
                ViewBag.Message = "Sao lưu không thành công , lỗi trong quá trình sao lưu hình ảnh !";
            }
            return View();
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }

    }
}
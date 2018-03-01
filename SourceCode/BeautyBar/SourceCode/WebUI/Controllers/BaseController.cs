using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using Repository;
using ViewModels;
using System.Security.Claims;

namespace WebUI.Controllers
{
    public class BaseController : Controller
    {
        public EntityDataContext _context = new EntityDataContext();
        protected string Upload(HttpPostedFileBase file, string folder, int minWidth = 250, int maxWidth = 1600, int minHeight = 500, int maxHeight = 1600)
        {
            string ret = "";
            string parth = "";
            string thumparth = "";
            try
            {
                if (file != null && file.ContentLength > 0)
                {// nếu có chọn file
                    
                    string filename = Library.ConvertToNoMarkString(file.FileName);
                    string type = filename.Substring(filename.Length - 4);
                    //Nếu là jpeg thì đổi thành jpg 
                    if (type.ToLower() == "jpeg")
                    {
                        filename = filename.Substring(0, filename.Length - 5) + ".jpg";
                    }
                    //Đổi tên lại thành chuỗi phân biệt tránh trùng
                    filename = filename.Substring(0, filename.Length - 3) + "." + filename.Substring(filename.Length - 3);
                    string strName = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + filename;

                    type = filename.Substring(filename.Length - 3);


                    parth = Server.MapPath("~/Upload/" + folder + "/" + strName);
                    thumparth = Server.MapPath("~/Upload/" + folder + "/thum/" + strName);
                    //gán giá trị trả về
                    ret = strName;

                    //Nếu không phải ảnh động hay ảnh trong suốt thì tiến hành resize
                    if (type.ToLower() != "gif" && type.ToLower() != "png")
                    {
                        var img = System.Drawing.Image.FromStream(file.InputStream, true, true);
                        int w = img.Width;
                        int h = img.Height;
                        //save to root folder
                        if (w >= maxWidth || h >= maxHeight)
                        {
                            Library.ResizeStream(maxWidth, maxHeight, file.InputStream, parth);
                        }
                        else
                        {
                            Library.ResizeStream(w, h, file.InputStream, parth);
                        }
                        //save to thum
                        if (w >= minWidth || h >= minHeight)
                        {
                            Library.ResizeStream(maxWidth, minHeight, file.InputStream, thumparth);
                        }
                        else
                        {
                            Library.ResizeStream(w, h, file.InputStream, thumparth);
                        }
                    }
                    else
                    {
                        file.SaveAs(parth);
                        file.SaveAs(thumparth);
                    }
                }
                else
                {
                    ret = "noimage.jpg";
                }
            }
            catch
            {
                ret = "noimage.jpg";
            }
            return ret;
        }

        public int LanguageId { get { return (int)Repository.LanguageEnum.VietNamese; } }
      
        public AccountModel currentAccount
        {
            get
            {
                if (Session["acc"] != null)
                {
                    return (AccountModel)Session["acc"];
                }
                else
                {
                    var tmp = _context.AccountModel.Find(CurrentUser.UserId);
                    Session["acc"] = tmp;
                    return tmp;
                }
            }
        }

        public EmployeeModel currentEmployee
        {
            get
            {
                if (Session["emp"] != null)
                {
                    return (EmployeeModel)Session["emp"];
                }
                else 
                {
                    var tmp = _context.EmployeeModel.Find(CurrentUser.EmpId);
                    Session["emp"] = tmp;
                    return tmp;
                }
            }
        }

        public AppUserPrincipal CurrentUser
        {
            get
            {
                return new AppUserPrincipal(this.User as ClaimsPrincipal);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}

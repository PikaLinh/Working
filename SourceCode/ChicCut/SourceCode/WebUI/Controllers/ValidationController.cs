using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository;

namespace WebUI.Controllers
{
    public class ValidationController : BaseController
    {
        #region check validate accountmodel
        //public JsonResult ValidationEmail(string Email, string InitialEmail)
        //{
        //    AccountRepository repository = new AccountRepository(db);
        //    if (repository.EmailExists(Email) && (InitialEmail != Email))
        //    {
        //        return base.Json("Email \"" + Email + "\" đ\x00e3 tồn tại!", JsonRequestBehavior.AllowGet);
        //    }
        //    return base.Json(true, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult ValidationPhone(string Phone, string InitialPhone)
        {
            CustomerRepository repository = new CustomerRepository(_context);
            if (repository.PhoneExists(Phone) && (InitialPhone != Phone))
            {
                //if (LanguageId == 10001)
                //{
                //    return base.Json("\"" + UserName + "\" is exists!", JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                return base.Json("Số điện thoại \"" + Phone + "\" đ\x00e3 tồn tại!", JsonRequestBehavior.AllowGet);
                //}
            }
            return base.Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationUserName(string UserName, string InitialUserName)
        {
            AccountRepository repository = new AccountRepository(_context);
            if (repository.UserExists(UserName) && (InitialUserName != UserName))
            {
                //if (LanguageId == 10001)
                //{
                //    return base.Json("\"" + UserName + "\" is exists!", JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                    return base.Json("T\x00e0i khoản \"" + UserName + "\" đ\x00e3 tồn tại!", JsonRequestBehavior.AllowGet);
                //}
            }
            return base.Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Check validate CodeAccount
        public JsonResult ValidationCodeAccount(string Code, int StoreId, string InitialCodeAccountCode)
        {
            AM_AccountRepository repository = new AM_AccountRepository(_context);
            if (repository.CodeAccountExists(Code, StoreId) && (InitialCodeAccountCode != Code) )
            {
                return base.Json("T\x00e0i khoản \"" + Code + "\" đ\x00e3 tồn tại!", JsonRequestBehavior.AllowGet);
            }
            return base.Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Check validate CodeAccount
        public JsonResult ValidationCodeProduct(string ProductCode, string InitialCodeProduct)
        {
            ProductRepository repository = new ProductRepository(_context);
            if (repository.CodeProductExists(ProductCode) && (InitialCodeProduct != ProductCode))
            {
                return base.Json("Mã sản phẩm \"" + ProductCode + "\" đ\x00e3 tồn tại!", JsonRequestBehavior.AllowGet);
            }
            return base.Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}

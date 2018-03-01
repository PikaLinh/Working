using Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers
{
    public class OrderReturnReportViewController : BaseController
    {
        //
        // GET: /OrderReturnReport/
         
        public ActionResult Index(int id)
        {
            ViewBag.OrderReturnMasterId = id;
           // int OrderId = _context.OrderReturnModel.Where(p => p.OrderReturnMasterId == id).Select(p => p.OrderId.Value).FirstOrDefault();
           // int TransactionId = _context.AM_TransactionModel.Where(p => p.OrderId == OrderId && p.TransactionTypeCode == EnumTransactionType.BHTRA).Select(p => p.TransactionId).FirstOrDefault();
            int TransactionId = (from p in _context.OrderReturnModel
                                      join tm in _context.AM_TransactionModel on p.OrderId equals tm.OrderId
                                      orderby (tm.TransactionId) ascending
                                      where (p.CreatedDate <= tm.CreateDate && tm.OrderId == p.OrderId && p.OrderReturnMasterId == id && tm.TransactionTypeCode == EnumTransactionType.BHTRA && tm.Amount != 0)
                                      select tm.TransactionId 
                                    ).FirstOrDefault();
            ViewBag.TransactionId = TransactionId;

            return View();
        }

    }
}

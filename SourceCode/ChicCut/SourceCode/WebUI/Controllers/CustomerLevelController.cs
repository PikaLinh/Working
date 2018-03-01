using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using System.Data;
using System.Data.Entity;


namespace WebUI.Controllers
{
    public class CustomerLevelController : Controller
    {
        //
        // GET: /CustomerLevel/
        private EntityDataContext db = new EntityDataContext();
        
        public ActionResult Index()
        {
            return View(db.CustomerLevelModel.OrderBy(p=>p.MinimumPurchase).ToList());
        }

        
        public ActionResult Details(int id = 0)
        {
            CustomerLevelModel CustomerLevel = db.CustomerLevelModel.Find(id);
            if (CustomerLevel == null)
            {
                return HttpNotFound();
            }
            return View(CustomerLevel);
        }

        //
        // GET: /CustomerLevel/Create

        
        public ActionResult Create()
        {
            CustomerLevelModel CustomerLevel = new CustomerLevelModel();
            CustomerLevel.Actived = true;
            return View(CustomerLevel);
        }

        //
        // POST: /CustomerLevel/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerLevelModel CustomerLevel)
        {
            if (ModelState.IsValid)
            {
                db.CustomerLevelModel.Add(CustomerLevel);
                db.SaveChanges();
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_InsertPrice";
                        cmd.Parameters.AddWithValue("@CustomerLevelId", CustomerLevel.CustomerLevelId);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                return RedirectToAction("Index");
            }

            return View(CustomerLevel);
        }

        //
        // GET: /CustomerLevel/Edit/5

        
        public ActionResult Edit(int id = 0)
        {
            CustomerLevelModel CustomerLevel = db.CustomerLevelModel.Find(id);
            if (CustomerLevel == null)
            {
                return HttpNotFound();
            }
            return View(CustomerLevel);
        }

        //
        // POST: /CustomerLevel/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerLevelModel CustomerLevel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(CustomerLevel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(CustomerLevel);
        }

     

    }
}

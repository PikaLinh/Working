using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class CategoryController : BaseController
    {
        bool isUseIsHasChildren = false;
        bool isUseDescription = false;
        bool isUseKeywords = false;
        bool isUseImageUrl = false;
        bool isUseOrderBy = true;
        bool isUseisDisplayOnHomePage = false;
        //For root category dropdownlist 
        bool isUseMultiParent = true;

        string Title = "Quản lý danh mục";

         
        public ActionResult Index(int id, int ParentId = 0)
        {
            CategoryRepository repository = new CategoryRepository(_context);
            List<CategoryViewModel> catlst = new List<CategoryViewModel>();
            if (ParentId == 0)
            {
                catlst = repository.GetCategoryByParent(id);
            }
            else
            {
                ViewBag.ParentId = ParentId;
                catlst = repository.GetCategoryByParent(ParentId);
            }

            //Tạo dropdownlist
            CreateRootMenu(id, ParentId);

            return View(catlst);
        }
        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost(int id, int Root)
        {
            CategoryRepository repository = new CategoryRepository(_context);
            CreateRootMenu(id);
            ViewBag.ParentId = Root;
            List<CategoryViewModel> catlst = repository.GetCategoryByParent(Root);
            return View(catlst);
        }
        //
        // GET: /Category/Create
        
        public ActionResult Create(int id, int ParentId = 0)
        {
            CreateRootMenu(id, ParentId);
            CategoryModel categorymodel = new CategoryModel();
            categorymodel.OrderBy = 0;
            categorymodel.isHasChildren = false;
            categorymodel.Actived = true;
            categorymodel.ImageUrl = "noimage.jpg";
            return View(categorymodel);
        }

        //
        // POST: /Category/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, CategoryModel categorymodel, int Root, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                CategoryRepository catRepository = new CategoryRepository(_context);
                categorymodel.ImageUrl = Upload(file, "Category");
                categorymodel.Parent = Root;
                catRepository.InsertCategory(categorymodel);
                return RedirectToAction("Index", new { id = id, ParentId = Root });
            }
            CreateRootMenu(id, Root);
            return View(categorymodel);
        }
        
        public ActionResult Details(int id = 0, int RootId = 0, int ParentId = 0)
        {
            CategoryRepository catRepository = new CategoryRepository(_context);
            // Khong co thong so RootId
            if (RootId == 0)
            {
                return RedirectToAction("Index", "Admin", null);
            }

            CategoryModel categorymodel = catRepository.Find(id);
            if (categorymodel == null)
            {
                return HttpNotFound();
            }

            CreateRootMenu(RootId, ParentId);
            return View(categorymodel);
        }


        //
        // GET: /Category/Edit/5
         
        public ActionResult Edit(int id = 0, int RootId = 0, int ParentId = 0)
        {
            CategoryRepository catRepository = new CategoryRepository(_context);
            // Khong co thong so RootId
            if (RootId == 0)
            {
                return RedirectToAction("Index", "Admin", null);
            }

            CategoryModel categorymodel = catRepository.Find(id);
            if (categorymodel == null)
            {
                return HttpNotFound();
            }

            CreateRootMenu(RootId, ParentId);
            return View(categorymodel);
        }

        //
        // POST: /Category/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryModel categorymodel, int Root, HttpPostedFileBase file, int RootId = 0, int ParentId = 0)
        {
            if (ModelState.IsValid)
            {
                CategoryRepository catRepository = new CategoryRepository(_context);
                string imagename = Upload(file, "Category");
                if (imagename != "noimage.jpg")
                {
                    categorymodel.ImageUrl = imagename;
                }
                categorymodel.Parent = Root;
                ViewBag.ParentId = ParentId;
                catRepository.UpdateCategory(categorymodel);
                return RedirectToAction("Index", new { id = RootId, ParentId = ParentId });
            }
            CreateRootMenu(RootId, ParentId);
            return View(categorymodel);
        }

        //
        // GET: /Category/Delete/5
         
        public ActionResult Delete(int id = 0, int Parent = 0)
        {
            CategoryRepository catRepository = new CategoryRepository(_context);
            CategoryModel categorymodel = catRepository.Find(id);
            if (categorymodel == null)
            {
                return HttpNotFound();
            }
            CreateRootMenu(Parent);
            return View(categorymodel);
        }

        //
        // POST: /Category/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int RootId)
        {
            CategoryRepository catRepository = new CategoryRepository(_context);
            CategoryModel categorymodel = catRepository.Find(id);
            ////repository.Remove(categorymodel);
            //categorymodel.Actived = false;
            //catRepository.SaveChanges();
            CategoryRepository catBLL = new CategoryRepository(_context);
            catBLL.Delete(id);
            return RedirectToAction("Index", new { id = RootId });
        }

        private void CreateRootMenu(int RootId, int selected = 0)
        {
            ViewBag.Title = Title;
            ViewBag.RootId = RootId;

            List<CategoryViewModel> catrootlst = new List<CategoryViewModel>();

            CategoryRepository repository = new CategoryRepository(_context);
            // nếu sử dụng multiParent => load tất cả các danh mục
            // không sử dụng => chỉ load những danh mục root
            if (isUseMultiParent)
            {
                catrootlst = repository.GetCategoryByParentWithFormat(RootId);
            }
            else
            {
                catrootlst = repository.GetCategoryByParent(RootId);
                catrootlst.Insert(0, new CategoryViewModel() { CategoryId = RootId, CategoryName = "Danh mục chính", OrderBy = 0 });
            }

            //if (selected != 0)
            //{
            //    ViewBag.Root = new SelectList(catrootlst, "CategoryId", "CategoryName", selected);
            //}
            //else
            //{
            //    ViewBag.Root = new SelectList(catrootlst, "CategoryId", "CategoryName");
            //}
            ViewBag.Root = new SelectList(catrootlst, "CategoryId", "CategoryName", selected);
            //Ẩn hiện các thuộc tính
            ViewBag.isUseIsHasChildren = isUseIsHasChildren;
            ViewBag.isUseDescription = isUseDescription;
            ViewBag.isUseKeywords = isUseKeywords;
            ViewBag.isUseImageUrl = isUseImageUrl;
            ViewBag.isUseOrderBy = isUseOrderBy;
            ViewBag.isUseisDisplayOnHomePage = isUseisDisplayOnHomePage;

            //Xết ẩn hiện dropdownlist
            //if (RootId == 212)
            //{
            ViewBag.IsVisibleDropdownlist = isUseMultiParent;
            //}
            //else
            //{
            //   ViewBag.IsVisibleDropdownlist = true;
            //}
        }

    }
}

using Constant;
using EntityModels;
using System.Collections.Generic;
using System.Linq;
using Tkwz.DAL;
using ViewModels;

namespace Repository
{
    public class CategoryRepository
    {
        public EntityDataContext Context;

        public CategoryRepository(EntityDataContext db)
        {
            Context = db;
        }
        public string GetRootCategoryId()
        {
            string ret = "";
            return ret;
        }
        public CategoryModel Find(int? id)
        {
            return Context.CategoryModel.Find(id);
        }
        public CategoryModel Find(int? id, string language)
        {
            return Context.CategoryModel
                    .Where(p => p.Actived == true && p.CategoryId == id)
                    .Select(p => new CategoryModel()
                    {
                        CategoryId = p.CategoryId,
                        CategoryName = language == ConstantLanguage.VietNamese ? p.CategoryName : p.CategoryNameEn,
                        SEOCategoryName = p.SEOCategoryName,
                        Description = language == ConstantLanguage.VietNamese ? p.Description : p.DescriptionEn,
                        Keywords = language == ConstantLanguage.VietNamese ? p.Keywords : p.KeywordsEn,
                        Parent = p.Parent
                    })
                    .FirstOrDefault();
        }
        public List<CategoryViewModel> GetByParent(int Parent, string LanguageID)
        {
            return Context.CategoryModel
                    .Where(p => p.Actived == true && p.Parent == Parent)
                    .Select(p => new CategoryViewModel()
                    {
                        CategoryId = p.CategoryId,
                        CategoryName = LanguageID == "vi-vn" ? p.CategoryName : p.CategoryNameEn,
                        SEOCategoryName = p.SEOCategoryName,
                        Parent = p.Parent
                    })
                    .ToList();
        }
        public bool InsertCategory(CategoryModel cat)
        {
            if (string.IsNullOrEmpty(cat.CategoryName))
            {
                cat.CategoryName = " ";
            }
            if (string.IsNullOrEmpty(cat.CategoryNameEn))
            {
                cat.CategoryNameEn = " ";
            }
            cat.SEOCategoryName = Library.ConvertToNoMarkString(cat.CategoryName);
            if (string.IsNullOrEmpty(cat.SEOCategoryName))
            {
                cat.SEOCategoryName = "-";
            }
            return 0 != this.Context.InsertCategory(cat.CategoryName, cat.CategoryNameEn, cat.OrderBy, cat.Parent, cat.Description, cat.DescriptionEn, cat.ImageUrl, cat.SEOCategoryName, cat.isHasChildren);
        }

        public bool UpdateCategory(CategoryModel cat)
        {
            if (string.IsNullOrEmpty(cat.CategoryName))
            {
                cat.CategoryName = " ";
            }
            if (string.IsNullOrEmpty(cat.CategoryNameEn))
            {
                cat.CategoryNameEn = " ";
            }
            cat.SEOCategoryName = Library.ConvertToNoMarkString(cat.CategoryName);
            if (string.IsNullOrEmpty(cat.SEOCategoryName))
            {
                cat.SEOCategoryName = "-";
            }

            return 0 != this.Context.UpdateCategory(cat.CategoryName, cat.CategoryNameEn, cat.OrderBy, cat.Parent, cat.Description, cat.DescriptionEn, cat.ImageUrl, cat.SEOCategoryName, cat.isHasChildren, cat.CategoryId, cat.isDisplayOnHomePage, cat.Keywords, cat.KeywordsEn);
        }



        CategoryDAL dal = new CategoryDAL();
        

        public List<CategoryViewModel> GetAll()
        {
            return dal.GetAll();
        }
        public List<CategoryViewModel> GetCategoryType()
        {
            return dal.GetCategoryType();
        }
        public List<CategoryViewModel> SearchCategoryName(string name)
        {
            return dal.SearchCategoryName(name);
        }
        public CategoryViewModel GetCategoryByCategoryID(int id, string Lang = "vi-vn")
        {
            return dal.GetCategoryByCategoryID(id, Lang);
        }
        public List<CategoryViewModel> GetCategoryByParent(int Parent, string Lang = "vi-vn")
        {
            return dal.GetCategoryByParent(Parent,Lang);
        }
        public List<CategoryViewModel> GetCategoryByParentWithFormat(int Parent, int CategoryId = 0)
        {
            return dal.GetCategoryByParentWithFormat(Parent,CategoryId);
        }
        //public bool Insert(CategoryViewModel Category)
        //{
        //    return dal.Insert(Category);
        //}
        //public bool Update(CategoryViewModel Category)
        //{
        //    return dal.Update(Category);
        //}

        public bool Delete(int ID)
        {
            return dal.Delete(ID);
        }

        public List<CategoryViewModel> CategoryByParent(int p)
        {
            throw new System.NotImplementedException();
        }
    }
}

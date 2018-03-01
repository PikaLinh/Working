using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ViewModels;

namespace Tkwz.DAL
{
    public class CategoryDAL : BaseClass
    {
        Connect Connect = new Connect();
        public List<CategoryViewModel> GetAll()
        {
            DataTable tb = Connect.Get_Data_DataTable(ref error,"h_Category_SelectAllCategory");
            return Fill(tb);
        }

        public List<CategoryViewModel> GetCategoryType()
        {
            DataTable tb = Connect.Get_Data_DataTable(ref error, "h_category_GetCategoryType");
            return Fill(tb);
        }

        public CategoryViewModel GetCategoryByCategoryID(int CategoryID, string Lang = "vi-vn")
        {
            SqlParameter param1 = new SqlParameter("@CategoryID", CategoryID);
            SqlParameter param2 = new SqlParameter("@Lang", Lang);
            DataTable tb = Connect.Get_Data_DataTable(ref error, "h_category_GetCatagoryBy", param1, param2);
            return Fill(tb).FirstOrDefault();
        }

        public List<CategoryViewModel> GetCategoryByParent(int Parent, string Lang = "vi-vn")
        {
            DataTable tb = new DataTable();
            try
            {
                SqlParameter param1 = new SqlParameter("@Parent", Parent);
                SqlParameter param2 = new SqlParameter("@Lang", Lang);
                tb = Connect.Get_Data_DataTable(ref error, "h_category_GetCatagoryByParent", param1, param2);
            }
            catch { }            
            return Fill(tb);
        }

        // Categoryid => category isn't display
        public List<CategoryViewModel> GetCategoryByParentWithFormat(int Parent,int CategoryId = 0)
        {
            SqlParameter parent = new SqlParameter("@Parent", Parent);
            SqlParameter categoryid = new SqlParameter("@CategoryId", CategoryId);
            DataTable tb = Connect.Get_Data_DataTable(ref error, "n_Category_GetWithFormat", parent, categoryid);
            return FillDropBox(tb);
        }

        public List<CategoryViewModel> SearchCategoryName(string Name)
        {
            SqlParameter param1 = new SqlParameter("@parameter", Name);
            DataTable tb = Connect.Get_Data_DataTable(ref error, "h_category_FindCategory", param1);
            return Fill(tb);
        }

        public bool Insert(CategoryViewModel Category)
        {
            SqlParameter param1 = new SqlParameter("@CategoryName", Category.CategoryName);
            SqlParameter param2 = new SqlParameter("@CategoryNameEn", Category.CategoryNameEn);
            SqlParameter param3 = new SqlParameter("@OrderBy", Category.OrderBy);
            SqlParameter param4 = new SqlParameter("@Description", Category.Description);
            SqlParameter param5 = new SqlParameter("@DescriptionEn", Category.DescriptionEn);
            SqlParameter param6 = new SqlParameter("@ImageUrl", Category.ImageUrl);
            SqlParameter param7 = new SqlParameter("@Parent", Category.Parent);
            SqlParameter param8 = new SqlParameter("@SEOCategoryName", Category.SEOCategoryName);
            SqlParameter param9 = new SqlParameter("@isHasChildren", Category.isHasChildren);

            int id = Connect.ExecuteSQl_int(ref error, "h_Category_InsertCategory", param1, param2, param3, param4, param5, param6, param7, param8, param9);
            if (id == 0)
            {
                return false;
            }
            else
            {
                Category.CategoryId = id;
                return true;
            }
        }

        public bool Update(CategoryViewModel Category)
        {
            SqlParameter param1 = new SqlParameter("@CategoryName", Category.CategoryName);
            SqlParameter param2 = new SqlParameter("@CategoryNameEn", Category.CategoryNameEn);

            SqlParameter param3 = new SqlParameter("@OrderBy", Category.OrderBy);
            SqlParameter param4 = new SqlParameter("@Description", Category.Description);
            SqlParameter param5 = new SqlParameter("@DescriptionEn", Category.DescriptionEn);
            SqlParameter param6 = new SqlParameter("@ImageUrl", Category.ImageUrl);
            SqlParameter param7 = new SqlParameter("@Parent", Category.Parent);
            SqlParameter param8 = new SqlParameter("@SEOCategoryName", Category.SEOCategoryName);
            SqlParameter param9 = new SqlParameter("@isHasChildren", Category.isHasChildren);            
            SqlParameter param10 = new SqlParameter("@CategoryID", Category.CategoryId);
            return 0 != Connect.ExecuteSQl_int(ref error, "h_Category_UpdateCategory", param1, param2, param3, param4, param5, param6, param7, param8, param9,param10);
        }

        public bool Delete(int ID)
        {
            SqlParameter param1 = new SqlParameter("@CategoryID", ID);
            return 0 != Connect.ExecuteSQl_int(ref error, "h_Category_DeleteCategory", param1);
        }
        List<CategoryViewModel> FillDropBox(DataTable tb)
        {
            List<CategoryViewModel> ret = new List<CategoryViewModel>();

            for (int i = 0; i < tb.Rows.Count; i++)
            {
                CategoryViewModel r = new CategoryViewModel();
                r.CategoryId = Convert.ToInt32(tb.Rows[i]["CategoryID"]);
                r.CategoryName = HttpUtility.HtmlDecode(tb.Rows[i]["CategoryName"].ToString());
                ret.Add(r);
            }
            return ret; 
        }

        List<CategoryViewModel> Fill(DataTable tb)
        {
            List<CategoryViewModel> ret = new List<CategoryViewModel>();

            for (int i = 0; i < tb.Rows.Count; i++)
            {
                CategoryViewModel r = new CategoryViewModel();
                try
                {
                    r.CategoryId = Convert.ToInt32(tb.Rows[i]["CategoryID"]);
                }
                catch{ }
                try
                {
                    r.ADNCode = tb.Rows[i]["ADNCode"].ToString();
                }
                catch{ }
                
                try
                {
                    r.CategoryName = tb.Rows[i]["CategoryName"].ToString();
                 
                }
                catch {}
                try
                {
                    r.CategoryNameEn = tb.Rows[i]["CategoryNameEn"].ToString();
                }
                catch { }
                try
                {
                    r.OrderBy = Convert.ToInt32(tb.Rows[i]["OrderBy"]);
                }
                catch { }
                try
                {
                    r.Description = tb.Rows[i]["Description"].ToString();
                }
                catch { }
                try
                {
                    r.DescriptionEn = tb.Rows[i]["DescriptionEn"].ToString();
                }
                catch { }
                try
                {
                    r.ImageUrl = tb.Rows[i]["ImageUrl"].ToString();
                }
                catch { }
                try
                {
                    r.Parent = Convert.ToInt32(tb.Rows[i]["Parent"]);
                }
                catch { }
                try
                {
                    r.isHasChildren = Convert.ToBoolean(tb.Rows[i]["isHasChildren"]);
                }
                catch { }
                try
                {
                    r.SEOCategoryName = tb.Rows[i]["SEOCategoryName"].ToString();
                }
                catch { }
                try
                {
                    r.isDisplayOnHomePage = Convert.ToBoolean(tb.Rows[i]["isDisplayOnHomePage"]);
                }
                catch { }

                ret.Add(r);
            }

            return ret;
        }
    }
}

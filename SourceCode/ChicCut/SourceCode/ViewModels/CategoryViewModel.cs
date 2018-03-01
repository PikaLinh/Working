
using EntityModels;
using System.ComponentModel.DataAnnotations;
namespace ViewModels
{

    public class CategoryViewModel : CategoryModel
    {
        //public int CategoryId { get; set; }
        //[Display(Name="Tên danh mục")]
        //[Required(ErrorMessage="Vui lòng nhập {0}")]
        //public string CategoryName { get; set; }
        //[Display(Name = "Tên danh mục")]
        //public string CategoryNameEn { get; set; }
        //[Display(Name="Thứ tự")]
        //public int OrderBy { get; set; }
        //[Display(Name = "Danh mục cha")]
        //public int Parent { get; set; }
        //[Display(Name = "Keywords")]
        //public string Keywords { get; set; }
        //[Display(Name = "Keywords")]
        //public string KeywordsEn { get; set; }
        //[Display(Name="Mô tả")]
        //public string Description { get; set; }
        //public string DescriptionEn { get; set; }
        //[Display(Name = "Ảnh đại diện")]
        //public string ImageUrl { get; set; }
        //public string SEOCategoryName { get; set; }
        //[Display(Name = "Lấy bài viết làm danh mục con")]
        //public bool isHasChildren { get; set; }
        //public bool Actived { get; set; }
        //public string ADNCode { get; set; }
        //[Display(Name = "Hiển thị sản phẩm trên trang chủ")]
        //public bool isDisplayOnHomePage { get; set; }
        public CategoryViewModel()
        {
            ImageUrl = "noimage.jpg";
            isHasChildren = false;
            Actived = true;
        }
    }
}

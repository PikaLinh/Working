using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EntityModels
{
    [MetadataTypeAttribute(typeof(AM_AccountModel.MetaData))]
    public partial class AM_AccountModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AMAccountTypeCode")]
            public string AMAccountTypeCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CodeAccount")]
            [Remote("ValidationCodeAccount", "Validation", AdditionalFields = "InitialCodeAccountCode,StoreId")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string Code { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NameAccount")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string Name { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            [Remote("ValidationCodeAccount", "Validation", AdditionalFields = "InitialCodeAccountCode,Code")]
            public string StoreId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public string Actived { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(AM_TransactionModel.MetaData))]
    public partial class AM_TransactionModel
    {
        internal sealed class MetaData
        {
            public int TransactionId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public Nullable<int> StoreId { get; set; }


            public Nullable<int> AMAccountId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TransactionTypeName")]
            public string TransactionTypeCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ContactItemTypeName")]
            public string ContactItemTypeCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer")]
            public Nullable<int> CustomerId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Supplier")]
            public Nullable<int> SupplierId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeName")]
            public Nullable<int> EmployeeId { get; set; }

            public Nullable<int> OtherId { get; set; }

            [Display(Name = "Số tiền")]
            [DisplayFormat(DataFormatString = "{0:n0}đ")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<decimal> Amount { get; set; }

            public Nullable<int> OrderId { get; set; }
            public Nullable<int> ImportMasterId { get; set; }
            public Nullable<int> IEOtherMasterId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> CreateDate { get; set; }

            public Nullable<int> CreateEmpId { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(CategoryModel.MetaData))]

    public partial class CategoryModel
    {
        // class không cho kế thừa
        internal sealed class MetaData
        {
            [Key]
            [Display(Name = "Mã danh mục")]
            public int CategoryId { get; set; }
            [Display(Name = "Tên danh mục")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string CategoryName { get; set; }
            [Display(Name = "Mã danh mục")]
            public string CategoryNameEn { get; set; }
            [Display(Name = "Thứ tự")]
            public Nullable<int> OrderBy { get; set; }
            [Display(Name = "Mã danh mục cha")]
            public Nullable<int> Parent { get; set; }
            [Display(Name = "Mô tả")]
            public string Description { get; set; }
            [Display(Name = "Mô tả")]
            public string DescriptionEn { get; set; }
            [Display(Name = "Hình ảnh")]
            public string ImageUrl { get; set; }
            public string SEOCategoryName { get; set; }
            [Display(Name = "Danh mục nước uống")]
            public bool isHasChildren { get; set; }
            [Display(Name = "Sử dụng")]
            public bool Actived { get; set; }
            public string ADNCode { get; set; }
            [Display(Name = "Hiển thị trên trang chủ")]
            public bool isDisplayOnHomePage { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(DiscountModel.MetaData))]
    public partial class DiscountModel
    {
        internal sealed class MetaData
        {
            public int DiscountId { get; set; }
            public int CalendarId { get; set; }

            [Display(Name = "Đăng ký trước (ngày)")]
            public Nullable<int> Days { get; set; }

            [Display(Name = "Số lượng")]
            public Nullable<int> Qty { get; set; }

            [Display(Name = "Mức giảm giá")]
            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> Discount { get; set; }

            [Display(Name = "SL Hiện tại")]
            public Nullable<int> Curent { get; set; }

        }
    }
    [MetadataTypeAttribute(typeof(EmployeeModel.MetaData))]

    public partial class EmployeeModel
    {
        internal sealed class MetaData
        {
            public int EmployeeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
            public string FullName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IdentityCard")]
            public string IdentityCard { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Gender")]
            public Nullable<bool> Gender { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BirthDay")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> BirthDay { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
            public string Phone { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Email")]
            public string Email { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public Nullable<int> StoreId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ParentId")]
            public Nullable<int> ParentId { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(CalendarModel.MetaData))]

    public partial class CalendarModel
    {

        internal sealed class MetaData
        {
            public int CalendarId { get; set; }

            [Display(Name = "Tên khóa học")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public int CourseId { get; set; }

            [Display(Name = "Khóa")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string Name { get; set; }

            [Display(Name = "Địa điểm")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public int LocationId { get; set; }

            [Display(Name = "Ngày khai giảng")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public System.DateTime StartDate { get; set; }

            [Display(Name = "Thời gian học")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string Time { get; set; }

            [Display(Name = "Học phí")]
            [DisplayFormat(DataFormatString = "{0:#,#}đ")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<decimal> Price { get; set; }

            [Display(Name = "Số lượng học viên tối đa")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public int NumberOfTrainees { get; set; }
            [Display(Name = "Số lượng học viên đã đăng ký")]
            public Nullable<int> TotalOfReg { get; set; }

            [Display(Name = "Nổi bật?")]
            public bool isHot { get; set; }

            [Display(Name = "Vị trí khóa học nổi bật")]
            public Nullable<int> HotIndex { get; set; }

            [Display(Name = "Sử dụng")]
            public bool Actived { get; set; }

            public CourseModel CourseModel { get; set; }
            public LocationModel LocationModel { get; set; }
            public ICollection<TrainerModel> TrainerModel { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(CalendarOfEventModel.MetaData))]
    public partial class CalendarOfEventModel
    {
        internal sealed class MetaData
        {
            public int EventId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Course")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<int> CourseId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EventCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string EventCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StartDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> StartDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StartTime")]
            //[DisplayFormat(DataFormatString = "{0:c}")]
            [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
            //.ToString(@"dd\.hh\:mm\:ss")
            public Nullable<System.TimeSpan> StartTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EndTime")]
            //[DisplayFormat(DataFormatString = "{0:c}")]
            [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
            public Nullable<System.TimeSpan> EndTime { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LocationId")]
            public Nullable<int> LocationId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateDate")]
            public Nullable<System.DateTime> CreateDate { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UserCreate")]
            public string UserCreate { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }

        }
    }
    [MetadataTypeAttribute(typeof(CourseModel.MetaData))]
    public partial class CourseModel
    {
        internal sealed class MetaData
        {
            public int CourseId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public int CategoryId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CourseName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string CourseName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }
            public string Url { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category")]
            public CategoryModel CategoryModel { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Details")]
            public string Details { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(CustomerModel.MetaData))]
    public partial class CustomerModel
    {
        internal sealed class MetaData
        {
            public int CustomerId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerLevel")]
            public Nullable<int> CustomerLevelId { get; set; }
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
            public string FullName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShortName")]
            public string ShortName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IdentityCard")]
            public string IdentityCard { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Gender")]
            public Nullable<bool> Gender { get; set; }
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BirthDay")]
            public Nullable<System.DateTime> BirthDay { get; set; }
            [Display(Name = "Công ty")]
            public string EnterpriseName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaxCode")]
            public string TaxCode { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Remote("ValidationPhone", "Validation", AdditionalFields = "InitialPhone")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
            public string Phone { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Fax")]
            public string Fax { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Email")]
            public string Email { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ImageUrl")]
            public string ImageUrl { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Notes")]
            public string Note { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProvinceModel_ProvinceId")]
            public Nullable<int> ProvinceId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_DistrictId")]
            public Nullable<int> DistrictId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Address")]
            public string Address { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AdditionalPurchase")]
            public Nullable<decimal> AdditionalPurchase { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Status")]
            public Nullable<bool> Actived { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeId")]
            public Nullable<int> EmployeeId { get; set; }
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RegDate")]
            public Nullable<System.DateTime> RegDate { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(Daily_ChicCut_OrderModel.MetaData))]
    public partial class Daily_ChicCut_OrderModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderId")]
            public int OrderId { get; set; }


            [Display(Name = "Giảm giá")]
            public Nullable<int> BillDiscountTypeId { get; set; }


            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> BillDiscount { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StaffId")]
            public Nullable<int> StaffId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Customer")]
            public int CustomerId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
            public string FullName { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IdentityCard")]
            public string IdentityCard { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
            public string Phone { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Gender")]
            public Nullable<bool> Gender { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderStatusId")]
            public Nullable<int> OrderStatusId { get; set; }



            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:HH:mm n\\g\\à\\y dd/MM/yyyy}")]
            public Nullable<System.DateTime> CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(Name = "Phương thức t.toán")]
            public Nullable<int> PaymentMethodId { get; set; }
            [Display(Name = "Loại ngày")]
            public Nullable<int> isHoliday { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AdditionalPrice")]
            public Nullable<decimal> AdditionalPrice { get; set; }
             [Display(Name = "Hoa hồng ngày lễ")]
            public Nullable<decimal> HolidayCommission { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(Daily_ChicCut_Pre_OrderModel.MetaData))]
    public partial class Daily_ChicCut_Pre_OrderModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderId")]
            public int OrderId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderId")]
            public int PreOrderId { get; set; }

            [Display(Name = "Giảm giá")]
            public Nullable<int> BillDiscountTypeId { get; set; }


            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> BillDiscount { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Customer")]
            public int CustomerId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
            public string FullName { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IdentityCard")]
            public string IdentityCard { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
            public string Phone { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Gender")]
            public Nullable<bool> Gender { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderStatusId")]
            public Nullable<int> OrderStatusId { get; set; }



            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:HH:mm n\\g\\à\\y dd/MM/yyyy}")]
            public Nullable<System.DateTime> CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ServicesNote")]
            public string ServicesNote { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AppointmentTime")]
            [DisplayFormat(DataFormatString = "{0:HH:mm n\\g\\à\\y dd/MM/yyyy}")]
            public Nullable<System.DateTime> AppointmentTime { get; set; }
        }
    }


    [MetadataTypeAttribute(typeof(CustomerLevelModel.MetaData))]
    public partial class CustomerLevelModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerLevelName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string CustomerLevelName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MinimumPurchase")]
            [DisplayFormat(DataFormatString = "{0:n0}đ")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<decimal> MinimumPurchase { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }


        }
    }

    [MetadataTypeAttribute(typeof(CRM_EmailParameterModel.MetaData))]
    public partial class CRM_EmailParameterModel
    {
        internal sealed class MetaData
        {
            [Display(Name = "Mã Tham số")]
            public int EmailParameterId { get; set; }

            public Nullable<int> EmailTemplateId { get; set; }


            [Display(Name = "Tên tham số")]
            public string Name { get; set; }


            [Display(Name = "Miêu tả")]
            public string Description { get; set; }


        }
    }

    [MetadataTypeAttribute(typeof(CRM_EmailTemplateModel.MetaData))]
    public partial class CRM_EmailTemplateModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmailTemplateId")]
            public int EmailTemplateId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmailTitle")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string EmailTitle { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmailTemplateName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string EmailTemplateName { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmailContent")]
            public string EmailContent { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }


        }
    }

    [MetadataTypeAttribute(typeof(CRM_RemiderModel.MetaData))]
    public partial class CRM_RemiderModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RemiderId")]
            public int RemiderId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RemiderName")]
            public string RemiderName { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ObjectId")]
            public Nullable<int> ObjectId { get; set; }


            [Display(Name = "Khách hàng")]
            public Nullable<int> CustomerId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Supplier")]
            public Nullable<int> SupplierId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PeriodType")]
            public Nullable<int> PeriodType { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExpiryDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyy}")]
            public Nullable<System.DateTime> ExpiryDate { get; set; }


            [Display(Name = "Số ngày báo trước")]
            public Nullable<int> DaysPriorNotice { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PeriodCode")]
            public string PeriodCode { get; set; }

            [Display(Name = "Số N ngày")]
            public Nullable<int> NDays { get; set; }

            [Display(Name = "Ngày bắt đầu")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyy}")]
            public Nullable<System.DateTime> StartDate { get; set; }

            public Nullable<bool> isEmailNotified { get; set; }
            public Nullable<bool> isSMSNotifred { get; set; }


            [Display(Name = "Mẫu Email")]
            public Nullable<int> EmailTemplateId { get; set; }


            [Display(Name = "Mẫu SMS")]
            public Nullable<int> SMSTemplateId { get; set; }


            [Display(Name = "Nhân viên quản lý")]
            public Nullable<int> EmployeeId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isCCEmail")]
            public Nullable<bool> isCCEmail { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmailOfEmployee")]
            public string EmailOfEmployee { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isCCSMS")]
            public Nullable<bool> isCCSMS { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SMSOfEmployee")]
            public string SMSOfEmployee { get; set; }



            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastDateRemind")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> LastDateRemind { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NextDateRemind")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> NextDateRemind { get; set; }

            [Display(Name = "Số tiền dịch vụ")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> Price { get; set; }


            [Display(Name = "Nội dung dịch vụ")]
            public string ServiceContent { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(CRM_SMSTemplateModel.MetaData))]
    public partial class CRM_SMSTemplateModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SMSTemplateId")]
            public int SMSTemplateId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SMSTemplateName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string SMSTemplateName { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SMSContent")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string SMSContent { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(CurrencyModel.MetaData))]
    public partial class CurrencyModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CurrencyName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string CurrencyName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }


        }
    }

    [MetadataTypeAttribute(typeof(ExchangeRateModel.MetaData))]
    public partial class ExchangeRateModel
    {
        internal sealed class MetaData
        {
            [Key]
            public int ExchangeRateId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Currency")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<int> CurrencyId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExchangeRate")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> ExchangeRate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExchangeDate")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> ExchangeDate { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(IEOtherDetailModel.MetaData))]
    public partial class IEOtherDetailModel
    {
        internal sealed class MetaData
        {
            public int IEOtherDetailId { get; set; }
            public Nullable<int> IEOtherMasterId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductId")]
            public Nullable<int> ProductId { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> ImportQty { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> ExportQty { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> Price { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> UnitShippingWeight { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> UnitPrice { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(IEOtherMasterModel.MetaData))]
    public partial class IEOtherMasterModel
    {
        internal sealed class MetaData
        {

            public int IEOtherMasterId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public int WarehouseId { get; set; }

            [Display(Name = "Loại")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public int InventoryTypeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IEOtherMasterCode")]
            public string IEOtherMasterCode { get; set; }

            [Display(Name = "Khách hàng")]
            public string CustomerName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
            public string Phone { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SenderName")]
            public string SenderName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReceiverName")]
            public string ReceiverName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Notes")]
            public string Note { get; set; }

            [Display(Name = "Tổng tiền")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> TotalPrice { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Money")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> Money { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
            public Nullable<System.DateTime> CreatedDate { get; set; }

            public string CreatedAccount { get; set; }

            public Nullable<int> CreatedEmployeeId { get; set; }

            public Nullable<System.DateTime> DeletedDate { get; set; }

            public string DeletedAccount { get; set; }

            public Nullable<int> DeletedEmployeeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }

        }
    }
    [MetadataTypeAttribute(typeof(ImportDetailModel.MetaData))]
    public partial class ImportDetailModel
    {
        internal sealed class MetaData
        {
            public int ImportDetailId { get; set; }
            public Nullable<int> ImportMasterId { get; set; }
            public Nullable<int> ProductId { get; set; }
            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> Qty { get; set; }

            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> Price { get; set; }

            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> UnitShippingWeight { get; set; }

            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> UnitPrice { get; set; }

            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> ShippingFee { get; set; }

            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> UnitCOGS { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(ImportMasterModel.MetaData))]
    public partial class ImportMasterModel
    {
        internal sealed class MetaData
        {
            public int ImportMasterId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public Nullable<int> StoreId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
            public Nullable<int> WarehouseId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Supplier")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<int> SupplierId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SalemanName")]
            public string SalemanName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SenderName")]
            public string SenderName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReceiverName")]
            public string ReceiverName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Notes")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "VATType")]
            public string VATType { get; set; }

            [Display(Name = "VATValue")]
            public Nullable<decimal> VATValue { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TAXBillCode")]
            public string TAXBillCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TAXBillDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> TAXBillDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ManualDiscountType")]
            public string ManualDiscountType { get; set; }

            [Display(Name = "ManualDiscount")]
            public Nullable<decimal> ManualDiscount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Cash")]
            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> Paid { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MoneyTransfer")]
            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> MoneyTransfer { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RemainingAmount")]
            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> RemainingAmount { get; set; }
            public Nullable<decimal> RemainingAmountAccrued { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DebtDueDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]

            public Nullable<System.DateTime> DebtDueDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Currency")]
            public Nullable<int> CurrencyId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExchangeRate")]
            [DisplayFormat(DataFormatString = "{0:n2}")]
            public Nullable<decimal> ExchangeRate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalPrice")]
            public Nullable<decimal> TotalPrice { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductName")]
            public bool Actived { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(InventoryDetailModel.MetaData))]
    public partial class InventoryDetailModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InventoryDetailId")]
            public int InventoryDetailId { get; set; }
            public Nullable<int> InventoryMasterId { get; set; }
            public Nullable<int> ProductId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BeginInventoryQty")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> BeginInventoryQty { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ImportPrice")]
            public Nullable<decimal> COGS { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExportPrice")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> Price { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ImportQty")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> ImportQty { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExportQty")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> ExportQty { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UnitCOGS")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> UnitCOGS { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UnitPrice")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> UnitPrice { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EndInventoryQty")]
            [DisplayFormat(DataFormatString = "{0:n2}")]
            public Nullable<decimal> EndInventoryQty { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(InventoryMasterModel.MetaData))]
    public partial class InventoryMasterModel
    {
        internal sealed class MetaData
        {
            public int InventoryMasterId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InventoryCode")]
            public string InventoryCode { get; set; }
            public Nullable<int> WarehouseModelId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InventoryTypeId")]
            public int InventoryTypeId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss tt}")]
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public string CreatedAccount { get; set; }
            public Nullable<int> CreatedEmployeeId { get; set; }
            public Nullable<System.DateTime> LastModifiedDate { get; set; }
            public string LastModifiedAccount { get; set; }
            public Nullable<int> LastModifiedEmployeeId { get; set; }
            public Nullable<System.DateTime> DeletedDate { get; set; }
            public string DeletedAccount { get; set; }
            public Nullable<int> DeletedEmployeeId { get; set; }
            public bool Actived { get; set; }
            public Nullable<int> BusinessId { get; set; }
            public string BusinessName { get; set; }
            public string ActionUrl { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(Master_ChicCut_HairTypeModel.MetaData))]
    public partial class Master_ChicCut_HairTypeModel
    {
        internal sealed class MetaData
        {
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "HairTypeName")]
            public string HairTypeName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public Nullable<int> OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ActivedHairType")]
            public bool Actived { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(Master_ChicCut_QuantificationMasterModel.MetaData))]
    public partial class Master_ChicCut_QuantificationMasterModel
    {
        internal sealed class MetaData
        {
            public int QuantificationMasterId { get; set; }
            public int ServiceId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuantificationName")]
            public string QuantificationName { get; set; }
            public string Detail { get; set; }
            public bool Actived { get; set; }

        }
    }


    [MetadataTypeAttribute(typeof(Master_ChicCut_ServiceModel.MetaData))]
    public partial class Master_ChicCut_ServiceModel
    {
        internal sealed class MetaData
        {
            public int ServiceId { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ServiceName")]
            public Nullable<int> ServiceCategoryId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ServiceName")]
            //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string ServiceName { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_Gender")]
            public Nullable<bool> Gender { get; set; }



            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "HairTypeName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<int> HairTypeId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price")]
            [DisplayFormat(DataFormatString = "{0:n2}")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<decimal> Price { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ActivedHairType")]
            public bool Actived { get; set; }

        }
    }


    [MetadataTypeAttribute(typeof(SupplierModel.MetaData))]
    public partial class SupplierModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SupplierCode")]
            public string SupplierCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SupplierName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string SupplierName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
            public string Phone { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Email")]
            public string Email { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaxCode")]
            public string TaxCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "hasInvoice")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public bool hasInvoice { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isPersonal")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public bool isPersonal { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IdentityCard")]
            public string IdentityCard { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProvinceId")]
            public Nullable<int> ProvinceId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DistrictId")]
            public Nullable<int> DistrictId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Address")]
            public string Address { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BankName")]
            public string BankName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BankBranch")]
            public string BankBranch { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BankAccountNumber")]
            public string BankAccountNumber { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BankOwner")]
            public string BankOwner { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(OrderMasterModel.MetaData))]
    public partial class OrderMasterModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderId")]
            public int OrderId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
            public Nullable<int> WarehouseId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public Nullable<int> StoreId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerLevel")]
            public Nullable<int> CustomerLevelId { get; set; }

            [Display(Name = "Giảm giá")]
            public Nullable<int> BillDiscountTypeId { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> BillDiscount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BillVAT")]
            public Nullable<int> BillVAT { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleName")]
            public string SaleName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DebtDueDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> DebtDueDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PaymentMethod")]
            public Nullable<int> PaymentMethodId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Paid")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> Paid { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MoneyTransfer")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> MoneyTransfer { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CompanyName")]
            public string CompanyName { get; set; }

            [Display(Name = "Mã số thuế")]
            public string TaxBillCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ContractNumber")]
            public string ContractNumber { get; set; }

            [Display(Name = "Ngày kí hợp đồng")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> TaxBillDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Customer")]
            public int CustomerId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
            public string FullName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IdentityCard")]
            public string IdentityCard { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
            public string Phone { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Gender")]
            public Nullable<bool> Gender { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Province")]
            public Nullable<int> ProvinceId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_DistrictId")]
            public Nullable<int> DistrictId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Address")]
            public string Address { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Email")]
            public string Email { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Address")]
            public Nullable<int> OrderStatusId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalPrice")]
            [DisplayFormat(DataFormatString = "{0:n0}đ")]
            public Nullable<decimal> TotalPrice { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:HH:mm n\\g\\à\\y dd/MM/yyyy}")]
            public Nullable<System.DateTime> CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccount")]
            public string CreatedAccount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedDate")]
            public Nullable<System.DateTime> LastModifiedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccount")]
            public string LastModifiedAccount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public bool Actived { get; set; }
        }
    }




    [MetadataTypeAttribute(typeof(OrderReturnModel.MetaData))]
    public partial class OrderReturnModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderReturnMasterId")]
            public int OrderReturnMasterId { get; set; }

            public string OrderReturnMasterCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderId")]
            public Nullable<int> OrderId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public Nullable<int> StoreId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
            public Nullable<int> WarehouseId { get; set; }

            [Display(Name = "Giảm giá")]
            public Nullable<int> BillDiscountTypeId { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> BillDiscount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BillVAT")]
            public Nullable<int> BillVAT { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Paid")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> Paid { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PaymentMethod")]
            public Nullable<int> PaymentMethodId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalPrice")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> TotalPrice { get; set; }

            public Nullable<decimal> TotalQty { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> RemainingAmount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
            public Nullable<System.DateTime> CreatedDate { get; set; }

            public string CreatedAccount { get; set; }
            public Nullable<int> CreatedEmployeeId { get; set; }
            public Nullable<System.DateTime> DeletedDate { get; set; }
            public string DeletedAccount { get; set; }
            public Nullable<int> DeletedEmployeeId { get; set; }
            public bool Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(OriginOfProductModel.MetaData))]
    public partial class OriginOfProductModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OriginOfProductName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string OriginOfProductName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }

        }
    }
    [MetadataTypeAttribute(typeof(LocationOfProductModel.MetaData))]
    public partial class LocationOfProductModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LocationOfProductName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string LocationOfProductName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Detail")]
            public string Detail { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(OrderModel.MetaData))]
    public partial class OrderModel
    {
        internal sealed class MetaData
        {
            //[Display(Name = "Mã đơn hàng")]
            //public int OrderId { get; set; }
            //[Display(Name = "Mã khách hàng")]
            //public Nullable<int> CustomerNumber { get; set; }
            //[Display(Name = "Người đặt hàng")]
            //public string CustomerId { get; set; }
            //[Display(Name = "Đơn vị")]
            //public string EnterpriseName { get; set; }
            //[Display(Name = "Điện thoại")]
            //public string Phone { get; set; }
            //[Display(Name = "Fax")]
            //public string Fax { get; set; }
            //[Display(Name = "Người nhận hàng")]
            //public string ReceivedPerson { get; set; }
            //[Display(Name = "Số xe")]
            //public string NumberOfTrucks { get; set; }
            //[Display(Name = "Địa chỉ nhận hàng")]
            //public string ReceivedAddress { get; set; }
            //[Display(Name = "Ngày đặt hàng")]
            //[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}")]
            //public Nullable<System.DateTime> OrderDate { get; set; }
            //[Display(Name = "Phương thức thanh toán")]
            //public Nullable<int> PaymentMethodId { get; set; }
            //[Display(Name = "Trạng thái")]
            //public Nullable<int> StatusId { get; set; }
            //[Display(Name = "Lý do từ chối")]
            //public string RejectReason { get; set; }
            //[DisplayFormat(DataFormatString = "{0:###.###}")]
            //[Display(Name = "Khối lượng đặt hàng (tấn)")]
            //public Nullable<double> TONTotal { get; set; }
            //[Display(Name = "SO Number")]
            //public string SalesDocumentNumber { get; set; }

            //[DisplayFormat(DataFormatString = "{0:###.###}")]
            //[Display(Name = "Khối lượng xác nhận (tấn)")]
            //public Nullable<double> ConfirmTONTotal { get; set; }
            //[Display(Name = "Ngày xác nhận")]
            //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            //public Nullable<System.DateTime> ConfirmDate { get; set; }
            //[Display(Name = "Ghi chú")]
            //public string Note { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(OrderDetailModel.MetaData))]
    public partial class OrderDetailModel
    {
        internal sealed class MetaData
        {
            public partial class OrderDetailModel
            {
                public int OrderDetailId { get; set; }
                public Nullable<int> OrderId { get; set; }
                [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductId")]
                public Nullable<int> ProductId { get; set; }

                [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Quantity")]
                [DisplayFormat(DataFormatString = "{0:n0}")]
                public Nullable<decimal> Quantity { get; set; }

                [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price")]
                [DisplayFormat(DataFormatString = "{0:n0}")]
                public Nullable<decimal> Price { get; set; }

                [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DiscountType")]
                public Nullable<int> DiscountTypeId { get; set; }

                [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DiscountType")]
                public Nullable<decimal> Discount { get; set; }

                public Nullable<decimal> UnitDiscount { get; set; }

                [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalAmount")]
                [DisplayFormat(DataFormatString = "{0:n0}")]
                public Nullable<decimal> UnitPrice { get; set; }
            }
        }
    }

    [MetadataTypeAttribute(typeof(UnitModel.MetaData))]
    public partial class UnitModel
    {
        internal sealed class MetaData
        {
            public int UnitId { get; set; }
            [Display(Name = "Đơn vị tính")]
            public string UnitName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            //[Display(Name = "Thông số 1/x")]
            public Nullable<bool> Actived { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(SteelMarkModel.MetaData))]
    public partial class SteelMarkModel
    {
        internal sealed class MetaData
        {
            public int SteelMarkId { get; set; }
            [Display(Name = "Mã mác")]
            public string Code { get; set; }

            [Display(Name = "Tên mác")]
            public string Name { get; set; }
            [Display(Name = "Sử dụng")]
            public bool Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(SteelFIModel.MetaData))]
    public partial class SteelFIModel
    {
        internal sealed class MetaData
        {
            public int SteelFIId { get; set; }
            [Display(Name = "Mã Phi")]
            public string Code { get; set; }

            [Display(Name = "Tên Phi")]
            public string Name { get; set; }
            [Display(Name = "Sử dụng")]
            public bool Actived { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(PaymentMethodModel.MetaData))]
    public partial class PaymentMethodModel
    {
        internal sealed class MetaData
        {

            [Display(Name = "Mã phương thức thanh toán")]
            public int PaymentMethodId { get; set; }
            [Display(Name = "Phương thức thanh toán")]
            public string PaymentMethodName { get; set; }
            [Display(Name = "Sử dụng")]
            public bool Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(PolicyModel.MetaData))]
    public partial class PolicyModel
    {
        internal sealed class MetaData
        {
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PolicyName")]
            public string PolicyName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }

        }
    }


    [MetadataTypeAttribute(typeof(PreImportMasterModel.MetaData))]
    public partial class PreImportMasterModel
    {
        internal sealed class MetaData
        {
            public int PreImportMasterId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PreImportMasterCode")]
            public string PreImportMasterCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public Nullable<int> StoreId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseId")]
            public Nullable<int> WarehouseId { get; set; }

            public Nullable<int> InventoryTypeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Supplier")]
            public Nullable<int> SupplierId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SalemanName")]
            public string SalemanName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SenderName")]
            public string SenderName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReceiverName")]
            public string ReceiverName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "VATType")]
            public string VATType { get; set; }

            public Nullable<decimal> VATValue { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TAXBillCode")]
            public string TAXBillCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TAXBillDate")]
            public Nullable<System.DateTime> TAXBillDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ManualDiscountType")]
            public string ManualDiscountType { get; set; }

            public Nullable<decimal> ManualDiscount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DebtDueDate")]
            public Nullable<System.DateTime> DebtDueDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Currency")]
            public Nullable<int> CurrencyId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExchangeRate")]
            public Nullable<decimal> ExchangeRate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalPrice")]
            public Nullable<decimal> TotalPrice { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalQty")]
            public Nullable<decimal> TotalQty { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalShippingWeight")]
            public Nullable<decimal> TotalShippingWeight { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Paid")]
            public Nullable<decimal> Paid { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MoneyTransfer")]
            public Nullable<decimal> MoneyTransfer { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RemainingAmount")]
            public Nullable<decimal> RemainingAmount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RemainingAmountAccrued")]
            public Nullable<decimal> RemainingAmountAccrued { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            public Nullable<System.DateTime> CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccount")]
            public string CreatedAccount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedEmployeeId")]
            public Nullable<int> CreatedEmployeeId { get; set; }

            public Nullable<System.DateTime> LastModifiedDate { get; set; }

            public string LastModifiedAccount { get; set; }

            public Nullable<int> LastModifiedEmployeeId { get; set; }

            public Nullable<System.DateTime> DeletedDate { get; set; }

            public string DeletedAccount { get; set; }

            public Nullable<int> DeletedEmployeeId { get; set; }

            public bool Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StatusCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string StatusCode { get; set; }

            public Nullable<int> ImportMasterId { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(PreOrderMasterModel.MetaData))]
    public partial class PreOrderMasterModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StatusCode")]
            public string StatusCode { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderId")]
            public int PreOrderId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
            public Nullable<int> WarehouseId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public Nullable<int> StoreId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerLevel")]
            public Nullable<int> CustomerLevelId { get; set; }

            [Display(Name = "Giảm giá")]
            public Nullable<int> BillDiscountTypeId { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> BillDiscount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BillVAT")]
            public Nullable<int> BillVAT { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleName")]
            public string SaleName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DebtDueDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> DebtDueDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PaymentMethod")]
            public Nullable<int> PaymentMethodId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Paid")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> Paid { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MoneyTransfer")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> MoneyTransfer { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CompanyName")]
            public string CompanyName { get; set; }

            [Display(Name = "Mã số thuế")]
            public string TaxBillCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ContractNumber")]
            public string ContractNumber { get; set; }

            [Display(Name = "Ngày kí hợp đồng")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> TaxBillDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Customer")]
            public int CustomerId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
            public string FullName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IdentityCard")]
            public string IdentityCard { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
            public string Phone { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Gender")]
            public Nullable<bool> Gender { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Province")]
            public Nullable<int> ProvinceId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_DistrictId")]
            public Nullable<int> DistrictId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Address")]
            public string Address { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Email")]
            public string Email { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Address")]
            public Nullable<int> OrderStatusId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalPrice")]
            [DisplayFormat(DataFormatString = "{0:n0}đ")]
            public Nullable<decimal> TotalPrice { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
            public Nullable<System.DateTime> CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccount")]
            public string CreatedAccount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedDate")]
            public Nullable<System.DateTime> LastModifiedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedAccount")]
            public string LastModifiedAccount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public bool Actived { get; set; }
        }
    }


    [MetadataTypeAttribute(typeof(ProductModel.MetaData))]
    public partial class ProductModel
    {
        internal sealed class MetaData
        {
            public int ProductId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public Nullable<int> StoreId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductTypeId")]
            //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<int> ProductTypeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BeginWarehouseId")]
            public Nullable<int> BeginWarehouseId { get; set; }

            [Display(Name = "Tồn đầu kỳ")]
            //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<decimal> BeginInventoryQty { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductStoreCode")]
            public string ProductStoreCode { get; set; }

            [Remote("ValidationCodeProduct", "Validation", AdditionalFields = "InitialCodeProduct")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductCode")]
            public string ProductCode { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductName")]
            public string ProductName { get; set; }

            //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShortProductName")]
            public string ShortProductName { get; set; }

            public string SEOProductName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<int> CategoryId { get; set; }


            [Display(Name = "Tông màu")]
            public Nullable<int> OriginOfProductId { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PolicyInStockId")]
            public Nullable<int> PolicyInStockId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PolicyOutOfStockId")]
            public Nullable<int> PolicyOutOfStockId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LocationOfProductId")]
            public Nullable<int> LocationOfProductId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductStatusId")]
            public Nullable<int> ProductStatusId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Keywords")]
            public string Keywords { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Barcode")]
            [Remote("BarCodeExist", "DynamicProduct", AdditionalFields = "id", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Remote")]
            public string Barcode { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications")]
            public string Specifications { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShippingWeight")]
            public Nullable<double> ShippingWeight { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UnitId")]
            public Nullable<int> UnitId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isShowOnWebsite")]
            public Nullable<bool> isShowOnWebsite { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ImportPrice")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> ImportPrice { get; set; }

            [Display(Name = "Ngày nhập")]
            public Nullable<System.DateTime> ImportDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CurrencyName")]
            public Nullable<int> CurrencyId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExchangeRate")]
            [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> ExchangeRate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShippingFee")]
            //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> ShippingFee { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "COGS")]
            [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> COGS { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Details")]
            public string Detail { get; set; }

            [Display(Name = "Hình ảnh")]
            public string ImageUrl { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isHot")]
            public Nullable<bool> isHot { get; set; }

            [Display(Name = "Vị trí nổi bật")]
            public Nullable<int> OrderBy { get; set; }

            public Nullable<System.DateTime> CreatedDate { get; set; }
            public string CreatedAccount { get; set; }
            public Nullable<System.DateTime> LastModifiedDate { get; set; }
            public string LastModifiedAccount { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IsMaterial")]
            public bool IsMaterial { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IsProduct")]
            public bool IsProduct { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isParentProduct")]
            public Nullable<bool> isParentProduct { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ParentProductId")]
            public Nullable<int> ParentProductId { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(ProductUpdateDetailModel.MetaData))]
    public partial class ProductUpdateDetailModel
    {
        internal sealed class MetaData
        {
            public int ProductUpdateDetailId { get; set; }
            public int ProductUpdateMasterId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductId")]
            public int ProductId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductStoreCode")]
            public string ProductStoreCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductCode")]
            public string ProductCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductName")]
            public string ProductName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ImportPrice")]
            [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> ImportPrice { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExchangeRate")]
            [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> ExchangeRate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShippingFee")]
            [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> ShippingFee { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price1")]
            [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> Price1 { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price2")]
            [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> Price2 { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price3")]
            [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> Price3 { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price4")]
            [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> Price4 { get; set; }
            public bool Actived { get; set; }
        }
    }



    [MetadataTypeAttribute(typeof(ProductUpdateMasterModel.MetaData))]
    public partial class ProductUpdateMasterModel
    {
        internal sealed class MetaData
        {
            public int ProductUpdateMasterId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductUpdateMasterCode")]
            public string ProductUpdateMasterCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalQty")]
            [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
            public Nullable<decimal> TotalQty { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccount")]
            public string CreateAccount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
            public Nullable<System.DateTime> CreateDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedEmployeeId")]
            public Nullable<int> CreatedEmployeeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DeletedDate")]
            public Nullable<System.DateTime> DeletedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DeletedAccount")]
            public string DeletedAccount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DeletedEmployeeId")]
            public Nullable<int> DeletedEmployeeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedIEOther")]
            public bool CreatedIEOther { get; set; }
            public bool Actived { get; set; }
        }
    }


    [MetadataTypeAttribute(typeof(StoreModel.MetaData))]
    public partial class StoreModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public int StoreId { get; set; }
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreCode")]
            public string StoreCode { get; set; }
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreName")]
            public string StoreName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProvinceId")]
            public Nullable<int> ProvinceId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DistrictId")]
            public Nullable<int> DistrictId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Address")]
            public string Address { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }

        }
    }


    [MetadataTypeAttribute(typeof(AccountModel.MetaData))]

    public partial class AccountModel
    {
        internal sealed class MetaData
        {
            public int UserId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_UserName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string UserName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_Password")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string Password { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_RolesId")]
            public Nullable<int> RolesId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_Actived")]
            public Nullable<bool> Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(RolesModel.MetaData))]
    public partial class RolesModel
    {

        internal sealed class MetaData
        {
            [Key]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RolesModel_RolesId")]
            public int RolesId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Code")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string Code { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RolesModel_RolesName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string RolesName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderBy")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<int> OrderBy { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }
        }
    }


    [MetadataTypeAttribute(typeof(ExchangeModel.MetaData))]
    public partial class ExchangeModel
    {
        internal sealed class MetaData
        {
            [Display(Name = "Mác thép")]
            public int SteelMarkId { get; set; }
            [Display(Name = "Phi thép")]
            public int SteelFIId { get; set; }
            [Display(Name = "Đơn vị tính")]
            public int UnitId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Value")]
            public Nullable<double> Value { get; set; }


            [Display(Name = "Phi thép")]
            public SteelFIModel SteelFIModel { get; set; }
            [Display(Name = "Mác thép")]
            public SteelMarkModel SteelMarkModel { get; set; }
            [Display(Name = "Đơn vị tính")]
            public UnitModel UnitModel { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(TrainerModel.MetaData))]
    public partial class TrainerModel
    {
        internal sealed class MetaData
        {
            public int TrainerId { get; set; }
            [Display(Name = "Tên Huấn luyện viên")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string TrainerName { get; set; }
            [Display(Name = "Url giới thiệu")]
            public string Url { get; set; }
            [Display(Name = "Còn hoạt động")]
            public bool Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(LocationModel.MetaData))]
    public partial class LocationModel
    {
        internal sealed class MetaData
        {
            public int LocationId { get; set; }
            [Display(Name = "Tên địa điểm")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string LocationName { get; set; }
            [Display(Name = "Url bản đồ")]
            public string Url { get; set; }
            [Display(Name = "Còn hoạt động")]
            public bool Actived { get; set; }
        }
    }


    [MetadataTypeAttribute(typeof(RegistryModel.MetaData))]
    public partial class RegistryModel
    {
        internal sealed class MetaData
        {
            public int RegistryId { get; set; }
            public int CourseId { get; set; }
            [Display(Name = "Họ và tên")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string FullName { get; set; }
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string Email { get; set; }
            [Display(Name = "Giới tính")]
            public Nullable<bool> Gender { get; set; }
            [Display(Name = "Sinh nhật")]
            public Nullable<System.DateTime> BirthDay { get; set; }
            [Display(Name = "Số điện thoại")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string Phone { get; set; }
            [Display(Name = "Ghi chú")]
            public string Note { get; set; }
            public Nullable<int> CalendarId { get; set; }
            public Nullable<int> EventId { get; set; }
            [Display(Name = "Tình trạng")]
            public Nullable<bool> Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(ReturnDetailModel.MetaData))]
    public partial class ReturnDetailModel
    {
        internal sealed class MetaData
        {
            public int ReturnDetailId { get; set; }
            public Nullable<int> ReturnMasterId { get; set; }
            public Nullable<int> ProductId { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> ImportQty { get; set; }


            public Nullable<decimal> ReturnedQty { get; set; }
            public Nullable<decimal> InventoryQty { get; set; }
            public Nullable<decimal> ReturnQty { get; set; }
            public Nullable<decimal> Price { get; set; }

            //[DisplayFormat(DataFormatString = "{0:n3}")]
            public Nullable<decimal> UnitShippingWeight { get; set; }

            public Nullable<decimal> UnitPrice { get; set; }
            //public Nullable<decimal> Note { get; set; }
            public Nullable<decimal> ShippingFee { get; set; }
            public Nullable<decimal> UnitCOGS { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(ReturnMasterModel.MetaData))]
    public partial class ReturnMasterModel
    {
        internal sealed class MetaData
        {
            public int ReturnMasterId { get; set; }
            public string ReturnMasterCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ImportMasterId")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public int ImportMasterId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public Nullable<int> StoreId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
            public Nullable<int> WarehouseId { get; set; }


            public Nullable<int> InventoryTypeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Supplier")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<int> SupplierId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SalemanName")]
            public string SalemanName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SenderName")]
            public string SenderName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReceiverName")]
            public string ReceiverName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Notes")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "VATType")]
            public string VATType { get; set; }

            [Display(Name = "VATValue")]
            public Nullable<decimal> VATValue { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TAXBillCode")]
            public string TAXBillCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TAXBillDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> TAXBillDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ManualDiscountType")]
            public string ManualDiscountType { get; set; }

            [Display(Name = "ManualDiscount")]
            public Nullable<decimal> ManualDiscount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DebtDueDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> DebtDueDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Currency")]
            public Nullable<int> CurrencyId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ExchangeRate")]
            [DisplayFormat(DataFormatString = "{0:n2}")]
            public Nullable<decimal> ExchangeRate { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalPrice")]
            public Nullable<decimal> TotalPrice { get; set; }

            [DisplayFormat(DataFormatString = "{0:n0}")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalQty")]
            public Nullable<decimal> TotalQty { get; set; }


            public Nullable<decimal> TotalShippingWeight { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Cash")]
            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> Paid { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MoneyTransfer")]
            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> MoneyTransfer { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RemainingAmount")]
            [DisplayFormat(DataFormatString = "{0:#,#}")]
            public Nullable<decimal> RemainingAmount { get; set; }
            public Nullable<decimal> RemainingAmountAccrued { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> CreatedDate { get; set; }

            public string CreatedAccount { get; set; }
            public Nullable<int> CreatedEmployeeId { get; set; }
            public Nullable<System.DateTime> LastModifiedDate { get; set; }
            public string LastModifiedAccount { get; set; }
            public Nullable<int> LastModifiedEmployeeId { get; set; }
            public Nullable<System.DateTime> DeletedDate { get; set; }
            public string DeletedAccount { get; set; }
            public Nullable<int> DeletedEmployeeId { get; set; }
            public bool Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(WarehouseInventoryDetailModel.MetaData))]
    public partial class WarehouseInventoryDetailModel
    {
        internal sealed class MetaData
        {
            public int WarehouseInventoryDetailId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseInventoryMasterCode")]
            public Nullable<int> WarehouseInventoryMasterId { get; set; }

            public Nullable<int> ProductId { get; set; }

            [Display(Name = "Tồn")]
            [DisplayFormat(DataFormatString = "{0:n2}")]
            public Nullable<decimal> Inventory { get; set; }

            [Display(Name = "Tồn thực tế")]
            [DisplayFormat(DataFormatString = "{0:n2}")]
            public Nullable<decimal> ActualInventory { get; set; }

            [Display(Name = "Khoảng chênh lệch")]
            [DisplayFormat(DataFormatString = "{0:n2}")]
            public Nullable<decimal> AmountDifference { get; set; }
            public bool Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(WarehouseInventoryMasterModel.MetaData))]
    public partial class WarehouseInventoryMasterModel
    {
        internal sealed class MetaData
        {
            //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            public int WarehouseInventoryMasterId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseInventoryMasterCode")]
            public string WarehouseInventoryMasterCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalQty")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> TotalQty { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
            public Nullable<System.DateTime> CreatedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccount")]
            public string CreatedAccount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedEmployeeId")]
            public Nullable<int> CreatedEmployeeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DeletedDate")]
            public Nullable<System.DateTime> DeletedDate { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
            public string DeletedAccount { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DeletedEmployeeId")]
            public Nullable<int> DeletedEmployeeId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedIEOther")]
            public bool CreatedIEOther { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(WarehouseModel.MetaData))]
    public partial class WarehouseModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string StoreId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string WarehouseCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string WarehouseName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Address")]
            public string Address { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProvinceId")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public Nullable<int> ProvinceId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DistrictId")]
            public Nullable<int> DistrictId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public bool Actived { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(Website_NewsModel.MetaData))]
    public partial class Website_NewsModel
    {
        internal sealed class MetaData
        {
            public int NewsID { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CategoryName")]
            public Nullable<int> CategoryId { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Title")]
            public string Title { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
            public string Description { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Details")]
            public string Details { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isHot")]
            public Nullable<bool> isHot { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            public Nullable<int> OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ImageUrl")]
            public string ImageUrl { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Visible")]
            public Nullable<bool> Visible { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UserId")]
            public Nullable<int> UserId { get; set; }

            public string SEOTitle { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PostDate")]
            public Nullable<System.DateTime> PostDate { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Views")]
            public Nullable<int> Views { get; set; }

            public string TitleEn { get; set; }

            public string DescriptionEn { get; set; }

            public string DetailsEn { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(SalaryDetailModel.MetaData))]
    public partial class SalaryDetailModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Salary")]
            public Nullable<decimal> Salary { get; set; }
            //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            //public bool Actived { get; set; }
            [Display(Name = "Tiền boa")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> Tip { get; set; }
            [Display(Name = "Hoa hồng")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> Commission { get; set; }

        }
    }

    [MetadataTypeAttribute(typeof(SalaryMasterModel.MetaData))]
    public partial class SalaryMasterModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
            public string Note { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
            public Nullable<System.DateTime> FromDate { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
            public Nullable<System.DateTime> ToDate { get; set; }
            [Display(Name = "Ngày trả lương")]
            public Nullable<System.DateTime> PayDate { get; set; }

        }
    }

}
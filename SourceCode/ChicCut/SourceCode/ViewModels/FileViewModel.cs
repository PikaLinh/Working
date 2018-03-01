using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class FileViewModel
    {
        [Key]
        public int FileId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FileName")]
        public string FileName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FileTitle")]
        public string FileTitle { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FileContent")]
        public byte[] FileContent { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FileDescription")]
        public string FileDescription { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Extension")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed.")]
        public string Extension { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ContentType")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed.")]
        public string ContentType { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FolderId")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public int FolderId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FolderKey")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string FolderKey { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FolderPath")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed.")]
        public string FolderPath { get; set; }

        public string FileUrl { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Size")]
        public Nullable<int> Size { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Width")]
        public Nullable<int> Width { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Height")]
        public Nullable<int> Height { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedByUserId")]
        public Nullable<int> CreatedByUserId { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedOnDate")]
        public Nullable<DateTime> CreatedOnDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedByUserId")]
        public Nullable<int> LastModifiedByUserId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastModifiedOnDate")]
        public Nullable<DateTime> LastModifiedOnDate { get; set; }
        #region elFinder.Net.Web.ViewModels =====================================================
        public string Folder { get; set; }
        public string SubFolder { get; set; }
        #endregion ==============================================================================
    }
}

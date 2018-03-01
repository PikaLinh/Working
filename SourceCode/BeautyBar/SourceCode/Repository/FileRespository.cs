using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Repository
{
    public class FileRespository
    {

        public static FileViewModel GetMainDetails(int? FileId)
        {
            using (EntityDataContext context = new EntityDataContext())
            {
                FileViewModel entity = new FileViewModel();
                try
                {
                    entity = (from file in context.SYS_tblFile
                              join folder in context.SYS_tblFolder on file.FolderId equals folder.FolderId
                              where file.FileId == FileId
                              select new FileViewModel()
                              {
                                  FileId = file.FileId,
                                  FileTitle = file.FileTitle,
                                  FileName = file.FileName,
                                  FileDescription = file.FileDescription,
                                  FileContent = file.FileContent,
                                  Extension = file.Extension,
                                  ContentType = file.ContentType,
                                  FolderId = file.FolderId,
                                  FileUrl = folder.FolderPath + "/" + file.FileName,
                                  Size = file.Size,
                                  Width = file.Width,
                                  Height = file.Height,
                                  FolderKey = folder.FolderKey,
                                  FolderPath = folder.FolderPath
                              }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                return entity;
            }
        }
    }
}

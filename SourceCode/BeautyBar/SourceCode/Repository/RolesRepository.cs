using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using EntityModels;

namespace Repository
{
    public class RolesRepository
    {
        public EntityDataContext context { get; set; }
        public RolesRepository(EntityDataContext context)
        {
            this.context = context;
        }
        public RolesModel Find(int id)
        {
            return context.RolesModel.Find(id);
        }
        public bool Add(RolesModel model)
        {
            try
            {
                LanguageRepository _languageRepository = new LanguageRepository(context);
                List<LanguageModel> lst = _languageRepository.GetAllWithoutDefault();
                if (lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        RolesLanguageModel languagemodel = new RolesLanguageModel();
                        languagemodel.RolesId = model.RolesId;
                        languagemodel.LanguageId = item.LanguageId;
                        languagemodel.RolesName = model.RolesName;
                        context.Entry(languagemodel).State = System.Data.Entity.EntityState.Added;
                    }
                }

                context.Entry(model).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Update(RolesViewModel model, int LanguageId = (int)LanguageEnum.VietNamese)
        {
            try
            {
                if (LanguageId == (int)LanguageEnum.VietNamese)
                {
                    var UpdateModel = new RolesModel()
                    {
                        RolesId = model.RolesId,
                        RolesName = model.RolesName,
                        OrderBy = model.OrderBy,
                        Actived = model.Actived,
                        Code = model.Code
                    };
                    context.Entry(UpdateModel).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {

                    var UpdateModel = context.RolesModel.Find(model.RolesId);
                    UpdateModel.OrderBy = model.OrderBy;
                    UpdateModel.Actived = model.Actived;
                    context.Entry(UpdateModel).State = System.Data.Entity.EntityState.Modified;

                    RolesLanguageModel languagemodel = new RolesLanguageModel();
                    languagemodel.RolesId = model.RolesId;
                    languagemodel.LanguageId = LanguageId;
                    languagemodel.RolesName = model.RolesName;
                    context.Entry(languagemodel).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public IEnumerable<RolesViewModel> GetAll(AccountModel currentUser, int LanguageId = (int)LanguageEnum.VietNamese)
        {
            try
            {
                int? level = 0;
                #region get
                if (currentUser.RolesId != null)
                {
                    level = context.RolesModel.Find(currentUser.RolesId).OrderBy;
                }
                #endregion
                if (LanguageId == (int)LanguageEnum.VietNamese)
                {
                    return context.RolesModel.Where(p => p.OrderBy >= level).OrderBy(p => p.OrderBy).Select(p => new RolesViewModel() { RolesId = p.RolesId, Code = p.Code, RolesName = p.RolesName, OrderBy = p.OrderBy, Actived = p.Actived });                    
                }
                else
                {
                    return from roles in context.RolesModel
                           join roleslang in context.RolesLanguageModel on roles.RolesId equals roleslang.RolesId
                           where roleslang.LanguageId == LanguageId && roles.OrderBy >= level
                           orderby roles.OrderBy
                           select new RolesViewModel() { RolesId = roles.RolesId, Code = roles.Code, RolesName = roleslang.RolesName, OrderBy = roles.OrderBy };
                }
            }
            catch
            {
                return new List<RolesViewModel>();
            } 

        }
        public List<RolesModel> All()
        {
            return context.RolesModel.ToList();
        }

        public IEnumerable<RolesViewModel> Search(string strSearch,string Status, AccountModel currentUser, int LanguageId = (int)LanguageEnum.VietNamese)
        {
            int? level = 0;
            #region get
            if (currentUser.RolesId != null)
            {
                level = context.RolesModel.Find(currentUser.RolesId).OrderBy;
            }
            #endregion
            bool? srStatus = null;
            if ( Status == "1")
            {
                srStatus = true;
            }
            if (Status == "0")
            {
                srStatus = false;
            }
            if (srStatus != null)
            {
                try
                {
                    if (LanguageId == (int)LanguageEnum.VietNamese)
                    {
                        return context.RolesModel
                            .Where(p => p.RolesName.Contains(strSearch) && p.Actived == srStatus && p.OrderBy >= level)
                            .OrderBy(p => p.OrderBy)
                            .Select(p => new RolesViewModel() { RolesId = p.RolesId, Code = p.Code, RolesName = p.RolesName, OrderBy = p.OrderBy, Actived = p.Actived });
                    }
                    else
                    {
                        return from roles in context.RolesModel
                               join roleslang in context.RolesLanguageModel on roles.RolesId equals roleslang.RolesId 
                               where roleslang.LanguageId == LanguageId && roles.Actived == srStatus &&
                                     roleslang.RolesName.Contains(strSearch) && roles.OrderBy >= level
                               orderby roles.OrderBy
                               select new RolesViewModel() { RolesId = roles.RolesId, Code = roles.Code, RolesName = roleslang.RolesName, OrderBy = roles.OrderBy, Actived = roles.Actived };
                    }
                }
                catch
                {
                    return new List<RolesViewModel>();
                }
            }
            else
            {
                try
                {
                    if (LanguageId == (int)LanguageEnum.VietNamese)
                    {
                        return context.RolesModel
                            .Where(p => p.RolesName.Contains(strSearch) && p.OrderBy >= level)
                            .OrderBy(p => p.OrderBy)
                            .Select(p => new RolesViewModel() { RolesId = p.RolesId, Code = p.Code, RolesName = p.RolesName, OrderBy = p.OrderBy, Actived = p.Actived });
                    }
                    else
                    {
                        return from roles in context.RolesModel
                               join roleslang in context.RolesLanguageModel on roles.RolesId equals roleslang.RolesId
                               where roleslang.LanguageId == LanguageId &&
                                     roleslang.RolesName.Contains(strSearch) && 
                                     roles.OrderBy >= level
                               orderby roles.OrderBy
                               select new RolesViewModel() { RolesId = roles.RolesId, Code = roles.Code, RolesName = roleslang.RolesName, OrderBy = roles.OrderBy, Actived = roles.Actived };
                    }
                }
                catch
                {
                    return new List<RolesViewModel>();
                }
            }
        }
    }
}

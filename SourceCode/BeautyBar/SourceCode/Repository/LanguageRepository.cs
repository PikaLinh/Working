using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EntityModels;
using ViewModels;
using AutoMapper;
using System.Data;
using Repository;


namespace Repository
{
    public class LanguageRepository
    {

        public EntityDataContext context { get; set; }

        public LanguageRepository(EntityDataContext context)
        {
            this.context = context;
        }
        public List<LanguageModel> GetAllWithoutDefault()
        {
            return context.LanguageModel.Where(p => p.LanguageId != 10001).ToList();
        }

        public List<LanguageModel> GetAll()
        {
            return context.LanguageModel.ToList();
        }

        public LanguageModel GetDetails(int LanguageId)
        {
            return context.LanguageModel.Where(x => x.LanguageId == LanguageId).SingleOrDefault();
        }

        public LanguageModel GetDetailsByCode(string Code)
        {
            return context.LanguageModel.Where(x=>x.Code == Code).SingleOrDefault();
        }
    }
}

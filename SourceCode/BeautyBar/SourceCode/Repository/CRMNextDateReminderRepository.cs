using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModels;
using ViewModels;
using System.Transactions;
using Constant;

namespace Repository
{
   public class CRMNextDateReminderRepository
    {
       EntityDataContext _context ;
       public CRMNextDateReminderRepository(EntityDataContext ct)
       {
           _context = ct;
       }

       public DateTime GetNextDateRemind(CRM_RemiderModel model)
       {
           model.DaysPriorNotice = model.DaysPriorNotice ?? 0;
              //Xét Tần suất 
           if (model.PeriodType == EnumPeriodType.MOTLAN)
           {
               model.StartDate = null;
               model.NextDateRemind = model.ExpiryDate.Value.AddDays(model.DaysPriorNotice.Value * (-1));
           }
           else //if (model.PeriodType == EnumPeriodType.DINHKY)
           {
               model.ExpiryDate = null;
               //model.NextDateRemind =  model.StartDate.Value.AddDays(model.DaysPriorNotice.Value * (-1));

               DateTime? NextDateRemind = model.StartDate;
               DateTime? StartDate = model.StartDate;

               while (NextDateRemind == null || NextDateRemind.Value.Date.AddDays(model.DaysPriorNotice.Value * (-1)).CompareTo(DateTime.Now.Date) <= 0)
               {
                   switch (model.PeriodCode)
                   {
                       case ConstantPeriod.NGAY:
                           NextDateRemind = StartDate.Value.AddDays(1);
                           StartDate = NextDateRemind;
                           break;
                       case ConstantPeriod.TUAN:
                           NextDateRemind = StartDate.Value.AddDays(7);
                           StartDate = NextDateRemind;
                           break;
                       case ConstantPeriod.THANG:
                           NextDateRemind = StartDate.Value.AddMonths(1);
                           StartDate = NextDateRemind;
                           break;
                       case ConstantPeriod.QUY:
                           NextDateRemind = StartDate.Value.AddMonths(3);
                           StartDate = NextDateRemind;
                           break;
                       case ConstantPeriod.NAM:
                           NextDateRemind = StartDate.Value.AddYears(1);
                           StartDate = NextDateRemind;
                           break;
                       case ConstantPeriod.NNgay:
                           NextDateRemind = StartDate.Value.AddDays(model.NDays.Value);
                           StartDate = NextDateRemind;
                           break;
                       default:
                           NextDateRemind = DateTime.Now.AddDays(1);
                           break;
                   }
               }
               model.NextDateRemind = NextDateRemind.Value.Date.AddDays(model.DaysPriorNotice.Value * (-1));
           }
           return model.NextDateRemind.Value;
       }

    }
}

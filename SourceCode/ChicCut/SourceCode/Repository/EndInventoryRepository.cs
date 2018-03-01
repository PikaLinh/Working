using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModels;

namespace Repository
{
    public class EndInventoryRepository
    {
        EntityDataContext _context;
        public EndInventoryRepository(EntityDataContext ct)
        {
            _context = ct;
        }

        public decimal GetQty(int ProductId)
        {
           decimal? Qty =(from detal in _context.InventoryDetailModel
                        join master in _context.InventoryMasterModel on detal.InventoryMasterId equals master.InventoryMasterId
                        orderby detal.InventoryDetailId descending
                        where master.Actived == true && detal.ProductId == ProductId
                        select detal.EndInventoryQty
                       ).FirstOrDefault();
           return Qty ?? 0;
        }
    }
}

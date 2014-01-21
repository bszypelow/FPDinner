using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;

namespace FPDinner.Models
{
    public class DinnerIndex : AbstractIndexCreationTask<Order, DinnerSummary>
    {
        public DinnerIndex()
        {
            Map = orders => from o in orders
                            select new DinnerSummary
                            {
                                MenuId = o.MenuId,
								Dinner = LoadDocument<Dinner>("dinners/" + o.Dinner.DinnerId).Name,
								HasPotatoes = LoadDocument<Menu>(o.MenuId).Dinners.Where(d => d.Id == o.Dinner.DinnerId).FirstOrDefault().HasPotatoes,
                                Full = o.Dinner.Potatoes == Potatoes.Full ? 1 :0,
                                Half = o.Dinner.Potatoes == Potatoes.Half ? 1 : 0,
                                None = o.Dinner.Potatoes == Potatoes.None ? 1 : 0,
                                Total = 1
                            };

            Reduce = results => from r in results
                                group r by new { r.MenuId, r.Dinner, r.HasPotatoes } into g
                                select new DinnerSummary
                                {
                                    MenuId = g.Key.MenuId,
                                    Dinner = g.Key.Dinner,
                                    HasPotatoes = g.Key.HasPotatoes,
                                    Full = g.Sum(o => o.Full),
                                    Half = g.Sum(o => o.Half),
                                    None = g.Sum(o => o.None),
                                    Total = g.Sum(o => o.Total)
                                };
        }
    }
}
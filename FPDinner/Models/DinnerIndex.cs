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
                                Dinner = o.Dinner.Dinner,
                                Potatoes = o.Dinner.Potatoes,
                                Notes = o.Dinner.Notes,
                                Count = 1
                            };

            Reduce = results => from r in results
                                group r by new { r.MenuId, r.Dinner, r.Potatoes, r.Notes } into g
                                select new DinnerSummary
                                {
                                    MenuId = g.Key.MenuId,
                                    Dinner = g.Key.Dinner,
                                    Potatoes = g.Key.Potatoes,
                                    Notes = g.Key.Notes,
                                    Count = g.Sum(o => o.Count)
                                };
        }
    }
}
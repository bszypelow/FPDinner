using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;

namespace FPDinner.Models
{
    public class SaladIndex : AbstractIndexCreationTask<Order, SaladSummary>
    {

        public SaladIndex()
        {
            Map = orders => from o in orders
                            from s in o.SaladIds
                            where s != 0
                            select new SaladSummary
                            {
                                MenuId = o.MenuId,
                                Salad = LoadDocument<Salad>("salads/" + s).Name,
                                Count = 1
                            };

            Reduce = results => from r in results
                                group r by new { r.MenuId, r.Salad } into g
                                select new SaladSummary
                                {
                                    MenuId = g.Key.MenuId,
                                    Salad = g.Key.Salad,
                                    Count = g.Sum(o => o.Count)
                                };
        }
    }
}
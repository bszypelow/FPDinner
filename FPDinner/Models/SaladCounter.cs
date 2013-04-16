using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;

namespace FPDinner.Models
{
    public class SaladCounter : AbstractIndexCreationTask<Order, SaladCounter.Result>
    {
        public class Result
        {
            public string MenuId { get; set; }
            public int SaladCount { get; set; }
        }

        public SaladCounter()
        {
            Map = orders => from o in orders
                            from s in o.Salads
                            where !string.IsNullOrEmpty(s)
                            select new Result
                            {
                                MenuId = o.MenuId,
                                SaladCount = 1
                            };

            Reduce = results => from r in results
                                group r by r.MenuId into g
                                select new Result
                                {
                                    MenuId = g.Key,
                                    SaladCount = g.Sum(s => s.SaladCount)
                                };
        }
    }
}
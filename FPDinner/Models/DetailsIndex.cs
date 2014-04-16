using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace FPDinner.Models
{
    public class DetailsIndex : AbstractTransformerCreationTask<Order>
    {
        public DetailsIndex()
        {
            TransformResults = orders => from o in orders
                                         select new Details
                                         {
                                             MenuId = o.MenuId,
                                             Person = o.Person,
                                             Dinner = LoadDocument<Dinner>("dinners/" + o.Dinner.DinnerId).Name,
                                             Potatoes = o.Dinner.Potatoes,
                                             Salad1 = o.SaladIds[0] != 0 ? LoadDocument<Salad>("salads/" + o.SaladIds[0]).Name : string.Empty,
                                             Salad2 = o.SaladIds[1] != 0 ? LoadDocument<Salad>("salads/" + o.SaladIds[1]).Name : string.Empty,
                                             Notes = o.Dinner.Notes
                                         };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FPDinner.Models
{
    public class SummaryViewModel
    {
        public IEnumerable<DinnerSummary> Dinners { get; set; }
        public IEnumerable<SaladSummary> Salads { get; set; }
        public IEnumerable<Order> Details { get; set; }
    }

    public class SaladSummary
    {
        public string MenuId { get; set; }
        public string Salad { get; set; }
        public int Count { get; set; }
    }

    public class DinnerSummary
    {
        public string MenuId { get; set; }
        public string Dinner { get; set; }
        public bool HasPotatoes { get; set; }
        public int Full { get; set; }
        public int Half { get; set; }
        public int None { get; set; }
        public int Total { get; set; }
    }
}
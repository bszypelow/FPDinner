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
        public IEnumerable<Details> Details { get; set; }
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

    public class Details
    {
        public string MenuId { get; set; }
        public string Person { get; set; }
        public string Dinner { get; set; }
        public Potatoes Potatoes { get; set; }
        public string Salad1 { get; set; }
        public string Salad2 { get; set; }
        public string Notes { get; set; }
    }
}
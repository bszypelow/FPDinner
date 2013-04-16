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
        public Potatoes Potatoes { get; set; }
        public string Notes { get; set; }
        public int Count { get; set; }
    }
}
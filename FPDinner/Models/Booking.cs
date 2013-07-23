using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FPDinner.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<Dinner> Dinners { get; set; }
        public IEnumerable<Salad> Salads { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string MenuId { get; set; }
        public string Person { get; set; }
        public OrderedDinner Dinner { get; set; }
        [Required]
        public string[] Salads { get; set; }
    }

    public class OrderedDinner
    {
        [Required]
        public string Dinner { get; set; }
        [Required]
        public Potatoes Potatoes { get; set; }
        public string Notes { get; set; }
    }

    public enum Potatoes
    {
        None,
        Half,
        Full
    }

    public class OrderingViewModel
    {
        public Menu Menu { get; set; }
        public Order Order { get; set; }
        public DateTime TimeLimit { get; set; }
    }
}
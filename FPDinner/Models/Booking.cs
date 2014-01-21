using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
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
        [Required]
        public int Id { get; set; }
        [Required, RegularExpression(@"menus/[0-9]+")]
        public string MenuId { get; set; }
        public string Person { get; set; }
        public OrderedDinner Dinner { get; set; }
        public int[] SaladIds { get; set; }
    }

    public class OrderedDinner
    {
        [Required]
        public int DinnerId { get; set; }
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

    public class OrderingViewModel : IValidatableObject
    {
        public Menu Menu { get; set; }
        public Order Order { get; set; }
        public DateTime TimeLimit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Menu != null)
            {
                if (!Menu.Dinners.Any(d => d.Id == Order.Dinner.DinnerId))
                {
                    yield return new ValidationResult("Illegal dinner");
                }

                if (!Menu.Salads.Any(s => s.Id == Order.SaladIds[0]) && Order.SaladIds[0] != 0)
                {
                    yield return new ValidationResult("Illegal salad");
                }

                if (!Menu.Salads.Any(s => s.Id == Order.SaladIds[1]) && Order.SaladIds[1] != 0)
                {
                    yield return new ValidationResult("Illegal salad");
                }
            }

            if (TimeLimit < DateTime.UtcNow)
            {
                yield return new ValidationResult("Time elapsed");
            }
        }
    }
}
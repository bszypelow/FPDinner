using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FPDinner.Models;

namespace FPDinner.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var menu = new Menu
            {
                Id = 1,
                Date = DateTime.UtcNow,
                Dinners = new List<Dinner>
                {
                    new Dinner { Id = 1, Name = "Pierogi", HasPotatoes = false},
                    new Dinner { Id = 2, Name = "Kotlet", HasPotatoes = true},
                    new Dinner { Id = 3, Name = "Placki", HasPotatoes = false},
                    new Dinner { Id = 3, Name = "Ryba", HasPotatoes = true},
                },
                Salads = new List<Salad>
                {
                    new Salad { Id = 1, Name = "Biała"},
                    new Salad { Id = 1, Name = "Marchewka"},
                    new Salad { Id = 1, Name = "Czerwona"}
                }
            };

            var order = new Order
            {
                Date = DateTime.Now,
                Dinner = new OrderedDinner(),
                Salads = new string[2]
            };
            return View(new OrderingViewModel { Menu = menu, Order = order });
        }

        [HttpPost]
        public ActionResult Order(OrderingViewModel model)
        {
            //TODO: Add logic here

            return RedirectToAction("index");
        }
    }
}

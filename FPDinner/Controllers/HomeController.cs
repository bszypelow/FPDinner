using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FPDinner.Models;
using Raven.Client;
using Raven.Database.Linq;
using Raven.Database.Queries;
namespace FPDinner.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var session = MvcApplication.DB.OpenSession())
            {
                var menu = GetMenu(session);
                if (menu == null)
                {
                    return RedirectToAction("index", "admin");
                }

                string menuId = "menus/" + menu.Id.ToString();
                DateTime timeLimit = DateTime.UtcNow.AddMinutes(-1);

                var order = GetOrCreateOrder(session, menuId);

                var viewModel = new OrderingViewModel
                {
                    Menu = menu,
                    Order = order,
                    TimeLimit = menu.Date.AddMinutes(1)
                };

                if (menu.Date > timeLimit)
                {
                    return View("order1", viewModel);
                }

                if (CountMissingSalads(session, menuId) > 0)
                {
                    return View("order2", viewModel);
                }

                return View();
            }
        }

        [HttpPost]
        public ActionResult Order1(OrderingViewModel model)
        {
            using (var session = MvcApplication.DB.OpenSession())
            {
                var order = model.Order;
                order.Person = User.Identity.Name;

                if (TryValidateModel(order))
                {
                    session.Store(model.Order);
                    session.SaveChanges();
                }
            }

            Session["Message"] = " Order placed.";
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Order2(OrderingViewModel model)
        {
            using (var session = MvcApplication.DB.OpenSession())
            {
                var order = session.Load<Order>("orders/" + model.Order.Id);
                order.Salads[1] = model.Order.Salads[1];

                if (TryValidateModel(order))
                {
                    session.Store(order);
                    session.SaveChanges();
                }
            }

            return RedirectToAction("index");
        }

        public ActionResult Summary()
        {
            using (var session = MvcApplication.DB.OpenSession())
            {
                var menu = GetMenu(session);
                string id = "menus/" + menu.Id;

                var dinners = from d in session.Query<DinnerSummary, DinnerIndex>()
                              where d.MenuId == id
                              orderby d.Dinner
                              select d;
                var salads = from s in session.Query<SaladSummary, SaladIndex>()
                             where s.MenuId == id
                             orderby s.Salad
                             select s;
                var details = from o in session.Query<Order>()
                              where o.MenuId == id
                              orderby o.Person
                              select o;
                return View(new SummaryViewModel { Dinners = dinners, Salads = salads, Details = details });
            }
        }

        #region Data Access Helpers

        private static Menu GetMenu(IDocumentSession session)
        {
            var menu = session.Query<Menu>()
                .Customize(q => q.WaitForNonStaleResults(TimeSpan.FromSeconds(10)))
                .OrderByDescending(m => m.Date)
                .FirstOrDefault();
            return menu;
        }

        private Order GetOrCreateOrder(IDocumentSession session, string menuId)
        {
            string user = User.Identity.Name;

            var myOrders = from o in session.Query<Order>().Customize(q => q.WaitForNonStaleResults(TimeSpan.FromSeconds(10)))
                           where o.Person == user
                           where o.MenuId == menuId
                           select o;

            var order = myOrders.FirstOrDefault() ?? new Order
            {
                MenuId = menuId,
                Dinner = new OrderedDinner(),
                Salads = new string[] { string.Empty, string.Empty }
            };

            return order;
        }

        private static int CountMissingSalads(IDocumentSession session, string menuId)
        {
            var orders = from o in session.Query<Order>()
                         where o.MenuId == menuId
                         select o;
            var saladsCount = from c in session.Query<SaladCounter.Result, SaladCounter>()
                              where c.MenuId == menuId
                              select c.SaladCount;
            var missingSalads = orders.Count() - saladsCount.FirstOrDefault();

            return missingSalads;
        }
    }

    #endregion
}

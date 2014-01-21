using FPDinner.Helpers;
using FPDinner.Models;
using Raven.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace FPDinner.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly int TimeLimit = int.Parse(ConfigurationManager.AppSettings["TimeLimit"]);

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
                DateTime timeLimit = DateTime.UtcNow.AddMinutes(-TimeLimit);

                var order = GetOrCreateOrder(session, menuId);

                var viewModel = new OrderingViewModel
                {
                    Menu = menu,
                    Order = order,
                    TimeLimit = menu.Date.AddMinutes(TimeLimit)
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
                model.Menu = GetMenu(session);
                model.Order.Person = User.Identity.Name;
                model.TimeLimit = model.Menu.Date.AddMinutes(TimeLimit);

                ModelState.Clear();
                if (TryValidateModel(model))
                {
                    if (!model.Menu.Dinners.Single(d => d.Id == model.Order.Dinner.DinnerId).HasPotatoes)
                    {
                        model.Order.Dinner.Potatoes = Potatoes.None;
                    }

                    session.Store(model.Order);
                    session.SaveChanges();

                    Session["Message"] = " Order placed.";

                    return RedirectToAction("index");
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Order2(OrderingViewModel model)
        {
            using (var session = MvcApplication.DB.OpenSession())
            {
                model.Menu = GetMenu(session);
                var order = session.Load<Order>("orders/" + model.Order.Id);
                order.SaladIds[1] = model.Order.SaladIds[1];
                model.Order = order;

                if (CountMissingSalads(session, order.MenuId) == 0)
                {
                    Session["Message"] = "Someone was faster than you.";
                    return RedirectToAction("index");
                }

                ModelState.Clear();
                if (TryValidateModel(model))
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
                var details = from o in session.Query<Order>().TransformWith<DetailsIndex, Details>()
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
                SaladIds = new int[] { 0, 0 }
            };

            return order;
        }

        private static int CountMissingSalads(IDocumentSession session, string menuId)
        {
            var orders = from o in session.Query<Order>()
                         where o.MenuId == menuId
                         select o;
            var salads = from c in session.Query<SaladCounter.Result, SaladCounter>()
                              where c.MenuId == menuId
                              select c;
            var saladCount = salads.FirstOrEmpty().SaladCount;
            var missingSalads = orders.Count() - saladCount;

            return missingSalads;
        }
    }

    #endregion
}

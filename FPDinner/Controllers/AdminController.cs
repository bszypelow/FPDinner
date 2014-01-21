using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FPDinner.Models;
using Raven.Client;

namespace FPDinner.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            using (IDocumentSession session = MvcApplication.DB.OpenSession())
            {
                var dinners = session.Query<Dinner>()
                    .OrderBy(d => d.Name)
                    .ToList()
                    .Select(d => new DinnerAvailability { Dinner = d, IsAvailable = d.AvailableByDefault });
                var salads = session.Query<Salad>()
                    .OrderBy(s => s.Name)
                    .ToList()
                    .Select(s => new SaladAvailability { Salad = s, IsAvailable = false });

                var viewModel = new AdminViewModel
                {
                    Dinners = dinners,
                    Salads = salads,
                };

                return View(viewModel);
            }
        }

        //
        // POST: /Admin/AddDinner

        [HttpPost]
        public ActionResult AddDinner(Dinner model)
        {
            if(TryValidateModel(model))
            {
                using (IDocumentSession session = MvcApplication.DB.OpenSession())
                {
                    session.Store(model);
                    session.SaveChanges();
                }
            }

            return PartialView("_AddedDinner", new DinnerAvailability { Dinner = model, IsAvailable = model.AvailableByDefault });
        }

        //
        // GET: /Admin/DeleteDinner/5

        public ActionResult DeleteDinner(int id)
        {
            using (IDocumentSession session = MvcApplication.DB.OpenSession())
            {
                var dinner = session.Load<Dinner>("dinners/" + id.ToString());
                session.Delete(dinner);
                session.SaveChanges();
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult AddSalad(Salad model)
        {
            if (TryValidateModel(model))
            {
                using (IDocumentSession session = MvcApplication.DB.OpenSession())
                {
                    session.Store(model);
                    session.SaveChanges();
                }
            }

            return PartialView("_AddedSalad", new SaladAvailability { Salad = model, IsAvailable = false });
        }

        //
        // GET: /Admin/DeleteDinner/5

        public ActionResult DeleteSalad(int id)
        {
            using (IDocumentSession session = MvcApplication.DB.OpenSession())
            {
                var dinner = session.Load<Salad>("salads/" + id.ToString());
                session.Delete(dinner);
                session.SaveChanges();
            }
            return new EmptyResult();
        }

        //
        // GET: /Admin/StartBooking

        public ActionResult StartBooking(AdminViewModel model)
        {
            using (IDocumentSession session = MvcApplication.DB.OpenSession())
            {
                var dinners = from d in session.Query<Dinner>().ToList()
                              join dd in model.Dinners on d.Id equals dd.Dinner.Id
                              where dd.IsAvailable == true
                              orderby d.Name
                              select d;

                var salads = from s in session.Query<Salad>().ToList()
                              join ss in model.Salads on s.Id equals ss.Salad.Id
                              where ss.IsAvailable == true
                              orderby s.Name
                              select s;
                
                var menu = new Menu
                {
                    Date = DateTime.UtcNow,
                    Dinners = dinners,
                    Salads = salads
                };

                session.Store(menu);
                session.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

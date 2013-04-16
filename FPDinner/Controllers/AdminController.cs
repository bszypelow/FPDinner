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
                    .ToList()
                    .Select(d => new DinnerAvailability { Dinner = d, IsAvailable = false });
                var salads = session.Query<Salad>()
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
            try
            {
                if(TryValidateModel(model))
                {
                    using (IDocumentSession session = MvcApplication.DB.OpenSession())
                    {
                        session.Store(model);
                        session.SaveChanges();
                    }
                }

                return PartialView("_AddedDinner", new DinnerAvailability { Dinner = model, IsAvailable = false });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return PartialView("Error");
            }
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
            try
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return PartialView("Error");
            }
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
                              select d;

                var salads = from s in model.Salads
                             where s.IsAvailable
                             select s.Salad;
                
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

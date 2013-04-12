using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FPDinner.Models;

namespace FPDinner.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            var dinners = new List<Dinner>
            {
                new Dinner { Id = 1, Name = "Pierogi", HasPotatoes = false},
                new Dinner { Id = 2, Name = "Kotlet", HasPotatoes = true},
                new Dinner { Id = 3, Name = "Placki", HasPotatoes = false},
                new Dinner { Id = 4, Name = "Ryba", HasPotatoes = true},
            };

            var salads = new List<Salad>
            {
                new Salad { Id = 1, Name = "Biała"},
                new Salad { Id = 2, Name = "Marchewka"},
                new Salad { Id = 3, Name = "Czerwona"}
            };

            var viewModel = new AdminViewModel
            {
                Dinners = dinners,
                Salads = salads,
            };

            return View(viewModel);
        }

        //
        // POST: /Admin/AddDinner

        [HttpPost]
        public ActionResult AddDinner(Dinner model)
        {
            try
            {
                // TODO: Add insert logic here

                return PartialView("_AddedDinner", model);
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
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult AddSalad(Salad model)
        {
            try
            {
                // TODO: Add insert logic here

                return PartialView("_AddedSalad", model);
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
            return View();
        }

        //
        // GET: /Admin/StartBooking

        public ActionResult StartBooking()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}

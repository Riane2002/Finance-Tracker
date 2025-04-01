using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Group_work.Models;

namespace Group_work.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add registration logic
                return RedirectToAction("Index");
            }
            return View(model);
        }

    }
}
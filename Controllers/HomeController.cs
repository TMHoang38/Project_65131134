using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_65131134.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Redirect đến Shop area
            return RedirectToAction("Index", "Home_65131134", new { area = "Shop" });
        }

        public ActionResult About()
        {
            // Redirect đến Shop area
            return RedirectToAction("Index", "Home_65131134", new { area = "Shop" });
        }

        public ActionResult Contact()
        {
            // Redirect đến Shop area
            return RedirectToAction("Index", "Home_65131134", new { area = "Shop" });
        }
    }
}
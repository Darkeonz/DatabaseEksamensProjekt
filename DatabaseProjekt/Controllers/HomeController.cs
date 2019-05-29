using DatabaseProjekt.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatabaseProjekt.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            ReadAndInsert readfile = new ReadAndInsert();
            readfile.ReadAllBooks();
            readfile.GetTownList();
            return View();
        }

        public ActionResult About()
        {
            ReadAndInsert readfile = new ReadAndInsert();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ReadAndInsert readfile = new ReadAndInsert();
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
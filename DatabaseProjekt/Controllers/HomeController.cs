using DatabaseProjekt.Database;
using DatabaseProjekt.Entities;
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
            //   ReadAndInsert readfile = new ReadAndInsert();
            //   readfile.HarvestDataFromBooks();   
            //  DBHandlerMYSQL dBHandler = new DBHandlerMYSQL();
            //  dBHandler.GetBooksXMilesFromGeolocation(5150850, -12574, 50000);
            DBHandlerMongo DbHandler = new DBHandlerMongo("Gutenberg");
            DbHandler.InsertRecord("Cities", new City() { CityId = 1, Latitude = 25, Longitude = 35, Name = "London" });
            
            return View();
        }

        public ActionResult About()
        {
           // ReadAndInsert readfile = new ReadAndInsert();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
           // ReadAndInsert readfile = new ReadAndInsert();
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetData()
        {
            DBHandlerMYSQL dBHandler = new DBHandlerMYSQL();
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
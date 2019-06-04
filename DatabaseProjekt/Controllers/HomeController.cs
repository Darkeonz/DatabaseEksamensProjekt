using DatabaseProjekt.Database;
using DatabaseProjekt.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatabaseProjekt.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            //   The following method is used for harvesting all the books from a folder and collecting author, title, cities ect.
            //   ReadAndInsert readfile = new ReadAndInsert();
            //   readfile.HarvestDataFromBooks(); 

            //  The following are the 4 queries needed with 5 iterations of each
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DBHandlerMYSQL dBHandler = new DBHandlerMYSQL();

            var listOfBooksByCity = dBHandler.GetBooksByCity("London");
            var listOfBooksByCity1 = dBHandler.GetBooksByCity("London");
            var listOfBooksByCity2 = dBHandler.GetBooksByCity("London");
            var listOfBooksByCity3 = dBHandler.GetBooksByCity("London");
            var listOfBooksByCity4 = dBHandler.GetBooksByCity("London");

            var listOfBooksByAuthor = dBHandler.GetBooksByAuthor("Roger McGuinn");
            var listOfBooksByAuthor1 = dBHandler.GetBooksByAuthor("Annie Besant");
            var listOfBooksByAuthor2 = dBHandler.GetBooksByAuthor("Algernon Blackwood");
            var listOfBooksByAuthor3 = dBHandler.GetBooksByAuthor("David Garnett");
            var listOfBooksByAutho4 = dBHandler.GetBooksByAuthor("Charles Lamb");

            var listOfBooksByTitle = dBHandler.GetBooksByTitle("Alcatraz");
            var listOfBooksByTitle1 = dBHandler.GetBooksByTitle("The King James Bible");
            var listOfBooksByTitle2 = dBHandler.GetBooksByTitle("Discourses");
            var listOfBooksByTitle3 = dBHandler.GetBooksByTitle("The Rome Express");
            var listOfBooksByTitle4 = dBHandler.GetBooksByTitle("The Mirror of Literature, Amusement, and Instruction");

            var listOfBooksWithinRadius = dBHandler.GetBooksXMilesFromGeolocation(5150850,-12574, 1000);
            var listOfBooksWithinRadius1 = dBHandler.GetBooksXMilesFromGeolocation(5297630, -2664, 2000);
            var listOfBooksWithinRadius2 = dBHandler.GetBooksXMilesFromGeolocation(5395760, -108271, 3000);
            var listOfBooksWithinRadius3 = dBHandler.GetBooksXMilesFromGeolocation(5356540, -7553, 4000);
            var listOfBooksWithinRadius4 = dBHandler.GetBooksXMilesFromGeolocation(5112600, 131257, 5000);
            sw.Stop();
            TimeSpan elapsedTime = sw.Elapsed;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {      
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetData()
        {
         
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
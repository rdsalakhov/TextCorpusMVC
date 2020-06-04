using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TextCorpusMVC.Models;

namespace TextCorpusMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult CheckAdminStatus()
        {
            string login = User.Identity.Name;
            using (TextCorpusContext db = new TextCorpusContext())
            {
                if (!db.UserSet.First(x => x.Login == login).IsAdmin)
                {

                    return PartialView("NonAdminError");
                }
            }
            return RedirectToAction("Index", "User");          
        }
    }
}
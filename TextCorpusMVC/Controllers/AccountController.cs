using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using TextCorpusMVC.Models;

namespace TextCorpusMVC.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                User user = null;
                int userId = 0;
                try
                {
                    userId = UserAccountManager.SignInUser(model.Login, model.Password);
                    using (TextCorpusContext db = new TextCorpusContext())
                    {
                        user = db.UserSet.FirstOrDefault(u => u.Id == userId);                 
                    }
                    FormsAuthentication.SetAuthCookie(model.Login, true);
                    return RedirectToAction("Index", "Home");
                }
                catch (InvalidPasswordException)
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет"); 
                }
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (TextCorpusContext db = new TextCorpusContext())
                {
                    user = db.UserSet.FirstOrDefault(u => u.Login == model.Login);
                }
                if (user == null)
                {
                    // создаем нового пользователя
                    using (TextCorpusContext db = new TextCorpusContext())
                    {
                        UserAccountManager.SignUpUser(model.Login, model.Password);

                        user = db.UserSet.Where(u => u.Login == model.Login).FirstOrDefault();
                    }
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Login, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
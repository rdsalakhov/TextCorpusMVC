using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TextCorpusMVC.Models;

namespace TextCorpusMVC.Controllers
{
    public class UserController : Controller
    {
        private TextCorpusContext db = new TextCorpusContext();

        // GET: User
        public ActionResult Index()
        {
            string login = User.Identity.Name;            
            if (!db.UserSet.First(x => x.Login == login).IsAdmin)
            {
                return RedirectToAction("NonAdminError", "Error");
            }

            ViewBag.AccessSet = db.UserAccessSet.ToList();
            return View(db.UserSet.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.UserSet.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.Texts = db.UserAccessSet.Where(x => x.UserId == id).Select(x => x.Text.Name).ToList();
            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Login,Password,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                db.UserSet.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.UserSet.Find(id);
            
            if (user == null)
            {
                return HttpNotFound();
            }
            MultiSelectList texts = new MultiSelectList(db.UserAccessSet.Where(x => x.UserId == id).Select(x => x.Text), "Id", "Name");
            ViewBag.Texts = texts;
            var othTexts = db.TextSet.Where(x => !db.UserAccessSet.Where(ac => ac.UserId == id).Select(ac => ac.Text.Id).Contains(x.Id));
            var list = db.UserAccessSet.Where(ac => ac.UserId == id).Select(ac => ac.Text.Id).ToList();
            MultiSelectList otherTexts = new MultiSelectList(othTexts, "Id", "Name");
            ViewBag.OtherTexts = otherTexts;

            return View(user);
        }

        // POST: User/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, int[] TextsList, int[] RemoveTexts)
        {
            User user = db.UserSet.First(x => x.Id == id);
            if (ModelState.IsValid)
            {
                if (TryUpdateModel(user, "",
                new string[] { "IsAdmin" }))
                {
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateException)
                    { }
                    if (TextsList != null)
                    {
                        foreach (var textId in TextsList)
                        {
                            var access = new UserAccess();
                            access.User = user;
                            access.TextId = textId;
                            db.UserAccessSet.Add(access);
                            db.SaveChanges();
                        }
                    }
                    if (RemoveTexts != null)
                    {
                        foreach (var textId in RemoveTexts)
                        {
                            var access = db.UserAccessSet.Where(x => x.UserId == id && x.TextId == textId).First();
                            db.UserAccessSet.Remove(access);
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index");
                }
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.UserSet.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.UserSet.Find(id);
            db.UserSet.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using System.Windows.Documents;
using TextCorpusMVC.Models;

namespace TextCorpusMVC.Controllers
{
    [Authorize]
    public class TextController : Controller
    {
        

        private TextCorpusContext db = new TextCorpusContext();

        // GET: Text
        public ActionResult Index()
        {
            
            return View(db.TextSet.ToList());
        }

        // GET: Text/Details/5
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Text text = db.TextSet.First(x => x.Id == id);
            ViewBag.TagNames = new SelectList(
                db.TagSet.Include("Name").Where(x => x.TextId == id).Select(x => x.Name.Name).Distinct().ToList());
            ViewBag.TagsNum = text.TagSet.Count();
            var annotationStrings = text.Annotation.Split('\n');
            ViewBag.AnnotationStrings = annotationStrings;
            if (text == null)
            {
                return HttpNotFound();
            }
            return View(text);
        }

        [HttpPost, ActionName("HighlightText")]
        public ActionResult HighlightText(string tagToHighlight, int id)
        {
            var highlightedText = new HighlightedText();
            var text = db.TextSet.Include(x => x.TagSet.Select(tag => tag.Name)).Where(x => x.Id == id).First();
            highlightedText.Text = text;
            highlightedText.StartPositions = text.TagSet.Where(x => x.Name.Name == tagToHighlight).Select(x => x.StartPos).ToList();
            highlightedText.EndPositions = text.TagSet.Where(x => x.Name.Name == tagToHighlight).Select(x => x.EndPos).ToList();
            return PartialView(highlightedText);
        }

        //[HttpGet, ActionName("DownloadRtf")]
        public FileResult DownloadRtf(string tagToHighlight, int id)
        {
            int i =int.Parse(Request.Params["id"]);
            string t = Request.Params["tagToHighlight"];
            var text = db.TextSet.First(x => x.Id == id);
            var tags = db.TagSet.Include("Name").Where(x => x.TextId == id && x.Name.Name == tagToHighlight).ToList();
            var flowDoc = TextHighlighter.GetHighlightedText(text.Txt, tags);
            var range = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);

            Directory.CreateDirectory(Server.MapPath("~/export/"));
            string fileName = text.Name + " c подсвеченными " + tagToHighlight + ".rtf";
            //fileName = fileName.Replace(' ', '_');
            fileName = Server.MapPath("~/export/" + fileName);

            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            range.Save(fs, DataFormats.Rtf);
            fs.Close();
            FileStream newFs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            fileName = text.Name + " c подсвеченными " + tagToHighlight + ".rtf";
            return File(newFs, "text/rtf", fileName);
        }


        // GET: Text/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Text/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase textUpload, HttpPostedFileBase annotationUpload, 
            [Bind(Include = "Name,Txt,Annotation, TextFile, AnnotationFile")] TextToUpload text)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    if (text.TextFile == null)
                    {
                        if (text.Txt == null)
                            text.Txt = string.Empty;
                    }
                    else if (Path.GetExtension(text.TextFile.FileName) != ".txt")
                    {
                        ModelState.AddModelError("", "Некорректное расширение файла с текстом. Загрузите файл с расширением .txt");
                        return View(text);
                    }
                    else
                    {
                        var sr = new StreamReader(text.TextFile.InputStream);
                        text.Txt = sr.ReadToEnd();
                        sr.Close();
                    }

                    if (text.AnnotationFile == null)
                    {
                        if (text.Annotation == null)
                            text.Annotation = string.Empty;
                    }
                    else if (Path.GetExtension(text.AnnotationFile.FileName) != ".ann")
                    {
                        ModelState.AddModelError("", "Некорректное расширение файла с аннотацией. Загрузите файл с расширением .ann");
                        return View(text);
                    }
                    else
                    {
                        var sr = new StreamReader(text.AnnotationFile.InputStream);
                        text.Annotation = sr.ReadToEnd();
                        sr.Close();
                    }

                    TextManager.UploadText(text.Txt, text.Annotation, text.Name);
                    return RedirectToAction("Index");
                }
                
            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Невозможно добавить текст. Проверьте правильность введенных данных.");
            }

            return View(text);
        }

        // GET: Text/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Text text = db.TextSet.Find(id);
            if (text == null)
            {
                return HttpNotFound();
            }
            return View(text);
        }

        // POST: Text/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var textToUpdate = db.TextSet.Find(id);
            var user = db.UserSet.First(x => x.Login == User.Identity.Name);

            if (!user.IsAdmin && !db.UserAccessSet.Any(x => x.UserId == user.Id && x.TextId == id))
            {
                ModelState.AddModelError("", "У вас нет доступа на редактирование этого текста");
                return View(textToUpdate);
            }
            string anno = ValueProvider.GetValue("Annotation").ConvertTo(typeof(string)).ToString();
            if (!TextManager.IsValidAnnotation(new StringReader(anno)))
            {
                ModelState.AddModelError("", "Некорректный формат аннотации");
                return View(textToUpdate);
            }
            if (TryUpdateModel(textToUpdate, "", new string[] { "Name", "Txt" }))
            {
                try
                {
                    db.SaveChanges();
                    var annotation = ValueProvider.GetValue("Annotation").ConvertTo(typeof(string)).ToString();
                    TextManager.UpdateTextAnnotation(id.Value, annotation);

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Невозможно сохранить изменения. Проверьте правильность введенных данных.");
                }
            }
            return View(textToUpdate);
        }

        // GET: Text/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Text text = db.TextSet.Find(id);

            var user = db.UserSet.First(x => x.Login == User.Identity.Name);
            if (!user.IsAdmin && !db.UserAccessSet.Any(x => x.UserId == user.Id && x.TextId == id))
            {
                return Content("<p class=\"text-warning\">Ошибка!<br/>У вас нет доступа на удаление этого текста</p>");
            }
            if (text == null)
            {
                return HttpNotFound();
            }
            return View(text);
        }

        // POST: Text/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Text text = db.TextSet.Find(id);
            
            db.TextSet.Remove(text);
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

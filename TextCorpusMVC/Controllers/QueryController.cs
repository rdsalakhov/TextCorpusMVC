using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TextCorpusMVC.Models;
using System.Data.Entity;

namespace TextCorpusMVC.Controllers
{
    public class QueryController : Controller
    {
        private TextCorpusContext db = new TextCorpusContext();
        // GET: Query
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExactFormSearch()
        {
            return View(new List<QueryResult>());
        }

        [HttpPost, ActionName("ExactFormSearch")]
        public ActionResult ExactFormSearch(string exactForm)
        {
            var entityQuery = db.TagSet.Include("Text").Where(x => x.TaggedText == exactForm).ToList();
            var queryResults = entityQuery
                .Select(x => new QueryResult(x.Text.Name, x.Text.Txt, new List<int> { x.StartPos }, new List<int> { x.EndPos }))
                .Distinct(new QueryResultComparer());
            return PartialView("LemmaPairPartialResult", queryResults);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult LemmaPairSearch()
        {
            return View();
        }

        [HttpPost, ActionName("LemmaPairSearch")]
        public ActionResult LemmaPairSearch([Bind(Include = "First,Second,MinRange, MaxRange")] PairQuery query)
        {
            if (ModelState.IsValid)
            {
                var lemmas = db.AnnotatorNoteSet.Where(x => x.AnnotationText.StartsWith("lemma = "));

                var lemmaGroups = db.AnnotatorNoteSet.Include(an => an.Tag.Text)
                    .Where(x => x.AnnotationText.StartsWith("lemma = ")).ToList().GroupBy(x => x.Tag.TextId);
                List<IndexedLemmaTag> lemmaTags = new List<IndexedLemmaTag>();
                foreach (var group in lemmaGroups)
                {
                    var indexedGroup = group.Select(x => new IndexedLemmaTag(
                        x.Tag,
                        x.AnnotationText.Replace("lemma = ", "").Trim('\'').ToLowerInvariant(),
                        x.Tag.Id - group.Min(g => g.Tag.Id))).ToList();
                    lemmaTags.AddRange(indexedGroup);
                }

                var firstLemma = lemmaTags.Where(x => x.Lemma.Contains(query.First));
                var secondLemma = lemmaTags.Where(x => x.Lemma.Contains(query.Second));
                var joinedLemmaTags = firstLemma.Join(secondLemma,
                    f => f.Tag.TextId,
                    s => s.Tag.TextId,
                    (f, s) => new { FirstLemmaTag = f, SecondLemmaTag = s });
                var queryResults = joinedLemmaTags
                    .Where(x => Math.Abs(x.FirstLemmaTag.Index - x.SecondLemmaTag.Index) <= query.MaxRange
                    && Math.Abs(x.FirstLemmaTag.Index - x.SecondLemmaTag.Index) >= query.MinRange)
                    .Select(x => new QueryResult(
                        x.FirstLemmaTag.Tag.Text.Name,
                        x.FirstLemmaTag.Tag.Text.Txt,
                        new List<int> { x.FirstLemmaTag.Tag.StartPos, x.SecondLemmaTag.Tag.StartPos },
                        new List<int> { x.FirstLemmaTag.Tag.EndPos, x.SecondLemmaTag.Tag.EndPos })).ToList();
                return PartialView("LemmaPairPartialResult", queryResults);
            }
            else 
            {
                ModelState.AddModelError("", "Ошибка запроса. Проверьте правильность введенных данных.");
                return PartialView("LemmaPairPartialResult", new List<QueryResult>()); 
            }
        }

        public ActionResult TagPairSearch()
        {
            var tagNames = db.AnnotatorNoteSet.Where(x => x.AnnotationText.StartsWith("lemma = ")).Select(x => x.Tag.Name.Name).Distinct().ToList();
            SelectList tagNamesList = new SelectList(tagNames);
            ViewBag.Tagnames = tagNamesList;
            return View();
        }

        [HttpPost, ActionName("TagPairSearch")]
        public ActionResult TagPairSearch([Bind(Include = "First,Second,MinRange, MaxRange")] PairQuery query)
        {
            if (ModelState.IsValid)
            {
                var lemmas = db.AnnotatorNoteSet.Where(x => x.AnnotationText.StartsWith("lemma = "));

                var lemmaGroups = db.AnnotatorNoteSet.Include(an => an.Tag.Text).Include(an => an.Tag.Name)
                    .Where(x => x.AnnotationText.StartsWith("lemma = ")).ToList().GroupBy(x => x.Tag.TextId);
                List<IndexedLemmaTag> lemmaTags = new List<IndexedLemmaTag>();
                foreach (var group in lemmaGroups)
                {
                    var indexedGroup = group.Select(x => new IndexedLemmaTag(
                        x.Tag,
                        x.AnnotationText.Replace("lemma = ", "").Trim('\'').ToLowerInvariant(),
                        x.Tag.Id - group.Min(g => g.Tag.Id))).ToList();
                    lemmaTags.AddRange(indexedGroup);
                }

                var firstTag = lemmaTags.Where(x => x.Tag.Name.Name == query.First);
                var secondTag = lemmaTags.Where(x => x.Tag.Name.Name == query.First);
                var joinedTags = firstTag.Join(secondTag,
                    f => f.Tag.TextId,
                    s => s.Tag.TextId,
                    (f, s) => new { FirstLemmaTag = f, SecondLemmaTag = s });
                var queryResults = joinedTags
                    .Where(x => Math.Abs(x.FirstLemmaTag.Index - x.SecondLemmaTag.Index) <= query.MaxRange
                    && Math.Abs(x.FirstLemmaTag.Index - x.SecondLemmaTag.Index) >= query.MinRange)
                    .Select(x => new QueryResult(
                        x.FirstLemmaTag.Tag.Text.Name,
                        x.FirstLemmaTag.Tag.Text.Txt,
                        new List<int> { x.FirstLemmaTag.Tag.StartPos, x.SecondLemmaTag.Tag.StartPos },
                        new List<int> { x.FirstLemmaTag.Tag.EndPos, x.SecondLemmaTag.Tag.EndPos })).ToList();
                return PartialView("LemmaPairPartialResult", queryResults);
            }
            else
            {
                ModelState.AddModelError("", "Некорректные параметры поиска.");
                return PartialView("LemmaPairPartialResult", new List<QueryResult>());
            }
        }
    }

    class QueryResultComparer : IEqualityComparer<QueryResult>
    {
        public bool Equals(QueryResult x, QueryResult y)
        {
            return x.Result.Equals(y.Result) && x.TextName.Equals(y.TextName);
        }

        public int GetHashCode(QueryResult obj)
        {
            return obj.Result.GetHashCode() + obj.TextName.GetHashCode();
        }
    }
}


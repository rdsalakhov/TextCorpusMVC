using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextCorpusMVC.Models
{
    public class TextManager
    {
        static public bool IsValidAnnotation(StringReader annotation)
        {
            string textSpanTagPattern = @"(T\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s([0-9]+)\s([0-9]+)\s([^\n]+)";
            string aNotePattern = @"(A\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s(T\d+)";
            string sharpNotePattern = @"(#\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s(T\d+)\s([^\n]+)";
            string relationTagPattern = @"(R\d +)\s([a - zA - Zа - яА - Я0 - 9_] +)\sArg1:(T\d +)\sArg2:(T\d +)";

            for (string annotationLine = annotation.ReadLine(); annotationLine != null; annotationLine = annotation.ReadLine())
            {
                if (!(Regex.IsMatch(annotationLine, textSpanTagPattern) ||
                      Regex.IsMatch(annotationLine, aNotePattern) ||
                      Regex.IsMatch(annotationLine, sharpNotePattern) ||
                      Regex.IsMatch(annotationLine, relationTagPattern)))
                {
                    return false;
                }
            }
            return true;
        }
        
        //private static MatchCollection[] ResolveAnnotation(StreamReader annotationStream)
        private static MatchCollection[] ResolveAnnotation(string annotation)
        {
            string textSpanTagPattern = @"(T\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s([0-9]+)\s([0-9]+)\s([^\n]+)";
            string aNotePattern = @"(A\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s(T\d+)";
            string sharpNotePattern = @"(#\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s(T\d+)\s([^\n]+)";
            string relationTagPattern = @"(R\d +)\s([a - zA - Zа - яА - Я0 - 9_] +)\sArg1: (T\d +)\sArg2: (T\d +)";

            

            //annotationStream.DiscardBufferedData();
            //annotationStream.BaseStream.Seek(0, SeekOrigin.Begin);
            //string annotation = annotationStream.ReadToEnd();


            var textSpanTags = Regex.Matches(annotation, textSpanTagPattern);
            var aNotes = Regex.Matches(annotation, aNotePattern);
            var sharpNotes = Regex.Matches(annotation, sharpNotePattern);
            var relationTags = Regex.Matches(annotation, relationTagPattern);

            return new MatchCollection[] { textSpanTags, aNotes, sharpNotes, relationTags };
        }

        
        //static public void UploadText(StreamReader textStream, StreamReader annotationStream, string textName)
        static public void UploadText(string text, string annotation, string textName)
        {
            //string text = textStream.ReadToEnd();
            //string annotation = annotationStream.ReadToEnd();

            if (!IsValidAnnotation(new StringReader(annotation)))
                throw new InvalidAnnotationException();
            var tagMatches = ResolveAnnotation(annotation);

            using (TextCorpusContext db = new TextCorpusContext())
            {
                var textToUpload = new Text();
                textToUpload.Name = textName;
                textToUpload.Txt = text;
                textToUpload.Annotation = annotation;

                
                if (!db.TextSet.Any(x => x.Name == textName))
                {
                    db.TextSet.Add(textToUpload);
                    db.SaveChanges();
                }
                else
                {
                    throw new TextNameAlreadyExistException();
                }

                UploadTextSpanTags(tagMatches[0], db, textToUpload);
                long tagIdOffset = 0;
                if (db.TagSet.Count() != 0)
                {
                    tagIdOffset = db.TagSet.Select(x => x.Id).Max() - tagMatches[0].Count;
                }
                UploadANotes(tagMatches[1], db, tagIdOffset);
                UploadSharpNotes(tagMatches[2], db, tagIdOffset);
                UploadRelationTags(tagMatches[3], db, tagIdOffset);
            }

        }
        
        private static void UploadTextSpanTags(MatchCollection textSpanTags, TextCorpusContext db, Text text)
        {
            foreach (Match match in textSpanTags)
            {
                var annoName = new AnnotationName();
                annoName.Name = match.Groups[2].Value;
                if (db.AnnotationNameSet.Where(x => x.Name == annoName.Name).Select(x => x).Count() == 0)
                {
                    db.AnnotationNameSet.Add(annoName);
                    db.SaveChanges();
                }
                else
                {
                    annoName = db.AnnotationNameSet.Where(x => x.Name == annoName.Name).First();
                }

                var tag = new Tag();
                tag.StartPos = int.Parse(match.Groups[3].Value);
                tag.EndPos = int.Parse(match.Groups[4].Value);
                tag.TaggedText = match.Groups[5].Value.Trim();
                tag.Name = annoName;
                tag.Text = text;
                db.TagSet.Add(tag);
                db.SaveChanges();
            }
        }
        
        private static void UploadANotes(MatchCollection ANotes, TextCorpusContext db, long tagIdOffset)
        {
            foreach (Match match in ANotes)
            {
                var annoName = new AnnotationName();
                annoName.Name = match.Groups[2].Value;
                if (db.AnnotationNameSet.Where(x => x.Name == annoName.Name).Select(x => x).Count() == 0)
                {
                    db.AnnotationNameSet.Add(annoName);
                    db.SaveChanges();
                }
                else
                {
                    annoName = db.AnnotationNameSet.Where(x => x.Name == annoName.Name).First();
                }

                long relatedTagId = tagIdOffset + int.Parse(match.Groups[3].Value.Trim('T'));
                var relatedTag = db.TagSet.First(x => x.Id == relatedTagId);

                var aNote = new ANote();
                aNote.Name = annoName;
                aNote.Tag = relatedTag;
                db.ANoteSet.Add(aNote);
                db.SaveChanges();
            }
        }
        
        private static void UploadSharpNotes(MatchCollection sharpNotes, TextCorpusContext db, long tagIdOffset)
        {
            foreach (Match match in sharpNotes)
            {
                long relatedTagId = tagIdOffset + int.Parse(match.Groups[3].Value.Trim('T'));
                var relatedTag = db.TagSet.First(x => x.Id == relatedTagId);
                
                var sharpNote = new AnnotatorNote();
                sharpNote.Tag = relatedTag;
                sharpNote.AnnotationText = match.Groups[4].Value;
                db.AnnotatorNoteSet.Add(sharpNote);
                db.SaveChanges();
            }
        }
        
        private static void UploadRelationTags(MatchCollection relationTags, TextCorpusContext db, long tagIdOffset)
        {
            foreach (Match match in relationTags)
            {
                var annoName = new AnnotationName();
                annoName.Name = match.Groups[2].Value;
                if (db.AnnotationNameSet.Where(x => x.Name == annoName.Name).Select(x => x).Count() == 0)
                {
                    db.AnnotationNameSet.Add(annoName);
                    db.SaveChanges();
                }
                else
                {
                    annoName = db.AnnotationNameSet.Where(x => x.Name == annoName.Name).First();
                }

                long firstRelatedTagId = tagIdOffset + int.Parse(match.Groups[3].Value.Trim('T'));
                long secondRelatedTagId = tagIdOffset + int.Parse(match.Groups[4].Value.Trim('T'));
                var firstRelatedTag = db.TagSet.First(x => x.Id == firstRelatedTagId);
                var secondRelatedTag = db.TagSet.First(x => x.Id == secondRelatedTagId);
                
                var relationNote = new RelationNote();
                relationNote.FirstTag = firstRelatedTag;
                relationNote.SecondTag = secondRelatedTag;
                relationNote.Name = annoName;
                db.RelationNoteSet.Add(relationNote);
                db.SaveChanges();
            }
        }

        public static string GetText(int textId)
        {
            string text;
            using (TextCorpusContext db = new TextCorpusContext())
            {
                text = db.TextSet.Where(x => x.Id == textId).Select(x => x.Txt).First();
            }
            return text;
        }

        public static List<Tag> GetTags(int textId)
        {
            var tagList = new List<Tag>();
            using (TextCorpusContext db = new TextCorpusContext())
            {
                tagList = db.TagSet.Where(x => x.TextId == textId).ToList();
                
            }
            return tagList;
        }

        //public static void UpdateTextAnnotation(int textId, StreamReader annotationStream)
        public static void UpdateTextAnnotation(int textId, string annotation)
        {
            //string annotation = annotationStream.ReadToEnd();

            if (!IsValidAnnotation(new StringReader(annotation)))
                throw new InvalidAnnotationException();
            //var tagMatches = ResolveAnnotation(annotationStream);
            var tagMatches = ResolveAnnotation(annotation);

            using (TextCorpusContext db = new TextCorpusContext())
            {
                

                var tagsToDelete = db.TagSet.Where(x => x.TextId == textId);
                db.TagSet.RemoveRange(tagsToDelete);
                db.SaveChanges();

                var textToUpdate = db.TextSet.Where(x => x.Id == textId).First();
                textToUpdate.Annotation = annotation;
                db.Entry(textToUpdate).State = EntityState.Modified;
                db.SaveChanges();



                UploadTextSpanTags(tagMatches[0], db, textToUpdate);
                long tagIdOffset = 0;
                if (db.TagSet.Count() != 0)
                {
                    tagIdOffset = db.TagSet.Select(x => x.Id).Max() - tagMatches[0].Count;
                }
                UploadANotes(tagMatches[1], db, tagIdOffset);
                UploadSharpNotes(tagMatches[2], db, tagIdOffset);
                UploadRelationTags(tagMatches[3], db, tagIdOffset);
            }
        }

        public static void DeleteText(int textId)
        {
            using (TextCorpusContext db = new TextCorpusContext())
            {
                var textToDelete = db.TextSet.Where(x => x.Id == textId).First();
                db.TextSet.Remove(textToDelete);
                db.SaveChanges();
            }
        }
    }

    public class TextNameAlreadyExistException : Exception
    {
    }

    public class InvalidAnnotationException : Exception
    {
        
    }
}
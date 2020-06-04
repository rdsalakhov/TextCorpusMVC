using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TextCorpusMVC.Models;
using GemBox;
using GemBox.Document;
using System.IO;
using DocumentFormat.OpenXml.Extensions;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            //var gemDoc = DocumentModel.Load("rtftext.rtf");
            //var opts = new HtmlSaveOptions();
            ////var fs = new FileStream("htmldoc.html", FileAccess.ReadWrite);
            //gemDoc.Save("htmldoc.html", opts);

            //DocumentReader reader;

            DocumentWriter.StreamToFile("ooxml.docx", DocumentReader.Copy("rtftext.rtf"));
        }
    }

    class IndexedLemmaTag
    {
        public Tag Tag { get; set; }
        public string Lemma { get; set; }
        public int Index { get; set; }

        public IndexedLemmaTag(Tag tag, string lemma, int index)
        {
            Tag = tag;
            Lemma = lemma;
            Index = index;
        }
    }
}

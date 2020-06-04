using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
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
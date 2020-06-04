using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int StartPos { get; set; }
        public int EndPos { get; set; }
        public string TaggedText { get; set; }
        
        public int? TextId { get; set; }
        public Text Text { get; set; }
        
        public int? NameId { get; set; }
        public AnnotationName Name { get; set; }
    }
}
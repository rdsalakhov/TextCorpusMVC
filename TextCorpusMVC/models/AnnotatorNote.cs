using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class AnnotatorNote
    {
        public int Id { get; set; }
        public string AnnotationText { get; set; }
        
        public int? TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
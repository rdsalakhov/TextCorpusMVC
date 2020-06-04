using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class ANote
    {
        public int Id { get; set; }
        
        public int? NameId { get; set; }
        public AnnotationName Name { get; set; }
        
        public int? TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
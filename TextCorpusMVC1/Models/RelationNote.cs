using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class RelationNote
    {
        public int Id { get; set; }
        
        public int? NameId { get; set; }
        public AnnotationName Name { get; set; }
        
        public int? FirstTagId { get; set; }
        public Tag FirstTag { get; set; }
        
        public int? SecondTagId { get; set; }
        public Tag SecondTag { get; set; }
    }
}
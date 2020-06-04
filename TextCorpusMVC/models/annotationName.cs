using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class AnnotationName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<Tag> TagSet { get; set; }
        public virtual ICollection<ANote> ANoteSet { get; set; }
        public virtual ICollection<RelationNote> RelationNoteSet { get; set; }
    }
}
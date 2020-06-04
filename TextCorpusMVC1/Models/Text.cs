using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class Text
    {
        public Text()
        {
            TagSet = new HashSet<Tag>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Txt { get; set; }

        [DataType(DataType.MultilineText)]
        public string Annotation { get; set; }
        public virtual ICollection<Tag> TagSet { get; set; }
    }

    public class TextToUpload
    {
        public TextToUpload()
        {
            TagSet = new HashSet<Tag>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Txt { get; set; }

        [DataType(DataType.MultilineText)]
        public string Annotation { get; set; }

        public HttpPostedFileBase TextFile { get; set; }
        public HttpPostedFileBase AnnotationFile { get; set; }

        public virtual ICollection<Tag> TagSet { get; set; }
    }
}
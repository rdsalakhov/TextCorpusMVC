using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class HighlightedText
    {
        public Text Text { get; set; }
        public List<int> StartPositions { get; set; }
        public List<int> EndPositions { get; set; }
        //public IEnumerable<IEnumerable<Tag>> TagsToHighlight { get; set; }
        //public Dictionary<string, Color> ColorDictionary { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class PairQuery
    {
        [Required]
        public string First{ get; set; }
        [Required]
        public string Second { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Допустимые значения от 0 до 100")]
        public int MinRange { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Допустимые значения от 0 до 100")]
        public int MaxRange { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class UserAccess
    {
        public int Id { get; set; }
        
        public int? UserId { get; set; }
        public User User { get; set; }
        
        public int? TextId { get; set; }
        public  Text Text { get; set; }
    }
}
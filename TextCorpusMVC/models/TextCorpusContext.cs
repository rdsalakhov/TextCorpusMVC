using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class TextCorpusContext : DbContext
    {
        //"Data Source=(localdb)\\MSSQLLOCALDB;Initial Catalog=TextCorpusMVC;Integrated Security=True"
        //TextCorpusMVC20200530003349_db
        //Server=tcp:textcorpusmvc20200530003349dbserver.database.windows.net,1433;Initial Catalog=TextCorpusMVC20200530003349_db;Persist Security Info=False;User ID=rdsalakhov;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
        public TextCorpusContext()
            : base("DefaultConnection")
        { }

        public DbSet<User> UserSet { get; set; }
        public DbSet<UserAccess> UserAccessSet { get; set; }
        public DbSet<Text> TextSet { get; set; }
        public DbSet<Tag> TagSet { get; set; }
        public DbSet<AnnotationName> AnnotationNameSet { get; set; }
        public DbSet<AnnotatorNote> AnnotatorNoteSet { get; set; }
        public DbSet<ANote> ANoteSet { get; set; }
        public DbSet<RelationNote> RelationNoteSet { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<TextCorpusContext>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}
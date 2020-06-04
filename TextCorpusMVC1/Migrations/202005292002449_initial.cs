namespace TextCorpusMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnnotationNames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ANotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameId = c.Int(),
                        TagId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnnotationNames", t => t.NameId)
                .ForeignKey("dbo.Tags", t => t.TagId)
                .Index(t => t.NameId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartPos = c.Int(nullable: false),
                        EndPos = c.Int(nullable: false),
                        TaggedText = c.String(),
                        TextId = c.Int(),
                        NameId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnnotationNames", t => t.NameId)
                .ForeignKey("dbo.Texts", t => t.TextId)
                .Index(t => t.TextId)
                .Index(t => t.NameId);
            
            CreateTable(
                "dbo.Texts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Txt = c.String(),
                        Annotation = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RelationNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameId = c.Int(),
                        FirstTagId = c.Int(),
                        SecondTagId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tags", t => t.FirstTagId)
                .ForeignKey("dbo.AnnotationNames", t => t.NameId)
                .ForeignKey("dbo.Tags", t => t.SecondTagId)
                .Index(t => t.NameId)
                .Index(t => t.FirstTagId)
                .Index(t => t.SecondTagId);
            
            CreateTable(
                "dbo.AnnotatorNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnnotationText = c.String(),
                        TagId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tags", t => t.TagId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.UserAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        TextId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Texts", t => t.TextId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.TextId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Password = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserAccesses", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAccesses", "TextId", "dbo.Texts");
            DropForeignKey("dbo.AnnotatorNotes", "TagId", "dbo.Tags");
            DropForeignKey("dbo.RelationNotes", "SecondTagId", "dbo.Tags");
            DropForeignKey("dbo.RelationNotes", "NameId", "dbo.AnnotationNames");
            DropForeignKey("dbo.RelationNotes", "FirstTagId", "dbo.Tags");
            DropForeignKey("dbo.ANotes", "TagId", "dbo.Tags");
            DropForeignKey("dbo.Tags", "TextId", "dbo.Texts");
            DropForeignKey("dbo.Tags", "NameId", "dbo.AnnotationNames");
            DropForeignKey("dbo.ANotes", "NameId", "dbo.AnnotationNames");
            DropIndex("dbo.UserAccesses", new[] { "TextId" });
            DropIndex("dbo.UserAccesses", new[] { "UserId" });
            DropIndex("dbo.AnnotatorNotes", new[] { "TagId" });
            DropIndex("dbo.RelationNotes", new[] { "SecondTagId" });
            DropIndex("dbo.RelationNotes", new[] { "FirstTagId" });
            DropIndex("dbo.RelationNotes", new[] { "NameId" });
            DropIndex("dbo.Tags", new[] { "NameId" });
            DropIndex("dbo.Tags", new[] { "TextId" });
            DropIndex("dbo.ANotes", new[] { "TagId" });
            DropIndex("dbo.ANotes", new[] { "NameId" });
            DropTable("dbo.Users");
            DropTable("dbo.UserAccesses");
            DropTable("dbo.AnnotatorNotes");
            DropTable("dbo.RelationNotes");
            DropTable("dbo.Texts");
            DropTable("dbo.Tags");
            DropTable("dbo.ANotes");
            DropTable("dbo.AnnotationNames");
        }
    }
}

namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnswerBlocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        CoordinateX = c.Int(nullable: false),
                        CoordinateY = c.Int(nullable: false),
                        AnswerOptionsNumber = c.Int(nullable: false),
                        Rows = c.Int(nullable: false),
                        FirstQuestionIndex = c.Int(nullable: false),
                        PatternId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patterns", t => t.PatternId, cascadeDelete: true)
                .Index(t => t.PatternId);
            
            AddColumn("dbo.ClassBooks", "User_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "ClassBook_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Photos", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ClassBooks", "User_Id");
            CreateIndex("dbo.AspNetUsers", "ClassBook_Id");
            CreateIndex("dbo.Photos", "User_Id");
            AddForeignKey("dbo.ClassBooks", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Photos", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "ClassBook_Id", "dbo.ClassBooks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "ClassBook_Id", "dbo.ClassBooks");
            DropForeignKey("dbo.Photos", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AnswerBlocks", "PatternId", "dbo.Patterns");
            DropForeignKey("dbo.ClassBooks", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Photos", new[] { "User_Id" });
            DropIndex("dbo.AnswerBlocks", new[] { "PatternId" });
            DropIndex("dbo.AspNetUsers", new[] { "ClassBook_Id" });
            DropIndex("dbo.ClassBooks", new[] { "User_Id" });
            DropColumn("dbo.Photos", "User_Id");
            DropColumn("dbo.AspNetUsers", "ClassBook_Id");
            DropColumn("dbo.ClassBooks", "User_Id");
            DropTable("dbo.AnswerBlocks");
        }
    }
}

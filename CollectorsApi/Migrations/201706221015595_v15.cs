namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatternAnswerSheets", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.PatternAnswerSheets", "StudentId", c => c.Int(nullable: false));
            AddColumn("dbo.PatternAnswerSheets", "Student_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.PatternAnswerSheets", "Student_Id");
            AddForeignKey("dbo.PatternAnswerSheets", "Student_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatternAnswerSheets", "Student_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PatternAnswerSheets", new[] { "Student_Id" });
            DropColumn("dbo.PatternAnswerSheets", "Student_Id");
            DropColumn("dbo.PatternAnswerSheets", "StudentId");
            DropColumn("dbo.PatternAnswerSheets", "CreatedBy");
        }
    }
}

namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v17 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PatternAnswerSheets", new[] { "Student_Id" });
            DropColumn("dbo.PatternAnswerSheets", "StudentId");
            RenameColumn(table: "dbo.PatternAnswerSheets", name: "Student_Id", newName: "StudentId");
            AlterColumn("dbo.PatternAnswerSheets", "StudentId", c => c.String(maxLength: 128));
            CreateIndex("dbo.PatternAnswerSheets", "StudentId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PatternAnswerSheets", new[] { "StudentId" });
            AlterColumn("dbo.PatternAnswerSheets", "StudentId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.PatternAnswerSheets", name: "StudentId", newName: "Student_Id");
            AddColumn("dbo.PatternAnswerSheets", "StudentId", c => c.Int(nullable: false));
            CreateIndex("dbo.PatternAnswerSheets", "Student_Id");
        }
    }
}

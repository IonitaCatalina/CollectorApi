namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Photos", "PatternId", "dbo.Patterns");
            DropIndex("dbo.Patterns", new[] { "Teacher_Id" });
            DropIndex("dbo.Photos", new[] { "PatternId" });
            DropIndex("dbo.Photos", new[] { "Student_Id" });
            DropIndex("dbo.Grades", new[] { "Student_Id" });
            DropColumn("dbo.Patterns", "TeacherId");
            DropColumn("dbo.Photos", "StudentId");
            DropColumn("dbo.Grades", "UserId");
            RenameColumn(table: "dbo.Patterns", name: "Teacher_Id", newName: "TeacherId");
            RenameColumn(table: "dbo.Photos", name: "Student_Id", newName: "StudentId");
            RenameColumn(table: "dbo.Grades", name: "Student_Id", newName: "UserId");
            AddColumn("dbo.Photos", "Pattern_Id", c => c.Int());
            AlterColumn("dbo.Patterns", "TeacherId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Photos", "StudentId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Photos", "PatternId", c => c.String());
            AlterColumn("dbo.Grades", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Patterns", "TeacherId");
            CreateIndex("dbo.Photos", "StudentId");
            CreateIndex("dbo.Photos", "Pattern_Id");
            CreateIndex("dbo.Grades", "UserId");
            AddForeignKey("dbo.Photos", "Pattern_Id", "dbo.Patterns", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "Pattern_Id", "dbo.Patterns");
            DropIndex("dbo.Grades", new[] { "UserId" });
            DropIndex("dbo.Photos", new[] { "Pattern_Id" });
            DropIndex("dbo.Photos", new[] { "StudentId" });
            DropIndex("dbo.Patterns", new[] { "TeacherId" });
            AlterColumn("dbo.Grades", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.Photos", "PatternId", c => c.Int(nullable: false));
            AlterColumn("dbo.Photos", "StudentId", c => c.Int(nullable: false));
            AlterColumn("dbo.Patterns", "TeacherId", c => c.Int(nullable: false));
            DropColumn("dbo.Photos", "Pattern_Id");
            RenameColumn(table: "dbo.Grades", name: "UserId", newName: "Student_Id");
            RenameColumn(table: "dbo.Photos", name: "StudentId", newName: "Student_Id");
            RenameColumn(table: "dbo.Patterns", name: "TeacherId", newName: "Teacher_Id");
            AddColumn("dbo.Grades", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.Photos", "StudentId", c => c.Int(nullable: false));
            AddColumn("dbo.Patterns", "TeacherId", c => c.Int(nullable: false));
            CreateIndex("dbo.Grades", "Student_Id");
            CreateIndex("dbo.Photos", "Student_Id");
            CreateIndex("dbo.Photos", "PatternId");
            CreateIndex("dbo.Patterns", "Teacher_Id");
            AddForeignKey("dbo.Photos", "PatternId", "dbo.Patterns", "Id", cascadeDelete: true);
        }
    }
}

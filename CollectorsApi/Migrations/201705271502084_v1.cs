namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Grades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Score = c.Single(nullable: false),
                        UserId = c.Int(nullable: false),
                        Student_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Student_Id)
                .Index(t => t.Student_Id);
            
            AddColumn("dbo.Patterns", "TeacherId", c => c.Int(nullable: false));
            AddColumn("dbo.Patterns", "Teacher_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Photos", "TeacherId", c => c.Int(nullable: false));
            AddColumn("dbo.Photos", "StudentId", c => c.Int(nullable: false));
            AddColumn("dbo.Photos", "GradeId", c => c.Int(nullable: false));
            AddColumn("dbo.Photos", "Student_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Photos", "Teacher_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "ClassName", c => c.String());
            CreateIndex("dbo.Patterns", "Teacher_Id");
            CreateIndex("dbo.Photos", "GradeId");
            CreateIndex("dbo.Photos", "Student_Id");
            CreateIndex("dbo.Photos", "Teacher_Id");
            AddForeignKey("dbo.Patterns", "Teacher_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Photos", "GradeId", "dbo.Grades", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Photos", "Student_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Photos", "Teacher_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "Teacher_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Photos", "Student_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Photos", "GradeId", "dbo.Grades");
            DropForeignKey("dbo.Grades", "Student_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Patterns", "Teacher_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Grades", new[] { "Student_Id" });
            DropIndex("dbo.Photos", new[] { "Teacher_Id" });
            DropIndex("dbo.Photos", new[] { "Student_Id" });
            DropIndex("dbo.Photos", new[] { "GradeId" });
            DropIndex("dbo.Patterns", new[] { "Teacher_Id" });
            DropColumn("dbo.AspNetUsers", "ClassName");
            DropColumn("dbo.Photos", "Teacher_Id");
            DropColumn("dbo.Photos", "Student_Id");
            DropColumn("dbo.Photos", "GradeId");
            DropColumn("dbo.Photos", "StudentId");
            DropColumn("dbo.Photos", "TeacherId");
            DropColumn("dbo.Patterns", "Teacher_Id");
            DropColumn("dbo.Patterns", "TeacherId");
            DropTable("dbo.Grades");
        }
    }
}

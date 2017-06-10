namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Grades", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Photos", "GradeId", "dbo.Grades");
            DropIndex("dbo.Photos", new[] { "GradeId" });
            DropIndex("dbo.Grades", new[] { "UserId" });
            AddColumn("dbo.Photos", "Grade", c => c.Int(nullable: false));
            DropColumn("dbo.Photos", "GradeId");
            DropTable("dbo.Grades");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Grades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Score = c.Single(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Photos", "GradeId", c => c.Int(nullable: false));
            DropColumn("dbo.Photos", "Grade");
            CreateIndex("dbo.Grades", "UserId");
            CreateIndex("dbo.Photos", "GradeId");
            AddForeignKey("dbo.Photos", "GradeId", "dbo.Grades", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Grades", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}

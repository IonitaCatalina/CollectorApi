namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassBooks",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        TeacherId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.TeacherId)
                .Index(t => t.TeacherId);
            
            DropColumn("dbo.AspNetUsers", "ClassName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "ClassName", c => c.String());
            DropForeignKey("dbo.ClassBooks", "TeacherId", "dbo.AspNetUsers");
            DropIndex("dbo.ClassBooks", new[] { "TeacherId" });
            DropTable("dbo.ClassBooks");
        }
    }
}

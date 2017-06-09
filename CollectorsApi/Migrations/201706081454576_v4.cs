namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v4 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Photos", new[] { "Teacher_Id" });
            DropColumn("dbo.Photos", "TeacherId");
            RenameColumn(table: "dbo.Photos", name: "Teacher_Id", newName: "TeacherId");
            AlterColumn("dbo.Photos", "TeacherId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Photos", "TeacherId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Photos", new[] { "TeacherId" });
            AlterColumn("dbo.Photos", "TeacherId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Photos", name: "TeacherId", newName: "Teacher_Id");
            AddColumn("dbo.Photos", "TeacherId", c => c.Int(nullable: false));
            CreateIndex("dbo.Photos", "Teacher_Id");
        }
    }
}

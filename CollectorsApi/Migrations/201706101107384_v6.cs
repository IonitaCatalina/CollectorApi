namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Photos", "Pattern_Id", "dbo.Patterns");
            DropIndex("dbo.Photos", new[] { "Pattern_Id" });
            DropColumn("dbo.Photos", "PatternId");
            RenameColumn(table: "dbo.Photos", name: "Pattern_Id", newName: "PatternId");
            AlterColumn("dbo.Photos", "PatternId", c => c.Int(nullable: false));
            AlterColumn("dbo.Photos", "PatternId", c => c.Int(nullable: false));
            CreateIndex("dbo.Photos", "PatternId");
            AddForeignKey("dbo.Photos", "PatternId", "dbo.Patterns", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "PatternId", "dbo.Patterns");
            DropIndex("dbo.Photos", new[] { "PatternId" });
            AlterColumn("dbo.Photos", "PatternId", c => c.Int());
            AlterColumn("dbo.Photos", "PatternId", c => c.String());
            RenameColumn(table: "dbo.Photos", name: "PatternId", newName: "Pattern_Id");
            AddColumn("dbo.Photos", "PatternId", c => c.String());
            CreateIndex("dbo.Photos", "Pattern_Id");
            AddForeignKey("dbo.Photos", "Pattern_Id", "dbo.Patterns", "Id");
        }
    }
}

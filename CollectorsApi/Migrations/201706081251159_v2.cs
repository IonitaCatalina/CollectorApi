namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patterns", "Width", c => c.Int(nullable: false));
            AddColumn("dbo.Patterns", "Height", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patterns", "Height");
            DropColumn("dbo.Patterns", "Width");
        }
    }
}

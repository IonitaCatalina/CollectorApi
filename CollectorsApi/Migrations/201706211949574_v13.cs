namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patterns", "MaxSizeRatio", c => c.Double(nullable: false));
            AddColumn("dbo.Patterns", "MinSizeRatio", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patterns", "MinSizeRatio");
            DropColumn("dbo.Patterns", "MaxSizeRatio");
        }
    }
}

namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patterns", "Published", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patterns", "Published");
        }
    }
}

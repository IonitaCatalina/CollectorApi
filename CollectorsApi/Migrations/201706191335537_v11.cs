namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "Description");
        }
    }
}

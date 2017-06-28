namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v21 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PatternAnswerSheets", "CreatedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatternAnswerSheets", "CreatedBy", c => c.Int(nullable: false));
        }
    }
}

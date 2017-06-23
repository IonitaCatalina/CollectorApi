namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatternAnswerSheets", "Question", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatternAnswerSheets", "Question");
        }
    }
}

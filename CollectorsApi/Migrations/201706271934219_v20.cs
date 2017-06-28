namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatternAnswerSheets", "AnswerString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatternAnswerSheets", "AnswerString");
        }
    }
}

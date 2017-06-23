namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v16 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatternAnswerSheets", "Answer", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatternAnswerSheets", "Answer", c => c.Int(nullable: false));
        }
    }
}

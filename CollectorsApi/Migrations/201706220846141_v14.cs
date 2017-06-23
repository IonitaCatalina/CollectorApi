namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v14 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatternAnswerSheets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionNumber = c.Int(nullable: false),
                        Answer = c.Int(nullable: false),
                        PatternId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patterns", t => t.PatternId, cascadeDelete: true)
                .Index(t => t.PatternId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatternAnswerSheets", "PatternId", "dbo.Patterns");
            DropIndex("dbo.PatternAnswerSheets", new[] { "PatternId" });
            DropTable("dbo.PatternAnswerSheets");
        }
    }
}

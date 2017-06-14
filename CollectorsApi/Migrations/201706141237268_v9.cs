namespace CollectorsApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v9 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ClassBooks");
            AlterColumn("dbo.ClassBooks", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.ClassBooks", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ClassBooks");
            AlterColumn("dbo.ClassBooks", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.ClassBooks", "Id");
        }
    }
}

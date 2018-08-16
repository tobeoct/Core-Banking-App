namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GLCategories", "MainAccountCategory", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GLCategories", "MainAccountCategory", c => c.String(nullable: false, maxLength: 1000));
        }
    }
}

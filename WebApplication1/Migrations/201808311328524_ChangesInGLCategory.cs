namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesInGLCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GLCategories", "Name", c => c.String(nullable: false, maxLength: 1000));
            DropColumn("dbo.GLCategories", "MainAccountCategory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GLCategories", "MainAccountCategory", c => c.String(nullable: false, maxLength: 1000));
            DropColumn("dbo.GLCategories", "Name");
        }
    }
}

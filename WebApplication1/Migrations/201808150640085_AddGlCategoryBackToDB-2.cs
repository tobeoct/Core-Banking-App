namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGlCategoryBackToDB2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GLCategories", "MainAccountCategory", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.GLCategories", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GLCategories", "Description", c => c.String());
            AlterColumn("dbo.GLCategories", "MainAccountCategory", c => c.String(maxLength: 1000));
        }
    }
}

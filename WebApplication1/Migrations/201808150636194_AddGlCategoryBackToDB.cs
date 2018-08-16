namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGlCategoryBackToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GLCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoriesId = c.Byte(nullable: false),
                        MainAccountCategory = c.String(nullable:false, maxLength: 1000),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoriesId, cascadeDelete: true)
                .Index(t => t.CategoriesId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GLCategories", "CategoriesId", "dbo.Categories");
            DropIndex("dbo.GLCategories", new[] { "CategoriesId" });
            DropTable("dbo.GLCategories");
        }
    }
}

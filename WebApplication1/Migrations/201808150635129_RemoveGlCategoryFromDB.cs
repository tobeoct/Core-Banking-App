namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveGlCategoryFromDB : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GLCategories", "CategoriesId", "dbo.Categories");
            DropIndex("dbo.GLCategories", new[] { "CategoriesId" });
            DropTable("dbo.GLCategories");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GLCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoriesId = c.Byte(nullable: false),
                        MainAccountCategory = c.String(maxLength: 1000),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.GLCategories", "CategoriesId");
            AddForeignKey("dbo.GLCategories", "CategoriesId", "dbo.Categories", "Id", cascadeDelete: true);
        }
    }
}

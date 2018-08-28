namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeTypeofGlCatIdInGlAccount : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GLAccounts", "GlCategories_Id", "dbo.GLCategories");
            DropIndex("dbo.GLAccounts", new[] { "GlCategories_Id" });
            DropColumn("dbo.GLAccounts", "GlCategoriesId");
            RenameColumn(table: "dbo.GLAccounts", name: "GlCategories_Id", newName: "GlCategoriesId");
            AlterColumn("dbo.GLAccounts", "GlCategoriesId", c => c.Int(nullable: false));
            AlterColumn("dbo.GLAccounts", "GlCategoriesId", c => c.Int(nullable: false));
            CreateIndex("dbo.GLAccounts", "GlCategoriesId");
            AddForeignKey("dbo.GLAccounts", "GlCategoriesId", "dbo.GLCategories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GLAccounts", "GlCategoriesId", "dbo.GLCategories");
            DropIndex("dbo.GLAccounts", new[] { "GlCategoriesId" });
            AlterColumn("dbo.GLAccounts", "GlCategoriesId", c => c.Int());
            AlterColumn("dbo.GLAccounts", "GlCategoriesId", c => c.Byte(nullable: false));
            RenameColumn(table: "dbo.GLAccounts", name: "GlCategoriesId", newName: "GlCategories_Id");
            AddColumn("dbo.GLAccounts", "GlCategoriesId", c => c.Byte(nullable: false));
            CreateIndex("dbo.GLAccounts", "GlCategories_Id");
            AddForeignKey("dbo.GLAccounts", "GlCategories_Id", "dbo.GLCategories", "Id");
        }
    }
}

namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGLAccountToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GLAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Code = c.String(),
                        BranchId = c.Int(nullable: false),
                        CategoriesId = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.CategoriesId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.CategoriesId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GLAccounts", "CategoriesId", "dbo.Categories");
            DropForeignKey("dbo.GLAccounts", "BranchId", "dbo.Branches");
            DropIndex("dbo.GLAccounts", new[] { "CategoriesId" });
            DropIndex("dbo.GLAccounts", new[] { "BranchId" });
            DropTable("dbo.GLAccounts");
        }
    }
}

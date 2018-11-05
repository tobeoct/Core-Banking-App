namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOnUsToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RemoteOnUs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GLAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.GLAccountId, cascadeDelete: true)
                .Index(t => t.GLAccountId);
            
            CreateTable(
                "dbo.TSSAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GLAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GLAccounts", t => t.GLAccountId, cascadeDelete: true)
                .Index(t => t.GLAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TSSAccounts", "GLAccountId", "dbo.GLAccounts");
            DropForeignKey("dbo.RemoteOnUs", "GLAccountId", "dbo.GLAccounts");
            DropIndex("dbo.TSSAccounts", new[] { "GLAccountId" });
            DropIndex("dbo.RemoteOnUs", new[] { "GLAccountId" });
            DropTable("dbo.TSSAccounts");
            DropTable("dbo.RemoteOnUs");
        }
    }
}

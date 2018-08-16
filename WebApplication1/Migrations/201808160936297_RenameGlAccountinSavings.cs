namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameGlAccountinSavings : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SavingsAccountTypes", new[] { "GlAccountId" });
            CreateIndex("dbo.SavingsAccountTypes", "GLAccountId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SavingsAccountTypes", new[] { "GLAccountId" });
            CreateIndex("dbo.SavingsAccountTypes", "GlAccountId");
        }
    }
}

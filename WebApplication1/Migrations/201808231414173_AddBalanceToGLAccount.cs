namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBalanceToGLAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GLAccounts", "AccountBalance", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GLAccounts", "AccountBalance");
        }
    }
}

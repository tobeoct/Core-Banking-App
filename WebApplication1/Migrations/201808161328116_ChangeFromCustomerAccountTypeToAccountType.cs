namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFromCustomerAccountTypeToAccountType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerAccounts", "CustomerAccountTypeId", "dbo.CustomerAccountTypes");
            DropIndex("dbo.CustomerAccounts", new[] { "CustomerAccountTypeId" });
            AddColumn("dbo.CustomerAccounts", "AccountTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.CustomerAccounts", "AccountTypeId");
            AddForeignKey("dbo.CustomerAccounts", "AccountTypeId", "dbo.AccountTypes", "Id", cascadeDelete: true);
            DropColumn("dbo.CustomerAccounts", "CustomerAccountTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerAccounts", "CustomerAccountTypeId", c => c.Int(nullable: false));
            DropForeignKey("dbo.CustomerAccounts", "AccountTypeId", "dbo.AccountTypes");
            DropIndex("dbo.CustomerAccounts", new[] { "AccountTypeId" });
            DropColumn("dbo.CustomerAccounts", "AccountTypeId");
            CreateIndex("dbo.CustomerAccounts", "CustomerAccountTypeId");
            AddForeignKey("dbo.CustomerAccounts", "CustomerAccountTypeId", "dbo.CustomerAccountTypes", "Id", cascadeDelete: true);
        }
    }
}

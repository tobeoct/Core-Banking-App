namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddChangeBalanceTypeToFloat4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CustomerAccounts", "AccountNumber", c => c.String(nullable: false));
        }


        public override void Down()
        {
            AlterColumn("dbo.CustomerAccounts", "AccountNumber", c => c.Int(nullable: false));
        }
    }
}

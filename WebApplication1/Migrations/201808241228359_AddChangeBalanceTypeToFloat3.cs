namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddChangeBalanceTypeToFloat3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CustomerAccounts", "AccountNumber", c => c.Long(nullable: false));
        }


        public override void Down()
        {
            AlterColumn("dbo.CustomerAccounts", "AccountNumber", c => c.Int(nullable: false));
        }
    }
}

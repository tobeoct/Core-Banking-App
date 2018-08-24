namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChangeBalanceTypeToFloat : DbMigration
    {
        public override void Up()
        {
            
            AlterColumn("dbo.GLAccounts", "AccountBalance", c => c.Single(nullable: false));
            AlterColumn("dbo.Tellers", "TillAccountBalance", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {

            AlterColumn("dbo.Tellers", "TillAccountBalance", c => c.Long(nullable: false));
            AlterColumn("dbo.GLAccounts", "AccountBalance", c => c.Long(nullable: false));
            
        }
    }
}

namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPaymentRateToDb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Terms", "PaymentRate", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Terms", "PaymentRate");
        }
    }
}

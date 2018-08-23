namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeTermsTenureTypeToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Terms", "Tenure", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Terms", "Tenure", c => c.Single(nullable: false));
        }
    }
}

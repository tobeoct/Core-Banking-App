namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateBusinessStatusinDb : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO BusinessStatus (Status) VALUES('false')");
        }
        
        public override void Down()
        {
        }
    }
}

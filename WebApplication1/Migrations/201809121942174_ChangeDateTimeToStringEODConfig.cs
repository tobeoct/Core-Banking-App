namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDateTimeToStringEODConfig : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EODConfigs", "StartTime", c => c.String());
            AlterColumn("dbo.EODConfigs", "EndTime", c => c.String());
            AlterColumn("dbo.EODConfigs", "EODTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EODConfigs", "EODTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EODConfigs", "EndTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EODConfigs", "StartTime", c => c.DateTime(nullable: false));
        }
    }
}

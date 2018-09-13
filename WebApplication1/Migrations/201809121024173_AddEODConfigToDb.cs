namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEODConfigToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EODConfigs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        EODTime = c.DateTime(nullable: false),
                        IsRunning = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EODConfigs");
        }
    }
}

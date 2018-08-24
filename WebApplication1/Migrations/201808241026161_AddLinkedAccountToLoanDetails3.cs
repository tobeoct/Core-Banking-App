namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLinkedAccountToLoanDetails3 : DbMigration
    {
        public override void Up()
        {
//            DropForeignKey("dbo.LoanDetails", "Id", "dbo.CustomerAccounts");
//            DropIndex("dbo.LoanDetails", new[] { "Id" });
//            DropPrimaryKey("dbo.LoanDetails");
//            AddColumn("dbo.CustomerAccounts", "LoanId", c => c.Int());
//            AlterColumn("dbo.LoanDetails", "Id", c => c.Int(nullable: false, identity: true));
//            AddPrimaryKey("dbo.LoanDetails", "Id");
//            CreateIndex("dbo.CustomerAccounts", "LoanId");
//            AddForeignKey("dbo.CustomerAccounts", "LoanId", "dbo.LoanDetails", "Id");
//            DropColumn("dbo.CustomerAccounts", "LoanDetailsId");
            //DropColumn("dbo.LoanDetails", "LinkedAccountId");
        }
        
        public override void Down()
        {
           // AddColumn("dbo.LoanDetails", "LinkedAccountId", c => c.Int(nullable: false));
//            AddColumn("dbo.CustomerAccounts", "LoanDetailsId", c => c.Int());
//            DropForeignKey("dbo.CustomerAccounts", "LoanId", "dbo.LoanDetails");
//            DropIndex("dbo.CustomerAccounts", new[] { "LoanId" });
//            DropPrimaryKey("dbo.LoanDetails");
//            AlterColumn("dbo.LoanDetails", "Id", c => c.Int(nullable: false));
//            DropColumn("dbo.CustomerAccounts", "LoanId");
//            AddPrimaryKey("dbo.LoanDetails", "Id");
//            CreateIndex("dbo.LoanDetails", "Id");
//            AddForeignKey("dbo.LoanDetails", "Id", "dbo.CustomerAccounts", "Id");
        }
    }
}

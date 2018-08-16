namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerAccountTypeToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerAccountTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SavingsAccountTypeId = c.String(maxLength: 128),
                        CurrentAccountTypeId = c.String(maxLength: 128),
                        LoanAccountTypeId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurrentAccountTypes", t => t.CurrentAccountTypeId)
                .ForeignKey("dbo.LoanAccountTypes", t => t.LoanAccountTypeId)
                .ForeignKey("dbo.SavingsAccountTypes", t => t.SavingsAccountTypeId)
                .Index(t => t.SavingsAccountTypeId)
                .Index(t => t.CurrentAccountTypeId)
                .Index(t => t.LoanAccountTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerAccountTypes", "SavingsAccountTypeId", "dbo.SavingsAccountTypes");
            DropForeignKey("dbo.CustomerAccountTypes", "LoanAccountTypeId", "dbo.LoanAccountTypes");
            DropForeignKey("dbo.CustomerAccountTypes", "CurrentAccountTypeId", "dbo.CurrentAccountTypes");
            DropIndex("dbo.CustomerAccountTypes", new[] { "LoanAccountTypeId" });
            DropIndex("dbo.CustomerAccountTypes", new[] { "CurrentAccountTypeId" });
            DropIndex("dbo.CustomerAccountTypes", new[] { "SavingsAccountTypeId" });
            DropTable("dbo.CustomerAccountTypes");
        }
    }
}

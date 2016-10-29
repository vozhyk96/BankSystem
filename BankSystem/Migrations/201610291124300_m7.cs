namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccounts", "isCredit", c => c.Boolean(nullable: false));
            AlterColumn("dbo.BankAccounts", "money", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BankAccounts", "money", c => c.Int(nullable: false));
            DropColumn("dbo.BankAccounts", "isCredit");
        }
    }
}

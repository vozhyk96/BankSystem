namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccounts", "percent", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccounts", "percent");
        }
    }
}

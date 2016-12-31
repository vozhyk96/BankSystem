namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DbTransacts", "AdminId", c => c.String());
            AddColumn("dbo.DbTransacts", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DbTransacts", "UserId");
            DropColumn("dbo.DbTransacts", "AdminId");
        }
    }
}

namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DbTransacts", "CardInId", c => c.Int(nullable: false));
            AddColumn("dbo.DbTransacts", "CardOutId", c => c.Int(nullable: false));
            DropColumn("dbo.DbTransacts", "UserId");
            DropColumn("dbo.DbTransacts", "CardId");
            DropColumn("dbo.DbTransacts", "isGetter");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DbTransacts", "isGetter", c => c.Boolean(nullable: false));
            AddColumn("dbo.DbTransacts", "CardId", c => c.Int(nullable: false));
            AddColumn("dbo.DbTransacts", "UserId", c => c.String());
            DropColumn("dbo.DbTransacts", "CardOutId");
            DropColumn("dbo.DbTransacts", "CardInId");
        }
    }
}

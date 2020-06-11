namespace UrlsProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HostHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        ResponseTime = c.Int(nullable: false),
                        PageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HostPages", t => t.PageId, cascadeDelete: true)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.HostPages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinResponse = c.Int(nullable: false),
                        MaxResponse = c.Int(nullable: false),
                        Url = c.String(),
                        HostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hosts", t => t.HostId, cascadeDelete: true)
                .Index(t => t.HostId);
            
            CreateTable(
                "dbo.Hosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameOfHost = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HostHistories", "PageId", "dbo.HostPages");
            DropForeignKey("dbo.HostPages", "HostId", "dbo.Hosts");
            DropIndex("dbo.HostPages", new[] { "HostId" });
            DropIndex("dbo.HostHistories", new[] { "PageId" });
            DropTable("dbo.Hosts");
            DropTable("dbo.HostPages");
            DropTable("dbo.HostHistories");
        }
    }
}

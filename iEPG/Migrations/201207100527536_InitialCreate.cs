namespace iEPG.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TvPrograms",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReserveStatus = c.Byte(nullable: false),
                        ContentType = c.String(),
                        Version = c.String(),
                        Station = c.String(),
                        StationName = c.String(),
                        StartDateTime = c.DateTime(nullable: false),
                        EndDateTime = c.DateTime(nullable: false),
                        ProgramTitle = c.String(),
                        ProgramSubTitle = c.String(),
                        Performer = c.String(),
                        ProgramId = c.String(),
                        Genre1 = c.String(),
                        SubGenre1 = c.String(),
                        Genre2 = c.String(),
                        SubGenre2 = c.String(),
                        CopyControl1 = c.String(),
                        ComponentVideo1 = c.String(),
                        ComponentAudio1 = c.String(),
                        Attribute1 = c.String(),
                        Detail = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PastTvPrograms",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContentType = c.String(),
                        Version = c.String(),
                        Station = c.String(),
                        StationName = c.String(),
                        StartDateTime = c.DateTime(nullable: false),
                        EndDateTime = c.DateTime(nullable: false),
                        ProgramTitle = c.String(),
                        ProgramSubTitle = c.String(),
                        Performer = c.String(),
                        ProgramId = c.String(),
                        Genre1 = c.String(),
                        SubGenre1 = c.String(),
                        Genre2 = c.String(),
                        SubGenre2 = c.String(),
                        CopyControl1 = c.String(),
                        ComponentVideo1 = c.String(),
                        ComponentAudio1 = c.String(),
                        Attribute1 = c.String(),
                        Detail = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SearchConditions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FreeWordConditons",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FreeWord = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FreeWordConditons");
            DropTable("dbo.SearchConditions");
            DropTable("dbo.PastTvPrograms");
            DropTable("dbo.TvPrograms");
        }
    }
}

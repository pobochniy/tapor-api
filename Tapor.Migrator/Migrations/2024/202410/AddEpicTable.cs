using FluentMigrator;

namespace Tapor.Migrator.Migrations._2024._202410;

[Migration(20241001_1850)]
public class AddEpicTable: Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            CREATE TABLE IF NOT EXISTS `Epic` (
              `Id` int(11) NOT NULL AUTO_INCREMENT,
              `Assignee` char(36) CHARACTER SET ascii DEFAULT NULL,
              `Reporter` char(36) CHARACTER SET ascii DEFAULT NULL,
              `Priority` int(11) NOT NULL,
              `Name` longtext,
              `Description` longtext,
              `DueDate` datetime(6) DEFAULT NULL,
              PRIMARY KEY (`Id`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
        ");
        
        Create.Index("FK_Issue_Epic_EpicLink")
            .OnTable("Issue").OnColumn("EpicLink")
            .Ascending()
            .WithOptions().NonClustered();
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}
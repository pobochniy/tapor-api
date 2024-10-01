using FluentMigrator;

namespace Tapor.Migrator.Migrations._2024._202410;

[Migration(20241001_1815)]
public class AddUsersTable: Migration
{
    private const string TableUsersName = "Users";
    private const string TableProfilesName = "Profiles";
    
    public override void Up()
    {
        Create.Table(TableUsersName)
            .WithColumn("Id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("PasswordHash").AsString().NotNullable()
            .WithColumn("SecurityStamp").AsString().NotNullable()
            .WithColumn("AccessFailedCount").AsByte().NotNullable().WithDefaultValue(0)
            .WithColumn("EmailConfirmed").AsBoolean().NotNullable().WithDefaultValue(0)
            .WithColumn("PhoneNumberConfirmed").AsBoolean().NotNullable().WithDefaultValue(0);

        Create.Table(TableProfilesName)
            .WithColumn("Id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("UserName").AsString(255)
            .WithColumn("Email").AsString(255)
            .WithColumn("PhoneNumber").AsString(255)
            .WithColumn("Cash").AsDecimal(18, 10).NotNullable().WithDefaultValue(0)
            .WithColumn("Comment").AsString(4000);
        
        Create.UniqueConstraint($"IX_{TableProfilesName}_Email")
            .OnTable(TableProfilesName)
            .Columns("Email");
        Create.UniqueConstraint($"IX_{TableProfilesName}_PhoneNumber")
            .OnTable(TableProfilesName)
            .Columns("PhoneNumber");
        Create.UniqueConstraint($"IX_{TableProfilesName}_UserName")
            .OnTable(TableProfilesName)
            .Columns("UserName");
            
        Create.Index($"FK_{TableProfilesName}_{TableUsersName}_Id")
            .OnTable(TableProfilesName).OnColumn("Id")
            .Ascending()
            .WithOptions().NonClustered();
    }
        
    public override void Down()
    {
        Delete.Table(TableUsersName);
        Delete.Table(TableProfilesName);
    }
}
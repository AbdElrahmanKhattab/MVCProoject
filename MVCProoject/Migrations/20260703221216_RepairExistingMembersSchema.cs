using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    /// <inheritdoc />
    public partial class RepairExistingMembersSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Members]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'Members', N'DateOfBirth') IS NULL ALTER TABLE [Members] ADD [DateOfBirth] datetime2 NOT NULL CONSTRAINT [DF_Members_DateOfBirth_Repair] DEFAULT ('2000-01-01');
    IF COL_LENGTH(N'Members', N'Gender') IS NULL ALTER TABLE [Members] ADD [Gender] int NOT NULL CONSTRAINT [DF_Members_Gender_Repair] DEFAULT (1);
    IF COL_LENGTH(N'Members', N'BuildingNumber') IS NULL ALTER TABLE [Members] ADD [BuildingNumber] nvarchar(20) NOT NULL CONSTRAINT [DF_Members_BuildingNumber_Repair] DEFAULT ('N/A');
    IF COL_LENGTH(N'Members', N'Street') IS NULL ALTER TABLE [Members] ADD [Street] nvarchar(100) NOT NULL CONSTRAINT [DF_Members_Street_Repair] DEFAULT ('N/A');
    IF COL_LENGTH(N'Members', N'City') IS NULL ALTER TABLE [Members] ADD [City] nvarchar(60) NOT NULL CONSTRAINT [DF_Members_City_Repair] DEFAULT ('N/A');
    IF COL_LENGTH(N'Members', N'PhotoFileName') IS NULL ALTER TABLE [Members] ADD [PhotoFileName] nvarchar(255) NULL;
    IF COL_LENGTH(N'Members', N'PhotoContentType') IS NULL ALTER TABLE [Members] ADD [PhotoContentType] nvarchar(100) NULL;
    IF COL_LENGTH(N'Members', N'PhotoData') IS NULL ALTER TABLE [Members] ADD [PhotoData] varbinary(max) NULL;
END

IF OBJECT_ID(N'[HealthRecords]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'HealthRecords', N'Note') IS NULL ALTER TABLE [HealthRecords] ADD [Note] nvarchar(500) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}

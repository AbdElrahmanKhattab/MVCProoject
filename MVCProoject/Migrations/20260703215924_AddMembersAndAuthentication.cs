using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddMembersAndAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Members]', N'U') IS NULL
BEGIN
    CREATE TABLE [Members] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        [DateOfBirth] datetime2 NOT NULL,
        [Gender] int NOT NULL,
        [BuildingNumber] nvarchar(20) NOT NULL,
        [Street] nvarchar(100) NOT NULL,
        [City] nvarchar(60) NOT NULL,
        [PhotoFileName] nvarchar(255) NULL,
        [PhotoContentType] nvarchar(100) NULL,
        [PhotoData] varbinary(max) NULL,
        CONSTRAINT [PK_Members] PRIMARY KEY ([Id])
    );
END
ELSE
BEGIN
    IF COL_LENGTH(N'Members', N'DateOfBirth') IS NULL ALTER TABLE [Members] ADD [DateOfBirth] datetime2 NOT NULL CONSTRAINT [DF_Members_DateOfBirth] DEFAULT ('2000-01-01');
    IF COL_LENGTH(N'Members', N'Gender') IS NULL ALTER TABLE [Members] ADD [Gender] int NOT NULL CONSTRAINT [DF_Members_Gender] DEFAULT (1);
    IF COL_LENGTH(N'Members', N'BuildingNumber') IS NULL ALTER TABLE [Members] ADD [BuildingNumber] nvarchar(20) NOT NULL CONSTRAINT [DF_Members_BuildingNumber] DEFAULT ('N/A');
    IF COL_LENGTH(N'Members', N'Street') IS NULL ALTER TABLE [Members] ADD [Street] nvarchar(100) NOT NULL CONSTRAINT [DF_Members_Street] DEFAULT ('N/A');
    IF COL_LENGTH(N'Members', N'City') IS NULL ALTER TABLE [Members] ADD [City] nvarchar(60) NOT NULL CONSTRAINT [DF_Members_City] DEFAULT ('N/A');
    IF COL_LENGTH(N'Members', N'PhotoFileName') IS NULL ALTER TABLE [Members] ADD [PhotoFileName] nvarchar(255) NULL;
    IF COL_LENGTH(N'Members', N'PhotoContentType') IS NULL ALTER TABLE [Members] ADD [PhotoContentType] nvarchar(100) NULL;
    IF COL_LENGTH(N'Members', N'PhotoData') IS NULL ALTER TABLE [Members] ADD [PhotoData] varbinary(max) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Members_Email' AND object_id = OBJECT_ID(N'[Members]'))
BEGIN
    CREATE UNIQUE INDEX [IX_Members_Email] ON [Members] ([Email]);
END

IF OBJECT_ID(N'[HealthRecords]', N'U') IS NULL
BEGIN
    CREATE TABLE [HealthRecords] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [Height] decimal(5,2) NOT NULL,
        [Weight] decimal(5,2) NOT NULL,
        [BloodType] nvarchar(5) NOT NULL,
        [Note] nvarchar(500) NULL,
        CONSTRAINT [PK_HealthRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HealthRecords_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE
    );
END
ELSE
BEGIN
    IF COL_LENGTH(N'HealthRecords', N'Note') IS NULL ALTER TABLE [HealthRecords] ADD [Note] nvarchar(500) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HealthRecords_MemberId' AND object_id = OBJECT_ID(N'[HealthRecords]'))
BEGIN
    CREATE UNIQUE INDEX [IX_HealthRecords_MemberId] ON [HealthRecords] ([MemberId]);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HealthRecords]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [HealthRecords];
END

IF OBJECT_ID(N'[Members]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [Members];
END
");
        }
    }
}

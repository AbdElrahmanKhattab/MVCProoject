using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    public partial class RestoreCourseModules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'Members', N'JoinDate') IS NULL ALTER TABLE [Members] ADD [JoinDate] datetime2 NOT NULL CONSTRAINT [DF_Members_JoinDate_Restore] DEFAULT (GETDATE());

IF OBJECT_ID(N'[GymSessions]', N'U') IS NULL
BEGIN
    CREATE TABLE [GymSessions] (
        [Id] int NOT NULL IDENTITY,
        [CategoryName] varchar(20) NOT NULL,
        [TrainerName] varchar(50) NOT NULL,
        [Description] varchar(200) NOT NULL,
        [Capacity] int NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        CONSTRAINT [PK_GymSessions] PRIMARY KEY ([Id]),
        CONSTRAINT [SessionCapacityCheck] CHECK (Capacity BETWEEN 1 AND 25),
        CONSTRAINT [SessionDateCheck] CHECK (EndDate > StartDate)
    );
END
ELSE
BEGIN
    IF COL_LENGTH(N'GymSessions', N'CategoryName') IS NULL ALTER TABLE [GymSessions] ADD [CategoryName] varchar(20) NOT NULL CONSTRAINT [DF_GymSessions_CategoryName_Restore] DEFAULT ('General');
    IF COL_LENGTH(N'GymSessions', N'TrainerName') IS NULL ALTER TABLE [GymSessions] ADD [TrainerName] varchar(50) NOT NULL CONSTRAINT [DF_GymSessions_TrainerName_Restore] DEFAULT ('Unassigned');
    IF COL_LENGTH(N'GymSessions', N'Description') IS NULL ALTER TABLE [GymSessions] ADD [Description] varchar(200) NOT NULL CONSTRAINT [DF_GymSessions_Description_Restore] DEFAULT ('Training session');
    IF COL_LENGTH(N'GymSessions', N'Capacity') IS NULL ALTER TABLE [GymSessions] ADD [Capacity] int NOT NULL CONSTRAINT [DF_GymSessions_Capacity_Restore] DEFAULT (10);
    IF COL_LENGTH(N'GymSessions', N'StartDate') IS NULL ALTER TABLE [GymSessions] ADD [StartDate] datetime2 NOT NULL CONSTRAINT [DF_GymSessions_StartDate_Restore] DEFAULT (DATEADD(day, 1, GETDATE()));
    IF COL_LENGTH(N'GymSessions', N'EndDate') IS NULL ALTER TABLE [GymSessions] ADD [EndDate] datetime2 NOT NULL CONSTRAINT [DF_GymSessions_EndDate_Restore] DEFAULT (DATEADD(hour, 25, GETDATE()));
END

IF OBJECT_ID(N'[Memberships]', N'U') IS NULL
BEGIN
    CREATE TABLE [Memberships] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [PlanId] int NOT NULL,
        [StartDate] datetime2 NOT NULL CONSTRAINT [DF_Memberships_StartDate] DEFAULT (GETDATE()),
        [EndDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Memberships] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Memberships_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Memberships_Plans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [Plans] ([Id]) ON DELETE NO ACTION
    );
END
ELSE
BEGIN
    IF COL_LENGTH(N'Memberships', N'MemberId') IS NULL ALTER TABLE [Memberships] ADD [MemberId] int NOT NULL CONSTRAINT [DF_Memberships_MemberId_Restore] DEFAULT (0);
    IF COL_LENGTH(N'Memberships', N'PlanId') IS NULL ALTER TABLE [Memberships] ADD [PlanId] int NOT NULL CONSTRAINT [DF_Memberships_PlanId_Restore] DEFAULT (0);
    IF COL_LENGTH(N'Memberships', N'StartDate') IS NULL ALTER TABLE [Memberships] ADD [StartDate] datetime2 NOT NULL CONSTRAINT [DF_Memberships_StartDate_Restore] DEFAULT (GETDATE());
    IF COL_LENGTH(N'Memberships', N'EndDate') IS NULL ALTER TABLE [Memberships] ADD [EndDate] datetime2 NOT NULL CONSTRAINT [DF_Memberships_EndDate_Restore] DEFAULT (DATEADD(day, 30, GETDATE()));
END

IF OBJECT_ID(N'[Trainers]', N'U') IS NULL
BEGIN
    CREATE TABLE [Trainers] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        [Specialties] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Trainers] PRIMARY KEY ([Id])
    );
END
ELSE
BEGIN
    IF COL_LENGTH(N'Trainers', N'Name') IS NULL ALTER TABLE [Trainers] ADD [Name] nvarchar(100) NOT NULL CONSTRAINT [DF_Trainers_Name_Restore] DEFAULT ('Trainer');
    IF COL_LENGTH(N'Trainers', N'Email') IS NULL ALTER TABLE [Trainers] ADD [Email] nvarchar(150) NOT NULL CONSTRAINT [DF_Trainers_Email_Restore] DEFAULT ('trainer@routefitness.local');
    IF COL_LENGTH(N'Trainers', N'Phone') IS NULL ALTER TABLE [Trainers] ADD [Phone] nvarchar(30) NOT NULL CONSTRAINT [DF_Trainers_Phone_Restore] DEFAULT ('N/A');
    IF COL_LENGTH(N'Trainers', N'Specialties') IS NULL ALTER TABLE [Trainers] ADD [Specialties] nvarchar(100) NOT NULL CONSTRAINT [DF_Trainers_Specialties_Restore] DEFAULT ('General');
END

IF OBJECT_ID(N'[Bookings]', N'U') IS NULL
BEGIN
    CREATE TABLE [Bookings] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [GymSessionId] int NOT NULL,
        [BookingDate] datetime2 NOT NULL CONSTRAINT [DF_Bookings_BookingDate] DEFAULT (GETDATE()),
        [IsAttended] bit NOT NULL,
        CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Bookings_GymSessions_GymSessionId] FOREIGN KEY ([GymSessionId]) REFERENCES [GymSessions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Bookings_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION
    );
END
ELSE
BEGIN
    IF COL_LENGTH(N'Bookings', N'MemberId') IS NULL ALTER TABLE [Bookings] ADD [MemberId] int NOT NULL CONSTRAINT [DF_Bookings_MemberId_Restore] DEFAULT (0);
    IF COL_LENGTH(N'Bookings', N'GymSessionId') IS NULL ALTER TABLE [Bookings] ADD [GymSessionId] int NOT NULL CONSTRAINT [DF_Bookings_GymSessionId_Restore] DEFAULT (0);
    IF COL_LENGTH(N'Bookings', N'BookingDate') IS NULL ALTER TABLE [Bookings] ADD [BookingDate] datetime2 NOT NULL CONSTRAINT [DF_Bookings_BookingDate_Restore] DEFAULT (GETDATE());
    IF COL_LENGTH(N'Bookings', N'IsAttended') IS NULL ALTER TABLE [Bookings] ADD [IsAttended] bit NOT NULL CONSTRAINT [DF_Bookings_IsAttended_Restore] DEFAULT (0);
END

IF OBJECT_ID(N'[Memberships]', N'U') IS NOT NULL AND COL_LENGTH(N'Memberships', N'MemberId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Memberships_MemberId' AND object_id = OBJECT_ID(N'[Memberships]')) CREATE INDEX [IX_Memberships_MemberId] ON [Memberships] ([MemberId]);
IF OBJECT_ID(N'[Memberships]', N'U') IS NOT NULL AND COL_LENGTH(N'Memberships', N'PlanId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Memberships_PlanId' AND object_id = OBJECT_ID(N'[Memberships]')) CREATE INDEX [IX_Memberships_PlanId] ON [Memberships] ([PlanId]);
IF OBJECT_ID(N'[Trainers]', N'U') IS NOT NULL AND COL_LENGTH(N'Trainers', N'Email') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Trainers_Email' AND object_id = OBJECT_ID(N'[Trainers]')) CREATE INDEX [IX_Trainers_Email] ON [Trainers] ([Email]);
IF OBJECT_ID(N'[Bookings]', N'U') IS NOT NULL AND COL_LENGTH(N'Bookings', N'GymSessionId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bookings_GymSessionId' AND object_id = OBJECT_ID(N'[Bookings]')) CREATE INDEX [IX_Bookings_GymSessionId] ON [Bookings] ([GymSessionId]);
IF OBJECT_ID(N'[Bookings]', N'U') IS NOT NULL AND COL_LENGTH(N'Bookings', N'MemberId') IS NOT NULL AND COL_LENGTH(N'Bookings', N'GymSessionId') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bookings_MemberId_GymSessionId' AND object_id = OBJECT_ID(N'[Bookings]')) CREATE INDEX [IX_Bookings_MemberId_GymSessionId] ON [Bookings] ([MemberId], [GymSessionId]);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}

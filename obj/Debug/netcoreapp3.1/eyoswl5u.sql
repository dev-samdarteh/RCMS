IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

ALTER TABLE [MemberRegistration] DROP CONSTRAINT [FK_MemberRegistration_ActivityPeriod_ChurchPeriodId];

GO

DROP TABLE [ActivityPeriod];

GO

ALTER TABLE [MemberRegistration] ADD CONSTRAINT [FK_MemberRegistration_ChurchPeriod_ChurchPeriodId] FOREIGN KEY ([ChurchPeriodId]) REFERENCES [ChurchPeriod] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210821073417_ef_c_upd23', N'3.1.7');

GO


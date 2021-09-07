IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210821073417_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberRegistration] DROP CONSTRAINT [FK_MemberRegistration_ActivityPeriod_ChurchPeriodId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210821073417_ef_c_upd23')
BEGIN
    DROP TABLE [ActivityPeriod];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210821073417_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberRegistration] ADD CONSTRAINT [FK_MemberRegistration_ChurchPeriod_ChurchPeriodId] FOREIGN KEY ([ChurchPeriodId]) REFERENCES [ChurchPeriod] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210821073417_ef_c_upd23')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210821073417_ef_c_upd23', N'3.1.7');
END;

GO


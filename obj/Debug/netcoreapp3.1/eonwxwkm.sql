IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserRole]') AND [c].[name] = N'RoleType');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [UserRole] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [UserRole] ALTER COLUMN [RoleType] nvarchar(15) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210811173308_ef_m_upd5', N'3.1.7');

GO

ALTER TABLE [UserProfileRole] DROP CONSTRAINT [FK_UserProfileRole_UserProfile_UserProfileId];

GO

ALTER TABLE [UserProfileRole] DROP CONSTRAINT [FK_UserProfileRole_UserRole_UserRoleId];

GO

ALTER TABLE [UserRolePermission] DROP CONSTRAINT [FK_UserRolePermission_UserPermission_UserPermissionId];

GO

ALTER TABLE [UserRolePermission] DROP CONSTRAINT [FK_UserRolePermission_UserRole_UserRoleId];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserRolePermission]') AND [c].[name] = N'UserRoleId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [UserRolePermission] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [UserRolePermission] ALTER COLUMN [UserRoleId] int NULL;

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserRolePermission]') AND [c].[name] = N'UserPermissionId');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [UserRolePermission] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [UserRolePermission] ALTER COLUMN [UserPermissionId] int NULL;

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserProfileRole]') AND [c].[name] = N'UserRoleId');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [UserProfileRole] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [UserProfileRole] ALTER COLUMN [UserRoleId] int NULL;

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserProfileRole]') AND [c].[name] = N'UserProfileId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [UserProfileRole] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [UserProfileRole] ALTER COLUMN [UserProfileId] int NULL;

GO

ALTER TABLE [UserProfileRole] ADD CONSTRAINT [FK_UserProfileRole_UserProfile_UserProfileId] FOREIGN KEY ([UserProfileId]) REFERENCES [UserProfile] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [UserProfileRole] ADD CONSTRAINT [FK_UserProfileRole_UserRole_UserRoleId] FOREIGN KEY ([UserRoleId]) REFERENCES [UserRole] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [UserRolePermission] ADD CONSTRAINT [FK_UserRolePermission_UserPermission_UserPermissionId] FOREIGN KEY ([UserPermissionId]) REFERENCES [UserPermission] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [UserRolePermission] ADD CONSTRAINT [FK_UserRolePermission_UserRole_UserRoleId] FOREIGN KEY ([UserRoleId]) REFERENCES [UserRole] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210812173356_ef_m_upd6', N'3.1.7');

GO


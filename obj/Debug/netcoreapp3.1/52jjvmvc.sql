DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberType]') AND [c].[name] = N'Comments');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [MemberType] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [MemberType] DROP COLUMN [Comments];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberStatus]') AND [c].[name] = N'AttachedNotes');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [MemberStatus] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [MemberStatus] DROP COLUMN [AttachedNotes];

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRank]') AND [c].[name] = N'Comments');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [MemberRank] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [MemberRank] DROP COLUMN [Comments];

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchUnit]') AND [c].[name] = N'IsCurrentMember');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchUnit] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [MemberChurchUnit] DROP COLUMN [IsCurrentMember];

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchRole]') AND [c].[name] = N'CompletionReason');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchRole] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [MemberChurchRole] DROP COLUMN [CompletionReason];

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlifeEventTask]') AND [c].[name] = N'Status');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlifeEventTask] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [MemberChurchlifeEventTask] DROP COLUMN [Status];

GO

ALTER TABLE [MemberWorkExperience] ADD [CtryAlpha3Code] nvarchar(3) NULL;

GO

ALTER TABLE [MemberWorkExperience] ADD [Location] nvarchar(50) NULL;

GO

ALTER TABLE [MemberType] ADD [Notes] nvarchar(100) NULL;

GO

ALTER TABLE [MemberType] ADD [Reason] nvarchar(30) NULL;

GO

ALTER TABLE [MemberStatus] ADD [Notes] nvarchar(100) NULL;

GO

ALTER TABLE [MemberStatus] ADD [Reason] nvarchar(30) NULL;

GO

ALTER TABLE [MemberRank] ADD [Notes] nvarchar(100) NULL;

GO

ALTER TABLE [MemberRank] ADD [Reason] nvarchar(30) NULL;

GO

ALTER TABLE [MemberChurchUnit] ADD [DepartReason] nvarchar(max) NULL;

GO

ALTER TABLE [MemberChurchRole] ADD [DepartReason] nvarchar(50) NULL;

GO

ALTER TABLE [MemberChurchlifeEventTask] ADD [TaskStatus] nvarchar(1) NULL;

GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'MemberlifeSummary');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [MemberChurchlife] ALTER COLUMN [MemberlifeSummary] nvarchar(500) NULL;

GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'EnrollReason');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [MemberChurchlife] ALTER COLUMN [EnrollReason] nvarchar(50) NULL;

GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'DepartReason');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [MemberChurchlife] ALTER COLUMN [DepartReason] nvarchar(50) NULL;

GO

ALTER TABLE [MemberChurchlife] ADD [NonConfirmReason] nvarchar(100) NULL;

GO

CREATE INDEX [IX_MemberWorkExperience_CtryAlpha3Code] ON [MemberWorkExperience] ([CtryAlpha3Code]);

GO

ALTER TABLE [MemberWorkExperience] ADD CONSTRAINT [FK_MemberWorkExperience_Country_CtryAlpha3Code] FOREIGN KEY ([CtryAlpha3Code]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210609134232_ef_c_upd37', N'3.1.7');

GO

ALTER TABLE [MemberType] DROP CONSTRAINT [FK_MemberType_AppUtilityNVP_ChurchMemTypeId];

GO

DROP INDEX [IX_MemberType_ChurchMemTypeId] ON [MemberType];

GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberType]') AND [c].[name] = N'ChurchMemTypeId');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [MemberType] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [MemberType] DROP COLUMN [ChurchMemTypeId];

GO

ALTER TABLE [MemberType] ADD [MemberTypeCode] nvarchar(1) NULL;

GO

ALTER TABLE [MemberChurchlife] ADD [HealthConditionStatus] nvarchar(1) NULL;

GO

ALTER TABLE [ChurchMember] ADD [StatusReason] nvarchar(100) NULL;

GO

CREATE INDEX [IX_MemberType_OwnedByChurchBodyId] ON [MemberType] ([OwnedByChurchBodyId]);

GO

ALTER TABLE [MemberType] ADD CONSTRAINT [FK_MemberType_ChurchBody_OwnedByChurchBodyId] FOREIGN KEY ([OwnedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210613162611_ef_c_upd38', N'3.1.7');

GO

ALTER TABLE [MemberChurchRole] DROP CONSTRAINT [FK_MemberChurchRole_ChurchBody_ChurchUnitId];

GO

ALTER TABLE [MemberChurchRole] ADD CONSTRAINT [FK_MemberChurchRole_ChurchUnit_ChurchUnitId] FOREIGN KEY ([ChurchUnitId]) REFERENCES [ChurchUnit] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210615044102_ef_c_upd39', N'3.1.7');

GO

ALTER TABLE [MemberChurchlifeEventTask] ADD [IsCurrentTask] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210615155100_ef_c_upd40', N'3.1.7');

GO


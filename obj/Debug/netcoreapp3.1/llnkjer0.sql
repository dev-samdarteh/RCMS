IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621005042_ef_c_upd1')
BEGIN
    ALTER TABLE [MemberRank] ADD CONSTRAINT [FK_MemberRank_ChurchRank_ChurchRankId] FOREIGN KEY ([ChurchRankId]) REFERENCES [ChurchRank] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621005042_ef_c_upd1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210621005042_ef_c_upd1', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621005816_ef_c_upd2')
BEGIN
    ALTER TABLE [MemberRank] ADD CONSTRAINT [FK_MemberRank_AppUtilityNVP_ChurchRankId] FOREIGN KEY ([ChurchRankId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621005816_ef_c_upd2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210621005816_ef_c_upd2', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621011835_ef_c_upd3')
BEGIN
    ALTER TABLE [MemberRank] DROP CONSTRAINT [FK_MemberRank_ChurchRank_ChurchRankId1];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621011835_ef_c_upd3')
BEGIN
    DROP TABLE [ChurchMemStatus];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621011835_ef_c_upd3')
BEGIN
    DROP TABLE [ChurchRank];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621011835_ef_c_upd3')
BEGIN
    DROP INDEX [IX_MemberRank_ChurchRankId1] ON [MemberRank];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621011835_ef_c_upd3')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRank]') AND [c].[name] = N'ChurchRankId1');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [MemberRank] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [MemberRank] DROP COLUMN [ChurchRankId1];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210621011835_ef_c_upd3')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210621011835_ef_c_upd3', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_ChurchPosition_FromChurchPositionId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_ReasonId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DROP TABLE [ChurchPosition];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DROP INDEX [IX_ChurchTransfer_FromChurchPositionId] ON [ChurchTransfer];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DROP INDEX [IX_ChurchTransfer_ReasonId] ON [ChurchTransfer];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'CustomPreambleMsg');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [ChurchTransfer] DROP COLUMN [CustomPreambleMsg];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'CustomReason');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [ChurchTransfer] DROP COLUMN [CustomReason];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'FromChurchPositionId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [ChurchTransfer] DROP COLUMN [FromChurchPositionId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'ReasonId');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [ChurchTransfer] DROP COLUMN [ReasonId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'ReceivedDate');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [ChurchTransfer] DROP COLUMN [ReceivedDate];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberEducation]') AND [c].[name] = N'InstitutionName');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [MemberEducation] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [MemberEducation] ALTER COLUMN [InstitutionName] nvarchar(100) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'TransferType');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [ChurchTransfer] ALTER COLUMN [TransferType] nvarchar(2) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [CustomTransMessage] nvarchar(100) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [FromChurchRankId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [TransferReason] nvarchar(30) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_FromChurchRankId] ON [ChurchTransfer] ([FromChurchRankId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_FromChurchRankId] FOREIGN KEY ([FromChurchRankId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210623101505_ef_c_upd4')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210623101505_ef_c_upd4', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ApprovalAction] DROP CONSTRAINT [FK_ApprovalAction_ChurchBody_ChurchBodyId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [FK_ApprovalActionStep_ChurchBody_ChurchBodyId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_ChurchMember_ChurchMemberId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_ChurchBody_FromChurchBodyId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_ChurchMember_RequestorMemberId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'DepartReason');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [MemberChurchlife] ALTER COLUMN [DepartReason] nvarchar(100) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [MemberChurchlife] ADD [DepartMode] nvarchar(1) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [MemberChurchlife] ADD [EnrollReason] nvarchar(100) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'TransferReason');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [ChurchTransfer] ALTER COLUMN [TransferReason] nvarchar(100) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'RequestorMemberId');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [ChurchTransfer] ALTER COLUMN [RequestorMemberId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'FromChurchBodyId');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [ChurchTransfer] ALTER COLUMN [FromChurchBodyId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'ChurchMemberId');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [ChurchTransfer] ALTER COLUMN [ChurchMemberId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [TempMemRankIdFrCB] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [TempMemRankIdToCB] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [TempMemStatusIdFrCB] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [TempMemStatusIdToCB] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [TempMemTypeCodeFrCB] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [TempMemTypeCodeToCB] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalActionStep]') AND [c].[name] = N'ChurchBodyId');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [ApprovalActionStep] ALTER COLUMN [ChurchBodyId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalAction]') AND [c].[name] = N'ChurchBodyId');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalAction] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [ApprovalAction] ALTER COLUMN [ChurchBodyId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ApprovalAction] ADD CONSTRAINT [FK_ApprovalAction_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ApprovalActionStep] ADD CONSTRAINT [FK_ApprovalActionStep_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_ChurchBody_FromChurchBodyId] FOREIGN KEY ([FromChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_ChurchMember_RequestorMemberId] FOREIGN KEY ([RequestorMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625044839_ef_c_upd5')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210625044839_ef_c_upd5', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625121209_ef_c_upd6')
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcess]') AND [c].[name] = N'ChurchLevelId');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [ApprovalProcess] DROP COLUMN [ChurchLevelId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625121209_ef_c_upd6')
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcess]') AND [c].[name] = N'EscalSLA_MinHrs');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [ApprovalProcess] ALTER COLUMN [EscalSLA_MinHrs] decimal(18, 2) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625121209_ef_c_upd6')
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcess]') AND [c].[name] = N'EscalSLA_MaxHrs');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [ApprovalProcess] ALTER COLUMN [EscalSLA_MaxHrs] decimal(18, 2) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625121209_ef_c_upd6')
BEGIN
    ALTER TABLE [ApprovalProcess] ADD [TargetChurchLevelId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210625121209_ef_c_upd6')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210625121209_ef_c_upd6', N'3.1.7');
END;

GO


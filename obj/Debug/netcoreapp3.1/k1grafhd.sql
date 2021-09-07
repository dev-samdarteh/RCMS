IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

ALTER TABLE [MemberRank] ADD CONSTRAINT [FK_MemberRank_ChurchRank_ChurchRankId] FOREIGN KEY ([ChurchRankId]) REFERENCES [ChurchRank] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210621005042_ef_c_upd1', N'3.1.7');

GO

ALTER TABLE [MemberRank] ADD CONSTRAINT [FK_MemberRank_AppUtilityNVP_ChurchRankId] FOREIGN KEY ([ChurchRankId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210621005816_ef_c_upd2', N'3.1.7');

GO

ALTER TABLE [MemberRank] DROP CONSTRAINT [FK_MemberRank_ChurchRank_ChurchRankId1];

GO

DROP TABLE [ChurchMemStatus];

GO

DROP TABLE [ChurchRank];

GO

DROP INDEX [IX_MemberRank_ChurchRankId1] ON [MemberRank];

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRank]') AND [c].[name] = N'ChurchRankId1');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [MemberRank] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [MemberRank] DROP COLUMN [ChurchRankId1];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210621011835_ef_c_upd3', N'3.1.7');

GO

ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_ChurchPosition_FromChurchPositionId];

GO

ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_ReasonId];

GO

DROP TABLE [ChurchPosition];

GO

DROP INDEX [IX_ChurchTransfer_FromChurchPositionId] ON [ChurchTransfer];

GO

DROP INDEX [IX_ChurchTransfer_ReasonId] ON [ChurchTransfer];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'CustomPreambleMsg');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [CustomPreambleMsg];

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'CustomReason');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [CustomReason];

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'FromChurchPositionId');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [FromChurchPositionId];

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'ReasonId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [ReasonId];

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'ReceivedDate');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [ReceivedDate];

GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberEducation]') AND [c].[name] = N'InstitutionName');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [MemberEducation] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [MemberEducation] ALTER COLUMN [InstitutionName] nvarchar(100) NULL;

GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'TransferType');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [ChurchTransfer] ALTER COLUMN [TransferType] nvarchar(2) NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [CustomTransMessage] nvarchar(100) NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [FromChurchRankId] int NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [TransferReason] nvarchar(30) NULL;

GO

CREATE INDEX [IX_ChurchTransfer_FromChurchRankId] ON [ChurchTransfer] ([FromChurchRankId]);

GO

ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_FromChurchRankId] FOREIGN KEY ([FromChurchRankId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210623101505_ef_c_upd4', N'3.1.7');

GO

ALTER TABLE [ApprovalAction] DROP CONSTRAINT [FK_ApprovalAction_ChurchBody_ChurchBodyId];

GO

ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [FK_ApprovalActionStep_ChurchBody_ChurchBodyId];

GO

ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_ChurchBody_FromChurchBodyId];

GO

ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_ChurchMember_RequestorMemberId];

GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'DepartReason');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [MemberChurchlife] ALTER COLUMN [DepartReason] nvarchar(100) NULL;

GO

ALTER TABLE [MemberChurchlife] ADD [DepartMode] nvarchar(1) NULL;

GO

ALTER TABLE [MemberChurchlife] ADD [EnrollReason] nvarchar(100) NULL;

GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'TransferReason');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [ChurchTransfer] ALTER COLUMN [TransferReason] nvarchar(100) NULL;

GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'RequestorMemberId');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [ChurchTransfer] ALTER COLUMN [RequestorMemberId] int NULL;

GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'FromChurchBodyId');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [ChurchTransfer] ALTER COLUMN [FromChurchBodyId] int NULL;

GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'ChurchMemberId');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [ChurchTransfer] ALTER COLUMN [ChurchMemberId] int NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [TempMemRankIdFrCB] int NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [TempMemRankIdToCB] int NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [TempMemStatusIdFrCB] int NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [TempMemStatusIdToCB] int NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [TempMemTypeCodeFrCB] nvarchar(max) NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [TempMemTypeCodeToCB] nvarchar(max) NULL;

GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalActionStep]') AND [c].[name] = N'ChurchBodyId');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [ApprovalActionStep] ALTER COLUMN [ChurchBodyId] int NULL;

GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalAction]') AND [c].[name] = N'ChurchBodyId');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalAction] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [ApprovalAction] ALTER COLUMN [ChurchBodyId] int NULL;

GO

ALTER TABLE [ApprovalAction] ADD CONSTRAINT [FK_ApprovalAction_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalActionStep] ADD CONSTRAINT [FK_ApprovalActionStep_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_ChurchBody_FromChurchBodyId] FOREIGN KEY ([FromChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_ChurchMember_RequestorMemberId] FOREIGN KEY ([RequestorMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210625044839_ef_c_upd5', N'3.1.7');

GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcess]') AND [c].[name] = N'ChurchLevelId');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [ApprovalProcess] DROP COLUMN [ChurchLevelId];

GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcess]') AND [c].[name] = N'EscalSLA_MinHrs');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [ApprovalProcess] ALTER COLUMN [EscalSLA_MinHrs] decimal(18, 2) NULL;

GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcess]') AND [c].[name] = N'EscalSLA_MaxHrs');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [ApprovalProcess] ALTER COLUMN [EscalSLA_MaxHrs] decimal(18, 2) NULL;

GO

ALTER TABLE [ApprovalProcess] ADD [TargetChurchLevelId] int NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210625121209_ef_c_upd6', N'3.1.7');

GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppUtilityNVP]') AND [c].[name] = N'ApplyToMemberStatus');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [AppUtilityNVP] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [AppUtilityNVP] DROP COLUMN [ApplyToMemberStatus];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210625123432_ef_c_upd7', N'3.1.7');

GO

ALTER TABLE [AppUtilityNVP] ADD [ApplyToMemberStatus] nvarchar(1) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210625125237_ef_c_upd8', N'3.1.7');

GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchMember]') AND [c].[name] = N'MemberClass');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [ChurchMember] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [ChurchMember] DROP COLUMN [MemberClass];

GO

ALTER TABLE [ChurchMember] ADD [MemberScope] nvarchar(1) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210627030047_ef_c_upd9', N'3.1.7');

GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'EnrollMode');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [MemberChurchlife] DROP COLUMN [EnrollMode];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210628120507_ef_c_upd11', N'3.1.7');

GO

ALTER TABLE [MemberChurchlife] ADD [EnrollMode] nvarchar(1) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210628120955_ef_c_upd13', N'3.1.7');

GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'AckStatus');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [AckStatus];

GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'AckStatusComments');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [AckStatusComments];

GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'Status');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [Status];

GO

ALTER TABLE [ChurchTransfer] ADD [ReqStatus] nvarchar(1) NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [StatusComments] nvarchar(100) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210705234618_ef_c_upd14', N'3.1.7');

GO

ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [FK_ApprovalActionStep_MemberChurchRole_ActionByMemberChurchRoleId];

GO

ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [FK_ApprovalActionStep_ApprovalAction_ApprovalActionId];

GO

ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [FK_ApprovalActionStep_MemberChurchRole_MemberChurchRoleId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchRole_ApproverChurchRoleId];

GO

DROP INDEX [IX_ApprovalProcessStep_ApproverChurchRoleId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalActionStep_ActionByMemberChurchRoleId] ON [ApprovalActionStep];

GO

DROP INDEX [IX_ApprovalActionStep_MemberChurchRoleId] ON [ApprovalActionStep];

GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'ApproverChurchRoleId');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [ApproverChurchRoleId];

GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalActionStep]') AND [c].[name] = N'ActionByMemberChurchRoleId');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [ApprovalActionStep] DROP COLUMN [ActionByMemberChurchRoleId];

GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalActionStep]') AND [c].[name] = N'CurrentStep');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [ApprovalActionStep] DROP COLUMN [CurrentStep];

GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalActionStep]') AND [c].[name] = N'MemberChurchRoleId');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [ApprovalActionStep] DROP COLUMN [MemberChurchRoleId];

GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalActionStep]') AND [c].[name] = N'ProcessStepRefId');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [ApprovalActionStep] DROP COLUMN [ProcessStepRefId];

GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'FrAckStatusComments');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [ChurchTransfer] ALTER COLUMN [FrAckStatusComments] nvarchar(100) NULL;

GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'Comments');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [ChurchTransfer] ALTER COLUMN [Comments] nvarchar(200) NULL;

GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'ApprovalStatusComments');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [ChurchTransfer] ALTER COLUMN [ApprovalStatusComments] nvarchar(100) NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [Approver1ChurchBodyId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [Approver1ChurchMemberId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [Approver1ChurchRoleId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [Approver1MemberChurchRoleId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [Approver2ChurchBodyId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [Approver2ChurchMemberId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [Approver2ChurchRoleId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [Approver2MemberChurchRoleId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [EscalChurchBodyId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [EscalChurchMemberId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [EscalChurchRoleId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [EscalMemberChurchRoleId] int NULL;

GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalActionStep]') AND [c].[name] = N'ApprovalActionId');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalActionStep] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [ApprovalActionStep] ALTER COLUMN [ApprovalActionId] int NULL;

GO

ALTER TABLE [ApprovalActionStep] ADD [ApprovalProcessStepRefId] int NULL;

GO

ALTER TABLE [ApprovalActionStep] ADD [ApproverChurchBodyId] int NULL;

GO

ALTER TABLE [ApprovalActionStep] ADD [ApproverChurchMemberId] int NULL;

GO

ALTER TABLE [ApprovalActionStep] ADD [ApproverChurchRoleId] int NULL;

GO

ALTER TABLE [ApprovalActionStep] ADD [ApproverMemberChurchRoleId] int NULL;

GO

ALTER TABLE [ApprovalActionStep] ADD [IsCurrentStep] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

CREATE INDEX [IX_MemberChurchlifeActivity_OfficiatedByChurchBodyId] ON [MemberChurchlifeActivity] ([OfficiatedByChurchBodyId]);

GO

CREATE INDEX [IX_MemberChurchlifeActivity_OfficiatedByChurchMemberId] ON [MemberChurchlifeActivity] ([OfficiatedByChurchMemberId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_Approver1ChurchBodyId] ON [ApprovalProcessStep] ([Approver1ChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_Approver1ChurchMemberId] ON [ApprovalProcessStep] ([Approver1ChurchMemberId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_Approver1ChurchRoleId] ON [ApprovalProcessStep] ([Approver1ChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_Approver1MemberChurchRoleId] ON [ApprovalProcessStep] ([Approver1MemberChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_Approver2ChurchBodyId] ON [ApprovalProcessStep] ([Approver2ChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_Approver2ChurchMemberId] ON [ApprovalProcessStep] ([Approver2ChurchMemberId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_Approver2ChurchRoleId] ON [ApprovalProcessStep] ([Approver2ChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_Approver2MemberChurchRoleId] ON [ApprovalProcessStep] ([Approver2MemberChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_EscalChurchBodyId] ON [ApprovalProcessStep] ([EscalChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_EscalChurchMemberId] ON [ApprovalProcessStep] ([EscalChurchMemberId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_EscalChurchRoleId] ON [ApprovalProcessStep] ([EscalChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_EscalMemberChurchRoleId] ON [ApprovalProcessStep] ([EscalMemberChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalActionStep_ApprovalProcessStepRefId] ON [ApprovalActionStep] ([ApprovalProcessStepRefId]);

GO

CREATE INDEX [IX_ApprovalActionStep_ApproverChurchBodyId] ON [ApprovalActionStep] ([ApproverChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalActionStep_ApproverChurchMemberId] ON [ApprovalActionStep] ([ApproverChurchMemberId]);

GO

CREATE INDEX [IX_ApprovalActionStep_ApproverChurchRoleId] ON [ApprovalActionStep] ([ApproverChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalActionStep_ApproverMemberChurchRoleId] ON [ApprovalActionStep] ([ApproverMemberChurchRoleId]);

GO

ALTER TABLE [ApprovalActionStep] ADD CONSTRAINT [FK_ApprovalActionStep_ApprovalAction_ApprovalActionId] FOREIGN KEY ([ApprovalActionId]) REFERENCES [ApprovalAction] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalActionStep] ADD CONSTRAINT [FK_ApprovalActionStep_ApprovalProcessStep_ApprovalProcessStepRefId] FOREIGN KEY ([ApprovalProcessStepRefId]) REFERENCES [ApprovalProcessStep] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalActionStep] ADD CONSTRAINT [FK_ApprovalActionStep_ChurchBody_ApproverChurchBodyId] FOREIGN KEY ([ApproverChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalActionStep] ADD CONSTRAINT [FK_ApprovalActionStep_ChurchMember_ApproverChurchMemberId] FOREIGN KEY ([ApproverChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalActionStep] ADD CONSTRAINT [FK_ApprovalActionStep_ChurchRole_ApproverChurchRoleId] FOREIGN KEY ([ApproverChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalActionStep] ADD CONSTRAINT [FK_ApprovalActionStep_MemberChurchRole_ApproverMemberChurchRoleId] FOREIGN KEY ([ApproverMemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_Approver1ChurchBodyId] FOREIGN KEY ([Approver1ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchMember_Approver1ChurchMemberId] FOREIGN KEY ([Approver1ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchRole_Approver1ChurchRoleId] FOREIGN KEY ([Approver1ChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_MemberChurchRole_Approver1MemberChurchRoleId] FOREIGN KEY ([Approver1MemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_Approver2ChurchBodyId] FOREIGN KEY ([Approver2ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchMember_Approver2ChurchMemberId] FOREIGN KEY ([Approver2ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchRole_Approver2ChurchRoleId] FOREIGN KEY ([Approver2ChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_MemberChurchRole_Approver2MemberChurchRoleId] FOREIGN KEY ([Approver2MemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_EscalChurchBodyId] FOREIGN KEY ([EscalChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchMember_EscalChurchMemberId] FOREIGN KEY ([EscalChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchRole_EscalChurchRoleId] FOREIGN KEY ([EscalChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_MemberChurchRole_EscalMemberChurchRoleId] FOREIGN KEY ([EscalMemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberChurchlifeActivity] ADD CONSTRAINT [FK_MemberChurchlifeActivity_ChurchBody_OfficiatedByChurchBodyId] FOREIGN KEY ([OfficiatedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberChurchlifeActivity] ADD CONSTRAINT [FK_MemberChurchlifeActivity_ChurchMember_OfficiatedByChurchMemberId] FOREIGN KEY ([OfficiatedByChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210708151719_ef_c_upd15', N'3.1.7');

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_Approver1ChurchBodyId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchMember_Approver1ChurchMemberId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchRole_Approver1ChurchRoleId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_MemberChurchRole_Approver1MemberChurchRoleId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_Approver2ChurchBodyId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchMember_Approver2ChurchMemberId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchRole_Approver2ChurchRoleId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_MemberChurchRole_Approver2MemberChurchRoleId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_EscalChurchBodyId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchMember_EscalChurchMemberId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchRole_EscalChurchRoleId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_MemberChurchRole_EscalMemberChurchRoleId];

GO

DROP INDEX [IX_ApprovalProcessStep_Approver1ChurchBodyId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_Approver1ChurchMemberId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_Approver1ChurchRoleId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_Approver1MemberChurchRoleId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_Approver2ChurchBodyId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_Approver2ChurchMemberId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_Approver2ChurchRoleId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_Approver2MemberChurchRoleId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_EscalChurchBodyId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_EscalChurchMemberId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_EscalChurchRoleId] ON [ApprovalProcessStep];

GO

DROP INDEX [IX_ApprovalProcessStep_EscalMemberChurchRoleId] ON [ApprovalProcessStep];

GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'Approver1ChurchBodyId');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [Approver1ChurchBodyId];

GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'Approver1ChurchMemberId');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [Approver1ChurchMemberId];

GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'Approver1ChurchRoleId');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [Approver1ChurchRoleId];

GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'Approver1MemberChurchRoleId');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [Approver1MemberChurchRoleId];

GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'Approver2ChurchBodyId');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [Approver2ChurchBodyId];

GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'Approver2ChurchMemberId');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [Approver2ChurchMemberId];

GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'Approver2ChurchRoleId');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [Approver2ChurchRoleId];

GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'Approver2MemberChurchRoleId');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var40 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [Approver2MemberChurchRoleId];

GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'EscalChurchBodyId');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var41 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [EscalChurchBodyId];

GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'EscalChurchMemberId');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [EscalChurchMemberId];

GO

DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'EscalChurchRoleId');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var43 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [EscalChurchRoleId];

GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'EscalMemberChurchRoleId');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [ApprovalProcessStep] DROP COLUMN [EscalMemberChurchRoleId];

GO

CREATE TABLE [ProcessStepApprover] (
    [Id] int NOT NULL IDENTITY,
    [AppGlobalOwnerId] int NULL,
    [OwnedByChurchBodyId] int NULL,
    [ChurchBodyId] int NULL,
    [ApprovalProcessStepId] int NULL,
    [Approver1ChurchBodyId] int NULL,
    [Approver1ChurchMemberId] int NULL,
    [Approver1ChurchRoleId] int NULL,
    [Approver1MemberChurchRoleId] int NULL,
    [Approver2ChurchBodyId] int NULL,
    [Approver2ChurchMemberId] int NULL,
    [Approver2ChurchRoleId] int NULL,
    [Approver2MemberChurchRoleId] int NULL,
    [EscalChurchBodyId] int NULL,
    [EscalChurchMemberId] int NULL,
    [EscalChurchRoleId] int NULL,
    [EscalMemberChurchRoleId] int NULL,
    [SharingStatus] nvarchar(1) NULL,
    [Created] datetime2 NULL,
    [LastMod] datetime2 NULL,
    [CreatedByUserId] int NULL,
    [LastModByUserId] int NULL,
    CONSTRAINT [PK_ProcessStepApprover] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProcessStepApprover_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ApprovalProcessStep_ApprovalProcessStepId] FOREIGN KEY ([ApprovalProcessStepId]) REFERENCES [ApprovalProcessStep] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchBody_Approver1ChurchBodyId] FOREIGN KEY ([Approver1ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchMember_Approver1ChurchMemberId] FOREIGN KEY ([Approver1ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchRole_Approver1ChurchRoleId] FOREIGN KEY ([Approver1ChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_MemberChurchRole_Approver1MemberChurchRoleId] FOREIGN KEY ([Approver1MemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchBody_Approver2ChurchBodyId] FOREIGN KEY ([Approver2ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchMember_Approver2ChurchMemberId] FOREIGN KEY ([Approver2ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchRole_Approver2ChurchRoleId] FOREIGN KEY ([Approver2ChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_MemberChurchRole_Approver2MemberChurchRoleId] FOREIGN KEY ([Approver2MemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchBody_EscalChurchBodyId] FOREIGN KEY ([EscalChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchMember_EscalChurchMemberId] FOREIGN KEY ([EscalChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchRole_EscalChurchRoleId] FOREIGN KEY ([EscalChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_MemberChurchRole_EscalMemberChurchRoleId] FOREIGN KEY ([EscalMemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcessStepApprover_ChurchBody_OwnedByChurchBodyId] FOREIGN KEY ([OwnedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_ProcessStepApprover_AppGlobalOwnerId] ON [ProcessStepApprover] ([AppGlobalOwnerId]);

GO

CREATE INDEX [IX_ProcessStepApprover_ApprovalProcessStepId] ON [ProcessStepApprover] ([ApprovalProcessStepId]);

GO

CREATE INDEX [IX_ProcessStepApprover_Approver1ChurchBodyId] ON [ProcessStepApprover] ([Approver1ChurchBodyId]);

GO

CREATE INDEX [IX_ProcessStepApprover_Approver1ChurchMemberId] ON [ProcessStepApprover] ([Approver1ChurchMemberId]);

GO

CREATE INDEX [IX_ProcessStepApprover_Approver1ChurchRoleId] ON [ProcessStepApprover] ([Approver1ChurchRoleId]);

GO

CREATE INDEX [IX_ProcessStepApprover_Approver1MemberChurchRoleId] ON [ProcessStepApprover] ([Approver1MemberChurchRoleId]);

GO

CREATE INDEX [IX_ProcessStepApprover_Approver2ChurchBodyId] ON [ProcessStepApprover] ([Approver2ChurchBodyId]);

GO

CREATE INDEX [IX_ProcessStepApprover_Approver2ChurchMemberId] ON [ProcessStepApprover] ([Approver2ChurchMemberId]);

GO

CREATE INDEX [IX_ProcessStepApprover_Approver2ChurchRoleId] ON [ProcessStepApprover] ([Approver2ChurchRoleId]);

GO

CREATE INDEX [IX_ProcessStepApprover_Approver2MemberChurchRoleId] ON [ProcessStepApprover] ([Approver2MemberChurchRoleId]);

GO

CREATE INDEX [IX_ProcessStepApprover_ChurchBodyId] ON [ProcessStepApprover] ([ChurchBodyId]);

GO

CREATE INDEX [IX_ProcessStepApprover_EscalChurchBodyId] ON [ProcessStepApprover] ([EscalChurchBodyId]);

GO

CREATE INDEX [IX_ProcessStepApprover_EscalChurchMemberId] ON [ProcessStepApprover] ([EscalChurchMemberId]);

GO

CREATE INDEX [IX_ProcessStepApprover_EscalChurchRoleId] ON [ProcessStepApprover] ([EscalChurchRoleId]);

GO

CREATE INDEX [IX_ProcessStepApprover_EscalMemberChurchRoleId] ON [ProcessStepApprover] ([EscalMemberChurchRoleId]);

GO

CREATE INDEX [IX_ProcessStepApprover_OwnedByChurchBodyId] ON [ProcessStepApprover] ([OwnedByChurchBodyId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210709094310_ef_c_upd16', N'3.1.7');

GO

ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [FK_ApprovalProcess_ChurchBody_ChurchBodyId];

GO

DROP TABLE [ProcessStepApprover];

GO

DROP INDEX [IX_ApprovalProcess_ChurchBodyId] ON [ApprovalProcess];

GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcess]') AND [c].[name] = N'ChurchBodyId');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [' + @var45 + '];');
ALTER TABLE [ApprovalProcess] DROP COLUMN [ChurchBodyId];

GO

CREATE TABLE [ApprovalProcessApprover] (
    [Id] int NOT NULL IDENTITY,
    [AppGlobalOwnerId] int NULL,
    [OwnedByChurchBodyId] int NULL,
    [ChurchBodyId] int NULL,
    [ApprovalProcessStepId] int NULL,
    [Approver1ChurchBodyId] int NULL,
    [Approver1ChurchMemberId] int NULL,
    [Approver1ChurchRoleId] int NULL,
    [Approver1MemberChurchRoleId] int NULL,
    [Approver2ChurchBodyId] int NULL,
    [Approver2ChurchMemberId] int NULL,
    [Approver2ChurchRoleId] int NULL,
    [Approver2MemberChurchRoleId] int NULL,
    [EscalChurchBodyId] int NULL,
    [EscalChurchMemberId] int NULL,
    [EscalChurchRoleId] int NULL,
    [EscalMemberChurchRoleId] int NULL,
    [SharingStatus] nvarchar(1) NULL,
    [Created] datetime2 NULL,
    [LastMod] datetime2 NULL,
    [CreatedByUserId] int NULL,
    [LastModByUserId] int NULL,
    CONSTRAINT [PK_ApprovalProcessApprover] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ApprovalProcessApprover_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ApprovalProcessStep_ApprovalProcessStepId] FOREIGN KEY ([ApprovalProcessStepId]) REFERENCES [ApprovalProcessStep] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchBody_Approver1ChurchBodyId] FOREIGN KEY ([Approver1ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchMember_Approver1ChurchMemberId] FOREIGN KEY ([Approver1ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchRole_Approver1ChurchRoleId] FOREIGN KEY ([Approver1ChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_MemberChurchRole_Approver1MemberChurchRoleId] FOREIGN KEY ([Approver1MemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchBody_Approver2ChurchBodyId] FOREIGN KEY ([Approver2ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchMember_Approver2ChurchMemberId] FOREIGN KEY ([Approver2ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchRole_Approver2ChurchRoleId] FOREIGN KEY ([Approver2ChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_MemberChurchRole_Approver2MemberChurchRoleId] FOREIGN KEY ([Approver2MemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchBody_EscalChurchBodyId] FOREIGN KEY ([EscalChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchMember_EscalChurchMemberId] FOREIGN KEY ([EscalChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchRole_EscalChurchRoleId] FOREIGN KEY ([EscalChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_MemberChurchRole_EscalMemberChurchRoleId] FOREIGN KEY ([EscalMemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalProcessApprover_ChurchBody_OwnedByChurchBodyId] FOREIGN KEY ([OwnedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_ApprovalProcessApprover_AppGlobalOwnerId] ON [ApprovalProcessApprover] ([AppGlobalOwnerId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_ApprovalProcessStepId] ON [ApprovalProcessApprover] ([ApprovalProcessStepId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_Approver1ChurchBodyId] ON [ApprovalProcessApprover] ([Approver1ChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_Approver1ChurchMemberId] ON [ApprovalProcessApprover] ([Approver1ChurchMemberId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_Approver1ChurchRoleId] ON [ApprovalProcessApprover] ([Approver1ChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_Approver1MemberChurchRoleId] ON [ApprovalProcessApprover] ([Approver1MemberChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_Approver2ChurchBodyId] ON [ApprovalProcessApprover] ([Approver2ChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_Approver2ChurchMemberId] ON [ApprovalProcessApprover] ([Approver2ChurchMemberId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_Approver2ChurchRoleId] ON [ApprovalProcessApprover] ([Approver2ChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_Approver2MemberChurchRoleId] ON [ApprovalProcessApprover] ([Approver2MemberChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_ChurchBodyId] ON [ApprovalProcessApprover] ([ChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_EscalChurchBodyId] ON [ApprovalProcessApprover] ([EscalChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_EscalChurchMemberId] ON [ApprovalProcessApprover] ([EscalChurchMemberId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_EscalChurchRoleId] ON [ApprovalProcessApprover] ([EscalChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_EscalMemberChurchRoleId] ON [ApprovalProcessApprover] ([EscalMemberChurchRoleId]);

GO

CREATE INDEX [IX_ApprovalProcessApprover_OwnedByChurchBodyId] ON [ApprovalProcessApprover] ([OwnedByChurchBodyId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210715033626_ef_c_upd20', N'3.1.7');

GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'MembershipStatus');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var46 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [MembershipStatus];

GO

ALTER TABLE [ChurchTransfer] ADD [StatusModDate] datetime2 NULL;

GO

CREATE INDEX [IX_ChurchTransfer_TempMemRankIdFrCB] ON [ChurchTransfer] ([TempMemRankIdFrCB]);

GO

CREATE INDEX [IX_ChurchTransfer_TempMemRankIdToCB] ON [ChurchTransfer] ([TempMemRankIdToCB]);

GO

CREATE INDEX [IX_ChurchTransfer_TempMemStatusIdFrCB] ON [ChurchTransfer] ([TempMemStatusIdFrCB]);

GO

CREATE INDEX [IX_ChurchTransfer_TempMemStatusIdToCB] ON [ChurchTransfer] ([TempMemStatusIdToCB]);

GO

ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_TempMemRankIdFrCB] FOREIGN KEY ([TempMemRankIdFrCB]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_TempMemRankIdToCB] FOREIGN KEY ([TempMemRankIdToCB]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_TempMemStatusIdFrCB] FOREIGN KEY ([TempMemStatusIdFrCB]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_TempMemStatusIdToCB] FOREIGN KEY ([TempMemStatusIdToCB]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210723021642_ef_c_upd17', N'3.1.7');

GO

DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'StatusComments');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var47 + '];');
ALTER TABLE [ChurchTransfer] DROP COLUMN [StatusComments];

GO

ALTER TABLE [ChurchTransfer] ADD [ReqStatusComments] nvarchar(100) NULL;

GO

ALTER TABLE [ChurchTransfer] ADD [WorkSpanStatus] nvarchar(1) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210726121506_ef_c_upd21', N'3.1.7');

GO

ALTER TABLE [ContactInfo] ADD [ChurchUnitId] int NULL;

GO

ALTER TABLE [ContactInfo] ADD [IsChurchBody] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [ContactInfo] ADD [IsChurchUnit] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210727110757_ef_c_upd22', N'3.1.7');

GO


IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210322134013_ef_c_upd16')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210322134013_ef_c_upd16', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchMember]') AND [c].[name] = N'CurrMemberTypeId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ChurchMember] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [ChurchMember] DROP COLUMN [CurrMemberTypeId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchMember]') AND [c].[name] = N'MemberCodeCustom');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ChurchMember] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [ChurchMember] DROP COLUMN [MemberCodeCustom];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE TABLE [ApprovalProcess] (
        [Id] int NOT NULL IDENTITY,
        [ChurchBodyId] int NOT NULL,
        [ProcessName] nvarchar(50) NOT NULL,
        [ProcessDesc] nvarchar(100) NULL,
        [ApprovalLevels] int NOT NULL,
        [OwnedByChurchBodyId] int NULL,
        [ProcessStatus] nvarchar(1) NULL,
        [ProcessCode] nvarchar(10) NULL,
        [ProcessSubCode] nvarchar(2) NULL,
        [EscalLeaderRoleId] int NULL,
        [RemindFreqHours] nvarchar(max) NULL,
        [EscalSLA_MaxHrs] decimal(18, 2) NOT NULL,
        [EscalSLA_MinHrs] decimal(18, 2) NOT NULL,
        [ChurchLevelId] int NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_ApprovalProcess] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ApprovalProcess_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ApprovalProcess_LeaderRole_EscalLeaderRoleId] FOREIGN KEY ([EscalLeaderRoleId]) REFERENCES [LeaderRole] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ApprovalProcess_ChurchBody_OwnedByChurchBodyId] FOREIGN KEY ([OwnedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE TABLE [ChurchAttend_Attendees] (
        [Id] int NOT NULL IDENTITY,
        [AppGlobalOwnerId] int NULL,
        [ChurchBodyId] int NULL,
        [DateAttended] datetime2 NULL,
        [ChurchEventId] int NULL,
        [AttendEventDesc] nvarchar(100) NULL,
        [AttendeeType] nvarchar(1) NULL,
        [ChurchMemberId] int NULL,
        [Title] nvarchar(15) NULL,
        [FirstName] nvarchar(30) NULL,
        [MiddleName] nvarchar(30) NULL,
        [LastName] nvarchar(50) NULL,
        [Gender] nvarchar(1) NULL,
        [AgeBracketId] int NULL,
        [NationalityId] nvarchar(3) NULL,
        [ResidenceLoc] nvarchar(50) NULL,
        [MobilePhone] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [VisitReasonId] int NULL,
        [VisitReasonOther] nvarchar(100) NULL,
        [TempRec] decimal(18, 2) NULL,
        [Notes] nvarchar(200) NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_ChurchAttend_Attendees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChurchAttend_Attendees_AppUtilityNVP_AgeBracketId] FOREIGN KEY ([AgeBracketId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchAttend_Attendees_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchAttend_Attendees_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchAttend_Attendees_ChurchCalendarEvent_ChurchEventId] FOREIGN KEY ([ChurchEventId]) REFERENCES [ChurchCalendarEvent] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchAttend_Attendees_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchAttend_Attendees_Country_NationalityId] FOREIGN KEY ([NationalityId]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchAttend_Attendees_AppUtilityNVP_VisitReasonId] FOREIGN KEY ([VisitReasonId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE TABLE [ChurchAttendance] (
        [Id] int NOT NULL IDENTITY,
        [ChurchBodyId] int NOT NULL,
        [DateRecorded] datetime2 NULL,
        [MTotal] int NOT NULL,
        [FTotal] int NOT NULL,
        [ChurchEventId] int NULL,
        [Occasion] nvarchar(100) NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_ChurchAttendance] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChurchAttendance_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ChurchAttendance_ChurchCalendarEvent_ChurchEventId] FOREIGN KEY ([ChurchEventId]) REFERENCES [ChurchCalendarEvent] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE TABLE [ChurchPosition] (
        [Id] int NOT NULL IDENTITY,
        [ChurchBodyId] int NOT NULL,
        [PositionName] nvarchar(50) NOT NULL,
        [Description] nvarchar(100) NULL,
        [GradeLevel] int NOT NULL,
        [OwnedByChurchBodyId] int NULL,
        [SharingStatus] nvarchar(1) NULL,
        [ApplyToClergyOnly] bit NOT NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_ChurchPosition] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChurchPosition_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ChurchPosition_ChurchBody_OwnedByChurchBodyId] FOREIGN KEY ([OwnedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE TABLE [ApprovalAction] (
        [Id] int NOT NULL IDENTITY,
        [ChurchBodyId] int NOT NULL,
        [ApprovalActionDesc] nvarchar(100) NULL,
        [ActionStatus] nvarchar(1) NULL,
        [Comments] nvarchar(100) NULL,
        [ApprovalProcessId] int NULL,
        [Status] nvarchar(1) NULL,
        [ProcessCode] nvarchar(6) NULL,
        [ProcessSubCode] nvarchar(2) NULL,
        [CallerRefId] int NULL,
        [LastActionDate] datetime2 NULL,
        [ActionRequestDate] datetime2 NULL,
        [CurrentScope] nvarchar(1) NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_ApprovalAction] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ApprovalAction_ApprovalProcess_ApprovalProcessId] FOREIGN KEY ([ApprovalProcessId]) REFERENCES [ApprovalProcess] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ApprovalAction_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE TABLE [ApprovalProcessStep] (
        [Id] int NOT NULL IDENTITY,
        [ApprovalProcessId] int NOT NULL,
        [StepIndex] int NOT NULL,
        [ProcessStepName] nvarchar(7) NULL,
        [StepDesc] nvarchar(100) NULL,
        [ApproverLeaderRoleId] int NULL,
        [IsConcurrentWithOther] bit NOT NULL,
        [ConcurrProcessStepId] int NULL,
        [StepStatus] nvarchar(1) NULL,
        [ChurchBodyId] int NOT NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_ApprovalProcessStep] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ApprovalProcessStep_ApprovalProcess_ApprovalProcessId] FOREIGN KEY ([ApprovalProcessId]) REFERENCES [ApprovalProcess] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ApprovalProcessStep_LeaderRole_ApproverLeaderRoleId] FOREIGN KEY ([ApproverLeaderRoleId]) REFERENCES [LeaderRole] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ApprovalProcessStep_ApprovalProcessStep_ConcurrProcessStepId] FOREIGN KEY ([ConcurrProcessStepId]) REFERENCES [ApprovalProcessStep] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE TABLE [ApprovalActionStep] (
        [Id] int NOT NULL IDENTITY,
        [ApprovalActionId] int NOT NULL,
        [MemberChurchRoleId] int NULL,
        [ActionStepDesc] nvarchar(100) NULL,
        [ApprovalStepIndex] int NOT NULL,
        [ActionStepStatus] nvarchar(1) NULL,
        [Comments] nvarchar(100) NULL,
        [CurrentStep] bit NOT NULL,
        [ProcessStepRefId] int NULL,
        [ChurchBodyId] int NOT NULL,
        [Status] nvarchar(1) NULL,
        [ActionByMemberChurchRoleId] int NULL,
        [ActionDate] datetime2 NULL,
        [StepRequestDate] datetime2 NULL,
        [CurrentScope] nvarchar(1) NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_ApprovalActionStep] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ApprovalActionStep_MemberChurchRole_ActionByMemberChurchRoleId] FOREIGN KEY ([ActionByMemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ApprovalActionStep_ApprovalAction_ApprovalActionId] FOREIGN KEY ([ApprovalActionId]) REFERENCES [ApprovalAction] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ApprovalActionStep_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ApprovalActionStep_MemberChurchRole_MemberChurchRoleId] FOREIGN KEY ([MemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE TABLE [ChurchTransfer] (
        [Id] int NOT NULL IDENTITY,
        [AppGlobalOwnerId] int NULL,
        [ChurchBodyId] int NULL,
        [IApprovalActionId] int NULL,
        [ChurchMemberId] int NOT NULL,
        [FromChurchBodyId] int NOT NULL,
        [FromChurchPositionId] int NULL,
        [FromMemberChurchRoleId] int NULL,
        [RequestorMemberId] int NOT NULL,
        [RequestorRoleId] int NULL,
        [ToChurchBodyId] int NULL,
        [ToLeaderRoleId] int NULL,
        [AckStatus] nvarchar(1) NULL,
        [RequestDate] datetime2 NULL,
        [TransferType] nvarchar(2) NOT NULL,
        [Comments] nvarchar(max) NULL,
        [ApprovalStatus] nvarchar(1) NULL,
        [RequireApproval] bit NOT NULL,
        [TransferDate] datetime2 NULL,
        [ReasonId] int NULL,
        [TransMessageId] int NULL,
        [RequestorChurchBodyId] int NULL,
        [ToRoleUnitId] int NULL,
        [AckStatusComments] nvarchar(max) NULL,
        [ApprovalStatusComments] nvarchar(max) NULL,
        [CurrentScope] nvarchar(1) NULL,
        [ToRequestDate] datetime2 NULL,
        [CustomPreambleMsg] nvarchar(100) NULL,
        [CustomReason] nvarchar(50) NULL,
        [EApprovalActionId] int NULL,
        [ReceivedDate] datetime2 NULL,
        [ToReceivedDate] datetime2 NULL,
        [Status] nvarchar(max) NULL,
        [AttachedToChurchBodyId] int NULL,
        [AttachedToChurchBodyList] nvarchar(max) NULL,
        [MembershipStatus] nvarchar(1) NULL,
        [FrAckStatus] nvarchar(1) NULL,
        [FrAckStatusComments] nvarchar(max) NULL,
        [FrReceivedDate] datetime2 NULL,
        [TransferSubType] nvarchar(3) NULL,
        [DesigRolesList] nvarchar(max) NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_ChurchTransfer] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChurchTransfer_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_ChurchBody_AttachedToChurchBodyId] FOREIGN KEY ([AttachedToChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ChurchTransfer_ApprovalAction_EApprovalActionId] FOREIGN KEY ([EApprovalActionId]) REFERENCES [ApprovalAction] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_ChurchBody_FromChurchBodyId] FOREIGN KEY ([FromChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ChurchTransfer_ChurchPosition_FromChurchPositionId] FOREIGN KEY ([FromChurchPositionId]) REFERENCES [ChurchPosition] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_MemberChurchRole_FromMemberChurchRoleId] FOREIGN KEY ([FromMemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_ApprovalAction_IApprovalActionId] FOREIGN KEY ([IApprovalActionId]) REFERENCES [ApprovalAction] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_ReasonId] FOREIGN KEY ([ReasonId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_ChurchBody_RequestorChurchBodyId] FOREIGN KEY ([RequestorChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_ChurchMember_RequestorMemberId] FOREIGN KEY ([RequestorMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_MemberChurchRole_RequestorRoleId] FOREIGN KEY ([RequestorRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_ChurchBody_ToChurchBodyId] FOREIGN KEY ([ToChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_LeaderRole_ToLeaderRoleId] FOREIGN KEY ([ToLeaderRoleId]) REFERENCES [LeaderRole] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_ChurchUnit_ToRoleUnitId] FOREIGN KEY ([ToRoleUnitId]) REFERENCES [ChurchUnit] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChurchTransfer_AppUtilityNVP_TransMessageId] FOREIGN KEY ([TransMessageId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalAction_ApprovalProcessId] ON [ApprovalAction] ([ApprovalProcessId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalAction_ChurchBodyId] ON [ApprovalAction] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalActionStep_ActionByMemberChurchRoleId] ON [ApprovalActionStep] ([ActionByMemberChurchRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalActionStep_ApprovalActionId] ON [ApprovalActionStep] ([ApprovalActionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalActionStep_ChurchBodyId] ON [ApprovalActionStep] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalActionStep_MemberChurchRoleId] ON [ApprovalActionStep] ([MemberChurchRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalProcess_ChurchBodyId] ON [ApprovalProcess] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalProcess_EscalLeaderRoleId] ON [ApprovalProcess] ([EscalLeaderRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalProcess_OwnedByChurchBodyId] ON [ApprovalProcess] ([OwnedByChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalProcessStep_ApprovalProcessId] ON [ApprovalProcessStep] ([ApprovalProcessId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalProcessStep_ApproverLeaderRoleId] ON [ApprovalProcessStep] ([ApproverLeaderRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalProcessStep_ChurchBodyId] ON [ApprovalProcessStep] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ApprovalProcessStep_ConcurrProcessStepId] ON [ApprovalProcessStep] ([ConcurrProcessStepId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchAttend_Attendees_AgeBracketId] ON [ChurchAttend_Attendees] ([AgeBracketId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchAttend_Attendees_AppGlobalOwnerId] ON [ChurchAttend_Attendees] ([AppGlobalOwnerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchAttend_Attendees_ChurchBodyId] ON [ChurchAttend_Attendees] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchAttend_Attendees_ChurchEventId] ON [ChurchAttend_Attendees] ([ChurchEventId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchAttend_Attendees_ChurchMemberId] ON [ChurchAttend_Attendees] ([ChurchMemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchAttend_Attendees_NationalityId] ON [ChurchAttend_Attendees] ([NationalityId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchAttend_Attendees_VisitReasonId] ON [ChurchAttend_Attendees] ([VisitReasonId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchAttendance_ChurchBodyId] ON [ChurchAttendance] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchAttendance_ChurchEventId] ON [ChurchAttendance] ([ChurchEventId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchPosition_ChurchBodyId] ON [ChurchPosition] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchPosition_OwnedByChurchBodyId] ON [ChurchPosition] ([OwnedByChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_AppGlobalOwnerId] ON [ChurchTransfer] ([AppGlobalOwnerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_AttachedToChurchBodyId] ON [ChurchTransfer] ([AttachedToChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_ChurchBodyId] ON [ChurchTransfer] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_ChurchMemberId] ON [ChurchTransfer] ([ChurchMemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_EApprovalActionId] ON [ChurchTransfer] ([EApprovalActionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_FromChurchBodyId] ON [ChurchTransfer] ([FromChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_FromChurchPositionId] ON [ChurchTransfer] ([FromChurchPositionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_FromMemberChurchRoleId] ON [ChurchTransfer] ([FromMemberChurchRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_IApprovalActionId] ON [ChurchTransfer] ([IApprovalActionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_ReasonId] ON [ChurchTransfer] ([ReasonId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_RequestorChurchBodyId] ON [ChurchTransfer] ([RequestorChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_RequestorMemberId] ON [ChurchTransfer] ([RequestorMemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_RequestorRoleId] ON [ChurchTransfer] ([RequestorRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_ToChurchBodyId] ON [ChurchTransfer] ([ToChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_ToLeaderRoleId] ON [ChurchTransfer] ([ToLeaderRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_ToRoleUnitId] ON [ChurchTransfer] ([ToRoleUnitId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_TransMessageId] ON [ChurchTransfer] ([TransMessageId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210329130000_ef_c_upd17')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210329130000_ef_c_upd17', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210413105822_ef_c_upd18')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchMember]') AND [c].[name] = N'IsActivated');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ChurchMember] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [ChurchMember] DROP COLUMN [IsActivated];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210413105822_ef_c_upd18')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210413105822_ef_c_upd18', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210414102924_ef_c_upd19')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchMember]') AND [c].[name] = N'MemberGlobalId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ChurchMember] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [ChurchMember] DROP COLUMN [MemberGlobalId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210414102924_ef_c_upd19')
BEGIN
    CREATE INDEX [IX_ChurchBody_ParentChurchBodyId] ON [ChurchBody] ([ParentChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210414102924_ef_c_upd19')
BEGIN
    ALTER TABLE [ChurchBody] ADD CONSTRAINT [FK_ChurchBody_ChurchBody_ParentChurchBodyId] FOREIGN KEY ([ParentChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210414102924_ef_c_upd19')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210414102924_ef_c_upd19', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210414201237_ef_c_upd20')
BEGIN
    ALTER TABLE [ChurchMember] DROP CONSTRAINT [FK_ChurchMember_National_IdType_IdTypeId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210414201237_ef_c_upd20')
BEGIN
    ALTER TABLE [ChurchMember] DROP CONSTRAINT [FK_ChurchMember_LanguageSpoken_MotherTongueId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210414201237_ef_c_upd20')
BEGIN
    ALTER TABLE [ChurchMember] ADD CONSTRAINT [FK_ChurchMember_AppUtilityNVP_IdTypeId] FOREIGN KEY ([IdTypeId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210414201237_ef_c_upd20')
BEGIN
    ALTER TABLE [ChurchMember] ADD CONSTRAINT [FK_ChurchMember_AppUtilityNVP_MotherTongueId] FOREIGN KEY ([MotherTongueId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210414201237_ef_c_upd20')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210414201237_ef_c_upd20', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchMember]') AND [c].[name] = N'ContactInfoId');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [ChurchMember] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [ChurchMember] DROP COLUMN [ContactInfoId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    ALTER TABLE [ContactInfo] ADD [IsPrimaryContact] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    ALTER TABLE [ChurchMember] ADD [PrimContactInfoId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    CREATE INDEX [IX_ContactInfo_AppGlobalOwnerId] ON [ContactInfo] ([AppGlobalOwnerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    CREATE INDEX [IX_ContactInfo_ChurchBodyId] ON [ContactInfo] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    CREATE INDEX [IX_ChurchMember_PrimContactInfoId] ON [ChurchMember] ([PrimContactInfoId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    ALTER TABLE [ChurchMember] ADD CONSTRAINT [FK_ChurchMember_ContactInfo_PrimContactInfoId] FOREIGN KEY ([PrimContactInfoId]) REFERENCES [ContactInfo] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    ALTER TABLE [ContactInfo] ADD CONSTRAINT [FK_ContactInfo_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    ALTER TABLE [ContactInfo] ADD CONSTRAINT [FK_ContactInfo_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210415114052_ef_c_upd21')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210415114052_ef_c_upd21', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberChurchlifeActivity] DROP CONSTRAINT [FK_MemberChurchlifeActivity_ChurchlifeActivity_ChurchlifeActivityId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberContact] DROP CONSTRAINT [FK_MemberContact_ChurchMember_InternalContactId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberLanguageSpoken] DROP CONSTRAINT [FK_MemberLanguageSpoken_LanguageSpoken_LanguageSpokenId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DROP TABLE [MemberEducHistory];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DROP INDEX [IX_MemberContact_InternalContactId] ON [MemberContact];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberWorkExperience]') AND [c].[name] = N'Ended');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [MemberWorkExperience] DROP COLUMN [Ended];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberWorkExperience]') AND [c].[name] = N'Started');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [MemberWorkExperience] DROP COLUMN [Started];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'ChurchFellow');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [MemberRelation] DROP COLUMN [ChurchFellow];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'ExternalNonMemAssociateId');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [MemberRelation] DROP COLUMN [ExternalNonMemAssociateId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberProfessionBrand]') AND [c].[name] = N'Since');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [MemberProfessionBrand] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [MemberProfessionBrand] DROP COLUMN [Since];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberProfessionBrand]') AND [c].[name] = N'Until');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [MemberProfessionBrand] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [MemberProfessionBrand] DROP COLUMN [Until];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'InternalContactId');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [MemberContact] DROP COLUMN [InternalContactId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'IsChurchFellow');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [MemberContact] DROP COLUMN [IsChurchFellow];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ContactInfo]') AND [c].[name] = N'ChurchFellow');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [ContactInfo] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [ContactInfo] DROP COLUMN [ChurchFellow];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ContactInfo]') AND [c].[name] = N'ContactName');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [ContactInfo] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [ContactInfo] DROP COLUMN [ContactName];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ContactInfo]') AND [c].[name] = N'ContactRef');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [ContactInfo] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [ContactInfo] DROP COLUMN [ContactRef];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberWorkExperience]') AND [c].[name] = N'WorkRole');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [MemberWorkExperience] ALTER COLUMN [WorkRole] nvarchar(50) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberWorkExperience]') AND [c].[name] = N'WorkPlace');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [MemberWorkExperience] ALTER COLUMN [WorkPlace] nvarchar(50) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberWorkExperience] ADD [FromDate] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberWorkExperience] ADD [ToDate] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberRelation] ADD [ChurchFellowCode] nvarchar(1) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberRelation] ADD [ExtChurchAssociateChurchBodyId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberRelation] ADD [ExtChurchAssociateMemberId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberProfessionBrand] ADD [FromDate] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberProfessionBrand] ADD [ToDate] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberContact] ADD [ChurchFellowCode] nvarchar(1) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberContact] ADD [ExtConChurchAssociateChurchBodyId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberContact] ADD [ExtConChurchAssociateMemberId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberContact] ADD [InternalContactChurchMemberId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [ContactInfo] ADD [ContactInfoDesc] nvarchar(50) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [ContactInfo] ADD [ExtHolderName] nvarchar(100) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [ContactInfo] ADD [IsChurchFellow] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE TABLE [MemberChurchlifeEventTask] (
        [Id] int NOT NULL IDENTITY,
        [AppGlobalOwnerId] int NULL,
        [ChurchBodyId] int NULL,
        [OwnedByChurchBodyId] int NULL,
        [ChurchMemberId] int NULL,
        [TaskMemberActivityId] int NULL,
        [RequirementDefId] int NULL,
        [MemberChurchRoleId] int NULL,
        [Status] nvarchar(1) NULL,
        [Completed] datetime2 NULL,
        [OrderIndex] int NULL,
        [Details] nvarchar(100) NULL,
        [PhotoUrl] nvarchar(max) NULL,
        [Notes] nvarchar(200) NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_MemberChurchlifeEventTask] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MemberChurchlifeEventTask_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MemberChurchlifeEventTask_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MemberChurchlifeEventTask_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MemberChurchlifeEventTask_MemberChurchRole_MemberChurchRoleId] FOREIGN KEY ([MemberChurchRoleId]) REFERENCES [MemberChurchRole] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MemberChurchlifeEventTask_AppUtilityNVP_RequirementDefId] FOREIGN KEY ([RequirementDefId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MemberChurchlifeEventTask_MemberChurchlifeActivity_TaskMemberActivityId] FOREIGN KEY ([TaskMemberActivityId]) REFERENCES [MemberChurchlifeActivity] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE TABLE [MemberEducation] (
        [Id] int NOT NULL IDENTITY,
        [AppGlobalOwnerId] int NULL,
        [ChurchBodyId] int NULL,
        [ChurchMemberId] int NOT NULL,
        [InstitutionName] nvarchar(50) NULL,
        [InstitutionTypeId] int NOT NULL,
        [CertificateId] int NULL,
        [Location] nvarchar(50) NULL,
        [CtryAlpha3Code] nvarchar(3) NULL,
        [IsCompleted] bit NULL,
        [FromDate] datetime2 NULL,
        [ToDate] datetime2 NULL,
        [Discipline] nvarchar(100) NULL,
        [Created] datetime2 NULL,
        [LastMod] datetime2 NULL,
        [CreatedByUserId] int NULL,
        [LastModByUserId] int NULL,
        CONSTRAINT [PK_MemberEducation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MemberEducation_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MemberEducation_CertificateType_CertificateId] FOREIGN KEY ([CertificateId]) REFERENCES [CertificateType] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MemberEducation_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MemberEducation_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MemberEducation_Country_CtryAlpha3Code] FOREIGN KEY ([CtryAlpha3Code]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MemberEducation_InstitutionType_InstitutionTypeId] FOREIGN KEY ([InstitutionTypeId]) REFERENCES [InstitutionType] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberRelation_ExtChurchAssociateChurchBodyId] ON [MemberRelation] ([ExtChurchAssociateChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberRelation_ExtChurchAssociateMemberId] ON [MemberRelation] ([ExtChurchAssociateMemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberRelation_RelationshipCode] ON [MemberRelation] ([RelationshipCode]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberContact_ExtConChurchAssociateChurchBodyId] ON [MemberContact] ([ExtConChurchAssociateChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberContact_ExtConChurchAssociateMemberId] ON [MemberContact] ([ExtConChurchAssociateMemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberContact_InternalContactChurchMemberId] ON [MemberContact] ([InternalContactChurchMemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberChurchlifeEventTask_AppGlobalOwnerId] ON [MemberChurchlifeEventTask] ([AppGlobalOwnerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberChurchlifeEventTask_ChurchBodyId] ON [MemberChurchlifeEventTask] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberChurchlifeEventTask_ChurchMemberId] ON [MemberChurchlifeEventTask] ([ChurchMemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberChurchlifeEventTask_MemberChurchRoleId] ON [MemberChurchlifeEventTask] ([MemberChurchRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberChurchlifeEventTask_RequirementDefId] ON [MemberChurchlifeEventTask] ([RequirementDefId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberChurchlifeEventTask_TaskMemberActivityId] ON [MemberChurchlifeEventTask] ([TaskMemberActivityId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberEducation_AppGlobalOwnerId] ON [MemberEducation] ([AppGlobalOwnerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberEducation_CertificateId] ON [MemberEducation] ([CertificateId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberEducation_ChurchBodyId] ON [MemberEducation] ([ChurchBodyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberEducation_ChurchMemberId] ON [MemberEducation] ([ChurchMemberId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberEducation_CtryAlpha3Code] ON [MemberEducation] ([CtryAlpha3Code]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    CREATE INDEX [IX_MemberEducation_InstitutionTypeId] ON [MemberEducation] ([InstitutionTypeId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberChurchlifeActivity] ADD CONSTRAINT [FK_MemberChurchlifeActivity_AppUtilityNVP_ChurchlifeActivityId] FOREIGN KEY ([ChurchlifeActivityId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_ChurchBody_ExtConChurchAssociateChurchBodyId] FOREIGN KEY ([ExtConChurchAssociateChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_ChurchMember_ExtConChurchAssociateMemberId] FOREIGN KEY ([ExtConChurchAssociateMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_ChurchMember_InternalContactChurchMemberId] FOREIGN KEY ([InternalContactChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberLanguageSpoken] ADD CONSTRAINT [FK_MemberLanguageSpoken_AppUtilityNVP_LanguageSpokenId] FOREIGN KEY ([LanguageSpokenId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberRelation] ADD CONSTRAINT [FK_MemberRelation_ChurchBody_ExtChurchAssociateChurchBodyId] FOREIGN KEY ([ExtChurchAssociateChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberRelation] ADD CONSTRAINT [FK_MemberRelation_ChurchMember_ExtChurchAssociateMemberId] FOREIGN KEY ([ExtChurchAssociateMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    ALTER TABLE [MemberRelation] ADD CONSTRAINT [FK_MemberRelation_RelationshipType_RelationshipCode] FOREIGN KEY ([RelationshipCode]) REFERENCES [RelationshipType] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210416154848_ef_c_upd22')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210416154848_ef_c_upd22', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [FK_ApprovalProcess_LeaderRole_EscalLeaderRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_LeaderRole_ApproverLeaderRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [FK_ChurchTransfer_LeaderRole_ToLeaderRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchlifeEventTask] DROP CONSTRAINT [FK_MemberChurchlifeEventTask_MemberChurchlifeActivity_TaskMemberActivityId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchRole] DROP CONSTRAINT [FK_MemberChurchRole_ChurchBody_AssociatedChurchBodyId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchRole] DROP CONSTRAINT [FK_MemberChurchRole_LeaderRole_LeaderRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DROP TABLE [LeaderRole];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DROP TABLE [LeaderRoleCategory];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DROP INDEX [IX_MemberChurchRole_AssociatedChurchBodyId] ON [MemberChurchRole];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DROP INDEX [IX_MemberChurchRole_LeaderRoleId] ON [MemberChurchRole];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DROP INDEX [IX_MemberChurchlifeEventTask_TaskMemberActivityId] ON [MemberChurchlifeEventTask];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DROP INDEX [IX_ChurchTransfer_ToLeaderRoleId] ON [ChurchTransfer];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DROP INDEX [IX_ApprovalProcessStep_ApproverLeaderRoleId] ON [ApprovalProcessStep];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DROP INDEX [IX_ApprovalProcess_EscalLeaderRoleId] ON [ApprovalProcess];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchRole]') AND [c].[name] = N'AssociatedChurchBodyId');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchRole] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [MemberChurchRole] DROP COLUMN [AssociatedChurchBodyId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchRole]') AND [c].[name] = N'LeaderRoleId');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchRole] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [MemberChurchRole] DROP COLUMN [LeaderRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlifeEventTask]') AND [c].[name] = N'Completed');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlifeEventTask] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [MemberChurchlifeEventTask] DROP COLUMN [Completed];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlifeEventTask]') AND [c].[name] = N'TaskMemberActivityId');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlifeEventTask] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [MemberChurchlifeEventTask] DROP COLUMN [TaskMemberActivityId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchTransfer]') AND [c].[name] = N'ToLeaderRoleId');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [ChurchTransfer] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [ChurchTransfer] DROP COLUMN [ToLeaderRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'ApproverLeaderRoleId');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [ApprovalProcessStep] DROP COLUMN [ApproverLeaderRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcess]') AND [c].[name] = N'EscalLeaderRoleId');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [ApprovalProcess] DROP COLUMN [EscalLeaderRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchRole] ADD [ChurchRoleId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchRole] ADD [SharingStatus] nvarchar(1) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchlifeEventTask] ADD [DateCommenced] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchlifeEventTask] ADD [DateCompleted] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchlifeEventTask] ADD [MemberChurchlifeActivityId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD [ToChurchRoleId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ChurchRole] ADD [OfficeTermMax_Yrs] decimal(18, 2) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ChurchRole] ADD [OfficeTermType] nvarchar(1) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ApprovalProcessStep] ADD [ApproverChurchRoleId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ApprovalProcess] ADD [EscalChurchRoleId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    CREATE INDEX [IX_MemberChurchRole_ChurchRoleId] ON [MemberChurchRole] ([ChurchRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    CREATE INDEX [IX_MemberChurchlifeEventTask_MemberChurchlifeActivityId] ON [MemberChurchlifeEventTask] ([MemberChurchlifeActivityId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    CREATE INDEX [IX_ChurchTransfer_ToChurchRoleId] ON [ChurchTransfer] ([ToChurchRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    CREATE INDEX [IX_ApprovalProcessStep_ApproverChurchRoleId] ON [ApprovalProcessStep] ([ApproverChurchRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    CREATE INDEX [IX_ApprovalProcess_EscalChurchRoleId] ON [ApprovalProcess] ([EscalChurchRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ApprovalProcess] ADD CONSTRAINT [FK_ApprovalProcess_ChurchRole_EscalChurchRoleId] FOREIGN KEY ([EscalChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchRole_ApproverChurchRoleId] FOREIGN KEY ([ApproverChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [ChurchTransfer] ADD CONSTRAINT [FK_ChurchTransfer_ChurchRole_ToChurchRoleId] FOREIGN KEY ([ToChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchlifeEventTask] ADD CONSTRAINT [FK_MemberChurchlifeEventTask_MemberChurchlifeActivity_MemberChurchlifeActivityId] FOREIGN KEY ([MemberChurchlifeActivityId]) REFERENCES [MemberChurchlifeActivity] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    ALTER TABLE [MemberChurchRole] ADD CONSTRAINT [FK_MemberChurchRole_ChurchRole_ChurchRoleId] FOREIGN KEY ([ChurchRoleId]) REFERENCES [ChurchRole] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210520163538_ef_c_upd23')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210520163538_ef_c_upd23', N'3.1.7');
END;

GO


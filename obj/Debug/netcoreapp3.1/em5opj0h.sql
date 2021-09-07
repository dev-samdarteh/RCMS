ALTER TABLE [ChurchCalendarEvent] DROP CONSTRAINT [FK_ChurchCalendarEvent_ChurchEventCategory_ChurchEventCategoryId];

GO

ALTER TABLE [ChurchCalendarEvent] DROP CONSTRAINT [FK_ChurchCalendarEvent_ChurchlifeActivity_ChurchlifeActivityId];

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRank]') AND [c].[name] = N'OwnedByChurchBodyId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [MemberRank] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [MemberRank] DROP COLUMN [OwnedByChurchBodyId];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchCalendarEvent]') AND [c].[name] = N'OwnedByChurchBodyId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ChurchCalendarEvent] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [ChurchCalendarEvent] DROP COLUMN [OwnedByChurchBodyId];

GO

ALTER TABLE [MemberType] ADD [SharingStatus] nvarchar(1) NULL;

GO

ALTER TABLE [MemberStatus] ADD [SharingStatus] nvarchar(1) NULL;

GO

ALTER TABLE [MemberRank] ADD [SharingStatus] nvarchar(1) NULL;

GO

ALTER TABLE [MemberChurchUnit] ADD [SharingStatus] nvarchar(1) NULL;

GO

ALTER TABLE [MemberChurchlifeEventTask] ADD [SharingStatus] nvarchar(1) NULL;

GO

ALTER TABLE [MemberChurchlifeActivity] ADD [SharingStatus] nvarchar(1) NULL;

GO

ALTER TABLE [MemberChurchlife] ADD [SharingStatus] nvarchar(1) NULL;

GO

ALTER TABLE [ChurchCalendarEvent] ADD CONSTRAINT [FK_ChurchCalendarEvent_AppUtilityNVP_ChurchEventCategoryId] FOREIGN KEY ([ChurchEventCategoryId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchCalendarEvent] ADD CONSTRAINT [FK_ChurchCalendarEvent_AppUtilityNVP_ChurchlifeActivityId] FOREIGN KEY ([ChurchlifeActivityId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210604023808_ef_c_upd32', N'3.1.7');

GO

ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [FK_ApprovalProcess_ChurchBody_ChurchBodyId];

GO

ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_ChurchBodyId];

GO

ALTER TABLE [ChurchAttend_Attendees] DROP CONSTRAINT [FK_ChurchAttend_Attendees_AppUtilityNVP_AgeBracketId];

GO

ALTER TABLE [ChurchAttend_Attendees] DROP CONSTRAINT [FK_ChurchAttend_Attendees_AppGlobalOwner_AppGlobalOwnerId];

GO

ALTER TABLE [ChurchAttend_Attendees] DROP CONSTRAINT [FK_ChurchAttend_Attendees_ChurchBody_ChurchBodyId];

GO

ALTER TABLE [ChurchAttend_Attendees] DROP CONSTRAINT [FK_ChurchAttend_Attendees_ChurchCalendarEvent_ChurchEventId];

GO

ALTER TABLE [ChurchAttend_Attendees] DROP CONSTRAINT [FK_ChurchAttend_Attendees_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [ChurchAttend_Attendees] DROP CONSTRAINT [FK_ChurchAttend_Attendees_Country_NationalityId];

GO

ALTER TABLE [ChurchAttend_Attendees] DROP CONSTRAINT [FK_ChurchAttend_Attendees_AppUtilityNVP_VisitReasonId];

GO

ALTER TABLE [ChurchAttend_HeadCount] DROP CONSTRAINT [FK_ChurchAttend_HeadCount_AppGlobalOwner_AppGlobalOwnerId];

GO

ALTER TABLE [ChurchAttend_HeadCount] DROP CONSTRAINT [FK_ChurchAttend_HeadCount_ChurchBody_ChurchBodyId];

GO

ALTER TABLE [ChurchAttend_HeadCount] DROP CONSTRAINT [FK_ChurchAttend_HeadCount_ChurchCalendarEvent_ChurchEventId];

GO

ALTER TABLE [ChurchAttend_HeadCount] DROP CONSTRAINT [FK_ChurchAttend_HeadCount_ChurchUnit_ChurchUnitId];

GO

ALTER TABLE [MemberLanguageSpoken] DROP CONSTRAINT [FK_MemberLanguageSpoken_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [MemberRelation] DROP CONSTRAINT [FK_MemberRelation_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [ChurchAttend_HeadCount] DROP CONSTRAINT [PK_ChurchAttend_HeadCount];

GO

ALTER TABLE [ChurchAttend_Attendees] DROP CONSTRAINT [PK_ChurchAttend_Attendees];

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'FaithTypeCategoryIdExtCon');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [MemberRelation] DROP COLUMN [FaithTypeCategoryIdExtCon];

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'MobilePhoneExtCon');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [MemberRelation] DROP COLUMN [MobilePhoneExtCon];

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CUMemberRollBal]') AND [c].[name] = N'Tot_OA');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [CUMemberRollBal] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [CUMemberRollBal] DROP COLUMN [Tot_OA];

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchBodyService]') AND [c].[name] = N'OwnedByChurchBodyId');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [ChurchBodyService] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [ChurchBodyService] DROP COLUMN [OwnedByChurchBodyId];

GO

EXEC sp_rename N'[ChurchAttend_HeadCount]', N'ChurchAttendHeadCount';

GO

EXEC sp_rename N'[ChurchAttend_Attendees]', N'ChurchAttendAttendee';

GO

EXEC sp_rename N'[ChurchAttendHeadCount].[IX_ChurchAttend_HeadCount_ChurchUnitId]', N'IX_ChurchAttendHeadCount_ChurchUnitId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendHeadCount].[IX_ChurchAttend_HeadCount_ChurchEventId]', N'IX_ChurchAttendHeadCount_ChurchEventId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendHeadCount].[IX_ChurchAttend_HeadCount_ChurchBodyId]', N'IX_ChurchAttendHeadCount_ChurchBodyId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendHeadCount].[IX_ChurchAttend_HeadCount_AppGlobalOwnerId]', N'IX_ChurchAttendHeadCount_AppGlobalOwnerId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendAttendee].[IX_ChurchAttend_Attendees_VisitReasonId]', N'IX_ChurchAttendAttendee_VisitReasonId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendAttendee].[IX_ChurchAttend_Attendees_NationalityId]', N'IX_ChurchAttendAttendee_NationalityId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendAttendee].[IX_ChurchAttend_Attendees_ChurchMemberId]', N'IX_ChurchAttendAttendee_ChurchMemberId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendAttendee].[IX_ChurchAttend_Attendees_ChurchEventId]', N'IX_ChurchAttendAttendee_ChurchEventId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendAttendee].[IX_ChurchAttend_Attendees_ChurchBodyId]', N'IX_ChurchAttendAttendee_ChurchBodyId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendAttendee].[IX_ChurchAttend_Attendees_AppGlobalOwnerId]', N'IX_ChurchAttendAttendee_AppGlobalOwnerId', N'INDEX';

GO

EXEC sp_rename N'[ChurchAttendAttendee].[IX_ChurchAttend_Attendees_AgeBracketId]', N'IX_ChurchAttendAttendee_AgeBracketId', N'INDEX';

GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'ChurchMemberId');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [MemberRelation] ALTER COLUMN [ChurchMemberId] int NULL;

GO

ALTER TABLE [MemberRelation] ADD [FaithTypeIdExtCon] int NULL;

GO

ALTER TABLE [MemberRelation] ADD [MobilePhone1ExtCon] nvarchar(15) NULL;

GO

ALTER TABLE [MemberRelation] ADD [MobilePhone2ExtCon] nvarchar(15) NULL;

GO

ALTER TABLE [MemberRegistration] ADD [SharingStatus] nvarchar(1) NULL;

GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberLanguageSpoken]') AND [c].[name] = N'ChurchMemberId');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [MemberLanguageSpoken] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [MemberLanguageSpoken] ALTER COLUMN [ChurchMemberId] int NULL;

GO

ALTER TABLE [CUMemberRollBal] ADD [Tot_AA] bigint NOT NULL DEFAULT CAST(0 AS bigint);

GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcessStep]') AND [c].[name] = N'ChurchBodyId');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcessStep] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [ApprovalProcessStep] ALTER COLUMN [ChurchBodyId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [AppGlobalOwnerId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [OwnedByChurchBodyId] int NULL;

GO

ALTER TABLE [ApprovalProcessStep] ADD [SharingStatus] nvarchar(1) NULL;

GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApprovalProcess]') AND [c].[name] = N'ChurchBodyId');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [ApprovalProcess] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [ApprovalProcess] ALTER COLUMN [ChurchBodyId] int NULL;

GO

ALTER TABLE [ApprovalProcess] ADD [AppGlobalOwnerId] int NULL;

GO

ALTER TABLE [ApprovalProcess] ADD [SharingStatus] nvarchar(1) NULL;

GO

ALTER TABLE [ApprovalActionStep] ADD [AppGlobalOwnerId] int NULL;

GO

ALTER TABLE [ApprovalActionStep] ADD [OwnedByChurchBodyId] int NULL;

GO

ALTER TABLE [ApprovalAction] ADD [AppGlobalOwnerId] int NULL;

GO

ALTER TABLE [ApprovalAction] ADD [OwnedByChurchBodyId] int NULL;

GO

ALTER TABLE [ChurchAttendHeadCount] ADD [Tot_AA] bigint NOT NULL DEFAULT CAST(0 AS bigint);

GO

ALTER TABLE [ChurchAttendHeadCount] ADD [Tot_C] bigint NOT NULL DEFAULT CAST(0 AS bigint);

GO

ALTER TABLE [ChurchAttendHeadCount] ADD [Tot_MA] bigint NOT NULL DEFAULT CAST(0 AS bigint);

GO

ALTER TABLE [ChurchAttendHeadCount] ADD [Tot_Y] bigint NOT NULL DEFAULT CAST(0 AS bigint);

GO

ALTER TABLE [ChurchAttendHeadCount] ADD [Tot_YA] bigint NOT NULL DEFAULT CAST(0 AS bigint);

GO

ALTER TABLE [ChurchAttendAttendee] ADD [SharingStatus] nvarchar(1) NULL;

GO

ALTER TABLE [ChurchAttendHeadCount] ADD CONSTRAINT [PK_ChurchAttendHeadCount] PRIMARY KEY ([Id]);

GO

ALTER TABLE [ChurchAttendAttendee] ADD CONSTRAINT [PK_ChurchAttendAttendee] PRIMARY KEY ([Id]);

GO

CREATE INDEX [IX_MemberRegistration_OwnedByChurchBodyId] ON [MemberRegistration] ([OwnedByChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_AppGlobalOwnerId] ON [ApprovalProcessStep] ([AppGlobalOwnerId]);

GO

CREATE INDEX [IX_ApprovalProcessStep_OwnedByChurchBodyId] ON [ApprovalProcessStep] ([OwnedByChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalActionStep_OwnedByChurchBodyId] ON [ApprovalActionStep] ([OwnedByChurchBodyId]);

GO

CREATE INDEX [IX_ApprovalAction_OwnedByChurchBodyId] ON [ApprovalAction] ([OwnedByChurchBodyId]);

GO

ALTER TABLE [ApprovalAction] ADD CONSTRAINT [FK_ApprovalAction_ChurchBody_OwnedByChurchBodyId] FOREIGN KEY ([OwnedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalActionStep] ADD CONSTRAINT [FK_ApprovalActionStep_ChurchBody_OwnedByChurchBodyId] FOREIGN KEY ([OwnedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcess] ADD CONSTRAINT [FK_ApprovalProcess_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ApprovalProcessStep] ADD CONSTRAINT [FK_ApprovalProcessStep_ChurchBody_OwnedByChurchBodyId] FOREIGN KEY ([OwnedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendAttendee] ADD CONSTRAINT [FK_ChurchAttendAttendee_AppUtilityNVP_AgeBracketId] FOREIGN KEY ([AgeBracketId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendAttendee] ADD CONSTRAINT [FK_ChurchAttendAttendee_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendAttendee] ADD CONSTRAINT [FK_ChurchAttendAttendee_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendAttendee] ADD CONSTRAINT [FK_ChurchAttendAttendee_ChurchCalendarEvent_ChurchEventId] FOREIGN KEY ([ChurchEventId]) REFERENCES [ChurchCalendarEvent] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendAttendee] ADD CONSTRAINT [FK_ChurchAttendAttendee_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendAttendee] ADD CONSTRAINT [FK_ChurchAttendAttendee_Country_NationalityId] FOREIGN KEY ([NationalityId]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendAttendee] ADD CONSTRAINT [FK_ChurchAttendAttendee_AppUtilityNVP_VisitReasonId] FOREIGN KEY ([VisitReasonId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendHeadCount] ADD CONSTRAINT [FK_ChurchAttendHeadCount_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendHeadCount] ADD CONSTRAINT [FK_ChurchAttendHeadCount_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendHeadCount] ADD CONSTRAINT [FK_ChurchAttendHeadCount_ChurchCalendarEvent_ChurchEventId] FOREIGN KEY ([ChurchEventId]) REFERENCES [ChurchCalendarEvent] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ChurchAttendHeadCount] ADD CONSTRAINT [FK_ChurchAttendHeadCount_ChurchUnit_ChurchUnitId] FOREIGN KEY ([ChurchUnitId]) REFERENCES [ChurchUnit] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberLanguageSpoken] ADD CONSTRAINT [FK_MemberLanguageSpoken_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRegistration] ADD CONSTRAINT [FK_MemberRegistration_ChurchBody_OwnedByChurchBodyId] FOREIGN KEY ([OwnedByChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRelation] ADD CONSTRAINT [FK_MemberRelation_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210606064820_ef_c_upd33', N'3.1.7');

GO

ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [FK_MemberChurchlife_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [MemberContact] DROP CONSTRAINT [FK_MemberContact_Country_CtryAlpha3Code];

GO

ALTER TABLE [MemberContact] DROP CONSTRAINT [FK_MemberContact_ChurchBody_ExtConChurchAssociateChurchBodyId];

GO

ALTER TABLE [MemberContact] DROP CONSTRAINT [FK_MemberContact_ChurchMember_ExtConChurchAssociateMemberId];

GO

ALTER TABLE [MemberContact] DROP CONSTRAINT [FK_MemberContact_CountryRegion_ExtConRegionId];

GO

ALTER TABLE [MemberContact] DROP CONSTRAINT [FK_MemberContact_ChurchMember_InternalContactChurchMemberId];

GO

ALTER TABLE [MemberEducation] DROP CONSTRAINT [FK_MemberEducation_CertificateType_CertificateId];

GO

ALTER TABLE [MemberEducation] DROP CONSTRAINT [FK_MemberEducation_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [MemberEducation] DROP CONSTRAINT [FK_MemberEducation_InstitutionType_InstitutionTypeId];

GO

ALTER TABLE [MemberProfessionBrand] DROP CONSTRAINT [FK_MemberProfessionBrand_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [MemberRank] DROP CONSTRAINT [FK_MemberRank_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [MemberRank] DROP CONSTRAINT [FK_MemberRank_ChurchRank_ChurchRankId];

GO

ALTER TABLE [MemberRegistration] DROP CONSTRAINT [FK_MemberRegistration_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [MemberStatus] DROP CONSTRAINT [FK_MemberStatus_ChurchMemStatus_ChurchMemStatusId];

GO

ALTER TABLE [MemberType] DROP CONSTRAINT [FK_MemberType_ChurchMemType_ChurchMemTypeId];

GO

ALTER TABLE [MemberType] DROP CONSTRAINT [FK_MemberType_ChurchMember_ChurchMemberId];

GO

ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [FK_MemberWorkExperience_ChurchMember_ChurchMemberId];

GO

DROP INDEX [IX_MemberContact_CtryAlpha3Code] ON [MemberContact];

GO

DROP INDEX [IX_MemberContact_ExtConChurchAssociateChurchBodyId] ON [MemberContact];

GO

DROP INDEX [IX_MemberContact_ExtConChurchAssociateMemberId] ON [MemberContact];

GO

DROP INDEX [IX_MemberContact_ExtConRegionId] ON [MemberContact];

GO

DROP INDEX [IX_MemberContact_InternalContactChurchMemberId] ON [MemberContact];

GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberType]') AND [c].[name] = N'Assigned');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [MemberType] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [MemberType] DROP COLUMN [Assigned];

GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberType]') AND [c].[name] = N'Until');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [MemberType] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [MemberType] DROP COLUMN [Until];

GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberStatus]') AND [c].[name] = N'Since');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [MemberStatus] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [MemberStatus] DROP COLUMN [Since];

GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberStatus]') AND [c].[name] = N'Until');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [MemberStatus] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [MemberStatus] DROP COLUMN [Until];

GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'ContactNameExtCon');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [MemberRelation] DROP COLUMN [ContactNameExtCon];

GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'FaithTypeIdExtCon');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [MemberRelation] DROP COLUMN [FaithTypeIdExtCon];

GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRank]') AND [c].[name] = N'Assigned');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [MemberRank] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [MemberRank] DROP COLUMN [Assigned];

GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRank]') AND [c].[name] = N'Until');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [MemberRank] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [MemberRank] DROP COLUMN [Until];

GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ChurchFellowCode');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ChurchFellowCode];

GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'CtryAlpha3Code');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [CtryAlpha3Code];

GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConChurchAssociateChurchBodyId');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConChurchAssociateChurchBodyId];

GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConChurchAssociateMemberId');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConChurchAssociateMemberId];

GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConContactName');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConContactName];

GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConDenomination');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConDenomination];

GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConDigitalAddress');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConDigitalAddress];

GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConEmail');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConEmail];

GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConFaithCategory');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConFaithCategory];

GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConLocation');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConLocation];

GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConMobilePhone');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConMobilePhone];

GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConPostalAddress');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConPostalAddress];

GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConRegionId');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConRegionId];

GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConResAddrSameAsPostAddr');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConResAddrSameAsPostAddr];

GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'ExtConResidenceAddress');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [ExtConResidenceAddress];

GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'InternalContactChurchMemberId');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [InternalContactChurchMemberId];

GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchUnit]') AND [c].[name] = N'DateDeparted');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchUnit] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [MemberChurchUnit] DROP COLUMN [DateDeparted];

GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchUnit]') AND [c].[name] = N'DateJoined');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchUnit] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [MemberChurchUnit] DROP COLUMN [DateJoined];

GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchRole]') AND [c].[name] = N'DateCommenced');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchRole] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [MemberChurchRole] DROP COLUMN [DateCommenced];

GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchRole]') AND [c].[name] = N'DateCompleted');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchRole] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [MemberChurchRole] DROP COLUMN [DateCompleted];

GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlifeActivity]') AND [c].[name] = N'Comments');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlifeActivity] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [MemberChurchlifeActivity] DROP COLUMN [Comments];

GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlifeActivity]') AND [c].[name] = N'IsHostedByChurchVenue');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlifeActivity] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [MemberChurchlifeActivity] DROP COLUMN [IsHostedByChurchVenue];

GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlifeActivity]') AND [c].[name] = N'IsOfficiatedByChurchFellow');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlifeActivity] DROP CONSTRAINT [' + @var40 + '];');
ALTER TABLE [MemberChurchlifeActivity] DROP COLUMN [IsOfficiatedByChurchFellow];

GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlifeActivity]') AND [c].[name] = N'PhotoUrl');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlifeActivity] DROP CONSTRAINT [' + @var41 + '];');
ALTER TABLE [MemberChurchlifeActivity] DROP COLUMN [PhotoUrl];

GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberWorkExperience]') AND [c].[name] = N'ChurchMemberId');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [MemberWorkExperience] ALTER COLUMN [ChurchMemberId] int NULL;

GO

DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberType]') AND [c].[name] = N'ChurchMemberId');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [MemberType] DROP CONSTRAINT [' + @var43 + '];');
ALTER TABLE [MemberType] ALTER COLUMN [ChurchMemberId] int NULL;

GO

ALTER TABLE [MemberType] ADD [FromDate] datetime2 NULL;

GO

ALTER TABLE [MemberType] ADD [ToDate] datetime2 NULL;

GO

ALTER TABLE [MemberStatus] ADD [FromDate] datetime2 NULL;

GO

ALTER TABLE [MemberStatus] ADD [ToDate] datetime2 NULL;

GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'RelationCategory');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [MemberRelation] ALTER COLUMN [RelationCategory] nvarchar(1) NULL;

GO

ALTER TABLE [MemberRelation] ADD [FaithAffiliationExtCon] nvarchar(max) NULL;

GO

ALTER TABLE [MemberRelation] ADD [PhotoUrlExtCon] nvarchar(max) NULL;

GO

ALTER TABLE [MemberRelation] ADD [RelationNameExtCon] nvarchar(100) NULL;

GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRegistration]') AND [c].[name] = N'ChurchYear');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [MemberRegistration] DROP CONSTRAINT [' + @var45 + '];');
ALTER TABLE [MemberRegistration] ALTER COLUMN [ChurchYear] nvarchar(4) NULL;

GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRegistration]') AND [c].[name] = N'ChurchMemberId');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [MemberRegistration] DROP CONSTRAINT [' + @var46 + '];');
ALTER TABLE [MemberRegistration] ALTER COLUMN [ChurchMemberId] int NULL;

GO

DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRank]') AND [c].[name] = N'ChurchMemberId');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [MemberRank] DROP CONSTRAINT [' + @var47 + '];');
ALTER TABLE [MemberRank] ALTER COLUMN [ChurchMemberId] int NULL;

GO

ALTER TABLE [MemberRank] ADD [ChurchRankId1] int NULL;

GO

ALTER TABLE [MemberRank] ADD [FromDate] datetime2 NULL;

GO

ALTER TABLE [MemberRank] ADD [ToDate] datetime2 NULL;

GO

DECLARE @var48 sysname;
SELECT @var48 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberProfessionBrand]') AND [c].[name] = N'ChurchMemberId');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [MemberProfessionBrand] DROP CONSTRAINT [' + @var48 + '];');
ALTER TABLE [MemberProfessionBrand] ALTER COLUMN [ChurchMemberId] int NULL;

GO

DECLARE @var49 sysname;
SELECT @var49 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberEducation]') AND [c].[name] = N'InstitutionTypeId');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [MemberEducation] DROP CONSTRAINT [' + @var49 + '];');
ALTER TABLE [MemberEducation] ALTER COLUMN [InstitutionTypeId] int NULL;

GO

DECLARE @var50 sysname;
SELECT @var50 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberEducation]') AND [c].[name] = N'ChurchMemberId');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [MemberEducation] DROP CONSTRAINT [' + @var50 + '];');
ALTER TABLE [MemberEducation] ALTER COLUMN [ChurchMemberId] int NULL;

GO

ALTER TABLE [MemberEducation] ADD [CertificatePhotoUrl] nvarchar(max) NULL;

GO

DECLARE @var51 sysname;
SELECT @var51 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'CityExtCon');
IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var51 + '];');
ALTER TABLE [MemberContact] ALTER COLUMN [CityExtCon] nvarchar(30) NULL;

GO

ALTER TABLE [MemberContact] ADD [ContactChurchBodyId] int NULL;

GO

ALTER TABLE [MemberContact] ADD [ContactChurchMemberId] int NULL;

GO

ALTER TABLE [MemberContact] ADD [ContactNameExtCon] nvarchar(100) NULL;

GO

ALTER TABLE [MemberContact] ADD [CtryAlpha3CodeExtCon] nvarchar(3) NULL;

GO

ALTER TABLE [MemberContact] ADD [DenominationExtCon] nvarchar(100) NULL;

GO

ALTER TABLE [MemberContact] ADD [DigitalAddressExtCon] nvarchar(30) NULL;

GO

ALTER TABLE [MemberContact] ADD [EmailExtCon] nvarchar(max) NULL;

GO

ALTER TABLE [MemberContact] ADD [FaithAffiliationExtCon] nvarchar(max) NULL;

GO

ALTER TABLE [MemberContact] ADD [LocationExtCon] nvarchar(30) NULL;

GO

ALTER TABLE [MemberContact] ADD [MobilePhone1ExtCon] nvarchar(15) NULL;

GO

ALTER TABLE [MemberContact] ADD [MobilePhone2ExtCon] nvarchar(15) NULL;

GO

ALTER TABLE [MemberContact] ADD [PhotoUrlExtCon] nvarchar(max) NULL;

GO

ALTER TABLE [MemberContact] ADD [PostalAddressExtCon] nvarchar(30) NULL;

GO

ALTER TABLE [MemberContact] ADD [RegionIdExtCon] int NULL;

GO

ALTER TABLE [MemberContact] ADD [RelationCategory] nvarchar(1) NULL;

GO

ALTER TABLE [MemberContact] ADD [RelationScope] nvarchar(1) NULL;

GO

ALTER TABLE [MemberContact] ADD [ResAddrSameAsPostAddrExtCon] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [MemberContact] ADD [ResidenceAddressExtCon] nvarchar(100) NULL;

GO

ALTER TABLE [MemberContact] ADD [Status] nvarchar(1) NULL;

GO

ALTER TABLE [MemberChurchUnit] ADD [FromDate] datetime2 NULL;

GO

ALTER TABLE [MemberChurchUnit] ADD [ToDate] datetime2 NULL;

GO

ALTER TABLE [MemberChurchRole] ADD [FromDate] datetime2 NULL;

GO

ALTER TABLE [MemberChurchRole] ADD [RolePhotoUrl] nvarchar(max) NULL;

GO

ALTER TABLE [MemberChurchRole] ADD [ToDate] datetime2 NULL;

GO

ALTER TABLE [MemberChurchlifeActivity] ADD [EventPhotoUrl] nvarchar(max) NULL;

GO

ALTER TABLE [MemberChurchlifeActivity] ADD [HostVenueScope] nvarchar(max) NULL;

GO

ALTER TABLE [MemberChurchlifeActivity] ADD [Notes] nvarchar(300) NULL;

GO

ALTER TABLE [MemberChurchlifeActivity] ADD [OfficiatedByChurchBodyId] int NULL;

GO

ALTER TABLE [MemberChurchlifeActivity] ADD [OfficiatedByChurchMemberId] int NULL;

GO

ALTER TABLE [MemberChurchlifeActivity] ADD [OfficiatedByScope] nvarchar(1) NULL;

GO

DECLARE @var52 sysname;
SELECT @var52 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'ChurchMemberId');
IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var52 + '];');
ALTER TABLE [MemberChurchlife] ALTER COLUMN [ChurchMemberId] int NULL;

GO

ALTER TABLE [MemberChurchlife] ADD [ChurchlifePhotoUrl] nvarchar(max) NULL;

GO

CREATE INDEX [IX_MemberRank_ChurchRankId1] ON [MemberRank] ([ChurchRankId1]);

GO

CREATE INDEX [IX_MemberContact_ContactChurchBodyId] ON [MemberContact] ([ContactChurchBodyId]);

GO

CREATE INDEX [IX_MemberContact_ContactChurchMemberId] ON [MemberContact] ([ContactChurchMemberId]);

GO

CREATE INDEX [IX_MemberContact_CtryAlpha3CodeExtCon] ON [MemberContact] ([CtryAlpha3CodeExtCon]);

GO

CREATE INDEX [IX_MemberContact_RegionIdExtCon] ON [MemberContact] ([RegionIdExtCon]);

GO

CREATE INDEX [IX_MemberContact_RelationshipCode] ON [MemberContact] ([RelationshipCode]);

GO

ALTER TABLE [MemberChurchlife] ADD CONSTRAINT [FK_MemberChurchlife_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_ChurchBody_ContactChurchBodyId] FOREIGN KEY ([ContactChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_ChurchMember_ContactChurchMemberId] FOREIGN KEY ([ContactChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_Country_CtryAlpha3CodeExtCon] FOREIGN KEY ([CtryAlpha3CodeExtCon]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_CountryRegion_RegionIdExtCon] FOREIGN KEY ([RegionIdExtCon]) REFERENCES [CountryRegion] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_RelationshipType_RelationshipCode] FOREIGN KEY ([RelationshipCode]) REFERENCES [RelationshipType] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberEducation] ADD CONSTRAINT [FK_MemberEducation_AppUtilityNVP_CertificateId] FOREIGN KEY ([CertificateId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberEducation] ADD CONSTRAINT [FK_MemberEducation_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberEducation] ADD CONSTRAINT [FK_MemberEducation_AppUtilityNVP_InstitutionTypeId] FOREIGN KEY ([InstitutionTypeId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberProfessionBrand] ADD CONSTRAINT [FK_MemberProfessionBrand_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRank] ADD CONSTRAINT [FK_MemberRank_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRank] ADD CONSTRAINT [FK_MemberRank_AppUtilityNVP_ChurchRankId] FOREIGN KEY ([ChurchRankId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRank] ADD CONSTRAINT [FK_MemberRank_ChurchRank_ChurchRankId1] FOREIGN KEY ([ChurchRankId1]) REFERENCES [ChurchRank] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRegistration] ADD CONSTRAINT [FK_MemberRegistration_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberStatus] ADD CONSTRAINT [FK_MemberStatus_AppUtilityNVP_ChurchMemStatusId] FOREIGN KEY ([ChurchMemStatusId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberType] ADD CONSTRAINT [FK_MemberType_AppUtilityNVP_ChurchMemTypeId] FOREIGN KEY ([ChurchMemTypeId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberType] ADD CONSTRAINT [FK_MemberType_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberWorkExperience] ADD CONSTRAINT [FK_MemberWorkExperience_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210607002929_ef_c_upd34', N'3.1.7');

GO

ALTER TABLE [MemberContact] DROP CONSTRAINT [FK_MemberContact_RelationshipType_RelationshipCode];

GO

ALTER TABLE [MemberRelation] DROP CONSTRAINT [FK_MemberRelation_RelationshipType_RelationshipCode];

GO

ALTER TABLE [RelationshipType] DROP CONSTRAINT [PK_RelationshipType];

GO

DECLARE @var53 sysname;
SELECT @var53 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RelationshipType]') AND [c].[name] = N'Id');
IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [RelationshipType] DROP CONSTRAINT [' + @var53 + '];');
ALTER TABLE [RelationshipType] DROP COLUMN [Id];

GO

ALTER TABLE [RelationshipType] ADD CONSTRAINT [PK_RelationshipType] PRIMARY KEY ([RelationCode]);

GO

CREATE INDEX [IX_AppUtilityNVP_CtryAlpha3Code] ON [AppUtilityNVP] ([CtryAlpha3Code]);

GO

ALTER TABLE [AppUtilityNVP] ADD CONSTRAINT [FK_AppUtilityNVP_Country_CtryAlpha3Code] FOREIGN KEY ([CtryAlpha3Code]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_RelationshipType_RelationshipCode] FOREIGN KEY ([RelationshipCode]) REFERENCES [RelationshipType] ([RelationCode]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRelation] ADD CONSTRAINT [FK_MemberRelation_RelationshipType_RelationshipCode] FOREIGN KEY ([RelationshipCode]) REFERENCES [RelationshipType] ([RelationCode]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210608194659_ef_c_35', N'3.1.7');

GO

DECLARE @var54 sysname;
SELECT @var54 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberType]') AND [c].[name] = N'Comments');
IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [MemberType] DROP CONSTRAINT [' + @var54 + '];');
ALTER TABLE [MemberType] DROP COLUMN [Comments];

GO

DECLARE @var55 sysname;
SELECT @var55 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberStatus]') AND [c].[name] = N'AttachedNotes');
IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [MemberStatus] DROP CONSTRAINT [' + @var55 + '];');
ALTER TABLE [MemberStatus] DROP COLUMN [AttachedNotes];

GO

DECLARE @var56 sysname;
SELECT @var56 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRank]') AND [c].[name] = N'Comments');
IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [MemberRank] DROP CONSTRAINT [' + @var56 + '];');
ALTER TABLE [MemberRank] DROP COLUMN [Comments];

GO

DECLARE @var57 sysname;
SELECT @var57 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchUnit]') AND [c].[name] = N'IsCurrentMember');
IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchUnit] DROP CONSTRAINT [' + @var57 + '];');
ALTER TABLE [MemberChurchUnit] DROP COLUMN [IsCurrentMember];

GO

DECLARE @var58 sysname;
SELECT @var58 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchRole]') AND [c].[name] = N'CompletionReason');
IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchRole] DROP CONSTRAINT [' + @var58 + '];');
ALTER TABLE [MemberChurchRole] DROP COLUMN [CompletionReason];

GO

DECLARE @var59 sysname;
SELECT @var59 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlifeEventTask]') AND [c].[name] = N'Status');
IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlifeEventTask] DROP CONSTRAINT [' + @var59 + '];');
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

DECLARE @var60 sysname;
SELECT @var60 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'MemberlifeSummary');
IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var60 + '];');
ALTER TABLE [MemberChurchlife] ALTER COLUMN [MemberlifeSummary] nvarchar(500) NULL;

GO

DECLARE @var61 sysname;
SELECT @var61 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'EnrollReason');
IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var61 + '];');
ALTER TABLE [MemberChurchlife] ALTER COLUMN [EnrollReason] nvarchar(50) NULL;

GO

DECLARE @var62 sysname;
SELECT @var62 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberChurchlife]') AND [c].[name] = N'DepartReason');
IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [MemberChurchlife] DROP CONSTRAINT [' + @var62 + '];');
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


DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchMember]') AND [c].[name] = N'ContactInfoId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ChurchMember] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [ChurchMember] DROP COLUMN [ContactInfoId];

GO

ALTER TABLE [ContactInfo] ADD [IsPrimaryContact] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [ChurchMember] ADD [PrimContactInfoId] int NULL;

GO

CREATE INDEX [IX_ContactInfo_AppGlobalOwnerId] ON [ContactInfo] ([AppGlobalOwnerId]);

GO

CREATE INDEX [IX_ContactInfo_ChurchBodyId] ON [ContactInfo] ([ChurchBodyId]);

GO

CREATE INDEX [IX_ChurchMember_PrimContactInfoId] ON [ChurchMember] ([PrimContactInfoId]);

GO

ALTER TABLE [ChurchMember] ADD CONSTRAINT [FK_ChurchMember_ContactInfo_PrimContactInfoId] FOREIGN KEY ([PrimContactInfoId]) REFERENCES [ContactInfo] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ContactInfo] ADD CONSTRAINT [FK_ContactInfo_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [ContactInfo] ADD CONSTRAINT [FK_ContactInfo_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210415114052_ef_c_upd21', N'3.1.7');

GO

ALTER TABLE [MemberChurchlifeActivity] DROP CONSTRAINT [FK_MemberChurchlifeActivity_ChurchlifeActivity_ChurchlifeActivityId];

GO

ALTER TABLE [MemberContact] DROP CONSTRAINT [FK_MemberContact_ChurchMember_InternalContactId];

GO

ALTER TABLE [MemberLanguageSpoken] DROP CONSTRAINT [FK_MemberLanguageSpoken_LanguageSpoken_LanguageSpokenId];

GO

DROP TABLE [MemberEducHistory];

GO

DROP INDEX [IX_MemberContact_InternalContactId] ON [MemberContact];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberWorkExperience]') AND [c].[name] = N'Ended');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [MemberWorkExperience] DROP COLUMN [Ended];

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberWorkExperience]') AND [c].[name] = N'Started');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [MemberWorkExperience] DROP COLUMN [Started];

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'ChurchFellow');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [MemberRelation] DROP COLUMN [ChurchFellow];

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRelation]') AND [c].[name] = N'ExternalNonMemAssociateId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [MemberRelation] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [MemberRelation] DROP COLUMN [ExternalNonMemAssociateId];

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberProfessionBrand]') AND [c].[name] = N'Since');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [MemberProfessionBrand] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [MemberProfessionBrand] DROP COLUMN [Since];

GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberProfessionBrand]') AND [c].[name] = N'Until');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [MemberProfessionBrand] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [MemberProfessionBrand] DROP COLUMN [Until];

GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'InternalContactId');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [InternalContactId];

GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberContact]') AND [c].[name] = N'IsChurchFellow');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [MemberContact] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [MemberContact] DROP COLUMN [IsChurchFellow];

GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ContactInfo]') AND [c].[name] = N'ChurchFellow');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [ContactInfo] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [ContactInfo] DROP COLUMN [ChurchFellow];

GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ContactInfo]') AND [c].[name] = N'ContactName');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [ContactInfo] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [ContactInfo] DROP COLUMN [ContactName];

GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ContactInfo]') AND [c].[name] = N'ContactRef');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [ContactInfo] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [ContactInfo] DROP COLUMN [ContactRef];

GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberWorkExperience]') AND [c].[name] = N'WorkRole');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [MemberWorkExperience] ALTER COLUMN [WorkRole] nvarchar(50) NULL;

GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberWorkExperience]') AND [c].[name] = N'WorkPlace');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [MemberWorkExperience] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [MemberWorkExperience] ALTER COLUMN [WorkPlace] nvarchar(50) NULL;

GO

ALTER TABLE [MemberWorkExperience] ADD [FromDate] datetime2 NULL;

GO

ALTER TABLE [MemberWorkExperience] ADD [ToDate] datetime2 NULL;

GO

ALTER TABLE [MemberRelation] ADD [ChurchFellowCode] nvarchar(1) NULL;

GO

ALTER TABLE [MemberRelation] ADD [ExtChurchAssociateChurchBodyId] int NULL;

GO

ALTER TABLE [MemberRelation] ADD [ExtChurchAssociateMemberId] int NULL;

GO

ALTER TABLE [MemberProfessionBrand] ADD [FromDate] datetime2 NULL;

GO

ALTER TABLE [MemberProfessionBrand] ADD [ToDate] datetime2 NULL;

GO

ALTER TABLE [MemberContact] ADD [ChurchFellowCode] nvarchar(1) NULL;

GO

ALTER TABLE [MemberContact] ADD [ExtConChurchAssociateChurchBodyId] int NULL;

GO

ALTER TABLE [MemberContact] ADD [ExtConChurchAssociateMemberId] int NULL;

GO

ALTER TABLE [MemberContact] ADD [InternalContactChurchMemberId] int NULL;

GO

ALTER TABLE [ContactInfo] ADD [ContactInfoDesc] nvarchar(50) NULL;

GO

ALTER TABLE [ContactInfo] ADD [ExtHolderName] nvarchar(100) NULL;

GO

ALTER TABLE [ContactInfo] ADD [IsChurchFellow] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

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

GO

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

GO

CREATE INDEX [IX_MemberRelation_ExtChurchAssociateChurchBodyId] ON [MemberRelation] ([ExtChurchAssociateChurchBodyId]);

GO

CREATE INDEX [IX_MemberRelation_ExtChurchAssociateMemberId] ON [MemberRelation] ([ExtChurchAssociateMemberId]);

GO

CREATE INDEX [IX_MemberRelation_RelationshipCode] ON [MemberRelation] ([RelationshipCode]);

GO

CREATE INDEX [IX_MemberContact_ExtConChurchAssociateChurchBodyId] ON [MemberContact] ([ExtConChurchAssociateChurchBodyId]);

GO

CREATE INDEX [IX_MemberContact_ExtConChurchAssociateMemberId] ON [MemberContact] ([ExtConChurchAssociateMemberId]);

GO

CREATE INDEX [IX_MemberContact_InternalContactChurchMemberId] ON [MemberContact] ([InternalContactChurchMemberId]);

GO

CREATE INDEX [IX_MemberChurchlifeEventTask_AppGlobalOwnerId] ON [MemberChurchlifeEventTask] ([AppGlobalOwnerId]);

GO

CREATE INDEX [IX_MemberChurchlifeEventTask_ChurchBodyId] ON [MemberChurchlifeEventTask] ([ChurchBodyId]);

GO

CREATE INDEX [IX_MemberChurchlifeEventTask_ChurchMemberId] ON [MemberChurchlifeEventTask] ([ChurchMemberId]);

GO

CREATE INDEX [IX_MemberChurchlifeEventTask_MemberChurchRoleId] ON [MemberChurchlifeEventTask] ([MemberChurchRoleId]);

GO

CREATE INDEX [IX_MemberChurchlifeEventTask_RequirementDefId] ON [MemberChurchlifeEventTask] ([RequirementDefId]);

GO

CREATE INDEX [IX_MemberChurchlifeEventTask_TaskMemberActivityId] ON [MemberChurchlifeEventTask] ([TaskMemberActivityId]);

GO

CREATE INDEX [IX_MemberEducation_AppGlobalOwnerId] ON [MemberEducation] ([AppGlobalOwnerId]);

GO

CREATE INDEX [IX_MemberEducation_CertificateId] ON [MemberEducation] ([CertificateId]);

GO

CREATE INDEX [IX_MemberEducation_ChurchBodyId] ON [MemberEducation] ([ChurchBodyId]);

GO

CREATE INDEX [IX_MemberEducation_ChurchMemberId] ON [MemberEducation] ([ChurchMemberId]);

GO

CREATE INDEX [IX_MemberEducation_CtryAlpha3Code] ON [MemberEducation] ([CtryAlpha3Code]);

GO

CREATE INDEX [IX_MemberEducation_InstitutionTypeId] ON [MemberEducation] ([InstitutionTypeId]);

GO

ALTER TABLE [MemberChurchlifeActivity] ADD CONSTRAINT [FK_MemberChurchlifeActivity_AppUtilityNVP_ChurchlifeActivityId] FOREIGN KEY ([ChurchlifeActivityId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_ChurchBody_ExtConChurchAssociateChurchBodyId] FOREIGN KEY ([ExtConChurchAssociateChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_ChurchMember_ExtConChurchAssociateMemberId] FOREIGN KEY ([ExtConChurchAssociateMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberContact] ADD CONSTRAINT [FK_MemberContact_ChurchMember_InternalContactChurchMemberId] FOREIGN KEY ([InternalContactChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberLanguageSpoken] ADD CONSTRAINT [FK_MemberLanguageSpoken_AppUtilityNVP_LanguageSpokenId] FOREIGN KEY ([LanguageSpokenId]) REFERENCES [AppUtilityNVP] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRelation] ADD CONSTRAINT [FK_MemberRelation_ChurchBody_ExtChurchAssociateChurchBodyId] FOREIGN KEY ([ExtChurchAssociateChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRelation] ADD CONSTRAINT [FK_MemberRelation_ChurchMember_ExtChurchAssociateMemberId] FOREIGN KEY ([ExtChurchAssociateMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [MemberRelation] ADD CONSTRAINT [FK_MemberRelation_RelationshipType_RelationshipCode] FOREIGN KEY ([RelationshipCode]) REFERENCES [RelationshipType] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210416154848_ef_c_upd22', N'3.1.7');

GO


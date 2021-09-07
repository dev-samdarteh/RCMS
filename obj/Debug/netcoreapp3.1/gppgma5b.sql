ALTER TABLE [OffertoryTrans] DROP CONSTRAINT [FK_OffertoryTrans_AccountPeriod_AccountPeriodId];

GO

ALTER TABLE [OffertoryTrans] DROP CONSTRAINT [FK_OffertoryTrans_Currency_CurrencyId];

GO

ALTER TABLE [OffertoryTransDetail] DROP CONSTRAINT [FK_OffertoryTransDetail_Currency_CurrencyId];

GO

ALTER TABLE [TitheTrans] DROP CONSTRAINT [FK_TitheTrans_AccountPeriod_AccountPeriodId];

GO

ALTER TABLE [TitheTrans] DROP CONSTRAINT [FK_TitheTrans_Currency_CurrencyId];

GO

ALTER TABLE [TitheTransDetail] DROP CONSTRAINT [FK_TitheTransDetail_Currency_CurrencyId];

GO

DROP INDEX [IX_TitheTransDetail_CurrencyId] ON [TitheTransDetail];

GO

DROP INDEX [IX_TitheTrans_AccountPeriodId] ON [TitheTrans];

GO

DROP INDEX [IX_TitheTrans_CurrencyId] ON [TitheTrans];

GO

DROP INDEX [IX_OffertoryTransDetail_CurrencyId] ON [OffertoryTransDetail];

GO

DROP INDEX [IX_OffertoryTrans_AccountPeriodId] ON [OffertoryTrans];

GO

DROP INDEX [IX_OffertoryTrans_CurrencyId] ON [OffertoryTrans];

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TitheTransDetail]') AND [c].[name] = N'CurrencyId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [TitheTransDetail] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [TitheTransDetail] DROP COLUMN [CurrencyId];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TitheTrans]') AND [c].[name] = N'AccountPeriodId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [TitheTrans] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [TitheTrans] DROP COLUMN [AccountPeriodId];

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TitheTrans]') AND [c].[name] = N'CurrencyId');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [TitheTrans] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [TitheTrans] DROP COLUMN [CurrencyId];

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OffertoryTransDetail]') AND [c].[name] = N'CurrencyId');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [OffertoryTransDetail] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [OffertoryTransDetail] DROP COLUMN [CurrencyId];

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OffertoryTrans]') AND [c].[name] = N'AccountPeriodId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [OffertoryTrans] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [OffertoryTrans] DROP COLUMN [AccountPeriodId];

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OffertoryTrans]') AND [c].[name] = N'CurrencyId');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [OffertoryTrans] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [OffertoryTrans] DROP COLUMN [CurrencyId];

GO

ALTER TABLE [TitheTransDetail] ADD [CtryAlpha3Code] nvarchar(3) NULL;

GO

ALTER TABLE [TitheTransDetail] ADD [Curr3LISOSymbol] nvarchar(3) NULL;

GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TitheTrans]') AND [c].[name] = N'TitheAmount');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [TitheTrans] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [TitheTrans] ALTER COLUMN [TitheAmount] decimal(18,2) NULL;

GO

ALTER TABLE [TitheTrans] ADD [ChurchPeriodId] int NULL;

GO

ALTER TABLE [TitheTrans] ADD [CtryAlpha3Code] nvarchar(3) NULL;

GO

ALTER TABLE [TitheTrans] ADD [Curr3LISOSymbol] nvarchar(3) NULL;

GO

ALTER TABLE [OffertoryTransDetail] ADD [CtryAlpha3Code] nvarchar(3) NULL;

GO

ALTER TABLE [OffertoryTransDetail] ADD [Curr3LISOSymbol] nvarchar(3) NULL;

GO

ALTER TABLE [OffertoryTrans] ADD [ChurchPeriodId] int NULL;

GO

ALTER TABLE [OffertoryTrans] ADD [CtryAlpha3Code] nvarchar(3) NULL;

GO

ALTER TABLE [OffertoryTrans] ADD [Curr3LISOSymbol] nvarchar(3) NULL;

GO

ALTER TABLE [CUMemberRollBal] ADD [ChurchPeriodId] int NULL;

GO

ALTER TABLE [CBTitheTransBal] ADD [ChurchPeriodId] int NULL;

GO

ALTER TABLE [CBTitheTransBal] ADD [CtryAlpha3Code] nvarchar(3) NULL;

GO

ALTER TABLE [CBTitheTransBal] ADD [Curr3LISOSymbol] nvarchar(3) NULL;

GO

ALTER TABLE [CBOffertoryTransBal] ADD [ChurchPeriodId] int NULL;

GO

ALTER TABLE [CBOffertoryTransBal] ADD [CtryAlpha3Code] nvarchar(3) NULL;

GO

ALTER TABLE [CBOffertoryTransBal] ADD [Curr3LISOSymbol] nvarchar(3) NULL;

GO

ALTER TABLE [CBMemberRollBal] ADD [ChurchPeriodId] int NULL;

GO

CREATE TABLE [AssetCategory] (
    [Id] int NOT NULL IDENTITY,
    [ChurchBodyId] int NOT NULL,
    [CategoryName] nvarchar(50) NULL,
    [Description] nvarchar(100) NULL,
    [Created] datetime2 NULL,
    [LastMod] datetime2 NULL,
    [CreatedByUserId] int NULL,
    [LastModByUserId] int NULL,
    CONSTRAINT [PK_AssetCategory] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AssetCategory_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [ChurchAsset] (
    [Id] int NOT NULL IDENTITY,
    [ChurchBodyId] int NOT NULL,
    [AssetName] nvarchar(100) NULL,
    [Description] nvarchar(100) NULL,
    [AssetCategoryId] int NULL,
    [IsCapitalized] bit NOT NULL,
    [Created] datetime2 NULL,
    [LastMod] datetime2 NULL,
    [CreatedByUserId] int NULL,
    [LastModByUserId] int NULL,
    CONSTRAINT [PK_ChurchAsset] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChurchAsset_AssetCategory_AssetCategoryId] FOREIGN KEY ([AssetCategoryId]) REFERENCES [AssetCategory] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ChurchAsset_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [DonationTrans] (
    [Id] bigint NOT NULL IDENTITY,
    [AppGlobalOwnerId] int NULL,
    [ChurchBodyId] int NULL,
    [AccountPeriodId] int NULL,
    [CurrencyId] int NULL,
    [AmtEquiv] decimal(18, 2) NULL,
    [DateDonated] datetime2 NULL,
    [ChurchAssetId] int NULL,
    [RelatedEventId] int NULL,
    [ChurchFellow] nvarchar(1) NULL,
    [ChurchMemberId] int NULL,
    [DonatedByAdhoc] nvarchar(100) NULL,
    [PmntMode] nvarchar(3) NULL,
    [PmntModeRefNo] nvarchar(15) NULL,
    [PmntRefPers] nvarchar(max) NULL,
    [PostedDate] datetime2 NULL,
    [PostStatus] nvarchar(max) NULL,
    [Comments] nvarchar(100) NULL,
    [Status] nvarchar(1) NULL,
    [Created] datetime2 NULL,
    [LastMod] datetime2 NULL,
    [CreatedByUserId] int NULL,
    [LastModByUserId] int NULL,
    CONSTRAINT [PK_DonationTrans] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DonationTrans_AccountPeriod_AccountPeriodId] FOREIGN KEY ([AccountPeriodId]) REFERENCES [AccountPeriod] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DonationTrans_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DonationTrans_ChurchAsset_ChurchAssetId] FOREIGN KEY ([ChurchAssetId]) REFERENCES [ChurchAsset] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DonationTrans_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DonationTrans_ChurchMember_ChurchMemberId] FOREIGN KEY ([ChurchMemberId]) REFERENCES [ChurchMember] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DonationTrans_Currency_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DonationTrans_ChurchCalendarEvent_RelatedEventId] FOREIGN KEY ([RelatedEventId]) REFERENCES [ChurchCalendarEvent] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_TitheTransDetail_CtryAlpha3Code] ON [TitheTransDetail] ([CtryAlpha3Code]);

GO

CREATE INDEX [IX_TitheTrans_ChurchPeriodId] ON [TitheTrans] ([ChurchPeriodId]);

GO

CREATE INDEX [IX_TitheTrans_CtryAlpha3Code] ON [TitheTrans] ([CtryAlpha3Code]);

GO

CREATE INDEX [IX_OffertoryTransDetail_CtryAlpha3Code] ON [OffertoryTransDetail] ([CtryAlpha3Code]);

GO

CREATE INDEX [IX_OffertoryTrans_ChurchPeriodId] ON [OffertoryTrans] ([ChurchPeriodId]);

GO

CREATE INDEX [IX_OffertoryTrans_CtryAlpha3Code] ON [OffertoryTrans] ([CtryAlpha3Code]);

GO

CREATE INDEX [IX_CUMemberRollBal_ChurchPeriodId] ON [CUMemberRollBal] ([ChurchPeriodId]);

GO

CREATE INDEX [IX_CBTitheTransBal_ChurchPeriodId] ON [CBTitheTransBal] ([ChurchPeriodId]);

GO

CREATE INDEX [IX_CBTitheTransBal_CtryAlpha3Code] ON [CBTitheTransBal] ([CtryAlpha3Code]);

GO

CREATE INDEX [IX_CBOffertoryTransBal_ChurchPeriodId] ON [CBOffertoryTransBal] ([ChurchPeriodId]);

GO

CREATE INDEX [IX_CBOffertoryTransBal_CtryAlpha3Code] ON [CBOffertoryTransBal] ([CtryAlpha3Code]);

GO

CREATE INDEX [IX_CBMemberRollBal_ChurchPeriodId] ON [CBMemberRollBal] ([ChurchPeriodId]);

GO

CREATE INDEX [IX_AssetCategory_ChurchBodyId] ON [AssetCategory] ([ChurchBodyId]);

GO

CREATE INDEX [IX_ChurchAsset_AssetCategoryId] ON [ChurchAsset] ([AssetCategoryId]);

GO

CREATE INDEX [IX_ChurchAsset_ChurchBodyId] ON [ChurchAsset] ([ChurchBodyId]);

GO

CREATE INDEX [IX_DonationTrans_AccountPeriodId] ON [DonationTrans] ([AccountPeriodId]);

GO

CREATE INDEX [IX_DonationTrans_AppGlobalOwnerId] ON [DonationTrans] ([AppGlobalOwnerId]);

GO

CREATE INDEX [IX_DonationTrans_ChurchAssetId] ON [DonationTrans] ([ChurchAssetId]);

GO

CREATE INDEX [IX_DonationTrans_ChurchBodyId] ON [DonationTrans] ([ChurchBodyId]);

GO

CREATE INDEX [IX_DonationTrans_ChurchMemberId] ON [DonationTrans] ([ChurchMemberId]);

GO

CREATE INDEX [IX_DonationTrans_CurrencyId] ON [DonationTrans] ([CurrencyId]);

GO

CREATE INDEX [IX_DonationTrans_RelatedEventId] ON [DonationTrans] ([RelatedEventId]);

GO

ALTER TABLE [CBMemberRollBal] ADD CONSTRAINT [FK_CBMemberRollBal_ChurchPeriod_ChurchPeriodId] FOREIGN KEY ([ChurchPeriodId]) REFERENCES [ChurchPeriod] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [CBOffertoryTransBal] ADD CONSTRAINT [FK_CBOffertoryTransBal_ChurchPeriod_ChurchPeriodId] FOREIGN KEY ([ChurchPeriodId]) REFERENCES [ChurchPeriod] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [CBOffertoryTransBal] ADD CONSTRAINT [FK_CBOffertoryTransBal_Country_CtryAlpha3Code] FOREIGN KEY ([CtryAlpha3Code]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

ALTER TABLE [CBTitheTransBal] ADD CONSTRAINT [FK_CBTitheTransBal_ChurchPeriod_ChurchPeriodId] FOREIGN KEY ([ChurchPeriodId]) REFERENCES [ChurchPeriod] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [CBTitheTransBal] ADD CONSTRAINT [FK_CBTitheTransBal_Country_CtryAlpha3Code] FOREIGN KEY ([CtryAlpha3Code]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

ALTER TABLE [CUMemberRollBal] ADD CONSTRAINT [FK_CUMemberRollBal_ChurchPeriod_ChurchPeriodId] FOREIGN KEY ([ChurchPeriodId]) REFERENCES [ChurchPeriod] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [OffertoryTrans] ADD CONSTRAINT [FK_OffertoryTrans_ChurchPeriod_ChurchPeriodId] FOREIGN KEY ([ChurchPeriodId]) REFERENCES [ChurchPeriod] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [OffertoryTrans] ADD CONSTRAINT [FK_OffertoryTrans_Country_CtryAlpha3Code] FOREIGN KEY ([CtryAlpha3Code]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

ALTER TABLE [OffertoryTransDetail] ADD CONSTRAINT [FK_OffertoryTransDetail_Country_CtryAlpha3Code] FOREIGN KEY ([CtryAlpha3Code]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

ALTER TABLE [TitheTrans] ADD CONSTRAINT [FK_TitheTrans_ChurchPeriod_ChurchPeriodId] FOREIGN KEY ([ChurchPeriodId]) REFERENCES [ChurchPeriod] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [TitheTrans] ADD CONSTRAINT [FK_TitheTrans_Country_CtryAlpha3Code] FOREIGN KEY ([CtryAlpha3Code]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

ALTER TABLE [TitheTransDetail] ADD CONSTRAINT [FK_TitheTransDetail_Country_CtryAlpha3Code] FOREIGN KEY ([CtryAlpha3Code]) REFERENCES [Country] ([CtryAlpha3Code]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210602112434_ef_c_upd31', N'3.1.7');

GO

ALTER TABLE [ChurchCalendarEvent] DROP CONSTRAINT [FK_ChurchCalendarEvent_ChurchEventCategory_ChurchEventCategoryId];

GO

ALTER TABLE [ChurchCalendarEvent] DROP CONSTRAINT [FK_ChurchCalendarEvent_ChurchlifeActivity_ChurchlifeActivityId];

GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MemberRank]') AND [c].[name] = N'OwnedByChurchBodyId');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [MemberRank] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [MemberRank] DROP COLUMN [OwnedByChurchBodyId];

GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChurchCalendarEvent]') AND [c].[name] = N'OwnedByChurchBodyId');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [ChurchCalendarEvent] DROP CONSTRAINT [' + @var8 + '];');
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


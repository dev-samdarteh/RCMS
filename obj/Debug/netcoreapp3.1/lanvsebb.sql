ALTER TABLE [AppUtilityNVP] ADD [NVPNumValTo] decimal(18, 2) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210524144704_ef_c_upd24', N'3.1.7');

GO

CREATE TABLE [CBMemberRollBal] (
    [Id] int NOT NULL IDENTITY,
    [AppGlobalOwnerId] int NULL,
    [ChurchBodyId] int NULL,
    [TotRoll] bigint NOT NULL,
    [Tot_M] bigint NOT NULL,
    [Tot_F] bigint NOT NULL,
    [Tot_O] bigint NOT NULL,
    [Tot_C] bigint NOT NULL,
    [Tot_Y] bigint NOT NULL,
    [Tot_YA] bigint NOT NULL,
    [Tot_MA] bigint NOT NULL,
    [Tot_OA] bigint NOT NULL,
    [Created] datetime2 NULL,
    [LastMod] datetime2 NULL,
    [CreatedByUserId] int NULL,
    [LastModByUserId] int NULL,
    CONSTRAINT [PK_CBMemberRollBal] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CBMemberRollBal_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CBMemberRollBal_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [CUMemberRollBal] (
    [Id] int NOT NULL IDENTITY,
    [AppGlobalOwnerId] int NULL,
    [ChurchBodyId] int NULL,
    [ChurchUnitId] int NULL,
    [TotRoll] bigint NOT NULL,
    [Tot_M] bigint NOT NULL,
    [Tot_F] bigint NOT NULL,
    [Tot_O] bigint NOT NULL,
    [Tot_C] bigint NOT NULL,
    [Tot_Y] bigint NOT NULL,
    [Tot_YA] bigint NOT NULL,
    [Tot_MA] bigint NOT NULL,
    [Tot_OA] bigint NOT NULL,
    [Created] datetime2 NULL,
    [LastMod] datetime2 NULL,
    [CreatedByUserId] int NULL,
    [LastModByUserId] int NULL,
    CONSTRAINT [PK_CUMemberRollBal] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CUMemberRollBal_AppGlobalOwner_AppGlobalOwnerId] FOREIGN KEY ([AppGlobalOwnerId]) REFERENCES [AppGlobalOwner] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CUMemberRollBal_ChurchBody_ChurchBodyId] FOREIGN KEY ([ChurchBodyId]) REFERENCES [ChurchBody] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CUMemberRollBal_ChurchUnit_ChurchUnitId] FOREIGN KEY ([ChurchUnitId]) REFERENCES [ChurchUnit] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_CBMemberRollBal_AppGlobalOwnerId] ON [CBMemberRollBal] ([AppGlobalOwnerId]);

GO

CREATE INDEX [IX_CBMemberRollBal_ChurchBodyId] ON [CBMemberRollBal] ([ChurchBodyId]);

GO

CREATE INDEX [IX_CUMemberRollBal_AppGlobalOwnerId] ON [CUMemberRollBal] ([AppGlobalOwnerId]);

GO

CREATE INDEX [IX_CUMemberRollBal_ChurchBodyId] ON [CUMemberRollBal] ([ChurchBodyId]);

GO

CREATE INDEX [IX_CUMemberRollBal_ChurchUnitId] ON [CUMemberRollBal] ([ChurchUnitId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210525164638_ef_c_upd25', N'3.1.7');

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CBMemberRollBal]') AND [c].[name] = N'Tot_OA');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [CBMemberRollBal] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [CBMemberRollBal] DROP COLUMN [Tot_OA];

GO

ALTER TABLE [CBMemberRollBal] ADD [Tot_AA] bigint NOT NULL DEFAULT CAST(0 AS bigint);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210526023900_ef_c_upd26', N'3.1.7');

GO


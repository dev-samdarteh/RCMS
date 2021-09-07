ALTER TABLE [AppUtilityNVP] ADD [NVPNumValTo] decimal(18, 2) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210524144704_ef_c_upd24', N'3.1.7');

GO


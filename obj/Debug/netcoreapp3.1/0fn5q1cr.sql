﻿ALTER TABLE [MSTRCountry] ADD [CapitalCity] nvarchar(50) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210325132510_ef_m_upd2', N'3.1.7');

GO

ALTER TABLE [UserProfile] ADD [IsCLNTInit] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [UserProfile] ADD [IsMSTRInit] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210504143459_ef_m_upd3', N'3.1.7');

GO


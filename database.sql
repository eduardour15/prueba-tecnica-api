IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE TABLE [BitacoraAuditoria] (
        [Id] bigint NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [Usuario] nvarchar(100) NOT NULL,
        [Accion] nvarchar(50) NOT NULL,
        [Entidad] nvarchar(50) NOT NULL,
        [EntidadId] nvarchar(50) NOT NULL,
        [Detalle] nvarchar(2000) NOT NULL,
        [CorrelationId] nvarchar(64) NOT NULL,
        [IpOrigen] nvarchar(45) NULL,
        CONSTRAINT [PK_BitacoraAuditoria] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE TABLE [Clientes] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(150) NOT NULL,
        [Identificacion] nvarchar(30) NOT NULL,
        [Correo] nvarchar(150) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(100) NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(100) NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE TABLE [Coberturas] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Tasa] decimal(5,2) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(100) NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(100) NULL,
        CONSTRAINT [PK_Coberturas] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE TABLE [Vehiculos] (
        [Id] int NOT NULL IDENTITY,
        [Placa] nvarchar(15) NOT NULL,
        [Marca] nvarchar(60) NOT NULL,
        [Modelo] nvarchar(60) NOT NULL,
        [Anio] int NOT NULL,
        [ValorComercial] decimal(18,2) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(100) NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(100) NULL,
        CONSTRAINT [PK_Vehiculos] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE TABLE [Polizas] (
        [Id] int NOT NULL IDENTITY,
        [NumeroPoliza] nvarchar(30) NOT NULL,
        [ClienteId] int NOT NULL,
        [VehiculoId] int NOT NULL,
        [FechaEmision] datetime2 NOT NULL,
        [SumaAsegurada] decimal(18,2) NOT NULL,
        [PrimaTotal] decimal(18,2) NOT NULL,
        [Activa] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(100) NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(100) NULL,
        CONSTRAINT [PK_Polizas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Polizas_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Polizas_Vehiculos_VehiculoId] FOREIGN KEY ([VehiculoId]) REFERENCES [Vehiculos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE TABLE [PolizaCoberturas] (
        [PolizaId] int NOT NULL,
        [CoberturaId] int NOT NULL,
        [TasaAplicada] decimal(5,2) NOT NULL,
        CONSTRAINT [PK_PolizaCoberturas] PRIMARY KEY ([PolizaId], [CoberturaId]),
        CONSTRAINT [FK_PolizaCoberturas_Coberturas_CoberturaId] FOREIGN KEY ([CoberturaId]) REFERENCES [Coberturas] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PolizaCoberturas_Polizas_PolizaId] FOREIGN KEY ([PolizaId]) REFERENCES [Polizas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Correo', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'Identificacion', N'ModificadoPor', N'Nombre') AND [object_id] = OBJECT_ID(N'[Clientes]'))
        SET IDENTITY_INSERT [Clientes] ON;
    EXEC(N'INSERT INTO [Clientes] ([Id], [Correo], [CreadoPor], [FechaCreacion], [FechaModificacion], [Identificacion], [ModificadoPor], [Nombre])
    VALUES (1, N''maria.lopez@correo.com'', N''Seed'', ''2026-01-01T00:00:00.0000000Z'', NULL, N''001-150490-0012A'', NULL, N''María Fernanda López''),
    (2, N''carlos.martinez@correo.com'', N''Seed'', ''2026-01-01T00:00:00.0000000Z'', NULL, N''001-220885-0045B'', NULL, N''Carlos Eduardo Martínez'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Correo', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'Identificacion', N'ModificadoPor', N'Nombre') AND [object_id] = OBJECT_ID(N'[Clientes]'))
        SET IDENTITY_INSERT [Clientes] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'ModificadoPor', N'Nombre', N'Tasa') AND [object_id] = OBJECT_ID(N'[Coberturas]'))
        SET IDENTITY_INSERT [Coberturas] ON;
    EXEC(N'INSERT INTO [Coberturas] ([Id], [CreadoPor], [FechaCreacion], [FechaModificacion], [ModificadoPor], [Nombre], [Tasa])
    VALUES (1, N''Seed'', ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, N''Robo'', 3.5),
    (2, N''Seed'', ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, N''Choque'', 4.5),
    (3, N''Seed'', ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, N''Responsabilidad Civil'', 2.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'ModificadoPor', N'Nombre', N'Tasa') AND [object_id] = OBJECT_ID(N'[Coberturas]'))
        SET IDENTITY_INSERT [Coberturas] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE INDEX [IX_BitacoraAuditoria_Entidad_EntidadId] ON [BitacoraAuditoria] ([Entidad], [EntidadId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE INDEX [IX_BitacoraAuditoria_Fecha] ON [BitacoraAuditoria] ([Fecha]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clientes_Identificacion] ON [Clientes] ([Identificacion]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Coberturas_Nombre] ON [Coberturas] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE INDEX [IX_PolizaCoberturas_CoberturaId] ON [PolizaCoberturas] ([CoberturaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Polizas_ClienteId_VehiculoId] ON [Polizas] ([ClienteId], [VehiculoId]) WHERE [Activa] = 1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Polizas_NumeroPoliza] ON [Polizas] ([NumeroPoliza]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE INDEX [IX_Polizas_VehiculoId] ON [Polizas] ([VehiculoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vehiculos_Placa] ON [Vehiculos] ([Placa]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261118205856_CreacionInicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261118205856_CreacionInicial', N'10.0.15');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261119021720_UnicaPolizaActivaPorVehiculo'
)
BEGIN
    DROP INDEX [IX_Polizas_VehiculoId] ON [Polizas];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261119021720_UnicaPolizaActivaPorVehiculo'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Polizas_VehiculoId] ON [Polizas] ([VehiculoId]) WHERE [Activa] = 1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261119021720_UnicaPolizaActivaPorVehiculo'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261119021720_UnicaPolizaActivaPorVehiculo', N'10.0.15');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261119022745_EliminarIndiceRedundantePoliza'
)
BEGIN
    DROP INDEX [IX_Polizas_ClienteId_VehiculoId] ON [Polizas];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261119022745_EliminarIndiceRedundantePoliza'
)
BEGIN
    CREATE INDEX [IX_Polizas_ClienteId] ON [Polizas] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261119022745_EliminarIndiceRedundantePoliza'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261119022745_EliminarIndiceRedundantePoliza', N'10.0.15');
END;

COMMIT;
GO


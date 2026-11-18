using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PruebaTecnica.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreacionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BitacoraAuditoria",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntidadId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IpOrigen = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraAuditoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coberturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tasa = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coberturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Placa = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    ValorComercial = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Polizas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroPoliza = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    VehiculoId = table.Column<int>(type: "int", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SumaAsegurada = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrimaTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polizas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Polizas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Polizas_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PolizaCoberturas",
                columns: table => new
                {
                    PolizaId = table.Column<int>(type: "int", nullable: false),
                    CoberturaId = table.Column<int>(type: "int", nullable: false),
                    TasaAplicada = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolizaCoberturas", x => new { x.PolizaId, x.CoberturaId });
                    table.ForeignKey(
                        name: "FK_PolizaCoberturas_Coberturas_CoberturaId",
                        column: x => x.CoberturaId,
                        principalTable: "Coberturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolizaCoberturas_Polizas_PolizaId",
                        column: x => x.PolizaId,
                        principalTable: "Polizas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Correo", "CreadoPor", "FechaCreacion", "FechaModificacion", "Identificacion", "ModificadoPor", "Nombre" },
                values: new object[,]
                {
                    { 1, "maria.lopez@correo.com", "Seed", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "001-150490-0012A", null, "María Fernanda López" },
                    { 2, "carlos.martinez@correo.com", "Seed", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "001-220885-0045B", null, "Carlos Eduardo Martínez" }
                });

            migrationBuilder.InsertData(
                table: "Coberturas",
                columns: new[] { "Id", "CreadoPor", "FechaCreacion", "FechaModificacion", "ModificadoPor", "Nombre", "Tasa" },
                values: new object[,]
                {
                    { 1, "Seed", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Robo", 3.50m },
                    { 2, "Seed", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Choque", 4.50m },
                    { 3, "Seed", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Responsabilidad Civil", 2.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraAuditoria_Entidad_EntidadId",
                table: "BitacoraAuditoria",
                columns: new[] { "Entidad", "EntidadId" });

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraAuditoria_Fecha",
                table: "BitacoraAuditoria",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Identificacion",
                table: "Clientes",
                column: "Identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coberturas_Nombre",
                table: "Coberturas",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolizaCoberturas_CoberturaId",
                table: "PolizaCoberturas",
                column: "CoberturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_ClienteId_VehiculoId",
                table: "Polizas",
                columns: new[] { "ClienteId", "VehiculoId" },
                unique: true,
                filter: "[Activa] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_NumeroPoliza",
                table: "Polizas",
                column: "NumeroPoliza",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_VehiculoId",
                table: "Polizas",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Placa",
                table: "Vehiculos",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BitacoraAuditoria");

            migrationBuilder.DropTable(
                name: "PolizaCoberturas");

            migrationBuilder.DropTable(
                name: "Coberturas");

            migrationBuilder.DropTable(
                name: "Polizas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Vehiculos");
        }
    }
}

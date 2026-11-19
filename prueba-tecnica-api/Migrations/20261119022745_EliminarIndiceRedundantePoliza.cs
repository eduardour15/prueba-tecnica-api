using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaTecnica.Api.Migrations
{
    /// <inheritdoc />
    public partial class EliminarIndiceRedundantePoliza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Polizas_ClienteId_VehiculoId",
                table: "Polizas");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_ClienteId",
                table: "Polizas",
                column: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Polizas_ClienteId",
                table: "Polizas");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_ClienteId_VehiculoId",
                table: "Polizas",
                columns: new[] { "ClienteId", "VehiculoId" },
                unique: true,
                filter: "[Activa] = 1");
        }
    }
}

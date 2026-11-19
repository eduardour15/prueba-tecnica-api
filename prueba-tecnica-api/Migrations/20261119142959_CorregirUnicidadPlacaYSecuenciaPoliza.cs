using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaTecnica.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorregirUnicidadPlacaYSecuenciaPoliza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "SeqNumeroPoliza");

            migrationBuilder.AddColumn<string>(
                name: "PlacaCanonica",
                table: "Vehiculos",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                computedColumnSql: "CAST(REPLACE([Placa], N'-', N'') AS nvarchar(15))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_PlacaCanonica",
                table: "Vehiculos",
                column: "PlacaCanonica",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_PlacaCanonica",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "PlacaCanonica",
                table: "Vehiculos");

            migrationBuilder.DropSequence(
                name: "SeqNumeroPoliza");
        }
    }
}

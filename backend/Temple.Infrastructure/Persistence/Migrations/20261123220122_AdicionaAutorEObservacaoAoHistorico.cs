using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temple.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaAutorEObservacaoAoHistorico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "HistoricoCargos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PromovidoPorId",
                table: "HistoricoCargos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromovidoPorNome",
                table: "HistoricoCargos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RegistradoEm",
                table: "HistoricoCargos",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "HistoricoCargos");

            migrationBuilder.DropColumn(
                name: "PromovidoPorId",
                table: "HistoricoCargos");

            migrationBuilder.DropColumn(
                name: "PromovidoPorNome",
                table: "HistoricoCargos");

            migrationBuilder.DropColumn(
                name: "RegistradoEm",
                table: "HistoricoCargos");
        }
    }
}

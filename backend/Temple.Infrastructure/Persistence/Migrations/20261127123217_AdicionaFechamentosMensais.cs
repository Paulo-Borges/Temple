using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temple.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaFechamentosMensais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FechamentosMensais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IgrejaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    SaldoAnterior = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEntradas = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalSaidas = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SaldoFinal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechadoPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechadoPorNome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechadoEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MotivoDaReabertura = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReabertoPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReabertoPorNome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ReabertoEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FechamentosMensais", x => x.Id);
                    table.CheckConstraint("CK_FechamentosMensais_Mes", "[Mes] BETWEEN 1 AND 12");
                    table.CheckConstraint("CK_FechamentosMensais_Reabertura", "([MotivoDaReabertura] IS NULL AND [ReabertoPorId] IS NULL AND [ReabertoPorNome] IS NULL AND [ReabertoEm] IS NULL) OR ([MotivoDaReabertura] IS NOT NULL AND [ReabertoPorId] IS NOT NULL AND [ReabertoPorNome] IS NOT NULL AND [ReabertoEm] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_FechamentosMensais_Igrejas_IgrejaId",
                        column: x => x.IgrejaId,
                        principalTable: "Igrejas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FechamentosMensais_IgrejaId_Ano_Mes",
                table: "FechamentosMensais",
                columns: new[] { "IgrejaId", "Ano", "Mes" },
                unique: true,
                filter: "[ReabertoEm] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FechamentosMensais");
        }
    }
}

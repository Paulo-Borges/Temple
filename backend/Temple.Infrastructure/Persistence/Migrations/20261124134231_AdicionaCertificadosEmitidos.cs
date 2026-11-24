using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temple.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCertificadosEmitidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificadosEmitidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    MembroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HistoricoCargoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmitidoPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmitidoPorNome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmitidoEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificadosEmitidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificadosEmitidos_HistoricoCargos_HistoricoCargoId",
                        column: x => x.HistoricoCargoId,
                        principalTable: "HistoricoCargos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CertificadosEmitidos_Membros_MembroId",
                        column: x => x.MembroId,
                        principalTable: "Membros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosEmitidos_HistoricoCargoId",
                table: "CertificadosEmitidos",
                column: "HistoricoCargoId",
                unique: true,
                filter: "[HistoricoCargoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosEmitidos_MembroId",
                table: "CertificadosEmitidos",
                column: "MembroId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosEmitidos_Numero",
                table: "CertificadosEmitidos",
                column: "Numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificadosEmitidos");
        }
    }
}

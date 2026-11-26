using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temple.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaLancamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lancamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IgrejaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    MembroId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Categoria = table.Column<int>(type: "int", nullable: true),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FormaPagamento = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RegistradoPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistradoPorNome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RegistradoEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MotivoDoEstorno = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstornadoPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EstornadoPorNome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EstornadoEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lancamentos", x => x.Id);
                    table.CheckConstraint("CK_Lancamentos_Categoria", "([Tipo] = 4 AND [Categoria] IS NOT NULL) OR ([Tipo] <> 4 AND [Categoria] IS NULL)");
                    table.CheckConstraint("CK_Lancamentos_Estorno", "([MotivoDoEstorno] IS NULL AND [EstornadoPorId] IS NULL AND [EstornadoPorNome] IS NULL AND [EstornadoEm] IS NULL) OR ([MotivoDoEstorno] IS NOT NULL AND [EstornadoPorId] IS NOT NULL AND [EstornadoPorNome] IS NOT NULL AND [EstornadoEm] IS NOT NULL)");
                    table.CheckConstraint("CK_Lancamentos_Membro", "([Tipo] = 1 AND [MembroId] IS NOT NULL) OR [Tipo] = 2 OR ([Tipo] IN (3, 4) AND [MembroId] IS NULL)");
                    table.CheckConstraint("CK_Lancamentos_Valor", "[Valor] > 0");
                    table.ForeignKey(
                        name: "FK_Lancamentos_Igrejas_IgrejaId",
                        column: x => x.IgrejaId,
                        principalTable: "Igrejas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lancamentos_Membros_MembroId",
                        column: x => x.MembroId,
                        principalTable: "Membros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_IgrejaId_Data",
                table: "Lancamentos",
                columns: new[] { "IgrejaId", "Data" });

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_MembroId_Data",
                table: "Lancamentos",
                columns: new[] { "MembroId", "Data" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lancamentos");
        }
    }
}

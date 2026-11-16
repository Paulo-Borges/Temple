using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temple.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Igrejas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cnpj = table.Column<string>(type: "nchar(14)", fixedLength: true, maxLength: 14, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DataFundacao = table.Column<DateOnly>(type: "date", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    IgrejaMatrizId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cep = table.Column<string>(type: "nchar(8)", fixedLength: true, maxLength: 8, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Logradouro = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Uf = table.Column<string>(type: "nchar(2)", fixedLength: true, maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Igrejas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Igrejas_Igrejas_IgrejaMatrizId",
                        column: x => x.IgrejaMatrizId,
                        principalTable: "Igrejas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Membros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Matricula = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    CargoAtual = table.Column<int>(type: "int", nullable: false),
                    Grupo = table.Column<int>(type: "int", nullable: true),
                    IgrejaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cep = table.Column<string>(type: "nchar(8)", fixedLength: true, maxLength: 8, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Logradouro = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Uf = table.Column<string>(type: "nchar(2)", fixedLength: true, maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Membros_Igrejas_IgrejaId",
                        column: x => x.IgrejaId,
                        principalTable: "Igrejas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoCargos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CargoAnterior = table.Column<int>(type: "int", nullable: false),
                    CargoNovo = table.Column<int>(type: "int", nullable: false),
                    DataPromocao = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoCargos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoCargos_Membros_MembroId",
                        column: x => x.MembroId,
                        principalTable: "Membros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoCargos_MembroId",
                table: "HistoricoCargos",
                column: "MembroId");

            migrationBuilder.CreateIndex(
                name: "IX_Igrejas_Cnpj",
                table: "Igrejas",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Igrejas_IgrejaMatrizId",
                table: "Igrejas",
                column: "IgrejaMatrizId");

            migrationBuilder.CreateIndex(
                name: "IX_Membros_IgrejaId",
                table: "Membros",
                column: "IgrejaId");

            migrationBuilder.CreateIndex(
                name: "IX_Membros_Matricula",
                table: "Membros",
                column: "Matricula",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoCargos");

            migrationBuilder.DropTable(
                name: "Membros");

            migrationBuilder.DropTable(
                name: "Igrejas");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temple.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaBatismosEApresentacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "MembroId",
                table: "CertificadosEmitidos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ApresentacaoCriancaId",
                table: "CertificadosEmitidos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatismoId",
                table: "CertificadosEmitidos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApresentacoesCriancas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IgrejaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeDaCrianca = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DataNascimentoDaCrianca = table.Column<DateOnly>(type: "date", nullable: false),
                    NomeDoPai = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NomeDaMae = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DataApresentacao = table.Column<DateOnly>(type: "date", nullable: false),
                    Oficiante = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApresentacoesCriancas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApresentacoesCriancas_Igrejas_IgrejaId",
                        column: x => x.IgrejaId,
                        principalTable: "Igrejas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Batismos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataBatismo = table.Column<DateOnly>(type: "date", nullable: false),
                    Oficiante = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Local = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batismos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batismos_Membros_MembroId",
                        column: x => x.MembroId,
                        principalTable: "Membros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosEmitidos_ApresentacaoCriancaId",
                table: "CertificadosEmitidos",
                column: "ApresentacaoCriancaId",
                unique: true,
                filter: "[ApresentacaoCriancaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosEmitidos_BatismoId",
                table: "CertificadosEmitidos",
                column: "BatismoId",
                unique: true,
                filter: "[BatismoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApresentacoesCriancas_IgrejaId",
                table: "ApresentacoesCriancas",
                column: "IgrejaId");

            migrationBuilder.CreateIndex(
                name: "IX_Batismos_MembroId",
                table: "Batismos",
                column: "MembroId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CertificadosEmitidos_ApresentacoesCriancas_ApresentacaoCriancaId",
                table: "CertificadosEmitidos",
                column: "ApresentacaoCriancaId",
                principalTable: "ApresentacoesCriancas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CertificadosEmitidos_Batismos_BatismoId",
                table: "CertificadosEmitidos",
                column: "BatismoId",
                principalTable: "Batismos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CertificadosEmitidos_ApresentacoesCriancas_ApresentacaoCriancaId",
                table: "CertificadosEmitidos");

            migrationBuilder.DropForeignKey(
                name: "FK_CertificadosEmitidos_Batismos_BatismoId",
                table: "CertificadosEmitidos");

            migrationBuilder.DropTable(
                name: "ApresentacoesCriancas");

            migrationBuilder.DropTable(
                name: "Batismos");

            migrationBuilder.DropIndex(
                name: "IX_CertificadosEmitidos_ApresentacaoCriancaId",
                table: "CertificadosEmitidos");

            migrationBuilder.DropIndex(
                name: "IX_CertificadosEmitidos_BatismoId",
                table: "CertificadosEmitidos");

            migrationBuilder.DropColumn(
                name: "ApresentacaoCriancaId",
                table: "CertificadosEmitidos");

            migrationBuilder.DropColumn(
                name: "BatismoId",
                table: "CertificadosEmitidos");

            migrationBuilder.AlterColumn<Guid>(
                name: "MembroId",
                table: "CertificadosEmitidos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}

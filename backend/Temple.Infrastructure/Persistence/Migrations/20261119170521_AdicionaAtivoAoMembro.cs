using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temple.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaAtivoAoMembro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Membros",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Membros");
        }
    }
}

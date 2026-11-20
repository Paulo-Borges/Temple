using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temple.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VinculaUsuarioAoMembro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MembroId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MembroId",
                table: "AspNetUsers",
                column: "MembroId",
                unique: true,
                filter: "[MembroId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Membros_MembroId",
                table: "AspNetUsers",
                column: "MembroId",
                principalTable: "Membros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Membros_MembroId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MembroId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MembroId",
                table: "AspNetUsers");
        }
    }
}

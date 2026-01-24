using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeHavuzu.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameProjetLinkToProjectLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjetLink",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "AdSoyad",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Bolum",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Fakulte",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Percentile",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectLink",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Percentile",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectLink",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "ProjetLink",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdSoyad",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Bolum",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Fakulte",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeHavuzu.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprovedByConsultant",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Projects");

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedByConsultant",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

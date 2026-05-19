using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeHavuzu.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffNumberAndAcademicTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcademicTitle",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademicTitle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StaffNumber",
                table: "AspNetUsers");
        }
    }
}

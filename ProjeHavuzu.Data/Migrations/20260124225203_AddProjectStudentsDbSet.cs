using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeHavuzu.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectStudentsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectStudentId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectStudentId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectStudents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Descripiton = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStudents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectStudentId",
                table: "Projects",
                column: "ProjectStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProjectStudentId",
                table: "AspNetUsers",
                column: "ProjectStudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ProjectStudents_ProjectStudentId",
                table: "AspNetUsers",
                column: "ProjectStudentId",
                principalTable: "ProjectStudents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectStudents_ProjectStudentId",
                table: "Projects",
                column: "ProjectStudentId",
                principalTable: "ProjectStudents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ProjectStudents_ProjectStudentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectStudents_ProjectStudentId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectStudents");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectStudentId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProjectStudentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProjectStudentId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectStudentId",
                table: "AspNetUsers");
        }
    }
}

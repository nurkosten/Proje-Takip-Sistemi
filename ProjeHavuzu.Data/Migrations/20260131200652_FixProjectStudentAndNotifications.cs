using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeHavuzu.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixProjectStudentAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ProjectStudents_ProjectStudentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectStudents_ProjectStudentId",
                table: "Projects");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "ProjectStudents",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudents_ProjectId",
                table: "ProjectStudents",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudents_StudentId",
                table: "ProjectStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudents_AspNetUsers_StudentId",
                table: "ProjectStudents",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudents_Projects_ProjectId",
                table: "ProjectStudents",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStudents_AspNetUsers_StudentId",
                table: "ProjectStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStudents_Projects_ProjectId",
                table: "ProjectStudents");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStudents_ProjectId",
                table: "ProjectStudents");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStudents_StudentId",
                table: "ProjectStudents");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "ProjectStudents");

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
    }
}

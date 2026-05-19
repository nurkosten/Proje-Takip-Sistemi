using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeHavuzu.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixProjectStudentRelations : Migration
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

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudents_ProjectId_StudentId",
                table: "ProjectStudents",
                columns: new[] { "ProjectId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudents_StudentId",
                table: "ProjectStudents",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudents_AspNetUsers_StudentId",
                table: "ProjectStudents",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudents_Projects_ProjectId",
                table: "ProjectStudents",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.DropIndex(
                name: "IX_ProjectStudents_ProjectId_StudentId",
                table: "ProjectStudents");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStudents_StudentId",
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

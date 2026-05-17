using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeHavuzu.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvisorApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_ConsultantId",
                table: "Projects");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_ConsultantId",
                table: "Projects",
                column: "ConsultantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_ConsultantId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_ConsultantId",
                table: "Projects",
                column: "ConsultantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

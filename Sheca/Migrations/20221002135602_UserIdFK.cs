using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sheca.Migrations
{
    public partial class UserIdFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Event",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Course",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Event_UserId",
                table: "Event",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_UserId",
                table: "Course",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_User_UserId",
                table: "Course",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_User_UserId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_UserId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Course_UserId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Course");
        }
    }
}

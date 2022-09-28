using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sheca.Migrations
{
    public partial class CourseTaskUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotiTime",
                table: "Event");

            migrationBuilder.AddColumn<int>(
                name: "NotiBeforeTime",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "Course",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NotiBeforeTime",
                table: "Course",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotiBeforeTime",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NotiBeforeTime",
                table: "Course");

            migrationBuilder.AddColumn<DateTime>(
                name: "NotiTime",
                table: "Event",
                type: "datetime2",
                nullable: true);
        }
    }
}

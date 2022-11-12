using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sheca.Migrations
{
    public partial class DayOfWeek_Course : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DayOfWeeks",
                table: "Course",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeeks",
                table: "Course");
        }
    }
}

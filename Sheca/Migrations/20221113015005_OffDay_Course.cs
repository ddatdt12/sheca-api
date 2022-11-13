using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sheca.Migrations
{
    public partial class OffDay_Course : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OffDays",
                table: "Course",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffDays",
                table: "Course");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sheca.Migrations
{
    public partial class RemoveCourseIdInModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Course_CourseId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_CourseId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Event");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_CourseId",
                table: "Event",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Course_CourseId",
                table: "Event",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sheca.Migrations
{
    public partial class EventModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotiTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaseEvent = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    RecurringStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecurringInterval = table.Column<int>(type: "int", nullable: false),
                    RecurringEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExceptDates = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_CourseId",
                table: "Event",
                column: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event");
        }
    }
}

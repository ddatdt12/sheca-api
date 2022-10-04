using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sheca.Migrations
{
    public partial class RenameBaseEventCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BaseEvent",
                table: "Event",
                newName: "BaseEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_BaseEventId",
                table: "Event",
                column: "BaseEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Event_BaseEventId",
                table: "Event",
                column: "BaseEventId",
                principalTable: "Event",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Event_BaseEventId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_BaseEventId",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "BaseEventId",
                table: "Event",
                newName: "BaseEvent");
        }
    }
}

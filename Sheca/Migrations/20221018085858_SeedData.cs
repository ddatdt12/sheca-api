using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sheca.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Password" },
                values: new object[] { new Guid("077f0ae7-b699-40a3-b22e-1f065705b8e3"), "test2@gmail.com", "123123123" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Password" },
                values: new object[] { new Guid("d0ee9b2a-71cd-4d32-a778-0461ca0f64ff"), "test@gmail.com", "123123123" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("077f0ae7-b699-40a3-b22e-1f065705b8e3"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("d0ee9b2a-71cd-4d32-a778-0461ca0f64ff"));
        }
    }
}

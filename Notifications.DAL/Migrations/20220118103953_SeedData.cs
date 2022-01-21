using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "58bc8002-0f03-4ed2-a7a3-8733a47e9ed1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8936f175-12c8-4567-ab38-9f2a514a897f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "283af1bc-58b8-41f6-a97a-783edfde9590", "fc945696-4b1b-41e9-bef2-353654511571", "Manager", "MANAGER" },
                    { "858a26b6-d542-40af-88ee-606484163a8d", "721c2327-4221-4492-a001-4cfbf23207f3", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 1L, "Universal" },
                    { 2L, "Quarantine" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "283af1bc-58b8-41f6-a97a-783edfde9590");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "858a26b6-d542-40af-88ee-606484163a8d");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2L);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "58bc8002-0f03-4ed2-a7a3-8733a47e9ed1", "fdf0a5de-7855-4e5b-bdf1-e47074977918", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8936f175-12c8-4567-ab38-9f2a514a897f", "3c944d8f-867e-43bc-820a-ade69682ca06", "Admin", "ADMIN" });
        }
    }
}

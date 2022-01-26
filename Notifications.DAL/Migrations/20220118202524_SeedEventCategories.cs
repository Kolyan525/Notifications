using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class SeedEventCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "283af1bc-58b8-41f6-a97a-783edfde9590");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "858a26b6-d542-40af-88ee-606484163a8d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "afede725-f382-4cda-b6c0-76c9d40dda09", "c94bfd31-098c-43d5-bb1e-3d7bc1944882", "Manager", "MANAGER" },
                    { "b224f9d1-db23-416a-a974-1b78c93c935f", "99d34d1b-aa1a-4883-862e-3eaf0de61888", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "EventId", "Description", "EventLink", "ShortDesc", "StartAt", "Title" },
                values: new object[,]
                {
                    { 1L, "Dear students! for the next three weeks we need all together (students and teachers) to unite so as not to lose precious time of the second semester. Therefore an online learning will be established.", "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA/edit", "Very short description for online learning", new DateTime(2022, 1, 18, 0, 0, 0, 0, DateTimeKind.Local), "Online Learning in NaU\"OA\" Starts" },
                    { 2L, "Congratulations, My name is Natalia, I deal with international rankings and NaU\"OA\" membership in them. This year, U - Multirank is conducting a survey amongstudents majoring in Computer Science. Please contribute to the high place of Na\"OA\" in this ranking by filling out a small survey. I quote the letter below", "https://che-survey.de/uc/umr2022/ ", "Very short description for international rating", new DateTime(2021, 12, 20, 11, 24, 0, 0, DateTimeKind.Unspecified), "International rating" }
                });

            migrationBuilder.InsertData(
                table: "EventCategories",
                columns: new[] { "EventCategoryId", "CategoryId", "EventId" },
                values: new object[] { 1L, 1L, 1L });

            migrationBuilder.InsertData(
                table: "EventCategories",
                columns: new[] { "EventCategoryId", "CategoryId", "EventId" },
                values: new object[] { 2L, 2L, 1L });

            migrationBuilder.InsertData(
                table: "EventCategories",
                columns: new[] { "EventCategoryId", "CategoryId", "EventId" },
                values: new object[] { 3L, 1L, 2L });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "afede725-f382-4cda-b6c0-76c9d40dda09");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b224f9d1-db23-416a-a974-1b78c93c935f");

            migrationBuilder.DeleteData(
                table: "EventCategories",
                keyColumn: "EventCategoryId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "EventCategories",
                keyColumn: "EventCategoryId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "EventCategories",
                keyColumn: "EventCategoryId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "EventId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "EventId",
                keyValue: 2L);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "283af1bc-58b8-41f6-a97a-783edfde9590", "fc945696-4b1b-41e9-bef2-353654511571", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "858a26b6-d542-40af-88ee-606484163a8d", "721c2327-4221-4492-a001-4cfbf23207f3", "Admin", "ADMIN" });
        }
    }
}

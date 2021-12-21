using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class SeedingData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 1L, "Universal" },
                    { 2L, "Quarantine" }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "EventId", "Description", "EventLink", "ShortDesc", "StartAt", "Title" },
                values: new object[,]
                {
                    { 1L, "Dear students! for the next three weeks we need all together (students and teachers) to unite so as not to lose precious time of the second semester. Therefore, an online learning will be established.", "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA/edit", "Very short description for online learning", new DateTime(2021, 12, 21, 0, 0, 0, 0, DateTimeKind.Local), "Online Learning in NaU OA Starts" },
                    { 2L, "Congratulations, My name is Natalia, I deal with international rankings and NaUAA membership in them. This year, U - Multirank is conducting a survey among students majoring in Computer Science. Please contribute to the high place of NaUAA in this ranking by filling out a small survey.I quote the letter", "https://che-survey.de/uc/umr2022/ ", "Very short description for international rating", new DateTime(2021, 12, 20, 11, 24, 0, 0, DateTimeKind.Unspecified), "International rating" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "EventId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "EventId",
                keyValue: 2L);
        }
    }
}

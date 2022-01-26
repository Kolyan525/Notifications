using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class UpdatedSeeder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64e3676c-9434-4eb1-9123-69f754319226");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d70fdc7b-fe07-4669-9d0c-78b6811a02a9");

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
                table: "NotificationTypeSubscription",
                keyColumn: "NotificaitonTypeSubscriptionId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "NotificationTypeId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "NotificationTypeId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "NotificationTypeId",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "SubscriptionEvents",
                keyColumn: "SubscriptionEventId",
                keyValue: 1L);

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

            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "NotificationTypeId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "SubscriptionId",
                keyValue: 1L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "64e3676c-9434-4eb1-9123-69f754319226", "3665112c-222e-4bd3-a2f1-40b6eaf781c3", "Manager", "MANAGER" },
                    { "d70fdc7b-fe07-4669-9d0c-78b6811a02a9", "69438723-fbc3-4c2d-8786-c01f9e5711ab", "Admin", "ADMIN" }
                });

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
                    { 1L, "Dear students! for the next three weeks we need all together (students and teachers) to unite so as not to lose precious time of the second semester. Therefore an online learning will be established.", "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA/edit", "Very short description for online learning", new DateTime(2022, 1, 21, 0, 0, 0, 0, DateTimeKind.Local), "Online Learning in NaU\"OA\" Starts" },
                    { 2L, "Congratulations, My name is Natalia, I deal with international rankings and NaU\"OA\" membership in them. This year, U - Multirank is conducting a survey amongstudents majoring in Computer Science. Please contribute to the high place of Na\"OA\" in this ranking by filling out a small survey. I quote the letter below", "https://che-survey.de/uc/umr2022/ ", "Very short description for international rating", new DateTime(2021, 12, 20, 11, 24, 0, 0, DateTimeKind.Unspecified), "International rating" }
                });

            migrationBuilder.InsertData(
                table: "NotificationTypes",
                columns: new[] { "NotificationTypeId", "NotificationName" },
                values: new object[,]
                {
                    { 1L, "Telegram" },
                    { 2L, "Instagram" },
                    { 3L, "Discord" },
                    { 4L, "Viber" }
                });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                column: "SubscriptionId",
                value: 1L);

            migrationBuilder.InsertData(
                table: "EventCategories",
                columns: new[] { "EventCategoryId", "CategoryId", "EventId" },
                values: new object[,]
                {
                    { 1L, 1L, 1L },
                    { 2L, 2L, 1L },
                    { 3L, 1L, 2L }
                });

            migrationBuilder.InsertData(
                table: "NotificationTypeSubscription",
                columns: new[] { "NotificaitonTypeSubscriptionId", "DiscordKey", "InstagramKey", "NotificationTypeId", "SubscriptionId", "TelegramKey" },
                values: new object[] { 1L, null, null, 1L, 1L, "@Nicolas_Cage525" });

            migrationBuilder.InsertData(
                table: "SubscriptionEvents",
                columns: new[] { "SubscriptionEventId", "EventId", "SubscriptionId" },
                values: new object[] { 1L, 1L, 1L });
        }
    }
}

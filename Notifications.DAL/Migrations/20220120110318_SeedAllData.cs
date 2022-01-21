using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class SeedAllData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "afede725-f382-4cda-b6c0-76c9d40dda09");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b224f9d1-db23-416a-a974-1b78c93c935f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3086f867-19cf-4424-9ed0-25466d181001", "3840fc94-6f1f-4bb5-8e84-096cd468d404", "Admin", "ADMIN" },
                    { "d8a2eb8b-73c7-46ac-8e53-4372e308915c", "916e198f-2766-473e-826c-76bccf60183b", "Manager", "MANAGER" }
                });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "EventId",
                keyValue: 1L,
                column: "StartAt",
                value: new DateTime(2022, 1, 20, 0, 0, 0, 0, DateTimeKind.Local));

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
                table: "NotificationTypeSubscription",
                columns: new[] { "NotificaitonTypeSubscriptionId", "DiscordKey", "InstagramKey", "NotificationTypeId", "SubscriptionId", "TelegramKey" },
                values: new object[] { 1L, null, null, 1L, 1L, "@Nicolas_Cage525" });

            migrationBuilder.InsertData(
                table: "SubscriptionEvents",
                columns: new[] { "SubscriptionEventId", "EventId", "SubscriptionId" },
                values: new object[] { 1L, 1L, 1L });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3086f867-19cf-4424-9ed0-25466d181001");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d8a2eb8b-73c7-46ac-8e53-4372e308915c");

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
                table: "NotificationTypes",
                keyColumn: "NotificationTypeId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "SubscriptionId",
                keyValue: 1L);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "afede725-f382-4cda-b6c0-76c9d40dda09", "c94bfd31-098c-43d5-bb1e-3d7bc1944882", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b224f9d1-db23-416a-a974-1b78c93c935f", "99d34d1b-aa1a-4883-862e-3eaf0de61888", "Admin", "ADMIN" });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "EventId",
                keyValue: 1L,
                column: "StartAt",
                value: new DateTime(2022, 1, 18, 0, 0, 0, 0, DateTimeKind.Local));
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class NTSAndSubCasDelChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTypeSubscription_Subscriptions_SubscriptionId",
                table: "NotificationTypeSubscription");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTypeSubscription_Subscriptions_SubscriptionId",
                table: "NotificationTypeSubscription",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "SubscriptionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTypeSubscription_Subscriptions_SubscriptionId",
                table: "NotificationTypeSubscription");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTypeSubscription_Subscriptions_SubscriptionId",
                table: "NotificationTypeSubscription",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "SubscriptionId");
        }
    }
}

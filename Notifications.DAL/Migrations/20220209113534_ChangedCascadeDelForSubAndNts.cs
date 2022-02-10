using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class ChangedCascadeDelForSubAndNts : Migration
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
                principalColumn: "SubscriptionId");
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
                principalColumn: "SubscriptionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

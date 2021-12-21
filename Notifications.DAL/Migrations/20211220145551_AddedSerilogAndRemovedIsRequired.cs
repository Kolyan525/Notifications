using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class AddedSerilogAndRemovedIsRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Events_EventId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationType_NotificationTypes_NotificationTypeId",
                table: "NotificationType");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationType_Subscriptions_SubscriptionId",
                table: "NotificationType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_EventId",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationType",
                table: "NotificationType");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "NotificationType",
                newName: "NotificationTypeSubscription");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationType_SubscriptionId",
                table: "NotificationTypeSubscription",
                newName: "IX_NotificationTypeSubscription_SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationType_NotificationTypeId",
                table: "NotificationTypeSubscription",
                newName: "IX_NotificationTypeSubscription_NotificationTypeId");

            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "Categories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationTypeSubscription",
                table: "NotificationTypeSubscription",
                column: "NotificaitonTypeSubscriptionId");

            migrationBuilder.CreateTable(
                name: "EventCategories",
                columns: table => new
                {
                    EventCategoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategories", x => x.EventCategoryId);
                    table.ForeignKey(
                        name: "FK_EventCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventCategories_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_CategoryId",
                table: "EventCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_EventId",
                table: "EventCategories",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTypeSubscription_NotificationTypes_NotificationTypeId",
                table: "NotificationTypeSubscription",
                column: "NotificationTypeId",
                principalTable: "NotificationTypes",
                principalColumn: "NotificationTypeId",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_NotificationTypeSubscription_NotificationTypes_NotificationTypeId",
                table: "NotificationTypeSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTypeSubscription_Subscriptions_SubscriptionId",
                table: "NotificationTypeSubscription");

            migrationBuilder.DropTable(
                name: "EventCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationTypeSubscription",
                table: "NotificationTypeSubscription");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "NotificationTypeSubscription",
                newName: "NotificationType");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationTypeSubscription_SubscriptionId",
                table: "NotificationType",
                newName: "IX_NotificationType_SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationTypeSubscription_NotificationTypeId",
                table: "NotificationType",
                newName: "IX_NotificationType_NotificationTypeId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "EventId",
                table: "Categories",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationType",
                table: "NotificationType",
                column: "NotificaitonTypeSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_EventId",
                table: "Categories",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Events_EventId",
                table: "Categories",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationType_NotificationTypes_NotificationTypeId",
                table: "NotificationType",
                column: "NotificationTypeId",
                principalTable: "NotificationTypes",
                principalColumn: "NotificationTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationType_Subscriptions_SubscriptionId",
                table: "NotificationType",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "SubscriptionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

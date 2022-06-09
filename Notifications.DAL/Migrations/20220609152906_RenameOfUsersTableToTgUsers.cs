using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class RenameOfUsersTableToTgUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "TelegramUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelegramUsers",
                table: "TelegramUsers",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TelegramUsers",
                table: "TelegramUsers");

            migrationBuilder.RenameTable(
                name: "TelegramUsers",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }
    }
}

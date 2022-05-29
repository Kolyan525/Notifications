using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifications.DAL.Migrations
{
    public partial class FixedtypoinRefreshtokenstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRevorked",
                table: "RefreshTokens",
                newName: "IsRevoked");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRevoked",
                table: "RefreshTokens",
                newName: "IsRevorked");
        }
    }
}

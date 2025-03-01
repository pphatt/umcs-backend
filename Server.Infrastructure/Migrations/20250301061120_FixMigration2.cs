using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_UserId",
                table: "ContributionActivityLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_UserId",
                table: "ContributionActivityLogs",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_UserId",
                table: "ContributionActivityLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_UserId",
                table: "ContributionActivityLogs",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

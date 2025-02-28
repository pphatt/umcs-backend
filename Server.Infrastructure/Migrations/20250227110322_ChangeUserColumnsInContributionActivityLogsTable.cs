using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserColumnsInContributionActivityLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_CoordinatorId",
                table: "ContributionActivityLogs");

            migrationBuilder.RenameColumn(
                name: "CoordinatorUsername",
                table: "ContributionActivityLogs",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ContributionActivityLogs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContributionActivityLogs_CoordinatorId",
                table: "ContributionActivityLogs",
                newName: "IX_ContributionActivityLogs_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_UserId",
                table: "ContributionActivityLogs",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_UserId",
                table: "ContributionActivityLogs");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "ContributionActivityLogs",
                newName: "CoordinatorUsername");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ContributionActivityLogs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContributionActivityLogs_UserId",
                table: "ContributionActivityLogs",
                newName: "IX_ContributionActivityLogs_CoordinatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_CoordinatorId",
                table: "ContributionActivityLogs",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

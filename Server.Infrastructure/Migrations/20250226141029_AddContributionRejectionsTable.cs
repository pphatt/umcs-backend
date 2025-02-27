using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContributionRejectionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContributionRejections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContributionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionRejections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContributionActivityLogs_CoordinatorId",
                table: "ContributionActivityLogs",
                column: "CoordinatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionRejections_ContributionId",
                table: "ContributionRejections",
                column: "ContributionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_CoordinatorId",
                table: "ContributionActivityLogs",
                column: "CoordinatorId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionActivityLogs_Contributions_ContributionId",
                table: "ContributionActivityLogs",
                column: "ContributionId",
                principalTable: "Contributions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_CoordinatorId",
                table: "ContributionActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ContributionActivityLogs_Contributions_ContributionId",
                table: "ContributionActivityLogs");

            migrationBuilder.DropTable(
                name: "ContributionRejections");

            migrationBuilder.DropIndex(
                name: "IX_ContributionActivityLogs_CoordinatorId",
                table: "ContributionActivityLogs");
        }
    }
}

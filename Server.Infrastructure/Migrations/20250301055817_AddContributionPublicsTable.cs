using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    public partial class AddContributionPublicsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Contributions_ContributionId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_ContributionTags_Contributions_ContributionId",
                table: "ContributionTags");

            migrationBuilder.DropTable(
                name: "Contributions");

            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Slug = table.Column<string>(type: "varchar(256)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublicDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllowedGuest = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CoordinatorApprovedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.Id);
                });

            migrationBuilder.RenameColumn(
                name: "CoordinatorUsername",
                table: "ContributionActivityLogs",
                newName: "Username");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ContributionActivityLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<Guid>(
                name: "ContributionPublicId",
                table: "Files",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContributionPublicId",
                table: "ContributionTags",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContributionPublics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Slug = table.Column<string>(type: "varchar(256)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FacultyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublicDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllowedGuest = table.Column<bool>(type: "bit", nullable: false),
                    CoordinatorApprovedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LikeQuantity = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionPublics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContributionPublics_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContributionPublics_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Contributions_Slug",
                table: "Contributions",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_FacultyId",
                table: "Contributions",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_AcademicYearId",
                table: "Contributions",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ContributionPublicId",
                table: "Files",
                column: "ContributionPublicId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionTags_ContributionPublicId",
                table: "ContributionTags",
                column: "ContributionPublicId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionActivityLogs_UserId",
                table: "ContributionActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionPublics_AcademicYearId",
                table: "ContributionPublics",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionPublics_FacultyId",
                table: "ContributionPublics",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionPublics_Slug",
                table: "ContributionPublics",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContributionRejections_ContributionId",
                table: "ContributionRejections",
                column: "ContributionId",
                unique: true);

            // Recreate foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_UserId",
                table: "ContributionActivityLogs",
                column: "UserId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_Faculties_FacultyId",
                table: "Contributions",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionTags_Contributions_ContributionId",
                table: "ContributionTags",
                column: "ContributionId",
                principalTable: "Contributions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionTags_ContributionPublics_ContributionPublicId",
                table: "ContributionTags",
                column: "ContributionPublicId",
                principalTable: "ContributionPublics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Contributions_ContributionId",
                table: "Files",
                column: "ContributionId",
                principalTable: "Contributions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_ContributionPublics_ContributionPublicId",
                table: "Files",
                column: "ContributionPublicId",
                principalTable: "ContributionPublics",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop all foreign keys added in Up
            migrationBuilder.DropForeignKey(
                name: "FK_ContributionActivityLogs_AppUsers_UserId",
                table: "ContributionActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ContributionActivityLogs_Contributions_ContributionId",
                table: "ContributionActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_Faculties_FacultyId",
                table: "Contributions");

            migrationBuilder.DropForeignKey(
                name: "FK_ContributionTags_Contributions_ContributionId",
                table: "ContributionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ContributionTags_ContributionPublics_ContributionPublicId",
                table: "ContributionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Contributions_ContributionId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_ContributionPublics_ContributionPublicId",
                table: "Files");

            migrationBuilder.DropTable(
                name: "Contributions");

            migrationBuilder.DropTable(
                name: "ContributionPublics");

            migrationBuilder.DropTable(
                name: "ContributionRejections");

            migrationBuilder.DropIndex(
                name: "IX_Files_ContributionPublicId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_ContributionTags_ContributionPublicId",
                table: "ContributionTags");

            migrationBuilder.DropIndex(
                name: "IX_ContributionActivityLogs_UserId",
                table: "ContributionActivityLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ContributionActivityLogs");

            migrationBuilder.DropColumn(
                name: "ContributionPublicId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ContributionPublicId",
                table: "ContributionTags");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "ContributionActivityLogs",
                newName: "CoordinatorUsername");

            // Recreate the original Contributions table (simplified version)
            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Slug = table.Column<string>(type: "varchar(256)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contributions_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contributions_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_Slug",
                table: "Contributions",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_FacultyId",
                table: "Contributions",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_AcademicYearId",
                table: "Contributions",
                column: "AcademicYearId");

            // Recreate original foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_ContributionActivityLogs_Contributions_ContributionId",
                table: "ContributionActivityLogs",
                column: "ContributionId",
                principalTable: "Contributions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Contributions_ContributionId",
                table: "Files",
                column: "ContributionId",
                principalTable: "Contributions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionTags_Contributions_ContributionId",
                table: "ContributionTags",
                column: "ContributionId",
                principalTable: "Contributions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

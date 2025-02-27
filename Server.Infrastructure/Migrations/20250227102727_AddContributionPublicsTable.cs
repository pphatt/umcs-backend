using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContributionPublicsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Files_ContributionPublicId",
                table: "Files",
                column: "ContributionPublicId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionTags_ContributionPublicId",
                table: "ContributionTags",
                column: "ContributionPublicId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionTags_ContributionPublics_ContributionPublicId",
                table: "ContributionTags",
                column: "ContributionPublicId",
                principalTable: "ContributionPublics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_ContributionPublics_ContributionPublicId",
                table: "Files",
                column: "ContributionPublicId",
                principalTable: "ContributionPublics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContributionTags_ContributionPublics_ContributionPublicId",
                table: "ContributionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_ContributionPublics_ContributionPublicId",
                table: "Files");

            migrationBuilder.DropTable(
                name: "ContributionPublics");

            migrationBuilder.DropIndex(
                name: "IX_Files_ContributionPublicId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_ContributionTags_ContributionPublicId",
                table: "ContributionTags");

            migrationBuilder.DropColumn(
                name: "ContributionPublicId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ContributionPublicId",
                table: "ContributionTags");
        }
    }
}

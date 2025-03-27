using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLastTimeTextingInPrivateChatRoomsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User2LastActivity",
                table: "PrivateChatRooms");

            migrationBuilder.AddColumn<DateTime>(
                name: "User2LastActivity",
                table: "PrivateChatRooms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimeTexting",
                table: "PrivateChatRooms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTimeTexting",
                table: "PrivateChatRooms");

            migrationBuilder.DropColumn(
                name: "User2LastActivity",
                table: "PrivateChatRooms");

            migrationBuilder.AddColumn<Guid>(
                name: "User2LastActivity",
                table: "PrivateChatRooms",
                type: "uniqueidentifier",
                nullable: false);
        }
    }
}

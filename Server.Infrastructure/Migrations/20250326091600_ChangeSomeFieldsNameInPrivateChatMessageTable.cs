using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSomeFieldsNameInPrivateChatMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "PrivateChatMessages",
                newName: "Content");

            migrationBuilder.AddColumn<Guid>(
                name: "ChatRoomId",
                table: "PrivateChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatRoomId",
                table: "PrivateChatMessages");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "PrivateChatMessages",
                newName: "Message");
        }
    }
}

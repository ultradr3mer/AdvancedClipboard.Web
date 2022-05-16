using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedClipboard.Web.Migrations
{
    public partial class addedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileAccessToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAccessToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lane",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lane", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lane_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClipboardContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    LaneId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastUsedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TextContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClipboardContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClipboardContent_ContentType_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalTable: "ContentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClipboardContent_FileAccessToken_FileTokenId",
                        column: x => x.FileTokenId,
                        principalTable: "FileAccessToken",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClipboardContent_Lane_LaneId",
                        column: x => x.LaneId,
                        principalTable: "Lane",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClipboardContent_ContentTypeId",
                table: "ClipboardContent",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClipboardContent_FileTokenId",
                table: "ClipboardContent",
                column: "FileTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ClipboardContent_LaneId",
                table: "ClipboardContent",
                column: "LaneId");

            migrationBuilder.CreateIndex(
                name: "IX_Lane_UserId",
                table: "Lane",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClipboardContent");

            migrationBuilder.DropTable(
                name: "ContentType");

            migrationBuilder.DropTable(
                name: "FileAccessToken");

            migrationBuilder.DropTable(
                name: "Lane");
        }
    }
}

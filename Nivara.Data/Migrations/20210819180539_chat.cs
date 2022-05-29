using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nivara.Data.Migrations
{
    public partial class chat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsersTaskId = table.Column<int>(type: "int", nullable: false),
                    SenderUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_UsersTask_UsersTaskId",
                        column: x => x.UsersTaskId,
                        principalTable: "UsersTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UsersTaskId",
                table: "Chats",
                column: "UsersTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}

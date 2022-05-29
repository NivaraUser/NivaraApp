using Microsoft.EntityFrameworkCore.Migrations;

namespace Nivara.Data.Migrations
{
    public partial class UsersTaskEnduserIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersTask_EndUsers_EndUsersId",
                table: "UsersTask");

            migrationBuilder.AlterColumn<int>(
                name: "EndUsersId",
                table: "UsersTask",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTask_EndUsers_EndUsersId",
                table: "UsersTask",
                column: "EndUsersId",
                principalTable: "EndUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersTask_EndUsers_EndUsersId",
                table: "UsersTask");

            migrationBuilder.AlterColumn<int>(
                name: "EndUsersId",
                table: "UsersTask",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTask_EndUsers_EndUsersId",
                table: "UsersTask",
                column: "EndUsersId",
                principalTable: "EndUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

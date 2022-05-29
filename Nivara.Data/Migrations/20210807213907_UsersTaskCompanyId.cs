using Microsoft.EntityFrameworkCore.Migrations;

namespace Nivara.Data.Migrations
{
    public partial class UsersTaskCompanyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "UsersTask",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersTask_CompanyId",
                table: "UsersTask",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTask_Companies_CompanyId",
                table: "UsersTask",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersTask_Companies_CompanyId",
                table: "UsersTask");

            migrationBuilder.DropIndex(
                name: "IX_UsersTask_CompanyId",
                table: "UsersTask");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "UsersTask");
        }
    }
}

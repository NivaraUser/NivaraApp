using Microsoft.EntityFrameworkCore.Migrations;

namespace Nivara.Data.Migrations
{
    public partial class UsersTaskRemarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "UsersTask",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "UsersTask");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Nivara.Data.Migrations
{
    public partial class EmployeeProfilePic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePiture",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePiture",
                table: "Employees");
        }
    }
}

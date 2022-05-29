using Microsoft.EntityFrameworkCore.Migrations;

namespace Nivara.Data.Migrations
{
    public partial class ComapnyProfilePic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePiture",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePiture",
                table: "Companies");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Nivara.Data.Migrations
{
    public partial class UpdateColumnDueDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "UsersTask",
                newName: "DueDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "UsersTask",
                newName: "EndDate");
        }
    }
}

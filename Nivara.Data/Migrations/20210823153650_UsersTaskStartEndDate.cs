using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nivara.Data.Migrations
{
    public partial class UsersTaskStartEndDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "UsersTask",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "UsersTask",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "UsersTask");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "UsersTask");
        }
    }
}

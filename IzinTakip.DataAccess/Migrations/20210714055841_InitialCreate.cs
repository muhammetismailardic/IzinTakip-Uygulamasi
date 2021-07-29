using Microsoft.EntityFrameworkCore.Migrations;

namespace IzinTakip.DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hours",
                table: "EmployeeSpecialLeaves",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hours",
                table: "EmployeeSpecialLeaves");
        }
    }
}

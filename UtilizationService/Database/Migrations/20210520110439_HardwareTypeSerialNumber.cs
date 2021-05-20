using Microsoft.EntityFrameworkCore.Migrations;

namespace UtilizationService.Database.Migrations
{
    public partial class HardwareTypeSerialNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "HardwareTypes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "HardwareTypes");
        }
    }
}

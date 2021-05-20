using Microsoft.EntityFrameworkCore.Migrations;

namespace UtilizationService.Database.Migrations
{
    public partial class HardwareTypesType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "HardwareTypes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "HardwareTypes");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class AddLocationStoreTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Store",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Store",
                nullable: false,
                oldClrType: typeof(float));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Store",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Store",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}

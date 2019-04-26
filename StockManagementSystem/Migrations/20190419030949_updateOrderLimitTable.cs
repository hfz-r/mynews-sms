using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class updateOrderLimitTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DaysofStock",
                table: "OrderLimit",
                newName: "Safety");

            migrationBuilder.RenameColumn(
                name: "DaysofSales",
                table: "OrderLimit",
                newName: "OrderRatio");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryPerWeek",
                table: "OrderLimit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InventoryCycle",
                table: "OrderLimit",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryPerWeek",
                table: "OrderLimit");

            migrationBuilder.DropColumn(
                name: "InventoryCycle",
                table: "OrderLimit");

            migrationBuilder.RenameColumn(
                name: "Safety",
                table: "OrderLimit",
                newName: "DaysofStock");

            migrationBuilder.RenameColumn(
                name: "OrderRatio",
                table: "OrderLimit",
                newName: "DaysofSales");
        }
    }
}

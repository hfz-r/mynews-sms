using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class updateOrderLimitTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Percentage",
                table: "OrderLimit",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "DaysofSales",
                table: "OrderLimit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DaysofStock",
                table: "OrderLimit",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysofSales",
                table: "OrderLimit");

            migrationBuilder.DropColumn(
                name: "DaysofStock",
                table: "OrderLimit");

            migrationBuilder.AlterColumn<double>(
                name: "Percentage",
                table: "OrderLimit",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}

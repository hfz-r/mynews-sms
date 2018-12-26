using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class AddTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_Store_StoreId",
                table: "Device");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Device",
                table: "Device");

            migrationBuilder.RenameTable(
                name: "Device",
                newName: "Device");

            migrationBuilder.RenameIndex(
                name: "IX_Device_StoreId",
                table: "Device",
                newName: "IX_Device_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Device",
                table: "Device",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Store_StoreId",
                table: "Device",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "P_BranchNo",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_Store_StoreId",
                table: "Device");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Device",
                table: "Device");

            migrationBuilder.RenameTable(
                name: "Device",
                newName: "Device");

            migrationBuilder.RenameIndex(
                name: "IX_Device_StoreId",
                table: "Device",
                newName: "IX_Device_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Device",
                table: "Device",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Store_StoreId",
                table: "Device",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "P_BranchNo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

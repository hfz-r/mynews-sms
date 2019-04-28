using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class AddFieldPushNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShelfLocation_Item_ItemId",
                table: "ShelfLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_ShelfLocation_ShelfLocationFormat_ShelfLocationFormatId",
                table: "ShelfLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_ShelfLocation_Store_StoreId",
                table: "ShelfLocation");

            migrationBuilder.DropIndex(
                name: "IX_ShelfLocation_ItemId",
                table: "ShelfLocation");

            migrationBuilder.DropIndex(
                name: "IX_ShelfLocation_ShelfLocationFormatId",
                table: "ShelfLocation");

            migrationBuilder.DropIndex(
                name: "IX_ShelfLocation_StoreId",
                table: "ShelfLocation");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "ShelfLocation");

            migrationBuilder.DropColumn(
                name: "ShelfLocationFormatId",
                table: "ShelfLocation");

            migrationBuilder.AddColumn<string>(
                name: "Stock_Code",
                table: "ShelfLocation",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "PushNotification",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "PushNotification",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobGroup",
                table: "PushNotification",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                table: "PushNotification",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RemindMe",
                table: "PushNotification",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "PushNotification",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock_Code",
                table: "ShelfLocation");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "PushNotification");

            migrationBuilder.DropColumn(
                name: "Interval",
                table: "PushNotification");

            migrationBuilder.DropColumn(
                name: "JobGroup",
                table: "PushNotification");

            migrationBuilder.DropColumn(
                name: "JobName",
                table: "PushNotification");

            migrationBuilder.DropColumn(
                name: "RemindMe",
                table: "PushNotification");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "PushNotification");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "ShelfLocation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShelfLocationFormatId",
                table: "ShelfLocation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShelfLocation_ItemId",
                table: "ShelfLocation",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfLocation_ShelfLocationFormatId",
                table: "ShelfLocation",
                column: "ShelfLocationFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfLocation_StoreId",
                table: "ShelfLocation",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfLocation_Item_ItemId",
                table: "ShelfLocation",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfLocation_ShelfLocationFormat_ShelfLocationFormatId",
                table: "ShelfLocation",
                column: "ShelfLocationFormatId",
                principalTable: "ShelfLocationFormat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfLocation_Store_StoreId",
                table: "ShelfLocation",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "P_BranchNo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

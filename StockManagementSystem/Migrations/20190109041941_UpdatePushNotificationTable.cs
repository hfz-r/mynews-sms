using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class UpdatePushNotificationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationCategory",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PushNotification",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Desc = table.Column<string>(maxLength: 256, nullable: false),
                    IsShift = table.Column<string>(nullable: true),
                    NotificationCategoryId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushNotification_NotificationCategory_NotificationCategoryId",
                        column: x => x.NotificationCategoryId,
                        principalTable: "NotificationCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PushNotificationStore",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PushNotificationId = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: false),
                    IsHHTDownloaded = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotificationStore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushNotificationStore_PushNotification_PushNotificationId",
                        column: x => x.PushNotificationId,
                        principalTable: "PushNotification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PushNotificationStore_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "P_BranchNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PushNotification_NotificationCategoryId",
                table: "PushNotification",
                column: "NotificationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PushNotificationStore_PushNotificationId",
                table: "PushNotificationStore",
                column: "PushNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_PushNotificationStore_StoreId",
                table: "PushNotificationStore",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PushNotificationStore");

            migrationBuilder.DropTable(
                name: "PushNotification");

            migrationBuilder.DropTable(
                name: "NotificationCategory");
        }
    }
}

﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class AddPushNotificationStoresTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationCategories",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(maxLength: 2147483647, nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(maxLength: 2147483647, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    P_BranchNo = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    P_Name = table.Column<string>(nullable: true),
                    P_RecStatus = table.Column<string>(nullable: true),
                    P_CompID = table.Column<string>(nullable: true),
                    P_SellPriceLevel = table.Column<string>(nullable: true),
                    P_AreaCode = table.Column<string>(nullable: true),
                    P_Addr1 = table.Column<string>(nullable: true),
                    P_Addr2 = table.Column<string>(nullable: true),
                    P_Addr3 = table.Column<string>(nullable: true),
                    P_State = table.Column<string>(nullable: true),
                    P_City = table.Column<string>(nullable: true),
                    P_Country = table.Column<string>(nullable: true),
                    P_PostCode = table.Column<string>(nullable: true),
                    P_Brand = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.P_BranchNo);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(maxLength: 2147483647, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(maxLength: 7, nullable: true),
                    Name = table.Column<string>(maxLength: 2147483647, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(maxLength: 2147483647, nullable: true),
                    SecurityStamp = table.Column<string>(maxLength: 2147483647, nullable: true),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    Branch = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PushNotification",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Desc = table.Column<string>(maxLength: 256, nullable: false),
                    IsShift = table.Column<string>(nullable: true),
                    NotificationCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushNotification_NotificationCategories_NotificationCategoryId",
                        column: x => x.NotificationCategoryId,
                        principalTable: "NotificationCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    SerialNo = table.Column<string>(maxLength: 256, nullable: false),
                    ModelNo = table.Column<string>(maxLength: 256, nullable: false),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    TokenId = table.Column<string>(nullable: true),
                    StoreId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Device_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "P_BranchNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(maxLength: 2147483647, nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(maxLength: 2147483647, nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PushNotificationStores",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    PushNotificationId = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: false),
                    IsHHTDownloaded = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotificationStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushNotificationStores_PushNotification_PushNotificationId",
                        column: x => x.PushNotificationId,
                        principalTable: "PushNotification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PushNotificationStores_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "P_BranchNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Device_StoreId",
                table: "Device",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PushNotification_NotificationCategoryId",
                table: "PushNotification",
                column: "NotificationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PushNotificationStores_PushNotificationId",
                table: "PushNotificationStores",
                column: "PushNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_PushNotificationStores_StoreId",
                table: "PushNotificationStores",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_NormalizedName",
                table: "Roles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedUserName",
                table: "Users",
                column: "NormalizedUserName");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId",
                table: "UserTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "PushNotificationStores");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "PushNotification");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "NotificationCategories");
        }
    }
}
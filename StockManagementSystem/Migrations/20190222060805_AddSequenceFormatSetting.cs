using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class AddSequenceFormatSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "FormatSetting",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreGrouping",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(nullable: true),
                    GroupName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreGrouping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreUserAssign",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(nullable: true),
                    StoreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreUserAssign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreUserAssign_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "P_BranchNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreGroupingStores",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(nullable: true),
                    StoreId = table.Column<int>(nullable: false),
                    StoreGroupingId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreGroupingStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreGroupingStores_StoreGrouping_StoreGroupingId",
                        column: x => x.StoreGroupingId,
                        principalTable: "StoreGrouping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreGroupingStores_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "P_BranchNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreUserAssignStores",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(nullable: true),
                    StoreUserAssignId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreUserAssignStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreUserAssignStores_StoreUserAssign_StoreUserAssignId",
                        column: x => x.StoreUserAssignId,
                        principalTable: "StoreUserAssign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreUserAssignStores_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreGroupingStores_StoreGroupingId",
                table: "StoreGroupingStores",
                column: "StoreGroupingId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreGroupingStores_StoreId",
                table: "StoreGroupingStores",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreUserAssign_StoreId",
                table: "StoreUserAssign",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreUserAssignStores_StoreUserAssignId",
                table: "StoreUserAssignStores",
                column: "StoreUserAssignId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreUserAssignStores_UserId",
                table: "StoreUserAssignStores",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreGroupingStores");

            migrationBuilder.DropTable(
                name: "StoreUserAssignStores");

            migrationBuilder.DropTable(
                name: "StoreGrouping");

            migrationBuilder.DropTable(
                name: "StoreUserAssign");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "FormatSetting");
        }
    }
}

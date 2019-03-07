using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class AddReplenishmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Replenishment",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(nullable: true),
                    BufferDays = table.Column<int>(nullable: false),
                    ReplenishmentQty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replenishment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReplenishmentStore",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(nullable: true),
                    ReplenishmentId = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplenishmentStore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplenishmentStore_Replenishment_ReplenishmentId",
                        column: x => x.ReplenishmentId,
                        principalTable: "Replenishment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReplenishmentStore_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "P_BranchNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentStore_ReplenishmentId",
                table: "ReplenishmentStore",
                column: "ReplenishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentStore_StoreId",
                table: "ReplenishmentStore",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReplenishmentStore");

            migrationBuilder.DropTable(
                name: "Replenishment");
        }
    }
}

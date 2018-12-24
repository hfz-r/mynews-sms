using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class AddStoreDeviceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    P_BranchNo = table.Column<int>(nullable: false),
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
                    P_Brand = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.P_BranchNo);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),

                    SerialNo = table.Column<string>(maxLength: 256, nullable: false),
                    ModelNo = table.Column<string>(maxLength: 256, nullable: false),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    TokenId = table.Column<string>(nullable: true),
                    StoreId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_Device_StoreId",
                table: "Device",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "Store");
        }
    }
}

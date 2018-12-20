using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagementSystem.Migrations
{
    public partial class AddStoreTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Devices",
                nullable: true,
                oldClrType: typeof(byte),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 450, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: true),
                    P_BranchNo = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_Store", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Devices",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

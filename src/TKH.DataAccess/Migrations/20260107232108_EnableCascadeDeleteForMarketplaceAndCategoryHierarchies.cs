using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDeleteForMarketplaceAndCategoryHierarchies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_MarketplaceAccounts_MarketplaceAccountId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialTransactions_MarketplaceAccounts_MarketplaceAccoun~",
                table: "FinancialTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_MarketplaceAccounts_MarketplaceAccountId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_MarketplaceAccounts_MarketplaceAccountId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentTransactions_MarketplaceAccounts_MarketplaceAccount~",
                table: "ShipmentTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_MarketplaceAccounts_MarketplaceAccountId",
                table: "Claims",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialTransactions_MarketplaceAccounts_MarketplaceAccoun~",
                table: "FinancialTransactions",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_MarketplaceAccounts_MarketplaceAccountId",
                table: "Orders",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MarketplaceAccounts_MarketplaceAccountId",
                table: "Products",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentTransactions_MarketplaceAccounts_MarketplaceAccount~",
                table: "ShipmentTransactions",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_MarketplaceAccounts_MarketplaceAccountId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialTransactions_MarketplaceAccounts_MarketplaceAccoun~",
                table: "FinancialTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_MarketplaceAccounts_MarketplaceAccountId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_MarketplaceAccounts_MarketplaceAccountId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentTransactions_MarketplaceAccounts_MarketplaceAccount~",
                table: "ShipmentTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_MarketplaceAccounts_MarketplaceAccountId",
                table: "Claims",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialTransactions_MarketplaceAccounts_MarketplaceAccoun~",
                table: "FinancialTransactions",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_MarketplaceAccounts_MarketplaceAccountId",
                table: "Orders",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MarketplaceAccounts_MarketplaceAccountId",
                table: "Products",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentTransactions_MarketplaceAccounts_MarketplaceAccount~",
                table: "ShipmentTransactions",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

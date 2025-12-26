using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OrderEntityRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Orders",
                newName: "TotalDiscount");

            migrationBuilder.RenameColumn(
                name: "SellerDiscount",
                table: "OrderItems",
                newName: "VatRate");

            migrationBuilder.RenameColumn(
                name: "MarketplaceDiscount",
                table: "OrderItems",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "OrderItems",
                newName: "SellerCoveredDiscount");

            migrationBuilder.AddColumn<double>(
                name: "CargoDeci",
                table: "Orders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CargoProviderName",
                table: "Orders",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CargoTrackingNumber",
                table: "Orders",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Orders",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "GrossAmount",
                table: "Orders",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsMicroExport",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShipmentPaidBySeller",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MarketplaceShipmentId",
                table: "Orders",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformCoveredDiscount",
                table: "Orders",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MarketplaceSku",
                table: "OrderItems",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformCoveredDiscount",
                table: "OrderItems",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CargoTrackingNumber",
                table: "Orders",
                column: "CargoTrackingNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MarketplaceShipmentId",
                table: "Orders",
                column: "MarketplaceShipmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDate",
                table: "Orders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MarketplaceSku",
                table: "OrderItems",
                column: "MarketplaceSku");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CargoTrackingNumber",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_MarketplaceShipmentId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderDate",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_MarketplaceSku",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CargoDeci",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CargoProviderName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CargoTrackingNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GrossAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsMicroExport",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsShipmentPaidBySeller",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MarketplaceShipmentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PlatformCoveredDiscount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MarketplaceSku",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PlatformCoveredDiscount",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "TotalDiscount",
                table: "Orders",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "VatRate",
                table: "OrderItems",
                newName: "SellerDiscount");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "OrderItems",
                newName: "MarketplaceDiscount");

            migrationBuilder.RenameColumn(
                name: "SellerCoveredDiscount",
                table: "OrderItems",
                newName: "Amount");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

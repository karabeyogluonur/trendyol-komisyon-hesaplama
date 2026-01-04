using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ProductPriceGenerationTypeProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_ProductId",
                table: "ProductPrices");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVatIncluded",
                table: "ProductPrices",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<int>(
                name: "GenerationType",
                table: "ProductPrices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ActiveRecords",
                table: "ProductPrices",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_Filter",
                table: "ProductPrices",
                columns: new[] { "ProductId", "Type", "GenerationType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_ActiveRecords",
                table: "ProductPrices");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_Filter",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "GenerationType",
                table: "ProductPrices");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVatIncluded",
                table: "ProductPrices",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductId",
                table: "ProductPrices",
                column: "ProductId");
        }
    }
}

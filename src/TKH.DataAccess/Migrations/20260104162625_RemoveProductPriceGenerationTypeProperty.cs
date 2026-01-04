using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductPriceGenerationTypeProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_Filter",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "GenerationType",
                table: "ProductPrices");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_Filter",
                table: "ProductPrices",
                columns: new[] { "ProductId", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_Filter",
                table: "ProductPrices");

            migrationBuilder.AddColumn<int>(
                name: "GenerationType",
                table: "ProductPrices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_Filter",
                table: "ProductPrices",
                columns: new[] { "ProductId", "Type", "GenerationType" });
        }
    }
}

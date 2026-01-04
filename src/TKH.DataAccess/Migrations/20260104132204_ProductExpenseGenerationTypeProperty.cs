using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ProductExpenseGenerationTypeProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductExpenses_ProductId",
                table: "ProductExpenses");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVatIncluded",
                table: "ProductExpenses",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<int>(
                name: "GenerationType",
                table: "ProductExpenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "ProductExpenses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductExpenses_ActiveRecords",
                table: "ProductExpenses",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductExpenses_Filter",
                table: "ProductExpenses",
                columns: new[] { "ProductId", "Type", "GenerationType" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductExpenses_ProductId1",
                table: "ProductExpenses",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductExpenses_Products_ProductId1",
                table: "ProductExpenses",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductExpenses_Products_ProductId1",
                table: "ProductExpenses");

            migrationBuilder.DropIndex(
                name: "IX_ProductExpenses_ActiveRecords",
                table: "ProductExpenses");

            migrationBuilder.DropIndex(
                name: "IX_ProductExpenses_Filter",
                table: "ProductExpenses");

            migrationBuilder.DropIndex(
                name: "IX_ProductExpenses_ProductId1",
                table: "ProductExpenses");

            migrationBuilder.DropColumn(
                name: "GenerationType",
                table: "ProductExpenses");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductExpenses");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVatIncluded",
                table: "ProductExpenses",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ProductExpenses_ProductId",
                table: "ProductExpenses",
                column: "ProductId");
        }
    }
}

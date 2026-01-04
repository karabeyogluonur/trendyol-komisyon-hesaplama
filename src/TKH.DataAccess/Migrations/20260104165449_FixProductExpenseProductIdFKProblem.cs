using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixProductExpenseProductIdFKProblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductExpenses_Products_ProductId1",
                table: "ProductExpenses");

            migrationBuilder.DropIndex(
                name: "IX_ProductExpenses_ProductId1",
                table: "ProductExpenses");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductExpenses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "ProductExpenses",
                type: "integer",
                nullable: true);

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
    }
}

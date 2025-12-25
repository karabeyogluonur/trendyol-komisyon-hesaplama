using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FinancialTransactionRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialTransactions_MarketplaceAccounts_MarketplaceAccoun~",
                table: "FinancialTransactions");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "FinancialTransactions");

            migrationBuilder.DropColumn(
                name: "CommissionRate",
                table: "FinancialTransactions");

            migrationBuilder.DropColumn(
                name: "OrderItemBarcode",
                table: "FinancialTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "FinancialTransactions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceTransactionId",
                table: "FinancialTransactions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FinancialTransactions",
                type: "text",
                maxLength: 2147483647,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarketplaceTransactionType",
                table: "FinancialTransactions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "FinancialTransactions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketplaceTransactionType",
                table: "FinancialTransactions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "FinancialTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "FinancialTransactions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MarketplaceTransactionId",
                table: "FinancialTransactions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FinancialTransactions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 2147483647,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "FinancialTransactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionRate",
                table: "FinancialTransactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OrderItemBarcode",
                table: "FinancialTransactions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialTransactions_MarketplaceAccounts_MarketplaceAccoun~",
                table: "FinancialTransactions",
                column: "MarketplaceAccountId",
                principalTable: "MarketplaceAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

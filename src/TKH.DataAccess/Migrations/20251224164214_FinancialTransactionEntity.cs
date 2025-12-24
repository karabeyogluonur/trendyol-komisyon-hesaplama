using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FinancialTransactionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketplaceAccountId = table.Column<int>(type: "integer", nullable: false),
                    MarketplaceTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    OrderItemBarcode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialTransactions_MarketplaceAccounts_MarketplaceAccoun~",
                        column: x => x.MarketplaceAccountId,
                        principalTable: "MarketplaceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_MarketplaceAccountId_MarketplaceTrans~",
                table: "FinancialTransactions",
                columns: new[] { "MarketplaceAccountId", "MarketplaceTransactionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_OrderNumber",
                table: "FinancialTransactions",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_TransactionDate",
                table: "FinancialTransactions",
                column: "TransactionDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialTransactions");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ShipmentTransactionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShipmentTransactionSyncStatus",
                table: "FinancialTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ShipmentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketplaceAccountId = table.Column<int>(type: "integer", nullable: false),
                    MarketplaceOrderNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MarketplaceParcelId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Desi = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentTransactions_MarketplaceAccounts_MarketplaceAccount~",
                        column: x => x.MarketplaceAccountId,
                        principalTable: "MarketplaceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTransactions_MarketplaceAccountId",
                table: "ShipmentTransactions",
                column: "MarketplaceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTransactions_MarketplaceAccountId_MarketplaceParcel~",
                table: "ShipmentTransactions",
                columns: new[] { "MarketplaceAccountId", "MarketplaceParcelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTransactions_MarketplaceOrderNumber",
                table: "ShipmentTransactions",
                column: "MarketplaceOrderNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentTransactions");

            migrationBuilder.DropColumn(
                name: "ShipmentTransactionSyncStatus",
                table: "FinancialTransactions");
        }
    }
}

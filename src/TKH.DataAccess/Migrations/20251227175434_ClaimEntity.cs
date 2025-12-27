using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ClaimEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketplaceAccountId = table.Column<int>(type: "integer", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalOrderNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalShipmentPackageId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ClaimDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CustomerFirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerLastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CargoTrackingNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CargoProviderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CargoSenderNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CargoTrackingLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RejectedExternalPackageId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RejectedCargoTrackingNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RejectedCargoProviderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RejectedCargoTrackingLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_MarketplaceAccounts_MarketplaceAccountId",
                        column: x => x.MarketplaceAccountId,
                        principalTable: "MarketplaceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClaimItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaimId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalOrderLineItemId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    VatRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CustomerNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    ReasonName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ReasonCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    IsAutoAccepted = table.Column<bool>(type: "boolean", nullable: false),
                    IsAcceptedBySeller = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimItems_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimItems_ClaimId",
                table: "ClaimItems",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimItems_ExternalId",
                table: "ClaimItems",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimItems_ProductId",
                table: "ClaimItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimItems_Status",
                table: "ClaimItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ClaimDate",
                table: "Claims",
                column: "ClaimDate");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ExternalId",
                table: "Claims",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ExternalOrderNumber",
                table: "Claims",
                column: "ExternalOrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_LastUpdateDateTime",
                table: "Claims",
                column: "LastUpdateDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_MarketplaceAccountId",
                table: "Claims",
                column: "MarketplaceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimItems");

            migrationBuilder.DropTable(
                name: "Claims");
        }
    }
}

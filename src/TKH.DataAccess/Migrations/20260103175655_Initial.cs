using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketplaceType = table.Column<int>(type: "integer", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ParentExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsLeaf = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultCommissionRate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketplaceType = table.Column<int>(type: "integer", nullable: false),
                    StoreName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ApiKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ApiSecretKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MerchantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ConnectionState = table.Column<int>(type: "integer", nullable: false),
                    LastErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LastErrorDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SyncState = table.Column<int>(type: "integer", nullable: false),
                    LastSyncStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsVariant = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryAttributes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "FinancialTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketplaceAccountId = table.Column<int>(type: "integer", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalOrderNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    ExternalTransactionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CommissionRate = table.Column<decimal>(type: "numeric", nullable: true),
                    ShipmentTransactionSyncStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialTransactions_MarketplaceAccounts_MarketplaceAccoun~",
                        column: x => x.MarketplaceAccountId,
                        principalTable: "MarketplaceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalOrderNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalShipmentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MarketplaceAccountId = table.Column<int>(type: "integer", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDiscount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PlatformCoveredDiscount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CargoTrackingNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CargoProviderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Deci = table.Column<double>(type: "double precision", nullable: false),
                    IsShipmentPaidBySeller = table.Column<bool>(type: "boolean", nullable: false),
                    IsMicroExport = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_MarketplaceAccounts_MarketplaceAccountId",
                        column: x => x.MarketplaceAccountId,
                        principalTable: "MarketplaceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalProductCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ModelCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Deci = table.Column<double>(type: "double precision", nullable: false),
                    VatRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    UnitType = table.Column<int>(type: "integer", nullable: false),
                    IsOnSale = table.Column<bool>(type: "boolean", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MarketplaceAccountId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_MarketplaceAccounts_MarketplaceAccountId",
                        column: x => x.MarketplaceAccountId,
                        principalTable: "MarketplaceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarketplaceAccountId = table.Column<int>(type: "integer", nullable: false),
                    ExternalOrderNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalParcelId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Deci = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentTransactions_MarketplaceAccounts_MarketplaceAccount~",
                        column: x => x.MarketplaceAccountId,
                        principalTable: "MarketplaceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttributeValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryAttributeId = table.Column<int>(type: "integer", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeValues_CategoryAttributes_CategoryAttributeId",
                        column: x => x.CategoryAttributeId,
                        principalTable: "CategoryAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    VatRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PlatformCoveredDiscount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SellerCoveredDiscount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OrderItemStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    VatRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsVatIncluded = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductExpenses_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsVatIncluded = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    CategoryAttributeId = table.Column<int>(type: "integer", nullable: false),
                    AttributeValueId = table.Column<int>(type: "integer", nullable: true),
                    CustomValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributes_AttributeValues_AttributeValueId",
                        column: x => x.AttributeValueId,
                        principalTable: "AttributeValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductAttributes_CategoryAttributes_CategoryAttributeId",
                        column: x => x.CategoryAttributeId,
                        principalTable: "CategoryAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttributes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValues_CategoryAttributeId_ExternalId",
                table: "AttributeValues",
                columns: new[] { "CategoryAttributeId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_MarketplaceType_ExternalId",
                table: "Categories",
                columns: new[] { "MarketplaceType", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributes_CategoryId_ExternalId",
                table: "CategoryAttributes",
                columns: new[] { "CategoryId", "ExternalId" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_ExternalOrderNumber",
                table: "FinancialTransactions",
                column: "ExternalOrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_ExternalTransactionId",
                table: "FinancialTransactions",
                column: "ExternalTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_MarketplaceAccountId",
                table: "FinancialTransactions",
                column: "MarketplaceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_TransactionDate",
                table: "FinancialTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ExternalOrderNumber",
                table: "Orders",
                column: "ExternalOrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ExternalShipmentId",
                table: "Orders",
                column: "ExternalShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MarketplaceAccountId",
                table: "Orders",
                column: "MarketplaceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDate",
                table: "Orders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributes_AttributeValueId",
                table: "ProductAttributes",
                column: "AttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributes_CategoryAttributeId",
                table: "ProductAttributes",
                column: "CategoryAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributes_ProductId",
                table: "ProductAttributes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductExpenses_ProductId",
                table: "ProductExpenses",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductId",
                table: "ProductPrices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ExternalId",
                table: "Products",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ExternalProductCode",
                table: "Products",
                column: "ExternalProductCode");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MarketplaceAccountId",
                table: "Products",
                column: "MarketplaceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Name",
                table: "Settings",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTransactions_MarketplaceAccountId",
                table: "ShipmentTransactions",
                column: "MarketplaceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimItems");

            migrationBuilder.DropTable(
                name: "FinancialTransactions");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductAttributes");

            migrationBuilder.DropTable(
                name: "ProductExpenses");

            migrationBuilder.DropTable(
                name: "ProductPrices");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "ShipmentTransactions");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "AttributeValues");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "CategoryAttributes");

            migrationBuilder.DropTable(
                name: "MarketplaceAccounts");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}

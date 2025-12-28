using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKH.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MarketplaceAccountSyncStatusProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "MarketplaceAccounts",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<int>(
                name: "ConnectionState",
                table: "MarketplaceAccounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastErrorDate",
                table: "MarketplaceAccounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastErrorMessage",
                table: "MarketplaceAccounts",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncStartTime",
                table: "MarketplaceAccounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SyncState",
                table: "MarketplaceAccounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionState",
                table: "MarketplaceAccounts");

            migrationBuilder.DropColumn(
                name: "LastErrorDate",
                table: "MarketplaceAccounts");

            migrationBuilder.DropColumn(
                name: "LastErrorMessage",
                table: "MarketplaceAccounts");

            migrationBuilder.DropColumn(
                name: "LastSyncStartTime",
                table: "MarketplaceAccounts");

            migrationBuilder.DropColumn(
                name: "SyncState",
                table: "MarketplaceAccounts");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "MarketplaceAccounts",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);
        }
    }
}

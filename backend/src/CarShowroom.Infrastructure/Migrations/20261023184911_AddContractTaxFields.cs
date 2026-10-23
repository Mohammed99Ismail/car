using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShowroom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractTaxFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_tax_applied",
                table: "contracts",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "monthly_rent_with_tax",
                table: "contracts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "monthly_tax_amount",
                table: "contracts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_percentage",
                table: "contracts",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_price_with_tax",
                table: "contracts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_tax_amount",
                table: "contracts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_tax_applied",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "monthly_rent_with_tax",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "monthly_tax_amount",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "tax_percentage",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "total_price_with_tax",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "total_tax_amount",
                table: "contracts");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarShowroom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCarColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "color_id",
                table: "cars",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "car_colors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    hex_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_car_colors", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cars_color_id",
                table: "cars",
                column: "color_id");

            migrationBuilder.CreateIndex(
                name: "ix_car_colors_name",
                table: "car_colors",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_cars_car_colors_color_id",
                table: "cars",
                column: "color_id",
                principalTable: "car_colors",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cars_car_colors_color_id",
                table: "cars");

            migrationBuilder.DropTable(
                name: "car_colors");

            migrationBuilder.DropIndex(
                name: "ix_cars_color_id",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "color_id",
                table: "cars");
        }
    }
}

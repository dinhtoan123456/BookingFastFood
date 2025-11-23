using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ass1_C_5_OrderFastFood.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbQuantityforFoodItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "FoodItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "FoodItems");
        }
    }
}

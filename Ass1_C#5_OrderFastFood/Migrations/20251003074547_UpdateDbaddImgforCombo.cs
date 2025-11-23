using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ass1_C_5_OrderFastFood.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbaddImgforCombo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Combos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Combos");
        }
    }
}

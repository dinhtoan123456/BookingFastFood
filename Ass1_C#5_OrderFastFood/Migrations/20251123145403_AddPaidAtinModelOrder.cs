using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ass1_C_5_OrderFastFood.Migrations
{
    /// <inheritdoc />
    public partial class AddPaidAtinModelOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Orders");
        }
    }
}

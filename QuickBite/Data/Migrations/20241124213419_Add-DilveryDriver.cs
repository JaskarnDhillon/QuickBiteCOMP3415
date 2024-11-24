using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickBite.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDilveryDriver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryDriverId",
                table: "Restaurant",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantDeliveryDriverId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryDriverId",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "RestaurantDeliveryDriverId",
                table: "AspNetUsers");
        }
    }
}

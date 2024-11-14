using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickBite.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RestaurantOwenrId",
                table: "Restaurant",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_RestaurantOwenrId",
                table: "Restaurant",
                column: "RestaurantOwenrId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurant_AspNetUsers_RestaurantOwenrId",
                table: "Restaurant",
                column: "RestaurantOwenrId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurant_AspNetUsers_RestaurantOwenrId",
                table: "Restaurant");

            migrationBuilder.DropIndex(
                name: "IX_Restaurant_RestaurantOwenrId",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "RestaurantOwenrId",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "AspNetUsers");
        }
    }
}

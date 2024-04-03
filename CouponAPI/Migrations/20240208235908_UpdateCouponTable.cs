﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mango.Services.CouponAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCouponTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimalAmountRequired",
                table: "Coupons");

            migrationBuilder.AddColumn<double>(
                name: "MinimalAmount",
                table: "Coupons",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponId", "CouponCode", "DiscountAmount", "MinimalAmount" },
                values: new object[,]
                {
                    { 1, "10OFF", 10.0, 20.0 },
                    { 2, "20OFF", 20.0, 40.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "MinimalAmount",
                table: "Coupons");

            migrationBuilder.AddColumn<int>(
                name: "MinimalAmountRequired",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
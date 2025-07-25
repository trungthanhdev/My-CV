﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class changeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "expOfYear",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d3c1945f-a4e0-470b-aabd-88e81fb2a1b6",
                column: "expOfYear",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
                column: "expOfYear",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "expOfYear",
                table: "AspNetUsers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d3c1945f-a4e0-470b-aabd-88e81fb2a1b6",
                column: "expOfYear",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
                column: "expOfYear",
                value: null);
        }
    }
}

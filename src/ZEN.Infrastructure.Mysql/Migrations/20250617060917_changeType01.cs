using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class changeType01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "dob",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d3c1945f-a4e0-470b-aabd-88e81fb2a1b6",
                column: "dob",
                value: "10/11/2003");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
                column: "dob",
                value: "10/11/2003");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "dob",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d3c1945f-a4e0-470b-aabd-88e81fb2a1b6",
                column: "dob",
                value: new DateTime(2001, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
                column: "dob",
                value: new DateTime(2003, 11, 10, 0, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}

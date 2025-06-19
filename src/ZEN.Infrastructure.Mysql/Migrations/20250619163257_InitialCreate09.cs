using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CERTIFICATE_AspNetUsers_user_id",
                table: "CERTIFICATE");

            migrationBuilder.AddForeignKey(
                name: "FK_CERTIFICATE_AspNetUsers_user_id",
                table: "CERTIFICATE",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CERTIFICATE_AspNetUsers_user_id",
                table: "CERTIFICATE");

            migrationBuilder.AddForeignKey(
                name: "FK_CERTIFICATE_AspNetUsers_user_id",
                table: "CERTIFICATE",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

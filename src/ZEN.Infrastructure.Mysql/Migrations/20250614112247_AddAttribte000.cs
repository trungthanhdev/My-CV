using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class AddAttribte000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USERSKILL_AspNetUsers_user_id",
                table: "USERSKILL");

            migrationBuilder.AddForeignKey(
                name: "FK_USERSKILL_AspNetUsers_user_id",
                table: "USERSKILL",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USERSKILL_AspNetUsers_user_id",
                table: "USERSKILL");

            migrationBuilder.AddForeignKey(
                name: "FK_USERSKILL_AspNetUsers_user_id",
                table: "USERSKILL",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

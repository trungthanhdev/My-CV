using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class changeType02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "url_contract",
                table: "PROJECT",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "url_excel",
                table: "PROJECT",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "url_contract",
                table: "PROJECT");

            migrationBuilder.DropColumn(
                name: "url_excel",
                table: "PROJECT");
        }
    }
}

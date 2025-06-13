using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributee02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tech",
                table: "PROJECT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tech",
                table: "PROJECT",
                type: "text",
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "position",
                table: "USERSKILL",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "background",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "expOfYear",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "facebook_url",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "linkedin_url",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mindset",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "position_career",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d3c1945f-a4e0-470b-aabd-88e81fb2a1b6",
                columns: new[] { "background", "expOfYear", "facebook_url", "linkedin_url", "mindset", "position_career" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
                columns: new[] { "background", "expOfYear", "facebook_url", "linkedin_url", "mindset", "position_career" },
                values: new object[] { null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "position",
                table: "USERSKILL");

            migrationBuilder.DropColumn(
                name: "background",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "expOfYear",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "facebook_url",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "linkedin_url",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "mindset",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "position_career",
                table: "AspNetUsers");
        }
    }
}

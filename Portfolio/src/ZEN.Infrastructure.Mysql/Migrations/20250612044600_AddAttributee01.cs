using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributee01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "position",
                table: "USERSKILL");

            migrationBuilder.AddColumn<string>(
                name: "position",
                table: "SKILL",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TECH",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tech_name = table.Column<string>(type: "text", nullable: true),
                    project_id = table.Column<string>(type: "character varying(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECH", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TECH_PROJECT_project_id",
                        column: x => x.project_id,
                        principalTable: "PROJECT",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TECH_project_id",
                table: "TECH",
                column: "project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TECH");

            migrationBuilder.DropColumn(
                name: "position",
                table: "SKILL");

            migrationBuilder.AddColumn<string>(
                name: "position",
                table: "USERSKILL",
                type: "text",
                nullable: true);
        }
    }
}

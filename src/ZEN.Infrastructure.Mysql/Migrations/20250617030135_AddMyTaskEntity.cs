using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class AddMyTaskEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MYTASK",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    we_id = table.Column<string>(type: "character varying(255)", nullable: true),
                    task_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MYTASK", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MYTASK_WORKEXPERIENCE_we_id",
                        column: x => x.we_id,
                        principalTable: "WORKEXPERIENCE",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MYTASK_we_id",
                table: "MYTASK",
                column: "we_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MYTASK");
        }
    }
}

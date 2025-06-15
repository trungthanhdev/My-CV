using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class AddAttribte001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USERSKILL_SKILL_skill_id",
                table: "USERSKILL");

            migrationBuilder.AddColumn<string>(
                name: "SkillId",
                table: "USERSKILL",
                type: "character varying(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_USERSKILL_SkillId",
                table: "USERSKILL",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_USERSKILL_SKILL_SkillId",
                table: "USERSKILL",
                column: "SkillId",
                principalTable: "SKILL",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_USERSKILL_SKILL_skill_id",
                table: "USERSKILL",
                column: "skill_id",
                principalTable: "SKILL",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USERSKILL_SKILL_SkillId",
                table: "USERSKILL");

            migrationBuilder.DropForeignKey(
                name: "FK_USERSKILL_SKILL_skill_id",
                table: "USERSKILL");

            migrationBuilder.DropIndex(
                name: "IX_USERSKILL_SkillId",
                table: "USERSKILL");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "USERSKILL");

            migrationBuilder.AddForeignKey(
                name: "FK_USERSKILL_SKILL_skill_id",
                table: "USERSKILL",
                column: "skill_id",
                principalTable: "SKILL",
                principalColumn: "Id");
        }
    }
}

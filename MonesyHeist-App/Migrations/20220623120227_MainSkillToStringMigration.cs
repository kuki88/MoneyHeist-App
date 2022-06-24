using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonesyHeist_App.Migrations
{
    public partial class MainSkillToStringMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Skill_MainSkillSkillId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_MainSkillSkillId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MainSkillSkillId",
                table: "Members");

            migrationBuilder.AddColumn<string>(
                name: "MainSkill",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainSkill",
                table: "Members");

            migrationBuilder.AddColumn<int>(
                name: "MainSkillSkillId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_MainSkillSkillId",
                table: "Members",
                column: "MainSkillSkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Skill_MainSkillSkillId",
                table: "Members",
                column: "MainSkillSkillId",
                principalTable: "Skill",
                principalColumn: "SkillId");
        }
    }
}

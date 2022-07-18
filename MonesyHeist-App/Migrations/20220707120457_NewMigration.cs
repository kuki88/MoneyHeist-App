using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonesyHeist_App.Migrations
{
    public partial class NewMigration : Migration
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

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Sex",
                table: "Members",
                type: "nvarchar(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "MainSkill",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Heists",
                columns: table => new
                {
                    HeistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heists", x => x.HeistId);
                });

            migrationBuilder.CreateTable(
                name: "HeistSkills",
                columns: table => new
                {
                    HeistSkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Members = table.Column<int>(type: "int", nullable: false),
                    HeistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeistSkills", x => x.HeistSkillId);
                    table.ForeignKey(
                        name: "FK_HeistSkills_Heists_HeistId",
                        column: x => x.HeistId,
                        principalTable: "Heists",
                        principalColumn: "HeistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeistSkills_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Heists_Name",
                table: "Heists",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeistSkills_HeistId",
                table: "HeistSkills",
                column: "HeistId");

            migrationBuilder.CreateIndex(
                name: "IX_HeistSkills_SkillId",
                table: "HeistSkills",
                column: "SkillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeistSkills");

            migrationBuilder.DropTable(
                name: "Heists");

            migrationBuilder.DropColumn(
                name: "MainSkill",
                table: "Members");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Members",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Sex",
                table: "Members",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)");

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

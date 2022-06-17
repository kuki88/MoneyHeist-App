using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonesyHeist_App.Migrations
{
    public partial class InitializeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainSkill = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => new { x.Id, x.Email });
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberEmail = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_Members_MemberId_MemberEmail",
                        columns: x => new { x.MemberId, x.MemberEmail },
                        principalTable: "Members",
                        principalColumns: new[] { "Id", "Email" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skills_MemberId_MemberEmail",
                table: "Skills",
                columns: new[] { "MemberId", "MemberEmail" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}

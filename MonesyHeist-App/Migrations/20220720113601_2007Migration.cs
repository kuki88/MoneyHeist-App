using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonesyHeist_App.Migrations
{
    public partial class _2007Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeistMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeistId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeistMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeistMembers_Heists_HeistId",
                        column: x => x.HeistId,
                        principalTable: "Heists",
                        principalColumn: "HeistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeistMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeistMembers_HeistId",
                table: "HeistMembers",
                column: "HeistId");

            migrationBuilder.CreateIndex(
                name: "IX_HeistMembers_MemberId",
                table: "HeistMembers",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeistMembers");
        }
    }
}

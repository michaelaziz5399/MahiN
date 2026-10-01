using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahiN.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_users_CaptainId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_CaptainId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CaptainId",
                table: "Teams");

            migrationBuilder.AddColumn<bool>(
                name: "IsCaptain",
                table: "TeamMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PlayerSuspensions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Restriction = table.Column<int>(type: "int", nullable: false),
                    TournamentId = table.Column<int>(type: "int", nullable: true),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumberOfTournaments = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSuspensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerSuspensions_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlayerSuspensions_users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                unique: true,
                filter: "[IsCaptain] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSuspensions_PlayerId",
                table: "PlayerSuspensions",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSuspensions_TournamentId",
                table: "PlayerSuspensions",
                column: "TournamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerSuspensions");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "IsCaptain",
                table: "TeamMembers");

            migrationBuilder.AddColumn<string>(
                name: "CaptainId",
                table: "Teams",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CaptainId",
                table: "Teams",
                column: "CaptainId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_users_CaptainId",
                table: "Teams",
                column: "CaptainId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

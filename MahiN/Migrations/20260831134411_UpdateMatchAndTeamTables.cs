using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahiN.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMatchAndTeamTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_TournamentId",
                table: "Teams");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "HomeTeamId",
                table: "TournamentMatches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AwayTeamId",
                table: "TournamentMatches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AwayPenaltyScore",
                table: "TournamentMatches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AwayPlaceholder",
                table: "TournamentMatches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "TournamentMatches",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomePenaltyScore",
                table: "TournamentMatches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomePlaceholder",
                table: "TournamentMatches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExtraTime",
                table: "TournamentMatches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPenaltyShootout",
                table: "TournamentMatches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NextMatchId",
                table: "TournamentMatches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "TournamentMatches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "Teams",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_NextMatchId",
                table: "TournamentMatches",
                column: "NextMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TournamentId_GroupName",
                table: "Teams",
                columns: new[] { "TournamentId", "GroupName" });

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatches_TournamentMatches_NextMatchId",
                table: "TournamentMatches",
                column: "NextMatchId",
                principalTable: "TournamentMatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_TournamentMatches_NextMatchId",
                table: "TournamentMatches");

            migrationBuilder.DropIndex(
                name: "IX_TournamentMatches_NextMatchId",
                table: "TournamentMatches");

            migrationBuilder.DropIndex(
                name: "IX_Teams_TournamentId_GroupName",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "AwayPenaltyScore",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "AwayPlaceholder",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "HomePenaltyScore",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "HomePlaceholder",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "IsExtraTime",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "IsPenaltyShootout",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "NextMatchId",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "Stage",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "Teams");

            migrationBuilder.AlterColumn<int>(
                name: "HomeTeamId",
                table: "TournamentMatches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AwayTeamId",
                table: "TournamentMatches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TournamentId",
                table: "Teams",
                column: "TournamentId");
        }
    }
}

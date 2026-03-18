using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceEdgeTrack.Migrations
{
    /// <inheritdoc />
    public partial class FixUserWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Carteira_CarteiraId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CarteiraId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CarteiraId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarteiraId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CarteiraId",
                table: "AspNetUsers",
                column: "CarteiraId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Carteira_CarteiraId",
                table: "AspNetUsers",
                column: "CarteiraId",
                principalTable: "Carteira",
                principalColumn: "CarteiraId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

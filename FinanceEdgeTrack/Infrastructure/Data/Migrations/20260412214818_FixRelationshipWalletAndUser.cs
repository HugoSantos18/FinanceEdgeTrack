using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceEdgeTrack.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationshipWalletAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AporteMetas",
                newName: "AporteMetasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AporteMetasId",
                table: "AporteMetas",
                newName: "Id");
        }
    }
}

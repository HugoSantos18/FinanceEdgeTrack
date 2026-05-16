using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceEdgeTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddXminAndAporteMetaIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Carteira: remove a coluna RowVersion (substituída pelo xmin do PG, que já existe como coluna de sistema)
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Carteira");

            // AporteMetas: limpa registros órfãos antes de tornar a FK NOT NULL
            migrationBuilder.Sql(@"DELETE FROM ""AporteMetas"" WHERE ""MetaId"" IS NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "MetaId",
                table: "AporteMetas",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "MetaId",
                table: "AporteMetas",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "Carteira",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}

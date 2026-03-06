using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FinanceEdgeTrack.Migrations
{
    /// <inheritdoc />
    public partial class Fixtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AporteMetas_Categorias_MetaId",
                table: "AporteMetas");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamento_AspNetUsers_UserId",
                table: "Lancamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamento_Categorias_CategoriaId",
                table: "Lancamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lancamento",
                table: "Lancamento");

            migrationBuilder.DropIndex(
                name: "IX_Lancamento_CategoriaId",
                table: "Lancamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Lancamento");

            migrationBuilder.DropColumn(
                name: "Concluida",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "DataAlvo",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "DataUltimoDeposito",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Fixa",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "PorcentagemAtual",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "PorcentagemRestante",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Receita_Data",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Receita_Valor",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "TipoCategoria",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UltimoDepositoEmReais",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "ValorAlvo",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "ValorRestante",
                table: "Categorias");

            migrationBuilder.RenameTable(
                name: "Lancamento",
                newName: "Lancamentos");

            migrationBuilder.RenameTable(
                name: "Categorias",
                newName: "Receitas");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "Lancamentos",
                newName: "LancamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Lancamento_UserId",
                table: "Lancamentos",
                newName: "IX_Lancamentos_UserId");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "Receitas",
                newName: "ReceitaId");

            migrationBuilder.AddColumn<int>(
                name: "CarteiraId",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpire",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalLancamentos",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "AspNetUsers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotalGasto",
                table: "AspNetUsers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotalInvestido",
                table: "AspNetUsers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "DespesaId",
                table: "Lancamentos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceitaId",
                table: "Lancamentos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Receitas",
                type: "numeric(15,2)",
                precision: 15,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(15,2)",
                oldPrecision: 15,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "Receitas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lancamentos",
                table: "Lancamentos",
                column: "LancamentoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receitas",
                table: "Receitas",
                column: "ReceitaId");

            migrationBuilder.CreateTable(
                name: "Carteira",
                columns: table => new
                {
                    CarteiraId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Saldo = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carteira", x => x.CarteiraId);
                    table.ForeignKey(
                        name: "FK_Carteira_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Despesas",
                columns: table => new
                {
                    DespesaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Valor = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    Fixa = table.Column<bool>(type: "boolean", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Despesas", x => x.DespesaId);
                });

            migrationBuilder.CreateTable(
                name: "Metas",
                columns: table => new
                {
                    MetaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    ValorAlvo = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    ValorAtual = table.Column<decimal>(type: "numeric", nullable: false),
                    UltimoDepositoEmReais = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    DataUltimoDeposito = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PorcentagemAtual = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorRestante = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlvo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metas", x => x.MetaId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CarteiraId",
                table: "AspNetUsers",
                column: "CarteiraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_DespesaId",
                table: "Lancamentos",
                column: "DespesaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_ReceitaId",
                table: "Lancamentos",
                column: "ReceitaId");

            migrationBuilder.CreateIndex(
                name: "IX_Carteira_UserId",
                table: "Carteira",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AporteMetas_Metas_MetaId",
                table: "AporteMetas",
                column: "MetaId",
                principalTable: "Metas",
                principalColumn: "MetaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Carteira_CarteiraId",
                table: "AspNetUsers",
                column: "CarteiraId",
                principalTable: "Carteira",
                principalColumn: "CarteiraId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamentos_AspNetUsers_UserId",
                table: "Lancamentos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamentos_Despesas_DespesaId",
                table: "Lancamentos",
                column: "DespesaId",
                principalTable: "Despesas",
                principalColumn: "DespesaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamentos_Receitas_ReceitaId",
                table: "Lancamentos",
                column: "ReceitaId",
                principalTable: "Receitas",
                principalColumn: "ReceitaId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AporteMetas_Metas_MetaId",
                table: "AporteMetas");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Carteira_CarteiraId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamentos_AspNetUsers_UserId",
                table: "Lancamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamentos_Despesas_DespesaId",
                table: "Lancamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamentos_Receitas_ReceitaId",
                table: "Lancamentos");

            migrationBuilder.DropTable(
                name: "Carteira");

            migrationBuilder.DropTable(
                name: "Despesas");

            migrationBuilder.DropTable(
                name: "Metas");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CarteiraId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lancamentos",
                table: "Lancamentos");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_DespesaId",
                table: "Lancamentos");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_ReceitaId",
                table: "Lancamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receitas",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "CarteiraId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpire",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TotalLancamentos",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ValorTotalGasto",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ValorTotalInvestido",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DespesaId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "ReceitaId",
                table: "Lancamentos");

            migrationBuilder.RenameTable(
                name: "Lancamentos",
                newName: "Lancamento");

            migrationBuilder.RenameTable(
                name: "Receitas",
                newName: "Categorias");

            migrationBuilder.RenameColumn(
                name: "LancamentoId",
                table: "Lancamento",
                newName: "CategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Lancamentos_UserId",
                table: "Lancamento",
                newName: "IX_Lancamento_UserId");

            migrationBuilder.RenameColumn(
                name: "ReceitaId",
                table: "Categorias",
                newName: "CategoriaId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Lancamento",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Categorias",
                type: "numeric(15,2)",
                precision: 15,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(15,2)",
                oldPrecision: 15,
                oldScale: 2);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "Categorias",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "Concluida",
                table: "Categorias",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAlvo",
                table: "Categorias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Categorias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimoDeposito",
                table: "Categorias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Fixa",
                table: "Categorias",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PorcentagemAtual",
                table: "Categorias",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PorcentagemRestante",
                table: "Categorias",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Receita_Data",
                table: "Categorias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Receita_Valor",
                table: "Categorias",
                type: "double precision",
                precision: 15,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Categorias",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCategoria",
                table: "Categorias",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "UltimoDepositoEmReais",
                table: "Categorias",
                type: "numeric(15,2)",
                precision: 15,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorAlvo",
                table: "Categorias",
                type: "numeric(15,2)",
                precision: 15,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorRestante",
                table: "Categorias",
                type: "numeric(15,2)",
                precision: 15,
                scale: 2,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lancamento",
                table: "Lancamento",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_CategoriaId",
                table: "Lancamento",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_AporteMetas_Categorias_MetaId",
                table: "AporteMetas",
                column: "MetaId",
                principalTable: "Categorias",
                principalColumn: "CategoriaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamento_AspNetUsers_UserId",
                table: "Lancamento",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamento_Categorias_CategoriaId",
                table: "Lancamento",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "CategoriaId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

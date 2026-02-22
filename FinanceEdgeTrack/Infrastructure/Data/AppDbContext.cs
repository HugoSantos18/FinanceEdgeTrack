using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Models.Abstract;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FinanceEdgeTrack.Infrastructure.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    DbSet<Categoria> Categorias { get; set; }
    DbSet<AporteMetas> AporteMetas { get; set; }
    DbSet<Lancamento> Lancamentos { get; set; }


    protected override void OnModelCreating(ModelBuilder model)
    {

        // TPH (Herança de classes e tabelas)
        model.Entity<Categoria>()
             .ToTable("Categorias")
             .HasDiscriminator<string>("TipoCategoria")
             .HasValue<Meta>("Meta")
             .HasValue<Despesa>("Despesa")
             .HasValue<Receita>("Receita");

        model.Entity<Meta>().ToTable("Categorias");
        model.Entity<Receita>().ToTable("Categorias");
        model.Entity<Despesa>().ToTable("Categorias");


        // Categorias
        model.Entity<Categoria>()
            .HasKey(c => c.CategoriaId);

        model.Entity<Categoria>()
            .Property(c => c.Titulo)
            .HasMaxLength(150)
            .IsRequired();

        model.Entity<Categoria>()
            .Property(c => c.Descricao)
            .HasMaxLength(200);


        // Metas
        model.Entity<Meta>()
       .HasMany(m => m.Aportes)
       .WithOne(a => a.Meta)
       .HasForeignKey(a => a.MetaId)
       .OnDelete(DeleteBehavior.Cascade);

        model.Entity<Meta>()
            .Property(m => m.ValorAlvo)
            .HasPrecision(15, 2)
            .IsRequired();

        model.Entity<Meta>()
            .Property(m => m.ValorRestante)
            .HasPrecision(15, 2);

        model.Entity<Meta>()
            .Property(m => m.UltimoDepositoEmReais)
            .HasPrecision(15, 2);



        // Aporte Metas
        model.Entity<AporteMetas>()
            .Property(a => a.Valor)
            .HasPrecision(15, 2);

        model.Entity<AporteMetas>()
            .HasKey(a => a.Id);


        // Lançamento
        model.Entity<Lancamento>()
       .HasOne(l => l.User)
       .WithMany()
       .HasForeignKey(l => l.UserId)
       .OnDelete(DeleteBehavior.Cascade);


        model.Entity<Lancamento>()
       .HasOne(l => l.Categoria)
       .WithMany()
       .HasForeignKey(l => l.CategoriaId)
       .OnDelete(DeleteBehavior.Restrict);

        model.Entity<Lancamento>()
            .HasKey(l => l.Id);
         
        model.Entity<Lancamento>()
            .Property(l => l.DataLancamento)
            .IsRequired();


        // Receita
        model.Entity<Receita>()
            .Property(r => r.Valor)
            .HasPrecision(15, 2)
            .IsRequired();

        model.Entity<Receita>()
            .Property(r => r.Titulo)
            .HasMaxLength(150)
            .IsRequired();

        model.Entity<Receita>()
            .Property(r => r.Data)
            .IsRequired();


        // Despesa
        model.Entity<Despesa>()
            .Property(d => d.Valor)
            .HasPrecision(15, 2)
            .IsRequired();

        model.Entity<Despesa>()
            .Property(d => d.Titulo)
            .HasMaxLength(150)
            .IsRequired();

        model.Entity<Despesa>()
            .Property(d => d.Data)
            .IsRequired();


        base.OnModelCreating(model);
    }
}

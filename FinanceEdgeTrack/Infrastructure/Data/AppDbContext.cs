using FinanceEdgeTrack.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FinanceEdgeTrack.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    DbSet<Receita> Receitas { get; set; }
    DbSet<Despesa> Despesas { get; set; }
    DbSet<Meta> Metas { get; set; }
    DbSet<Carteira> Carteiras { get; set; }
    DbSet<AporteMetas> AporteMetas { get; set; }


    protected override void OnModelCreating(ModelBuilder model)
    {

        // Categorias
        model.Entity<Receita>()
            .HasKey(r => r.ReceitaId);

        model.Entity<Receita>()
            .Property(c => c.Titulo)
            .HasMaxLength(150)
            .IsRequired();

        model.Entity<Receita>()
            .Property(c => c.Descricao)
            .HasMaxLength(200);

        // despesa
        model.Entity<Despesa>()
            .HasKey(d => d.DespesaId);

        model.Entity<Despesa>()
            .Property(d => d.Titulo)
            .HasMaxLength(150)
            .IsRequired();

        model.Entity<Despesa>()
            .Property(d => d.Descricao)
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


        // Carteira
        model.Entity<Carteira>(entity =>
        {
            entity.ToTable("Carteira");

            entity.Property(c => c.CarteiraId)
            .UseIdentityByDefaultColumn();

            entity.Property(s => s.Saldo)
           .HasPrecision(15, 2);
        });


        // User
        model.Entity<ApplicationUser>()
             .HasOne(u => u.Carteira)
             .WithOne(c => c.User)
             .HasForeignKey<Carteira>(c => c.UserId)
             .OnDelete(DeleteBehavior.Cascade);


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

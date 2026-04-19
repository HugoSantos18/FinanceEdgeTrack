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
        // Carteira
        model.Entity<Carteira>(entity =>
        {
            entity.HasKey(c => c.CarteiraId);

            entity.Property(c => c.CarteiraId)
            .ValueGeneratedNever();

            entity.Property(c => c.UserId)
                  .IsRequired();

            entity.HasIndex(c => c.UserId)
                 .IsUnique();

            entity.Property(s => s.Saldo)
           .HasPrecision(15, 2);

            // User (1:1)
            entity.HasOne<ApplicationUser>()
             .WithOne(u => u.Carteira)
             .HasForeignKey<Carteira>(c => c.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            // Metas (1:N)
            entity.HasMany(c => c.Metas)
              .WithOne(m => m.Carteira)
              .HasForeignKey(m => m.CarteiraId)
              .OnDelete(DeleteBehavior.Cascade);

            // Receitas (1:N)
            entity.HasMany(c => c.Receitas)
             .WithOne(r => r.Carteira)
             .HasForeignKey(r => r.CarteiraId)
             .OnDelete(DeleteBehavior.Cascade);

            // Despesas (1:N)
            entity.HasMany(c => c.Despesas)
             .WithOne(d => d.Carteira)
             .HasForeignKey(d => d.CarteiraId)
             .OnDelete(DeleteBehavior.Cascade);

        });

        // Metas
        model.Entity<Meta>(entity =>
        {
            entity.HasKey(m => m.MetaId);

            entity.Property(m => m.Titulo)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(m => m.ValorAlvo)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(m => m.ValorAtual)
                  .HasColumnType("decimal(18,2)");

            entity.Property(m => m.UltimoDepositoEmReais)
                  .HasColumnType("decimal(18,2)");

            entity.Property(m => m.PorcentagemAtual)
                  .HasColumnType("decimal(5,2)");

            entity.Property(m => m.ValorRestante)
                  .HasColumnType("decimal(18,2)");

            entity.Property(m => m.Status)
                  .HasConversion<string>();

            // Carteira (1:N)
            entity.HasOne(m => m.Carteira)
                  .WithMany(c => c.Metas)
                  .HasForeignKey(m => m.CarteiraId);

            // Aportes (1:N)
            entity.HasMany(m => m.Aportes)
                  .WithOne()
                  .HasForeignKey("MetaId")
                  .OnDelete(DeleteBehavior.Cascade);
        });


        // Aporte Metas
        model.Entity<AporteMetas>(entity =>
        {
            entity.HasKey(a => a.AporteMetasId);
            
            entity.Property(a => a.Valor)
                  .HasPrecision(18, 2)
                  .IsRequired();
        });


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

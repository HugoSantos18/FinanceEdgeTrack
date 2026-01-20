using FinanceEdgeTrack.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Infrastructure.Data;

public class AppDbContext : IdentityDbContext
{



    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        // Mapeamento de entidades para o DB.
    }
}

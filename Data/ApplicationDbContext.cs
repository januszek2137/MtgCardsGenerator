using AiMagicCardsGenerator.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AiMagicCardsGenerator.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Card>          Cards          => Set<Card>();
    public DbSet<GeneratedCard> GeneratedCards => Set<GeneratedCard>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Card>(e =>
        {
            e.HasIndex(c => c.ScryfallId).IsUnique();
            e.HasIndex(c => c.Name);
            e.HasIndex(c => c.Cmc);
            e.HasIndex(c => c.TypeLine);
            e.HasIndex(c => c.Colors);
        });
    }
}
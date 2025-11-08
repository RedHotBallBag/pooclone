using LaunchBox.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LaunchBox.Data;

public class LaunchBoxDbContext : DbContext
{
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Platform> Platforms { get; set; } = null!;
    public DbSet<Emulator> Emulators { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<AppSettings> Settings { get; set; } = null!;

    public LaunchBoxDbContext(DbContextOptions<LaunchBoxDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Platform configuration
        modelBuilder.Entity<Platform>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Manufacturer).HasMaxLength(200);
        });

        // Game configuration
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.PlatformId);
            entity.HasIndex(e => e.IsFavorite);

            entity.HasOne(e => e.Platform)
                  .WithMany(p => p.Games)
                  .HasForeignKey(e => e.PlatformId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PreferredEmulator)
                  .WithMany()
                  .HasForeignKey(e => e.PreferredEmulatorId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Tags)
                  .WithMany(t => t.Games)
                  .UsingEntity(j => j.ToTable("GameTags"));
        });

        // Emulator configuration
        modelBuilder.Entity<Emulator>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ExecutablePath).IsRequired().HasMaxLength(1000);
            entity.HasIndex(e => e.PlatformId);

            entity.HasOne(e => e.Platform)
                  .WithMany(p => p.Emulators)
                  .HasForeignKey(e => e.PlatformId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Tag configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // AppSettings configuration
        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed common platforms
        modelBuilder.Entity<Platform>().HasData(
            new Platform { Id = 1, Name = "Nintendo Entertainment System", Description = "8-bit home console", Manufacturer = "Nintendo", ReleaseYear = 1983 },
            new Platform { Id = 2, Name = "Super Nintendo Entertainment System", Description = "16-bit home console", Manufacturer = "Nintendo", ReleaseYear = 1990 },
            new Platform { Id = 3, Name = "Nintendo 64", Description = "64-bit home console", Manufacturer = "Nintendo", ReleaseYear = 1996 },
            new Platform { Id = 4, Name = "PlayStation", Description = "32-bit home console", Manufacturer = "Sony", ReleaseYear = 1994 },
            new Platform { Id = 5, Name = "PlayStation 2", Description = "128-bit home console", Manufacturer = "Sony", ReleaseYear = 2000 },
            new Platform { Id = 6, Name = "Sega Genesis", Description = "16-bit home console", Manufacturer = "Sega", ReleaseYear = 1988 },
            new Platform { Id = 7, Name = "Game Boy", Description = "8-bit handheld console", Manufacturer = "Nintendo", ReleaseYear = 1989 },
            new Platform { Id = 8, Name = "Game Boy Advance", Description = "32-bit handheld console", Manufacturer = "Nintendo", ReleaseYear = 2001 },
            new Platform { Id = 9, Name = "Nintendo DS", Description = "Dual-screen handheld console", Manufacturer = "Nintendo", ReleaseYear = 2004 },
            new Platform { Id = 10, Name = "Arcade", Description = "Arcade machines", Manufacturer = "Various", ReleaseYear = 1970 }
        );

        // Seed default app settings
        modelBuilder.Entity<AppSettings>().HasData(
            new AppSettings { Id = 1 }
        );
    }
}

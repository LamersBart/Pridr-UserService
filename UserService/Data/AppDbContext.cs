using Microsoft.EntityFrameworkCore;
using UserService.Data.Encryption;
using UserService.Models;

namespace UserService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Profile> Profiles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Profile>(entity =>
        { 
            entity.Property(e => e.UserName)
                .HasColumnType("text");
            entity.Property(e => e.UserName)
                .HasConversion(new EncryptedReferenceConverter<string>());
            
            entity.Property(e => e.Latitude)
                .HasColumnType("text");
            entity.Property(e => e.Latitude)
                .HasConversion(new EncryptedValueConverter<double>());
            
            entity.Property(e => e.Longitude)
                .HasColumnType("text");
            entity.Property(e => e.Longitude)
                .HasConversion(new EncryptedValueConverter<double>());

            entity.Property(e => e.Weight)
                .HasColumnType("text");
            entity.Property(e => e.Weight)
                .HasConversion(new EncryptedValueConverter<double>());

            entity.Property(e => e.Height)
                .HasColumnType("text");
            entity.Property(e => e.Height)
                .HasConversion(new EncryptedValueConverter<double>());

            entity.Property(e => e.Age)
                .HasColumnType("text");
            entity.Property(e => e.Age)
                .HasConversion(new EncryptedValueConverter<int>());
        });
        // data seeden 
    }
}
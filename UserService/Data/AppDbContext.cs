using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<User> User { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile);
        
        modelBuilder.Entity<Profile>(entity =>
        { 
            entity.Property(e => e.UserName)
                .HasColumnType("text");
            entity.Property(e => e.UserName)
                .HasConversion(new EncryptedValueConverter<string>());
            
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
                .HasConversion(new EncryptedValueConverter<double?>());

            entity.Property(e => e.Height)
                .HasColumnType("text");
            entity.Property(e => e.Height)
                .HasConversion(new EncryptedValueConverter<double?>());

            entity.Property(e => e.Age)
                .HasColumnType("text");
            entity.Property(e => e.Age)
                .HasConversion(new EncryptedValueConverter<int?>());
            
            entity.Property(e => e.PartnerUserId)
                .HasColumnType("text");
            entity.Property(e => e.PartnerUserId)
                .HasConversion(new EncryptedValueConverter<int?>());
        });
    }
}
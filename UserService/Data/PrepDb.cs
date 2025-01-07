using Microsoft.EntityFrameworkCore;
using UserService.Enums;
using UserService.Models;

namespace UserService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetRequiredService<AppDbContext>(), isProduction);
        }
    }

    private static void SeedData(AppDbContext context, bool isProduction)
    {
        if(isProduction)
        {
            Console.WriteLine("--> Attemt to apply migrations...");
            try
            { 
                context.Database.Migrate();
                Console.WriteLine("--> Migrations applied.");
                
                // Controleer of de tabel bestaat
                var tableExists = context.Database.ExecuteSqlRaw(
                    "SELECT 1 FROM information_schema.tables WHERE table_name = 'Profiles';");
                if (tableExists == 0)
                {
                    Console.WriteLine("--> Profiles table does not exist! Creating seed data...");
                    SeedProfiles(context);
                }
                else
                {
                    Console.WriteLine("--> Profiles table already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not run migrations: {ex.Message}");
            }
        }
    }
    
    private static void SeedProfiles(AppDbContext context)
    {
        if (!context.Profiles.Any())
        {
            Console.WriteLine("--> Seeding data");
            context.Profiles.AddRange(
                new Profile {
                    KeyCloakId = "23097294857348968936",
                    Sexuality = Sexuality.Unknown,
                    LookingFor = LookingFor.Friendship,
                    Latitude = 120.20291,
                    Longitude = 12349.20202,
                    UserName = "user1",
                    RelationStatus = RelationStatus.Unknown,
                    Age = 28,
                    Height = 190.0,
                    Weight = 70.0,
                    // PartnerUserId = 0,
                },
                new Profile {
                    KeyCloakId = "290823097238926257253",
                    Sexuality = Sexuality.Unknown,
                    LookingFor = LookingFor.Friendship,
                    Latitude = 120.20291,
                    Longitude = 12349.20202,
                    UserName = "user2",
                    RelationStatus = RelationStatus.Unknown,
                    Age = 20,
                    Height = 170.0,
                    Weight = 60.0,
                    // PartnerUserId = 0,
                });
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> We already have data");
        }
    }
    
}
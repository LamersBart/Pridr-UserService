using Microsoft.EntityFrameworkCore;
using UserService.Enums;
using UserService.Models;

namespace UserService.Data;

public static class PrepDb
{
    public static async Task PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            await SeedData(serviceScope.ServiceProvider.GetRequiredService<AppDbContext>(), isProduction);
        }
    }

    private static async Task SeedData(AppDbContext context, bool isProduction)
    {
        if(isProduction)
        {
            Console.WriteLine("--> Attemt to apply migrations...");
            try
            { 
                await context.Database.MigrateAsync();
                Console.WriteLine("--> Migrations applied.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not run migrations: {ex.Message}");
            }
            if (!await context.Profiles.AnyAsync())
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
                    });
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}
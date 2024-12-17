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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not run migrations: {ex.Message}");
            }
        }

        if (!context.Profiles.Any())
        {
            Console.WriteLine("--> Seeding data");
            context.User.AddRange(
                new User
                {
                    KeyCloakId = "",
                    Email = "admin@admin.com",
                    Profile = new Profile
                    {
                        Sexuality = Sexuality.Unknown,
                        LookingFor = LookingFor.Friendship,
                        Latitude = 120.20291,
                        Longitude = 12349.20202,
                        UserName = "user2",
                        RelationStatus = RelationStatus.Unknown,
                        Age = 20,
                        Height = 170.0,
                        Weight = 60.0,
                        PartnerUserId = 0,
                    }
                },
                new User
                {
                    KeyCloakId = "",
                    Email = "user@test.com",
                    Profile = new Profile
                    {
                        Sexuality = Sexuality.Unknown,
                        LookingFor = LookingFor.Fun,
                        Latitude = 120.20291,
                        Longitude = 12349.20202,
                        UserName = "user1",
                        RelationStatus = RelationStatus.Unknown,
                        Age = 26,
                        Height = 190.0,
                        Weight = 72.0,
                        PartnerUserId = 0
                    }
                });
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> We already have data");
        }
    }
}
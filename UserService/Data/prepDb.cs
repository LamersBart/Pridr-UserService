using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using UserService.Enums;
using UserService.Models;

namespace UserService.Data;

public class prepDb
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
                    UserName = "admin",
                    Email = "admin@admin.com",
                    Profile = new Profile
                    {
                        Sexuality = Sexuality.Gay,
                        LookingFor = LookingFor.Friendship,
                        Latitude = 120.20291,
                        Longitude = 12349.20202,
                        UserId = 0,
                        UserName = "user2"
                    }
                },
                new User
                {
                    KeyCloakId = "",
                    UserName = "user1",
                    Email = "user@test.com",
                    Profile = new Profile
                    {
                        Sexuality = Sexuality.Lesbian,
                        LookingFor = LookingFor.Fun,
                        Latitude = 120.20291,
                        Longitude = 12349.20202,
                        Id = 0,
                        UserId = 0,
                        UserName = "user1"
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
using System.Text.Json;
using AutoMapper;
using UserService.Data;
using UserService.DTOs;
using UserService.Enums;
using Profile = UserService.Models.Profile;

namespace UserService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
    }
    
    public async Task ProcessEvent(string message)
    {
        Console.WriteLine($"--> Event received");
        var eventType = DetermineEvent(message);
        switch (eventType)
        {
            case EventType.Register:
                Console.WriteLine($"--> Event: {message}");
                await AddUser(message);
                break;
            case EventType.Delete:
                Console.WriteLine($"--> Event: {message}");
                await DeleteUser(message);
                break;
            default:
                break;
        }
    }

    private static EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");
        var keycloakEvent = JsonSerializer.Deserialize<KeycloakEventDto>(notificationMessage);
        if (keycloakEvent != null)
        {
            switch (keycloakEvent.Type)
            {
                case "LOGIN":
                    Console.WriteLine("--> Login Event Detected");
                    return EventType.Login;
                case "LOGOUT":
                    Console.WriteLine("--> Logout Event Detected");
                    return EventType.Logout;
                case "REGISTER":
                    Console.WriteLine("--> Register Event Detected");
                    return EventType.Register;
                case "DELETE_ACCOUNT":
                    Console.WriteLine("--> Delete account Event Detected");
                    return EventType.Delete;
                default:
                    Console.WriteLine("--> Other Event Detected");
                    return EventType.Undetermined;
            }
        }
        Console.WriteLine("--> Received Event is 'NULL'");
        return EventType.Undetermined;
    }

    private async Task AddUser(string keyCloakPublishedMessage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var profileRepo = scope.ServiceProvider.GetRequiredService<IProfileRepo>();
            var keycloakEvent = JsonSerializer.Deserialize<KeycloakEventDto>(keyCloakPublishedMessage);
            try
            {
                Profile newProfile = _mapper.Map<Profile>(keycloakEvent);
                if (!await profileRepo.ProfileExistAsync(newProfile.KeyCloakId))
                {
                    await profileRepo.CreateProfileAsync(newProfile);
                    await profileRepo.SaveChangesAsync();
                    Console.WriteLine("--> User Added!");
                }
                else
                {
                    Console.WriteLine($"--> User {newProfile.KeyCloakId} already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add user to DB {ex.Message}");
            }
        }
    }
    
    private async Task DeleteUser(string keyCloakPublishedMessage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var profileRepo = scope.ServiceProvider.GetRequiredService<IProfileRepo>();
            var keycloakEvent = JsonSerializer.Deserialize<KeycloakEventDto>(keyCloakPublishedMessage);
            try
            {
                if (keycloakEvent != null)
                {
                    if (await profileRepo.ProfileExistAsync(keycloakEvent.UserId))
                    {
                        var profile = await profileRepo.GetProfileByIdAsync(keycloakEvent.UserId);
                        profileRepo.DeleteProfile(profile);
                        await profileRepo.SaveChangesAsync();
                        Console.WriteLine("--> User Deleted!");
                    }
                    else
                    {
                        Console.WriteLine($"--> User {keycloakEvent.UserId} does not exist");
                    }
                }
                else
                {
                    Console.WriteLine($"--> Failed reading keycloak event {keyCloakPublishedMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not remove user from DB {ex.Message}");
            }
        }
    }
}
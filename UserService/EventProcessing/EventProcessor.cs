using System.Text.Json;
using AutoMapper;
using UserService.Data;
using UserService.DTOs;
using UserService.Enums;
using UserService.Models;
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
    
    public void ProcessEvent(string message)
    {
        Console.WriteLine($"--> Event received");
        var eventType = DetermineEvent(message);
        switch (eventType)
        {
            case EventType.Register:
                Console.WriteLine($"--> Event: {message}");
                AddUser(message);
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
                default:
                    Console.WriteLine("--> Other Event Detected");
                    return EventType.Undetermined;
            }
        Console.WriteLine("--> Other Event Detected");
        return EventType.Undetermined;
    }

    private void AddUser(string keyCloakPublishedMessage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var profileRepo = scope.ServiceProvider.GetRequiredService<IProfileRepo>();
            var keycloakEvent = JsonSerializer.Deserialize<KeycloakEventDto>(keyCloakPublishedMessage);
            try
            {
                Profile newProfile = _mapper.Map<Profile>(keycloakEvent);
                if (!profileRepo.ProfileExist(newProfile.KeyCloakId))
                {
                    profileRepo.CreateProfile(newProfile);
                    profileRepo.SaveChanges();
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
}
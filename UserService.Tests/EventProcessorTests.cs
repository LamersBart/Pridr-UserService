using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserService.Data;
using UserService.DTOs;
using UserService.Enums;
using UserService.EventProcessing;
using Profile = UserService.Models.Profile;

namespace UserService.Tests;

public class EventProcessorTests
{
    private readonly Mock<IProfileRepo> _mockRepo;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IServiceScope> _mockScope;
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
    private readonly IMapper _mapper;
    private readonly EventProcessor _processor;

    public EventProcessorTests()
    {
        _mockRepo = new Mock<IProfileRepo>();

        // Stel de ServiceProvider mock in
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IProfileRepo)))
            .Returns(_mockRepo.Object);

        // Stel de Scope mock in
        _mockScope = new Mock<IServiceScope>();
        _mockScope.Setup(s => s.ServiceProvider).Returns(_mockServiceProvider.Object);

        // Stel de ScopeFactory mock in
        _mockScopeFactory = new Mock<IServiceScopeFactory>();
        _mockScopeFactory.Setup(sf => sf.CreateScope()).Returns(_mockScope.Object);

        // Configureer de mapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<KeycloakEventDto, Profile>()
                .ForMember(dest => dest.KeyCloakId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Details.Email));
        });
        _mapper = mapperConfig.CreateMapper();

        // Instantieer de processor
        _processor = new EventProcessor(_mockScopeFactory.Object, _mapper);
    }

    // Wat doet deze test?
    // Doel:
    // * Controleert of een nieuwe gebruiker wordt toegevoegd aan de repository wanneer een REGISTER-event wordt ontvangen.
    // Belangrijkste checks:
    // * ProfileExist retourneert false, dus CreateProfile wordt aangeroepen.
    // * SaveChanges wordt aangeroepen om de wijzigingen op te slaan.
        
    [Fact]
    public async Task ProcessEvent_CallsAddUser_WhenRegisterEventReceived()
    {
        // Arrange
        _mockRepo.Setup(r => r.ProfileExistAsync("a6427685-84e3-4fbf-8716-c94d1053b020")).ReturnsAsync(false);
        _mockRepo.Setup(r => r.CreateProfileAsync(It.IsAny<Profile>()))
            .Callback<Profile>(p =>
            {
                Console.WriteLine($"Profile created: KeyCloakId={p.KeyCloakId}, UserName={p.UserName}");
            });

        var message = "{\"@class\":\"com.github.aznamier.keycloak.event.provider.EventClientNotificationMqMsg\",\"time\":1736270017968,\"type\":\"REGISTER\",\"realmId\":\"4ad2a6d1-4fc2-4f01-82b1-31ca3a382fb0\",\"clientId\":\"account-console\",\"userId\":\"a6427685-84e3-4fbf-8716-c94d1053b020\",\"ipAddress\":\"192.168.65.3\",\"details\":{\"auth_method\":\"openid-connect\",\"auth_type\":\"code\",\"register_method\":\"form\",\"redirect_uri\":\"http://localhost:8080/realms/pridr/account/\",\"code_id\":\"ceb07c03-89fc-4ee2-b2c5-c7a477ce3535\",\"email\":\"testjebla@blabla.com\",\"username\":\"testjebla@blabla.com\"}}";

        // Act
        await _processor.ProcessEvent(message);

        // Assert
        _mockRepo.Verify(r => r.CreateProfileAsync(It.Is<Profile>(p =>
            p.KeyCloakId == "a6427685-84e3-4fbf-8716-c94d1053b020" &&
            p.UserName == "testjebla@blabla.com")), Times.Once);

        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    // Wat doet deze test?
    // Doel:
    // * Controleert of een gebruiker wordt verwijderd uit de repository wanneer een DELETE_ACCOUNT-event wordt ontvangen.
    // Belangrijkste checks:
    // * ProfileExist retourneert true, dus GetProfileById en DeleteProfile worden aangeroepen.
    // * SaveChanges wordt aangeroepen om de verwijdering op te slaan.

    [Fact]
    public async Task ProcessEvent_CallsDeleteUser_WhenDeleteAccountEventReceived()
    {
        // Arrange
        var mockProfile = new Profile
        {
            KeyCloakId = "a6427685-84e3-4fbf-8716-c94d1053b020",
            UserName = "testjebla@blabla.com",
            Sexuality = Sexuality.Unknown,
            LookingFor = LookingFor.Friendship,
            RelationStatus = RelationStatus.Unknown,
            Latitude = 0,
            Longitude = 0,
        };

        _mockRepo.Setup(r => r.ProfileExistAsync("a6427685-84e3-4fbf-8716-c94d1053b020")).ReturnsAsync(true);
        _mockRepo.Setup(r => r.GetProfileByIdAsync("a6427685-84e3-4fbf-8716-c94d1053b020")).ReturnsAsync(mockProfile);
        _mockRepo.Setup(r => r.DeleteProfile(mockProfile))
                .Callback(() => Console.WriteLine($"Profile deleted: KeyCloakId={mockProfile.KeyCloakId}"));

        var message = "{\"@class\":\"com.github.aznamier.keycloak.event.provider.EventClientNotificationMqMsg\",\"time\":1736270017968,\"type\":\"DELETE_ACCOUNT\",\"realmId\":\"4ad2a6d1-4fc2-4f01-82b1-31ca3a382fb0\",\"clientId\":\"account-console\",\"userId\":\"a6427685-84e3-4fbf-8716-c94d1053b020\",\"ipAddress\":\"192.168.65.3\",\"details\":{\"auth_method\":\"openid-connect\",\"auth_type\":\"code\",\"register_method\":\"form\",\"redirect_uri\":\"http://localhost:8080/realms/pridr/account?referrer=Frontend&referrer_uri=http%3A%2F%2Flocalhost%3A5173%2Fprofile\",\"code_id\":\"ceb07c03-89fc-4ee2-b2c5-c7a477ce3535\",\"email\":\"testjebla@blabla.com\",\"username\":\"testjebla@blabla.com\"}}";

        // Act
        await _processor.ProcessEvent(message);

        // Assert
        _mockRepo.Verify(r => r.GetProfileByIdAsync("a6427685-84e3-4fbf-8716-c94d1053b020"), Times.Once);
        _mockRepo.Verify(r => r.DeleteProfile(It.Is<Profile>(p =>
            p.KeyCloakId == "a6427685-84e3-4fbf-8716-c94d1053b020" &&
            p.UserName == "testjebla@blabla.com")), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}

using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.AsyncDataServices;
using UserService.Controllers;
using UserService.Data;
using UserService.DTOs;
using UserService.Enums;
using Profile = UserService.Models.Profile;

namespace UserService.Tests;

public class ProfilesControllerTests
{
    private readonly Mock<IProfileRepo> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IMessageBusClient> _mockBus;
    private readonly ProfilesController _controller;

    public ProfilesControllerTests()
    {
        _mockRepo = new Mock<IProfileRepo>();
        _mockMapper = new Mock<IMapper>();
        _mockBus = new Mock<IMessageBusClient>();
        _controller = new ProfilesController(_mockRepo.Object, _mockMapper.Object, _mockBus.Object);
    }
    
    // Wat doet deze test?
    // Mocking:
    // * Alle afhankelijkheden (IProfileRepo, IMapper, en IMessageBusClient) zijn gemockt.
    // * De ProfileExist-methode retourneert true om aan te geven dat het profiel bestaat.
    // * GetProfileById retourneert een bestaand profiel met een andere UserName.
    // Actie:
    // * De UpdateProfile-methode wordt aangeroepen met een keyCloakId en een bijgewerkte UserName.
    // Asserties:
    //  Controleert of:
    //  * Het profiel correct wordt opgehaald en bijgewerkt in de repository.
    //  * SaveChanges wordt aangeroepen om de wijzigingen op te slaan.
    //  * _messageBusClient.ProfileUserNameUpdate wordt aangeroepen met de juiste waarden.

    [Fact]
    public async Task UpdateProfile_SendsMessage_WhenUserNameChanges()
    {
        var keyCloakId = "a6427685-84e3-4fbf-8716-c94d1053b020";
        
        var existingProfile = new Profile
        {
            KeyCloakId = keyCloakId,
            UserName = "oldUsername",
            Sexuality = Sexuality.Unknown,
            LookingFor = LookingFor.Friendship,
            RelationStatus = RelationStatus.Unknown,
            Latitude = 0,
            Longitude = 0
        };

        var updatedProfile = new Profile
        {
            KeyCloakId = keyCloakId,
            UserName = "newUsername",
            Sexuality = Sexuality.Unknown,
            LookingFor = LookingFor.Friendship,
            RelationStatus = RelationStatus.Unknown,
            Latitude = 0,
            Longitude = 0
        };

        var profileUpdateDto = new ProfileUpdateDto
        {
            UserName = "newUsername",
            Sexuality = Sexuality.Unknown,
            LookingFor = LookingFor.Friendship,
            RelationStatus = RelationStatus.Unknown
        };

        var profileUserNameDto = new ProfileUserNameDto
        {
            KeyCloakId = keyCloakId,
            UserName = "newUsername",
            EventType = "Updated_UserName"
        };

        var profileReadDto = new ProfileReadDto
        {
            KeyCloakId = keyCloakId,
            UserName = "newUsername",
            Id = 0,
            Sexuality = Sexuality.Unknown,
            LookingFor = LookingFor.Friendship,
            RelationStatus = RelationStatus.Unknown,
            Latitude = 0,
            Longitude = 0
        };

        _mockRepo.Setup(r => r.ProfileExistAsync(keyCloakId)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.GetProfileByIdAsync(keyCloakId)).ReturnsAsync(existingProfile);
        _mockMapper.Setup(m => m.Map(profileUpdateDto, existingProfile)).Returns(updatedProfile);
        _mockMapper.Setup(m => m.Map<ProfileUserNameDto>(updatedProfile)).Returns(profileUserNameDto);
        _mockMapper.Setup(m => m.Map<ProfileReadDto>(updatedProfile)).Returns(profileReadDto);

        // Act
        var result = await _controller.UpdateProfile(keyCloakId, profileUpdateDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ProfileReadDto>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedProfile = Assert.IsType<ProfileReadDto>(okResult.Value);

        Assert.Equal("newUsername", returnedProfile.UserName);

        _mockRepo.Verify(r => r.GetProfileByIdAsync(keyCloakId), Times.Once);
        _mockRepo.Verify(r => r.UpdateProfile(updatedProfile), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);

        _mockBus.Verify(mbc => mbc.ProfileUserNameUpdate(It.Is<ProfileUserNameDto>(dto =>
            dto.KeyCloakId == keyCloakId &&
            dto.UserName == "newUsername" &&
            dto.EventType == "Updated_UserName")), Times.Once);
    }
}

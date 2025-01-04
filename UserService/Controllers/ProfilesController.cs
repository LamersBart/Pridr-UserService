using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.AsyncDataServices;
using UserService.Data;
using UserService.DTOs;
using Profile = UserService.Models.Profile;

namespace UserService.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/profiles")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileRepo _repo;
    private readonly IMapper _mapper;
    private readonly IMessageBusClient _messageBusClient;
    private readonly IConfiguration _configuration;

    public ProfilesController(IProfileRepo repo, IMapper mapper, IMessageBusClient messageBusClient, IConfiguration configuration)
    {
        _repo = repo;
        _mapper = mapper;
        _messageBusClient = messageBusClient;
        _configuration = configuration;
    }
    


    [HttpGet()]
    public ActionResult<IEnumerable<ProfileReadDto>> GetProfiles()
    {
        Console.WriteLine("--> Getting all profiles...");
        var platformItems = _repo.GetAllProfiles();
        return Ok(_mapper.Map<IEnumerable<ProfileReadDto>>(platformItems));
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<ProfileReadDto>> GetMyProfile() 
    {
        string? accessToken = await HttpContext.GetTokenAsync("access_token");
        if (accessToken != null)
        {
            string keyCloakId = FetchKeycloakUserId(accessToken);
            if (!_repo.ProfileExist(keyCloakId))
            {
                return NotFound();
            }
            Console.WriteLine($"--> Getting Profile By Id: {keyCloakId}...");
            Profile profile = _repo.GetProfileById(keyCloakId);
            return Ok(_mapper.Map<ProfileReadDto>(profile));
        }
        return BadRequest("Access token is invalid.");
    }
    
    [HttpGet("{keyCloakId}")]
    public ActionResult<ProfileReadDto> GetProfileById(string keyCloakId) 
    {
        if (!_repo.ProfileExist(keyCloakId))
        {
            return NotFound();
        }
        Console.WriteLine($"--> Getting Profile By Id: {keyCloakId}...");
        Profile profile = _repo.GetProfileById(keyCloakId);
        return Ok(_mapper.Map<ProfileReadDto>(profile));
    }
    
    [HttpPost("batch")]
    public async Task<IActionResult> GetUserNamesByIds(List<string> ids)
    {
        Console.WriteLine("--> Getting batch profiles...");
        var profiles = _repo.GetUserNames(ids);
        return Ok(_mapper.Map<IEnumerable<ProfileUserNameDto>>(profiles));
    }
    
    [HttpPatch("{keyCloakId}")]
    public ActionResult<ProfileReadDto> UpdateProfile(string keyCloakId, ProfileUpdateDto profileUpdateDto)
    {
        if (!_repo.ProfileExist(keyCloakId))
        {
            return NotFound();
        }
        string oldName = String.Empty;
        Console.WriteLine($"--> Updating Profile By Id: {keyCloakId}...");
        Profile profile = _repo.GetProfileById(keyCloakId);
        if (profile.UserName != null)
        {
            oldName = profile.UserName!;
        }
        Profile updatedProfile = _mapper.Map(profileUpdateDto, profile);
        _repo.UpdateProfile(updatedProfile);
        _repo.SaveChanges();
        if (oldName != updatedProfile.UserName)
        {
            try
            {
                var x = _mapper.Map<ProfileUserNameDto>(updatedProfile);
                x.EventType = "Updated_UserName";
                _messageBusClient.ProfileUserNameUpdate(x);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }
        }
        return Ok(_mapper.Map<ProfileReadDto>(updatedProfile));
    }
    
    [HttpPatch("location/{keyCloakId}")]
    public ActionResult<ProfileReadDto> UpdateLocationOfProfile(string keyCloakId, ProfileUpdateLocationDto profileUpdateLocationDto)
    {
        if (!_repo.ProfileExist(keyCloakId))
        {
            return NotFound();
        }
        Console.WriteLine($"--> Updating Location Of Profile By Id: {keyCloakId}...");
        Profile profile = _repo.GetProfileById(keyCloakId);
        Profile updatedProfile = _mapper.Map(profileUpdateLocationDto, profile);
        _repo.UpdateProfile(updatedProfile);
        _repo.SaveChanges();
        return Ok(_mapper.Map<ProfileReadDto>(updatedProfile));
    }
    
    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        var mqHost = Environment.GetEnvironmentVariable("MQHOST");
        var pgUser = Environment.GetEnvironmentVariable("PGUSER");
        var authUrl = Environment.GetEnvironmentVariable("KEYCLOAK_URL");
        var authAdress = Environment.GetEnvironmentVariable("AUTH_METADATA");
        var encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");

        return Ok(new { MQHost = mqHost, PGUser = pgUser, AuthUrl = authUrl, AuthAdress = authAdress, EncryptionKey = encryptionKey });
    }
    
    private static Dictionary<string, object> DecodeJwt(string bearerToken)
    {
        try
        {
            // Strip "Bearer " als prefix
            var token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

            // Lees het token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Claims verwerken
            var claims = new Dictionary<string, object>();

            foreach (var claim in jwtToken.Claims)
            {
                // Controleer of de sleutel al bestaat
                if (claims.ContainsKey(claim.Type))
                {
                    // Voeg meerdere waarden toe als lijst
                    if (claims[claim.Type] is List<object> list)
                    {
                        list.Add(claim.Value); // Voeg toe aan bestaande lijst
                    }
                    else
                    {
                        claims[claim.Type] = new List<object> { claims[claim.Type], claim.Value };
                    }
                }
                else
                {
                    // Voeg nieuwe sleutel/waarde toe
                    claims[claim.Type] = claim.Value;
                }
            }

            // Voeg standaardwaarden toe
            claims["iss"] = jwtToken.Issuer;
            claims["aud"] = jwtToken.Audiences.ToList(); // Audiences expliciet als lijst

            return claims;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid token", ex);
        }
    }
    private static string FetchKeycloakUserId(string bearerToken)
    {
        Dictionary<string, object> dictionary = DecodeJwt(bearerToken);
        if (dictionary.TryGetValue("sub", out object? subValue))
        {
            return subValue.ToString() ?? throw new InvalidOperationException();
        }
        throw new InvalidOperationException();
    }
}
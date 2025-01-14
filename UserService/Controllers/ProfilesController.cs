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

    public ProfilesController(IProfileRepo repo, IMapper mapper, IMessageBusClient messageBusClient)
    {
        _repo = repo;
        _mapper = mapper;
        _messageBusClient = messageBusClient;
    }
    
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<ProfileReadDto>>> GetProfiles()
    {
        Console.WriteLine("--> Getting all profiles...");
        var platformItems = await _repo.GetAllProfilesAsync();
        return Ok(_mapper.Map<IEnumerable<ProfileReadDto>>(platformItems));
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<ProfileReadDto>> GetMyProfile() 
    {
        string? accessToken = await HttpContext.GetTokenAsync("access_token");
        if (accessToken != null)
        {
            string keyCloakId = FetchKeycloakUserId(accessToken);
            if (!await _repo.ProfileExistAsync(keyCloakId))
            {
                return NotFound();
            }
            Console.WriteLine($"--> Getting Profile By Id: {keyCloakId}...");
            Profile profile = await _repo.GetProfileByIdAsync(keyCloakId);
            return Ok(_mapper.Map<ProfileReadDto>(profile));
        }
        return BadRequest("Access token is invalid.");
    }
    
    [HttpGet("{keyCloakId}")]
    public async Task<ActionResult<ProfileReadDto>> GetProfileById(string keyCloakId) 
    {
        if (!await _repo.ProfileExistAsync(keyCloakId))
        {
            return NotFound();
        }
        Console.WriteLine($"--> Getting Profile By Id: {keyCloakId}...");
        Profile profile = await _repo.GetProfileByIdAsync(keyCloakId);
        return Ok(_mapper.Map<ProfileReadDto>(profile));
    }
    
    [HttpPost("batch")]
    public async Task<IActionResult> GetUserNamesByIds(List<string> ids)
    {
        Console.WriteLine("--> Getting batch profiles...");
        var profiles = await _repo.GetUserNamesAsync(ids);
        return Ok(_mapper.Map<IEnumerable<ProfileUserNameDto>>(profiles));
    }
    
    [HttpPatch("{keyCloakId}")]
    public async Task<ActionResult<ProfileReadDto>> UpdateProfile(string keyCloakId, ProfileUpdateDto profileUpdateDto)
    {
        if (!await _repo.ProfileExistAsync(keyCloakId))
        {
            return NotFound();
        }
        string oldName = String.Empty;
        Console.WriteLine($"--> Updating Profile By Id: {keyCloakId}...");
        Profile profile = await _repo.GetProfileByIdAsync(keyCloakId);
        if (profile.UserName != null)
        {
            oldName = profile.UserName!;
        }
        Profile updatedProfile = _mapper.Map(profileUpdateDto, profile);
        _repo.UpdateProfile(updatedProfile);
        await _repo.SaveChangesAsync();
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
    public async Task<ActionResult<ProfileReadDto>> UpdateLocationOfProfile(string keyCloakId, ProfileUpdateLocationDto profileUpdateLocationDto)
    {
        if (!await _repo.ProfileExistAsync(keyCloakId))
        {
            return NotFound();
        }
        Console.WriteLine($"--> Updating Location Of Profile By Id: {keyCloakId}...");
        Profile profile = await _repo.GetProfileByIdAsync(keyCloakId);
        Profile updatedProfile = _mapper.Map(profileUpdateLocationDto, profile);
        _repo.UpdateProfile(updatedProfile);
        await _repo.SaveChangesAsync();
        return Ok(_mapper.Map<ProfileReadDto>(updatedProfile));
    }
    
    [AllowAnonymous]
    [HttpPatch("test")]
    public async Task<ActionResult<ProfileReadDto>> UpdateProfileTest(ProfileUpdateDto profileUpdateDto)
    {
        var disableAuth = Environment.GetEnvironmentVariable("DISABLE_AUTH") == "true";
        if (!disableAuth)
        {
            return NotFound("This endpoint is available only when testmode is enabled.");
        }
        if (!await _repo.ProfileExistAsync("a6427685-84e3-4fbf-8716-c94d1053b020"))
        {
            return NotFound();
        }
        string oldName = String.Empty;
        Console.WriteLine($"--> Updating Profile By Id: a6427685-84e3-4fbf-8716-c94d1053b020...");
        Profile profile = await _repo.GetProfileByIdAsync("a6427685-84e3-4fbf-8716-c94d1053b020");
        if (profile.UserName != null)
        {
            oldName = profile.UserName!;
        }
        Profile updatedProfile = _mapper.Map(profileUpdateDto, profile);
        _repo.UpdateProfile(updatedProfile);
        await _repo.SaveChangesAsync();
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
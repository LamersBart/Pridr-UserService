using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public ProfilesController(IProfileRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    


    [HttpGet()]
    public ActionResult<IEnumerable<ProfileReadDto>> GetProfiles()
    {
        Console.WriteLine("--> Getting all profiles...");
        var platformItems = _repo.GetAllProfiles();
        return Ok(_mapper.Map<IEnumerable<ProfileReadDto>>(platformItems));
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
        Console.WriteLine($"--> Updating Profile By Id: {keyCloakId}...");
        Profile profile = _repo.GetProfileById(keyCloakId);
        Profile updatedProfile = _mapper.Map(profileUpdateDto, profile);
        _repo.UpdateProfile(updatedProfile);
        _repo.SaveChanges();
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
}
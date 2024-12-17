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
    
    [HttpGet("{profileId}")]
    public ActionResult<ProfileReadDto> GetProfileById(int profileId) 
    {
        if (!_repo.ProfileExist(profileId))
        {
            return NotFound();
        }
        Console.WriteLine($"--> Getting Profile By Id: {profileId}...");
        Profile profile = _repo.GetProfileById(profileId);
        return Ok(_mapper.Map<ProfileReadDto>(profile));
    }
    
    [HttpPatch("{profileId}")]
    public ActionResult<ProfileReadDto> UpdateProfile(int profileId, ProfileUpdateDto profileUpdateDto)
    {
        if (!_repo.ProfileExist(profileId))
        {
            return NotFound();
        }
        Console.WriteLine($"--> Updating Profile By Id: {profileId}...");
        Profile profile = _repo.GetProfileById(profileId);
        Profile updatedProfile = _mapper.Map(profileUpdateDto, profile);
        _repo.UpdateProfile(updatedProfile);
        _repo.SaveChanges();
        return Ok(_mapper.Map<ProfileReadDto>(updatedProfile));
    }
    
    [HttpPatch("location/{profileId}")]
    public ActionResult<ProfileReadDto> UpdateLocationOfProfile(int profileId, ProfileUpdateLocationDto profileUpdateLocationDto)
    {
        if (!_repo.ProfileExist(profileId))
        {
            return NotFound();
        }
        Console.WriteLine($"--> Updating Location Of Profile By Id: {profileId}...");
        Profile profile = _repo.GetProfileById(profileId);
        Profile updatedProfile = _mapper.Map(profileUpdateLocationDto, profile);
        _repo.UpdateProfile(updatedProfile);
        _repo.SaveChanges();
        return Ok(_mapper.Map<ProfileReadDto>(updatedProfile));
    }
}
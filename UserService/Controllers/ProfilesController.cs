using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.DTOs;

namespace UserService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileRepo _repo;
    private readonly IMapper _mapper;

    public ProfilesController(IProfileRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    

    [Authorize]
    [HttpGet(Name="GetProfiles")]
    public ActionResult<IEnumerable<ProfileReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting all profiles...");
        var platformItems = _repo.GetAllProfiles();
        return Ok(_mapper.Map<IEnumerable<ProfileReadDto>>(platformItems));
    }
}
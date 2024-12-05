using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.DTOs;

namespace UserService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepo _repo;
    private readonly IMapper _mapper;

    public UsersController(IUserRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    
    [HttpGet(Name="GetUsers")]
    public ActionResult<IEnumerable<UserReadDto>> GetUsers()
    {
        Console.WriteLine("--> Getting all profiles...");
        var users = _repo.GetAllUsers();
        return Ok(_mapper.Map<IEnumerable<UserReadDto>>(users));
    }
}
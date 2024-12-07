using UserService.Models;

namespace UserService.DTOs;

public class UserReadDto
{
    public int Id { get; set; }
    public required string KeyCloakId { get; set; }
    public required string Email { get; set; }
    public required Profile Profile { get; set; }
}
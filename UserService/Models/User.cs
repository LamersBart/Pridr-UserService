using System.ComponentModel.DataAnnotations;
using UserService.Enums;

namespace UserService.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string KeyCloakId { get; set; }
    [Required]
    public required string UserName { get; set; }
    [Required]
    public required string Email { get; set; }
    [Required]
    public required Profile Profile { get; set; }
}
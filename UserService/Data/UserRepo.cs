using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class UserRepo : IUserRepo
{
    private readonly AppDbContext _context;

    public UserRepo(AppDbContext context)
    {
        _context = context;
    }
    
    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _context.User
            .Include(u => u.Profile)
            .ToList();
    }

    public User GetUserById(string id)
    {
        return _context.User.FirstOrDefault(p => p.KeyCloakId == id);
    }

    public int CreateUser(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        _context.User.Add(user);
        return user.Id;
    }

    public bool UserExist(string keycloakUserId)
    {
        return _context.User.Any(p => p.KeyCloakId == keycloakUserId);
    }
}
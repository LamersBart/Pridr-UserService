using UserService.Models;

namespace UserService.Data;

public class ProfileRepo : IProfileRepo
{
    private readonly AppDbContext _context;

    public ProfileRepo(AppDbContext context)
    {
        _context = context;
    }
    
    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }

    public IEnumerable<Profile> GetAllProfiles()
    {
        return _context.Profiles.ToList();
    }

    public IEnumerable<Profile> GetUserNames(List<string> ids)
    {
        return _context.Profiles
            .Where(p => ids.Contains(p.KeyCloakId))
            .ToList();
    }

    public Profile GetProfileById(string keycloakUserId)
    {
        return _context.Profiles.FirstOrDefault(p => p.KeyCloakId == keycloakUserId)!;
    }

    public void CreateProfile(Profile profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }
        _context.Profiles.Add(profile);
    }

    public bool ProfileExist(string keycloakUserId)
    {
        return _context.Profiles.Any(p => p.KeyCloakId == keycloakUserId);
    }

    public void UpdateProfile(Profile profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }
        _context.Profiles.Update(profile);
    }

    public void DeleteProfile(Profile profile)
    {
        _context.Profiles.Remove(profile);
    }
}
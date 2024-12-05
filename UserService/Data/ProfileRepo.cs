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

    public Profile GetProfileById(int id)
    {
        return _context.Profiles.FirstOrDefault(p => p.Id == id);
    }

    public void CreateProfile(Profile profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }
        _context.Profiles.Add(profile);
    }

    public bool ProfileExist(int profileId)
    {
        return _context.Profiles.Any(p => p.Id == profileId);
    }

    public void UpdateProfile(Profile profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }
        _context.Profiles.Update(profile);
    }
}
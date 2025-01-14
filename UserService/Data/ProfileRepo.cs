using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class ProfileRepo : IProfileRepo
{
    private readonly AppDbContext _context;

    public ProfileRepo(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
    {
        return await _context.Profiles.ToListAsync();
    }

    public async Task<IEnumerable<Profile>> GetUserNamesAsync(List<string> ids)
    {
        return await _context.Profiles
            .Where(p => ids.Contains(p.KeyCloakId))
            .ToListAsync();
    }

    public async Task<Profile> GetProfileByIdAsync(string keycloakUserId)
    {
        return await _context.Profiles.FirstOrDefaultAsync(p => p.KeyCloakId == keycloakUserId)!;
    }

    public async Task CreateProfileAsync(Profile profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }
        await _context.Profiles.AddAsync(profile);
    }

    public async Task<bool> ProfileExistAsync(string keycloakUserId)
    {
        return await _context.Profiles.AnyAsync(p => p.KeyCloakId == keycloakUserId);
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
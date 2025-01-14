using UserService.Models;

namespace UserService.Data;

public interface IProfileRepo
{
    Task<bool> SaveChangesAsync();
    Task<IEnumerable<Profile>> GetAllProfilesAsync();
    Task<IEnumerable<Profile>> GetUserNamesAsync(List<string> ids);
    Task<Profile> GetProfileByIdAsync(string keycloakUserId);
    Task CreateProfileAsync(Profile profile);
    Task<bool> ProfileExistAsync(string keycloakUserId);
    void UpdateProfile(Profile profile);
    void DeleteProfile(Profile profile);
}
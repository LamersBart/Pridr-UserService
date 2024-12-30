using UserService.Models;

namespace UserService.Data;

public interface IProfileRepo
{
    bool SaveChanges();
    IEnumerable<Profile> GetAllProfiles();
    IEnumerable<Profile> GetUserNames(List<string> ids);
    Profile GetProfileById(string keycloakUserId);
    void CreateProfile(Profile profile);
    bool ProfileExist(string keycloakUserId);
    void UpdateProfile(Profile profile);
}
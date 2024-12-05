using UserService.Models;

namespace UserService.Data;

public interface IProfileRepo
{
    bool SaveChanges();
    IEnumerable<Profile> GetAllProfiles();
    Profile GetProfileById(int id);
    void CreateProfile(Profile profile);
    bool ProfileExist(int profileId);
}
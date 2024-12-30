using UserService.DTOs;
using UserService.Models;
using Profile = AutoMapper.Profile;

namespace UserService.Profiles;

public class ProfilesProfile : Profile
{
    public ProfilesProfile()
    {
        // source -> target
        CreateMap<Models.Profile, ProfileReadDto>();
        CreateMap<KeycloakEventDto, Models.Profile>()
            .ForMember(dest => dest.KeyCloakId, opt => opt.MapFrom(src => src.UserId));
        CreateMap<Models.Profile, ProfileUserNameDto>();
        CreateMap<ProfileUpdateDto, Models.Profile>();
        CreateMap<ProfileUpdateLocationDto, Models.Profile>();
    }
}
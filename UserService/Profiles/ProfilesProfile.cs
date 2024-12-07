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
        CreateMap<KeycloakEventDto, User>()
            .ForMember(dest => dest.KeyCloakId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Details.Email));
        CreateMap<User, UserReadDto>();
        CreateMap<ProfileUpdateDto, Models.Profile>();
        CreateMap<ProfileUpdateLocationDto, Models.Profile>();
    }
}
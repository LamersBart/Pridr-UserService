using UserService.DTOs;

namespace UserService.AsyncDataServices;

public interface IMessageBusClient
{
    void ProfileUserNameUpdate(ProfileUserNameDto profileUserNameDto);
}
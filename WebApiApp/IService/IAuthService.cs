using WebApiApp.Dto;

namespace WebApiApp.IService;

public interface IAuthService
{
    Task<Tuple<int, string>> LoginUser(UserDto dto);
    Task<Tuple<int, string>> RegisterUser(CreateUserDto dto);
}
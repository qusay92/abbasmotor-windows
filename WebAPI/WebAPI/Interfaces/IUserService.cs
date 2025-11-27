

namespace WebAPI.Interfaces
{
    public interface IUserService
    {
        Task<ResponseResult<LoginResonse>> Register(RegisterationDto dto);
        Task<ActionResult<ResponseResult<LoginResonse>>> Login(LoginDto dto);
        Task<ActionResult<ResponseResult<List<UserVM>>>> GetClients();
    }
}

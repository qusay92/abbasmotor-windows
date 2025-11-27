using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        public readonly IUserService _repo;
        private readonly IStringLocalizer<UsersController> _localizerUsers;

        public UsersController(IConfiguration configuration, ILoggerService loggerService, IUserService repo, IStringLocalizer<UsersController> localizerUsers) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerUsers = localizerUsers;
        }

        [HttpGet("GetClients")]
        public async Task<ActionResult<ResponseResult<List<UserVM>>>> GetClients()
        {
            try
            {
                return await _repo.GetClients();
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, null, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }
    }
}

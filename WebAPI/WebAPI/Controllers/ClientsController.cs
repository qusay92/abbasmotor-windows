using WebAPI.ViewModel.Balances;
using WebAPI.ViewModel.ManageClients;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : BaseController
    {
        public readonly IClientService _repo;
        private readonly IStringLocalizer<ClientsController> _localizerClient;

        public ClientsController(IConfiguration configuration, ILoggerService loggerService, IClientService repo, IStringLocalizer<ClientsController> localizerClient) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerClient = localizerClient;
        }

        [HttpGet("GetClients/{search?}")]
        public async Task<ResponseResult<List<GetClientsVM>>> GetClients(string search)
        {
            var userId = UserManager.GetUserId(this.User);
            var userType = UserManager.GetUserType(this.User);

            try
            {
                return await _repo.GetClients(userId, search, userType);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, search, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("SaveClient/{editPassword}")]
        public async Task<ResponseResult<List<GetClientsVM>>> SaveClient([FromBody] User model, bool editPassword)
        {
            try
            {
                return await _repo.SaveClient(model, editPassword, UserId);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("DeleteClient")]
        public async Task<ResponseResult<List<GetClientsVM>>> DeleteClient([FromBody] List<User> model)
        {
            try
            {
                return await _repo.Delete(model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("GetClientsInfo")]
        public async Task<ResponseResult<ClientInfo>> GetClientsInfo([FromBody] GetBalancesInput input)
        {
            try
            {
                return await _repo.GetClientsInfo(input, UserId);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, input, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
         
        }

        [HttpGet("GetClientsBalance/{search?}")]
        public async Task<ResponseResult<ClientBalance>> GetClientsInfo(string search)
        {
            try
            {
                return await _repo.GetClientsBalance(search, UserId, UserTypes);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, search, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpGet("GetClientById")]
        public async Task<ResponseResult<GetClientsVM>> GetClientById()
        {
            try
            {
                return await _repo.GetClientById(UserId);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, UserId, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("UpdatePassword")]
        public async Task<ResponseResult<bool>> UpdatePassword([FromBody] UpdatePasswordInput input)
        {
            try
            {
                return await _repo.UpdatePassword(UserId, input.Password);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, UserId, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }        
        }
    }
}

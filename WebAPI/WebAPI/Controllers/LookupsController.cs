namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LookupsController : BaseController
    {
        public readonly ILookupService _repo;
        private readonly IStringLocalizer<LookupsController> _localizerLookups;

        public LookupsController(IConfiguration configuration, ILoggerService loggerService, ILookupService repo, IStringLocalizer<LookupsController> localizerLookups) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerLookups = localizerLookups;
        }

        [HttpGet("GetAllLookupValues")]
        public async Task<ActionResult<ResponseResult<List<LookupValueVM>>>> GetAllLookupValues()
        {
            try
            {
                return await _repo.GetAll();
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, null, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpGet("GetLookups")]
        public async Task<ActionResult<ResponseResult<List<Lookup>>>> GetLookups()
        {
            try
            {
                return await _repo.GetLookups();
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, null, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpGet("GetLookupValues/{lookupId}/{search?}")]
        public async Task<ResponseResult<List<LookupValue>>> GetLookupValues(string search, int lookupId)
        {
            try
            {
                return await _repo.GetLookupValues(UserId, search, lookupId);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, search, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }

        }

        [HttpPost("Save")]
        public async Task<ResponseResult<List<LookupValue>>> Save([FromBody] LookupValue model)
        {
            try
            {
                return await _repo.Save(model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("Delete")]
        public async Task<ResponseResult<List<LookupValue>>> Delete([FromBody] List<LookupValue> models)
        {
            try
            {
                return await _repo.Delete(models);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, models, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

    }
}

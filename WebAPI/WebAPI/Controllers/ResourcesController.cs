using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : BaseController
    {
        public readonly IResourcesService _repo;
        private readonly IStringLocalizer<ResourcesController> _localizerResources;

        public ResourcesController(IConfiguration configuration, ILoggerService loggerService, IResourcesService repo, IStringLocalizer<ResourcesController> localizerResources) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerResources = localizerResources;
        }

        [Authorize]
        [HttpGet("HandleResourcesByUrl")]
        public async Task<ResponseResult<List<Resources>>> HandleResourcesByUrl([FromQuery] string url)
        {
            return await _repo.HandleResourcesByUrl(url);
        }

        [Authorize]
        [HttpGet("GetResources/{resourceId}/{search?}")]
        public async Task<ResponseResult<List<GroupedResources>>> GetResources(int resourceId, string search)
        {
            try
            {
                return await _repo.GetResources(search, resourceId);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, resourceId, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [Authorize]
        [HttpPost("SaveResource")]
        public async Task<ResponseResult<List<GroupedResources>>> SaveResource([FromBody] ResourceParamDto body)
        {
            try
            {
                return await _repo.SaveResource(body);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, body, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [Authorize]
        [HttpGet("GetResourceByKey/{key}")]
        public async Task<ResponseResult<Resources>> GetResourceByKey(string key)
        {
            try
            {
                return await _repo.GetResourceByKey(key);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, key, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [Authorize]
        [HttpGet("GetResourcesKeys")]
        public async Task<ResponseResult<List<ResourcesKeysVM>>> GetResourcesKeys()
        {
            try
            {
                return await _repo.GetResourcesKeys();
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, null, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        
        [HttpGet("GetHomePageResources")]
        public async Task<ResponseResult<List<Resources>>> GetHomePageResources()
        {
            try
            {
                return await _repo.GetHomePageResources();
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, null, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }

        }

    }
}

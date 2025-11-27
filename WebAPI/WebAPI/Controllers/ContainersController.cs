using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController : BaseController
    {
        public readonly IContainersService _repo;
        private readonly IStringLocalizer<ContainersController> _localizerContainers;

        public ContainersController(IConfiguration configuration, ILoggerService loggerService, IContainersService repo, IStringLocalizer<ContainersController> localizerContainers) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerContainers = localizerContainers;
        }

        [HttpPost("GetAllByUser")]
        public async Task<ResponseResult<List<Container>>> GetAllByUser([FromBody] ContainerFilterParams param)
        {
            try
            {
                return await _repo.GetAllByUser(UserId, param);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, param, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("GetAllArchiveByUser")]
        public async Task<ResponseResult<List<Container>>> GetAllArchiveByUser([FromBody] ContainerFilterParams param)
        {
            try
            {
                return await _repo.GetAllArchiveByUser(UserId, param);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, param, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }


        [HttpPost("SaveContainer")]
        public async Task<ResponseResult<List<Container>>> SaveContainer([FromBody] Container model)
        {
            try
            {
                return await _repo.SaveContainer(model, UserId);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("DeleteContainers")]
        public async Task<ResponseResult<List<Container>>> DeleteContainers([FromBody] List<Container> model)
        {
            try
            {
                return await _repo.DeleteContainers(model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpGet("GetSideMenuByUser/{isArchive}")]
        public dynamic GetSideMenuByUser(byte isArchive)
        {
            try
            {
                return _repo.GetSideMenuByUser(UserId, isArchive);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, isArchive, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpGet("GetContainerById/{id}")]
        public Task<ResponseResult<Container>> GetContainerById(long id)
        {
            try
            {
                return _repo.GetContainerById(id);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, id, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("DeleteContainer/{id}")]
        public async Task<ResponseResult<List<Container>>> DeleteContainer(long id)
        {
            try
            {
                return await _repo.Delete(id);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, id, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("ArchiveContainer/{id}")]
        public async Task<ResponseResult<List<Container>>> ArchiveContainer(long id)
        {
            try
            {
                return await _repo.Archive(id);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, id, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }


        [HttpPost("ArchiveContainers")]
        public async Task<ResponseResult<List<Container>>> ArchiveContainers([FromBody] List<Container> model)
        {
            try
            {
                return await _repo.ArchiveContainers(model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }


        [HttpPost("DeleteImages/{id}")]
        public async Task<ResponseResult<List<Container>>> DeleteImages(long id)
        {
            try
            {
                return await _repo.DeleteImages(id);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, id, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }


        [HttpPost("UnArchiveContainers")]
        public async Task<ResponseResult<List<Container>>> UnArchiveContainers([FromBody] List<Container> model)
        {
            try
            {
                return await _repo.UnArchiveContainers(model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }


    }
}

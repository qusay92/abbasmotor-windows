using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerImagesController : BaseController
    {
        public readonly IContainerImagesService _repo;
        private readonly IStringLocalizer<ContainerImagesController> _localizerContainerImages;

        public ContainerImagesController(IConfiguration configuration, ILoggerService loggerService, IContainerImagesService repo, IStringLocalizer<ContainerImagesController> localizerContainerImages) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerContainerImages = localizerContainerImages;
        }

        [HttpGet("GetImagesByContainer/{id}")]
        public async Task<ResponseResult<List<ContainerImageDto>>> GetImagesByContainer(long id)
        {
            try
            {
                return await _repo.GetImagesByContainer(id);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, id, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpGet("DeleteContainerImage/{id}")]
        public async Task<ResponseResult<bool>> DeleteContainerImage(long id)
        {
            try
            {
                return await _repo.DeleteContainerImage(id);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, id, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpGet("DeleteAllImages/{id}")]
        public async Task<ResponseResult<bool>> DeleteAllImages(long id)
        {
            try
            {
                return await _repo.DeleteAllImages(id);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, id, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("DeleteContainersImages")]
        public async Task<ResponseResult<bool>> DeleteContainersImages([FromBody] List<Container> model)
        {
            try
            {
                return await _repo.DeleteContainersImages(model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }            
        }
    }
}

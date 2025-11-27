using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AutoImagesController : BaseController
    {
        public readonly IAutoImagesService _repo;
        private readonly IStringLocalizer<AutoImagesController> _localizerAutoImages;

        public AutoImagesController(IConfiguration configuration, ILoggerService loggerService, IAutoImagesService repo, IStringLocalizer<AutoImagesController> localizerAutoImages) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerAutoImages = localizerAutoImages;
        }

        [HttpGet("GetImagesByAuto/{id}")]
        public async Task<ResponseResult<List<AutoImageDto>>> GetImagesByAuto(long id)
        {
            try
            {
                return await _repo.GetImagesByAuto(id);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, id, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpGet("DeleteAutoImage/{id}")]
        public async Task<ResponseResult<bool>> DeleteAutoImage(long id)
        {
            try
            {
                return await _repo.DeleteAutoImage(id);
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

        [HttpPost("DeleteAutosImages")]
        public async Task<ResponseResult<bool>> DeleteAutosImages([FromBody] List<Auto> model)
        {
            try
            {
                return await _repo.DeleteAutosImages(model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }
    }
}

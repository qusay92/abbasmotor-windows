using System.IO.Compression;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : BaseController
    {
        public readonly IAttachmentService _repo;
        private readonly IStringLocalizer<AttachmentsController> _localizerattachments;
        public readonly IAutoImagesService _auto;
        public readonly IContainersService _container;
        public AttachmentsController(IConfiguration configuration, ILoggerService loggerService, IAttachmentService repo, IStringLocalizer<AttachmentsController> localizerattachments, IAutoImagesService autoImages, IContainersService container) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerattachments = localizerattachments;
            _auto = autoImages;
            _container = container;
        }


        [HttpPost("UploadImages/{type}/{autoId}/{containerId}")]
        public async Task<IActionResult> UploadImages(long containerId, long autoId, UploadType type)
        {
            try
            {
                var uploadFiles = Request.Form.Files;
                var result = new List<string>();
                string strFolderPath = _configuration["AttachmentsPath"];
                bool isSavedimages = false;

                if (type == UploadType.Auto)
                {
                    isSavedimages = await _auto.SaveImages(uploadFiles, autoId, UserId);
                }
                else
                {
                    isSavedimages = await _container.SaveImages(uploadFiles, containerId, UserId);
                }
                if (isSavedimages)
                {
                    foreach (var file in uploadFiles)
                    {
                        var newFileName = file.FileName;
                        if (file.FileName.Contains("."))
                        {
                            var at = file.FileName.IndexOf('.');
                            newFileName = (type == UploadType.Auto) ? DateTime.Now.ToString("yyMMdd") + "-" + (file.FileName.Substring(0, at) + "A" + autoId + file.FileName.Substring(at)) : DateTime.Now.ToString("yyMMdd") + "-" + (file.FileName.Substring(0, at) + "C" + containerId + file.FileName.Substring(at));
                        }
                        var filePath = Path.Combine(strFolderPath, newFileName);
                        Task<string> task = Task.Run(() =>
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            return file.FileName;
                        });
                        await task;

                        result.Add(task.Result.ToString());
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, type, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return BadRequest(ex.Message);
            }
        }


    }
}

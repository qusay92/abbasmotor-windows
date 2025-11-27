namespace WebAPI.Services
{
    public class AutoImagesService : IAutoImagesService
    {
        public readonly AmdDBContext _dbContext;
        public readonly IConfiguration _configuration;
        public AutoImagesService(AmdDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<ResponseResult<List<AutoImageDto>>> GetImagesByAuto(long autoId)
        {
            ResponseResult<List<AutoImageDto>> result = new ResponseResult<List<AutoImageDto>>();
            List<AutoImage> dbImagesByAutoId = _dbContext.AutoImages.Where(x => x.AutoId == autoId).ToList();
            List<AutoImageDto> models = dbImagesByAutoId.Select(a => new AutoImageDto
            {
                Id = a.Id,
                alt = a.Alt,
                title = a.Title,
                previewImageSrc = a.PreviewImageSrc,
                thumbnailImageSrc = a.ThumbnailImageSrc
            })
            .ToList();

            result.Data = models;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

        public async Task<bool> SaveImages(IFormFileCollection uploadFiles, long autoId, long userId)
        {
            bool isSaved = false;
            string strFolderPath = _configuration["Attachmentspath"];
            List<AutoImage> dbImagesByAutoId = _dbContext.AutoImages.Where(x => x.AutoId == autoId).ToList();
            List<AutoImage> newImages = new List<AutoImage>();
            foreach (var file in uploadFiles)
            {
                var newFileName = file.FileName;
                if (file.FileName.Contains("."))
                {
                    var at = file.FileName.IndexOf('.');
                    newFileName = DateTime.Now.ToString("yyMMdd") + "-" + file.FileName.Substring(0, at) + "A" + autoId + file.FileName.Substring(at);
                }
                if (dbImagesByAutoId.Any(x => x.Title == newFileName))
                {
                    continue;
                }
                var filePath = Path.Combine(strFolderPath, newFileName);
                AutoImage newImage = new AutoImage()
                {
                    AutoId = autoId,
                    Alt = newFileName,
                    Title = newFileName,
                    PreviewImageSrc = filePath,
                    ThumbnailImageSrc = filePath,
                    Extintion = null,
                    Path = filePath,
                    CreationDate = DateTime.Now,
                    CreationUserId = userId,
                    ModificationDate = null,
                    ModificationUserId = null
                };
                newImages.Add(newImage);
            }
            _dbContext.AutoImages.AddRange(newImages);
            isSaved = await _dbContext.SaveChangesAsync() > 0;
            return isSaved;
        }

        public async Task<ResponseResult<bool>> DeleteAutoImage(long id)
        {
            ResponseResult<bool> result = new ResponseResult<bool>();
            bool isSaved = false;

            try
            {
                AutoImage autoImage = _dbContext.AutoImages.Where(i => i.Id == id).FirstOrDefault();
                string[] files = Directory.GetFiles(_configuration["Attachmentspath"]);
                foreach (string file in files)
                {
                    if (file.Contains(autoImage.Title))
                    {
                        File.Delete(file);
                    }
                }
                _dbContext.AutoImages.Remove(autoImage);
                isSaved = await _dbContext.SaveChangesAsync() > 0;
                if (isSaved)
                {
                    result.Data = true;
                    result.Errors = null;
                    result.Status = StatusType.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Errors = new List<string> { ex.Message };
                result.Status = StatusType.Failed;
            }

            return result;
        }

        public async Task<ResponseResult<bool>> DeleteAllImages(long autoId)
        {
            ResponseResult<bool> result = new ResponseResult<bool>();
            bool isSaved = false;

            try
            {
                List<AutoImage> autoImages = _dbContext.AutoImages.Where(i => i.AutoId == autoId).ToList();
                string[] files = Directory.GetFiles(_configuration["Attachmentspath"]);
                foreach (string file in files)
                {
                    if (autoImages.Any(i => file.Contains(i.Title)))
                    {
                        File.Delete(file);
                    }
                }

                _dbContext.AutoImages.RemoveRange(autoImages);

                isSaved = await _dbContext.SaveChangesAsync() > 0;

                if (isSaved)
                {
                    result.Data = true;
                    result.Errors = null;
                    result.Status = StatusType.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Errors = new List<string> { ex.Message };
                result.Status = StatusType.Failed;
            }

            return result;
        }

        public async Task<ResponseResult<bool>> DeleteAutosImages(List<Auto> model)
        {
            ResponseResult<bool> result = new ResponseResult<bool>();
            bool isSaved = false;

            try
            {
                List<AutoImage> autoImages = _dbContext.AutoImages.Where(i => model.Any(m => m.Id == i.AutoId)).ToList();
                string[] files = Directory.GetFiles(_configuration["Attachmentspath"]);
                foreach (string file in files)
                {
                    if (autoImages.Any(i => file.Contains(i.Title)))
                    {
                        File.Delete(file);
                    }
                }

                _dbContext.AutoImages.RemoveRange(autoImages);

                isSaved = await _dbContext.SaveChangesAsync() > 0;

                if (isSaved)
                {
                    result.Data = true;
                    result.Errors = null;
                    result.Status = StatusType.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Errors = new List<string> { ex.Message };
                result.Status = StatusType.Failed;
            }

            return result;
        }

    }
}

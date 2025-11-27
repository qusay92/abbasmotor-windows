namespace WebAPI.Services
{
    public class ContainerImagesService : IContainerImagesService
    {
        public readonly AmdDBContext _dbContext;
        public readonly IConfiguration _configuration;
        public ContainerImagesService(AmdDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<ResponseResult<List<ContainerImageDto>>> GetImagesByContainer(long id)
        {
            ResponseResult<List<ContainerImageDto>> result = new ResponseResult<List<ContainerImageDto>>();
            List<ContainerImages> dbImagesByAutoId = _dbContext.ContainerImages.Where(x => x.ContainerId == id).ToList();
            List<ContainerImageDto> models = dbImagesByAutoId.Select(a => new ContainerImageDto
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

        public async Task<ResponseResult<bool>> DeleteContainerImage(long id)
        {
            ResponseResult<bool> result = new ResponseResult<bool>();
            bool isSaved = false;

            try
            {
                ContainerImages containerImage = await _dbContext.ContainerImages.Where(i => i.Id == id).FirstOrDefaultAsync();
                string[] files = Directory.GetFiles(_configuration["Attachmentspath"]);
                foreach (string file in files)
                {
                    if (file.Contains(containerImage.Title))
                    {
                        File.Delete(file);
                    }
                }
                _dbContext.ContainerImages.Remove(containerImage);
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

        public async Task<ResponseResult<bool>> DeleteAllImages(long containerId)
        {
            ResponseResult<bool> result = new ResponseResult<bool>();
            bool isSaved = false;

            try
            {
                List<ContainerImages> containerImages = _dbContext.ContainerImages.Where(i => i.ContainerId == containerId).ToList();
                string[] files = Directory.GetFiles(_configuration["Attachmentspath"]);
                foreach (string file in files)
                {
                    if (containerImages.Any(i => file.Contains(i.Title)))
                    {
                        File.Delete(file);
                    }
                }

                _dbContext.ContainerImages.RemoveRange(containerImages);

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

        public async Task<ResponseResult<bool>> DeleteContainersImages(List<Container> model)
        {
            ResponseResult<bool> result = new ResponseResult<bool>();
            bool isSaved = false;

            try
            {
                List<ContainerImages> containerImages = _dbContext.ContainerImages.Where(i => model.Any(m => m.Id == i.ContainerId)).ToList();
                string[] files = Directory.GetFiles(_configuration["Attachmentspath"]);
                foreach (string file in files)
                {
                    if (containerImages.Any(i => file.Contains(i.Title)))
                    {
                        File.Delete(file);
                    }
                }

                _dbContext.ContainerImages.RemoveRange(containerImages);

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

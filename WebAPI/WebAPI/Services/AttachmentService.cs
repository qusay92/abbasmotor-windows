namespace WebAPI.Services
{
    public class AttachmentService : IAttachmentService
    {
        public readonly AmdDBContext _dbContext;
        public AttachmentService(AmdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseResult<bool>> UploadImages(UploadParams model)
        {
            try
            {
                model.Path = "C:\\Attachments";
                if (model != null && model.Files != null && model.Files.Count > 0)
                {
                    //ConvertFileToByte(model.Files);
                    var result = new List<string>();
                    string strFolderPath = model.Path;
                    if (!Directory.Exists(strFolderPath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(strFolderPath);
                    }
                    foreach (var file in model.Files)
                    {
                        var filePath = Path.Combine(strFolderPath, file.FileName);
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
                var res = new ResponseResult<bool>();
                res.Data = true;
                res.Errors = null;
                res.Status = StatusType.Success;
                return res;
            }
            catch (Exception ex)
            {
                var res = new ResponseResult<bool>();
                res.Data = false;
                res.Errors = new List<string> { ex.Message };
                res.Status = StatusType.Failed;
                return res;
            }

        }

        private byte[] ConvertFileToByte(List<IFormFile> files)
        {
            byte[] bytes = default(byte[]);
            foreach (var file in files)
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    using (MemoryStream memstream = new MemoryStream())
                    {
                        reader.BaseStream.CopyTo(memstream);
                        bytes = memstream.ToArray();
                    }
                }
            }

            return bytes;
        }
    }
}

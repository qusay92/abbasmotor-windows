namespace WebAPI.Interfaces
{
    public interface IAttachmentService
    {
        Task<ResponseResult<bool>> UploadImages(UploadParams model);
    }
}

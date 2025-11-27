namespace WebAPI.Interfaces
{
    public interface IAutoImagesService
    {
        Task<bool> SaveImages(IFormFileCollection uploadFiles, long autoId, long userId);
        Task<ResponseResult<List<AutoImageDto>>> GetImagesByAuto(long id);
        Task<ResponseResult<bool>> DeleteAutoImage(long id);
        Task<ResponseResult<bool>> DeleteAllImages(long id);
        Task<ResponseResult<bool>> DeleteAutosImages(List<Auto> model);
    }
}

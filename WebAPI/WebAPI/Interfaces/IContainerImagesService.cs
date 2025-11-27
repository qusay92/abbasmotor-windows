namespace WebAPI.Interfaces
{
    public interface IContainerImagesService
    {
        Task<ResponseResult<List<ContainerImageDto>>> GetImagesByContainer(long id);
        Task<ResponseResult<bool>> DeleteContainerImage(long id);
        Task<ResponseResult<bool>> DeleteAllImages(long id);
        Task<ResponseResult<bool>> DeleteContainersImages(List<Container> model);
    }
}

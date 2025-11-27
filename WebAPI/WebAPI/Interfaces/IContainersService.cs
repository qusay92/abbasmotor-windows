namespace WebAPI.Interfaces
{
    public interface IContainersService
    {
        Task<ResponseResult<List<Container>>> GetAllByUser(long id, ContainerFilterParams search);
        Task<ResponseResult<List<Container>>> GetAllArchiveByUser(long id, ContainerFilterParams search);
        Task<ResponseResult<List<Container>>> SaveContainer(Container model, long UserId);
        Task<ResponseResult<List<Container>>> DeleteContainers(List<Container> model);
        Task<ResponseResult<List<Container>>> Delete(long Id);
        Task<ResponseResult<List<Container>>> ArchiveContainers(List<Container> model);
        Task<ResponseResult<List<Container>>> UnArchiveContainers(List<Container> model);
        Task<ResponseResult<List<Container>>> Archive(long Id);
        Task<ResponseResult<List<Container>>> DeleteImages(long Id);
        Task<bool> SaveImages(IFormFileCollection uploadFiles, long containerId, long userId);
        Task<ResponseResult<Container>> GetContainerById(long id);
        dynamic GetSideMenuByUser(long id, byte isArchive);

    }
}

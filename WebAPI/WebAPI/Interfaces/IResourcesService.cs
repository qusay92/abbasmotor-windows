namespace WebAPI.Interfaces
{
    public interface IResourcesService
    {
        Task<ResponseResult<List<Resources>>> HandleResourcesByUrl(string url);
        Task<ResponseResult<List<GroupedResources>>> GetResources(string search,int resourceId);
        Task<ResponseResult<List<GroupedResources>>> SaveResource(ResourceParamDto body);
        Task<ResponseResult<Resources>> GetResourceByKey(string key);
        Task<ResponseResult<List<ResourcesKeysVM>>> GetResourcesKeys();
        Task<ResponseResult<List<Resources>>> GetHomePageResources();
        
    }
}

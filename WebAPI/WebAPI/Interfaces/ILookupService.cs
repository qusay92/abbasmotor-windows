namespace WebAPI.Interfaces
{
    public interface ILookupService
    {
        Task<ResponseResult<List<LookupValueVM>>> GetAll();
        Task<ResponseResult<List<LookupValue>>> GetLookupValues(long userId, string search, int LookupId);
        Task<ResponseResult<List<Lookup>>> GetLookups();
        Task<ResponseResult<List<LookupValue>>> Save(LookupValue model);
        Task<ResponseResult<List<LookupValue>>> Delete(List<LookupValue> models);
    }
}

namespace WebAPI.Interfaces
{
    public interface IAutoService
    {
        Task<ResponseResult<List<Auto>>> GetAllByUser(long id, AutoFilterParams search);
        Task<ResponseResult<List<Auto>>> GetArchiveAllByUser(long id, AutoFilterParams search);
        Task<ResponseResult<List<Auto>>> SaveAuto(SaveAutoInput model,long UserId);
        Task<ResponseResult<List<Auto>>> DeleteAutos(List<Auto> model);
        Task<ResponseResult<List<Auto>>> Delete(long Id);
        Task<ResponseResult<List<Auto>>> ArchiveAutos(List<Auto> model);
        Task<ResponseResult<List<Auto>>> UnArchiveAutos(List<Auto> model);
        Task<ResponseResult<List<Auto>>> Archive(long Id);
        Task<ResponseResult<List<Auto>>> DeleteImages(long Id);
        dynamic GetSideMenuByUser(long id, byte isArchive);
        Task<ResponseResult<Auto>> GetAutoById(long id);
        Task<ResponseResult<List<AutoNameVM>>> GetCarNameByUser(long Id);
        Task<ResponseResult<List<UserVM>>> GetClientNameByUser(long Id);
        Task<ResponseResult<List<ClientBuyerNameVM>>> GetClientBuyerNameByUser(long Id);
        Task<ResponseResult<List<UpdatePaymentStatusVM>>> GetUpdatePaymentStatusByUser(long Id);

    }
}

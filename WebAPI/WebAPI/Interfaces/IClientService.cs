using Microsoft.AspNetCore.SignalR;
using WebAPI.ViewModel.Balances;
using WebAPI.ViewModel.Clients;
using WebAPI.ViewModel.ManageClients;

namespace WebAPI.Interfaces
{
    public interface IClientService
    {
        Task<ResponseResult<List<GetClientsVM>>> GetClients(long userId, string search, int userType);
        Task<ResponseResult<List<GetClientsVM>>> SaveClient(User model, bool editPassword, long loggedInUser);
        Task<ResponseResult<List<GetClientsVM>>> Delete(List<User> model);
        Task<ResponseResult<ClientPaymentDetails>> GetClientDetails(long clientId, string search);
        Task<ResponseResult<ClientInfo>> GetClientsInfo(GetBalancesInput input, long UserId);
        Task<ResponseResult<ClientBalance>> GetClientsBalance(string search, long UserId, UserType type);
        Task<ResponseResult<GetClientsVM>> GetClientById(long Id);
        Task<ResponseResult<bool>> UpdatePassword(long UserId,string Password);

    }
}

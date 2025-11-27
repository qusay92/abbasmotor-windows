namespace WebAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<ResponseResult<object>> GetPayment(long autoId);
        Task<ResponseResult<object>> SavePaymentDetails(PaymentDetails model);
        Task<ResponseResult<object>> Delete(List<PaymentDetails> model);
       
        // Task<ResponseResult<object>> SavePayment(Payment model);
        Task<ResponseResult<object>> SavePayment(PaymentInput model, long UserId);
        Task<ResponseResult<object>> GetPayments(GetPaymentsInput input,long UserId);
    }
}

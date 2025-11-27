using Entities;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : BaseController
    {
        public readonly IPaymentService _repo;
        private readonly IStringLocalizer<PaymentsController> _localizerPayments;

        public PaymentsController(IConfiguration configuration, ILoggerService loggerService, IPaymentService repo, IStringLocalizer<PaymentsController> localizerPayments) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerPayments = localizerPayments;
        }

        [HttpGet("GetPayment/{autoId}")]
        public async Task<ResponseResult<object>> GetPayment(long autoId)
        {
            try
            {
                return await _repo.GetPayment(autoId);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, autoId, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("GetPayments")]
        public async Task<ResponseResult<object>> GetPayments([FromBody] GetPaymentsInput input)
        {
            try
            {
                return await _repo.GetPayments(input, UserId);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, input, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("SavePayment")]
        public async Task<ResponseResult<object>> SavePayment([FromBody] PaymentInput model)
        {
            try
            {
                return await _repo.SavePayment(model, UserId);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
        }

        [HttpPost("SavePaymentDetails")]
        public async Task<ResponseResult<object>> SavePaymentDetails([FromBody] PaymentDetails model)
        {
            try
            {
                return await _repo.SavePaymentDetails(model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
           
        }

        [HttpPost("Delete")]
        public async Task<ResponseResult<object>> Delete([FromBody] List<PaymentDetails> model)
        {
            try
            {
                return await _repo.Delete(model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, model, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
                return null;
            }
          
        }


    }
}

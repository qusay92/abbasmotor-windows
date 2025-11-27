using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Logger;

namespace WebAPI.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    public class BaseController : ControllerBase
    {
        private string _acceptLanguage = string.Empty;

        [FromHeader(Name = "Accept-Language")]
        public string AcceptLanguage
        {
            get
            {
                if (string.IsNullOrEmpty(_acceptLanguage))
                {
                    return "en";
                }
                return _acceptLanguage;
            }
            set
            {
                _acceptLanguage = value;
            }
        }

        protected IConfiguration _configuration { get; set; }
        protected readonly ILoggerService _loggerService;

        public BaseController(IConfiguration configuration, ILoggerService loggerService)
        {
            _configuration = configuration;
            _loggerService = loggerService;
        }

        public long UserId
        {
            get
            {
                var userId = UserManager.GetUserId(this.User);
                return userId != 0 ? userId : 0;
            }
        }

        public UserType UserTypes
        {
            get 
            {
                var type = UserManager.GetUserType(this.User);
                return (UserType)type;
            }
        }
    }
}

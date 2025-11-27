using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using WebAPI.Logger;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : BaseController
    {
        public readonly IUserService _repo;
        private readonly IStringLocalizer<LoginController> _localizerLogin;

        public LoginController(IConfiguration configuration, ILoggerService loggerService, IUserService repo, IStringLocalizer<LoginController> localizerLogin) : base(configuration, loggerService)
        {
            _repo = repo;
            _localizerLogin = localizerLogin;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ResponseResult<LoginResonse>>> Login([FromBody] LoginDto dto)
        {
            ResponseResult<LoginResonse> response = new ResponseResult<LoginResonse>();
            response.Data = new LoginResonse();
            var user = await _repo.Login(dto);

            if (user != null && user.Value.Status == StatusType.Success)
            {
                var tokenString = GenerateJSONWebToken(dto, user.Value.Data.Type, user.Value.Data.Id,user.Value.Data.Name);
                response.Status = StatusType.Success;
                response.Data.Id = user.Value.Data.Id;
                response.Data.Name = user.Value.Data.Name;
                response.Data.Type = user.Value.Data.Type;
                response.Data.Token = tokenString;
            }
            else
            {
                response.Status = StatusType.Failed;
                response.Data = user.Value.Data;
                response.Errors = user.Value.Errors;
                _loggerService.LogError(new Exception(user.Value.Errors.FirstOrDefault()), dto, ControllerContext.ActionDescriptor.ControllerName, ControllerContext.ActionDescriptor.ActionName);
            }

            return Ok(response);
        }

        private string GenerateJSONWebToken(LoginDto dto, int type, long id,string name)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim("Id", id.ToString()));
            claims.Add(new Claim("UserType", type.ToString()));
            claims.Add(new Claim("UserName", name));

            var token = new JwtSecurityToken(
              type.ToString(),
              claims: new List<Claim>(claims),
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

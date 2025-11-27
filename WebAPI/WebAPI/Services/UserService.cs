using Microsoft.Extensions.Localization;
using System.Security.Cryptography;

namespace WebAPI.Services
{
    public class UserService : IUserService
    {
        public readonly AmdDBContext _dbContext;
        public readonly IMapper _mapper;

        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;
        public UserService(AmdDBContext dbContext, IStringLocalizer<SharedResource> sharedResourceLocalizer, IMapper mapper)
        {
            _dbContext = dbContext;
            _sharedResourceLocalizer = sharedResourceLocalizer;
            _mapper = mapper;
        }

        public async Task<ResponseResult<LoginResonse>> Register(RegisterationDto dto)
        {
            ResponseResult<LoginResonse> result = new ResponseResult<LoginResonse>();
            List<User> users = new List<User>();
            users = _dbContext.Users.ToList();
            if (String.IsNullOrEmpty(dto.Name))
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["NameIsRequired"] };
                result.Status = StatusType.Failed;
            }
            else if (String.IsNullOrEmpty(dto.Password))
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["PasswordIsRequired"] };
                result.Status = StatusType.Failed;
            }
            else if (dto.Type < 0)
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["SelectUserType"] };
                result.Status = StatusType.Failed;
            }
            else if (users != null && users.Count > 0 && users.Any(u => u.Name.ToLower() == dto.Name.ToLower()))
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["NameAlreadyUsed"] };
                result.Status = StatusType.Failed;
            }
            else
            {
                try
                {
                    var isSaved = false;
                    User user = new User();
                    string encryptedPassword = EncryptPassword(dto.Password);
                    user.Name = dto.Name;
                    user.Password = encryptedPassword;
                    user.Type = dto.Type;
                    _dbContext.Users.Add(user);
                    isSaved = await _dbContext.SaveChangesAsync() > 0;
                    if (isSaved)
                    {
                        User savedUser = _dbContext.Users.Where(u => u.Name.ToLower() == dto.Name.ToLower()).FirstOrDefault();
                        result.Data = new LoginResonse { Id = savedUser.Id, Name = savedUser.Name, Type = savedUser.Type };
                        result.Errors = null;
                        result.Status = StatusType.Success;
                    }
                    else
                    {
                        result.Data = null;
                        result.Errors = new List<string> { _sharedResourceLocalizer["InternalSystemError"] };
                        result.Status = StatusType.Failed;
                    }
                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.Errors = new List<string> { _sharedResourceLocalizer["InternalSystemError"] };
                    result.Status = StatusType.Failed;
                }
            }

            return result;
        }

        public async Task<ActionResult<ResponseResult<LoginResonse>>> Login(LoginDto dto)
        {
            ResponseResult<LoginResonse> result = new ResponseResult<LoginResonse>();
            try
            {
                var user = await _dbContext.Users.Where(u => u.UserName.ToLower() == dto.UserName.ToLower()).FirstOrDefaultAsync();

                if (user == null)
                {
                    result.Data = null;
                    result.Errors = new List<string> { _sharedResourceLocalizer["UserNameDoesNotExist"] };
                    result.Status = StatusType.Failed;
                }
                else
                {
                    try
                    {
                        bool isValidPassword = ValidatePassword(dto.Password, user);
                        if (isValidPassword)
                        {
                            result.Data = new LoginResonse { Id = user.Id, Name = user.Name, Type = user.Type };
                            result.Errors = null;
                            result.Status = StatusType.Success;
                        }
                        else
                        {
                            result.Data = null;
                            result.Errors = new List<string> { _sharedResourceLocalizer["IncorrectPassword"] };
                            result.Status = StatusType.Failed;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Data = null;
                        result.Errors = new List<string> { _sharedResourceLocalizer["InternalSystemError"] };
                        result.Status = StatusType.Failed;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["InternalSystemError"] };
                result.Status = StatusType.Failed;
            }
            return result;
        }

        public async Task<ActionResult<ResponseResult<List<UserVM>>>> GetClients()
        {
            ResponseResult<List<UserVM>> result = new ResponseResult<List<UserVM>>();
            List<User> users = await _dbContext.Users.Where(c => c.Type == (int)UserType.Client).ToListAsync();

            var mapUsers = _mapper.Map<List<User>, List<UserVM>>(users);

            result.Data = mapUsers;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

      
        private bool ValidatePassword(string password, User user)
        {
            string savedPasswordHash = user.Password;
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pdkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pdkdf2.GetBytes(20);

            int ok = 1;
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    ok = 0;
            }
            if (ok == 1)
            {
                return true;
            }

            return false;
        }

        private string EncryptPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pdk = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pdk.GetBytes(20);
            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            return savedPasswordHash;
        }
    }
}

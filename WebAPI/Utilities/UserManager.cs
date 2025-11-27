using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class UserManager
    {
        public static long GetUserId(ClaimsPrincipal user)
        {
            var UserId = user.Claims.Where(u => u.Type.ToLower() == "Id".ToLower()).FirstOrDefault();

            return UserId != null ? long.Parse(UserId.Value) : 0;
        }
        public static int GetUserType(ClaimsPrincipal user)
        {
            var UserType = user.Claims.Where(u => u.Type.ToLower() == "UserType".ToLower()).FirstOrDefault();

            return UserType != null ? int.Parse(UserType.Value) : 0;
        }
        public static string GetUserName(ClaimsPrincipal user)
        {
            var UserName = user.Claims.Where(u => u.Type.ToLower() == "UserName".ToLower()).FirstOrDefault();

            return UserName != null ? UserName.Value : "";
        }
    }
}

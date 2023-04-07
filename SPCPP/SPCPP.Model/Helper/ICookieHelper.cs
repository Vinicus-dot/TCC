using SPCPP.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Helper
{
    public interface ICookieHelper
    {
        void SetUserCookie(UserCookieData userCookieData, string CookieName);

        UserCookieData GetUserCookie(string CookieName);

        void RemoveUserCookie(string CookieName);
    }
}

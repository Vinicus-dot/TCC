using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SPCPP.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Helper
{
    public class CookieHelper : ICookieHelper
    {
        
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetUserCookie(UserCookieData userCookieData , string CookieName)
        {
            var options = new CookieOptions
            {
                Expires = userCookieData.ExpirationTime,
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            string encryptedUserCookieData = Encrypt(JsonConvert.SerializeObject(userCookieData));
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieName, encryptedUserCookieData, options);
        }

        public UserCookieData GetUserCookie(string CookieName)
        {
            string encryptedUserCookieData = _httpContextAccessor.HttpContext.Request.Cookies[CookieName];

            if (string.IsNullOrEmpty(encryptedUserCookieData))
            {
                return null;
            }

            try
            {
                string decryptedUserCookieData = Decrypt(encryptedUserCookieData);
                return JsonConvert.DeserializeObject<UserCookieData>(decryptedUserCookieData);
            }
            catch (Exception ex)
            {
                // O cookie pode ter sido alterado, corrompido ou expirado
                // Se ocorrer uma exceção, ignore o cookie e faça com que o usuário faça login novamente
                return null;
            }
        }

        public void RemoveUserCookie(string CookieName)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(CookieName);
        }

        private string Encrypt(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] protectedBytes = ProtectedData.Protect(inputBytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        private string Decrypt(string encryptedInput)
        {
            byte[] protectedBytes = Convert.FromBase64String(encryptedInput);
            byte[] inputBytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(inputBytes);
        }
    }

}

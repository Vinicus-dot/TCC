using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models
{
    public class UserCookieData
    {
        public ulong UserId { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}

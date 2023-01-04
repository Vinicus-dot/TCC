using SPCPP.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Repository.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User BuscarPorLogin(string login);
        User BuscarPorEmailElogin(string email, string login);
    }
}

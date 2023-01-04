using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Repository.Repositorys
{ 
    public class UserRepository : GenericRepository<User, ApplicationDbContext>, IUserRepository
    {  
        public UserRepository(ApplicationDbContext db) : base(db)
        {

        }

        public User BuscarPorLogin(string login)
        {
            return _db.User.FirstOrDefault(p => p.Login.ToUpper() == login.ToUpper());
        }

        public User BuscarPorEmailElogin(string email, string login)
        {
            return _db.User.FirstOrDefault(p => p.Email.ToUpper() == email.ToUpper() && p.Login.ToUpper() == login.ToUpper());
        }

        
    }
}

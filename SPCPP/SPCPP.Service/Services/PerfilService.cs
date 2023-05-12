using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
using SPCPP.Repository.Repositorys;
using SPCPP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Service.Services
{
    public class PerfilService : IPerfilService
    {
        private readonly IUserRepository _userRepository;

        public PerfilService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<bool> Atualizar(User user)
        {


            try
            {
                user.SetSenhaHash();
                user.DataAtualizacao = DateTime.Now;
                return _userRepository.Editar(user);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

using com.sun.xml.@internal.bind.v2.model.core;
using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
using SPCPP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository  = userRepository;
        }
        public async Task<bool> Adicionar(User usuario)
        {
            try
            {
                usuario.DataCadastro = DateTime.Now;
                usuario.SetSenhaHash();
                return await _userRepository.Cadastrar(usuario);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<bool> Atualizar(User usuario)
        {
            try
            {
                User usuarioDb = PesquisarPorId(usuario.Id);

                if (usuarioDb == null) throw new Exception("Houve um erro na atualização do usuário!");

                usuarioDb.Nome = usuario.Nome;
                usuarioDb.Email = usuario.Email;
                usuarioDb.Login = usuario.Login;
                usuarioDb.Perfil = usuario.Perfil;
                usuarioDb.DataAtualizacao = DateTime.Now;

                return await _userRepository.Editar(usuarioDb);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public User BuscarPorEmailElogin(string email, string login)
        {
            try
            {
                return _userRepository.BuscarPorEmailElogin(email, login);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public User BuscarPorLogin(string login)
        {
            try
            {
                return _userRepository.BuscarPorLogin(login);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<User> BuscarTodos()
        {
            try
            {
                return _userRepository.Listar();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Deletar(ulong id)
        {
            try
            {
                User usuarioDb = _userRepository.PesquisarPorId(id);

                if (usuarioDb == null) throw new Exception("Houve um erro na deleção do usuário!");

                return await _userRepository.Excluir(id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public User PesquisarPorId(ulong id)
        {
            try
            {
                return _userRepository.PesquisarPorId(id);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}

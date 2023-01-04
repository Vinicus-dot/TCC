using SPCPP.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Service.Interface
{
    public interface IUserService
    {
        User BuscarPorLogin(string login);
        User BuscarPorEmailElogin(string email, string login);
        List<User> BuscarTodos();  
        Task<bool> Adicionar(User usuario);
        Task<bool> Atualizar(User usuario);
        Task<bool> Deletar(ulong id);

        User PesquisarPorId(ulong id);
    }
}

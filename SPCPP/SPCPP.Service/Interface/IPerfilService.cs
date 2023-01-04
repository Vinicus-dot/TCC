using SPCPP.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Service.Interface
{
    public interface IPerfilService
    {

        Task<bool> Atualizar(User user);


    }
}
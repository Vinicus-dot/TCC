using SPCPP.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Helper
{
    public interface ISessao
    {
        void CriarSesaoDoUsuario(User usuario);
        void RemoverSessaoDoUsuario();
        User BuscarSessaoDoUsuario();
    }
}

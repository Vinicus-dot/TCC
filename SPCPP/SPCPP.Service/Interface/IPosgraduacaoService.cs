using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Service.Interface
{
    public interface IPosgraduacaoService
    {
        Task<bool> Create(Posgraduacao posgraduacao);

        List<Posgraduacao> Listar();

        Posgraduacao PesquisarPorId(ulong id);

        Task<bool> Atualizar(Posgraduacao posgraduacao);

        Task<bool> Excluir(ulong id);
    }
}

using SPCPP.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Service.Interface
{
    public interface IPosgraduacao_ProfessorService
    {

        Task<bool> Incluir(ulong posgraduacao_id, User usuario);

        List<Professor> ListarProfVinculados(ulong posgraduacao_id);

        List<Professor> PesquisarPorNome(ulong posgraduacao_id ,string nome);

        Task<bool> deletar(ulong id, ulong posid);
    }
}

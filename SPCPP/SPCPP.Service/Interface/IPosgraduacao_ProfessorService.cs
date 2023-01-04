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

        List<Professor> ListarProfVinculados(ulong posgraducao_id);

        List<Professor> PesquisarPorNome(ulong posgraducao_id ,string nome);
    }
}

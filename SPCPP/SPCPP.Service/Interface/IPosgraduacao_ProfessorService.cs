using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SPCPP.Service.Interface
{
    public interface IPosgraduacao_ProfessorService
    {

        Task<bool> Incluir(ulong posgraduacao_id, User usuario, double nota);

        Task<List<ProfessorCadastrado>> ListarProfVinculados(ulong posgraduacao_id);

        Task<List<ProfessorCadastrado>> PesquisarPorNome(ulong posgraduacao_id ,string nome);

        Task<bool> deletar(ulong id, ulong posid);

        double calcularNota(XElement root, string nome);

        Posgraduacao_Professor verifcarUsuarioCadastrado(ulong professorId, ulong posgraducaoId);
    }
}

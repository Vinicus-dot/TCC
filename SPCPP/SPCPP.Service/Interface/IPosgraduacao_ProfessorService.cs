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

        Task<bool> Incluir(ulong posgraduacao_id, User usuario, SolucaoMecanica notas);

        Task<List<ProfessorCadastrado>> ListarProfVinculados(ulong posgraduacao_id);

        Task<List<ProfessorCadastrado>> PesquisarPorNome(ulong posgraduacao_id ,string nome);

        Task<bool> deletar(ulong id, ulong posid);

        SolucaoMecanica calcularNota(XElement root, double indiceh, string nome, ulong posgraducao_id);

        Posgraduacao_Professor verifcarUsuarioCadastrado(ulong professorId, ulong posgraducaoId);

        ProfessorCadastrado SalvarStatus(ulong professor_id, ulong posgraduacao_id, string status);
    }
}

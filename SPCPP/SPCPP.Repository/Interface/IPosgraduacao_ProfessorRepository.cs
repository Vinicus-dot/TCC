using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SPCPP.Repository.Interface
{
    public interface IPosgraduacao_ProfessorRepository : IGenericRepository<Posgraduacao_Professor>
    {
        Task<List<ProfessorCadastrado>> ListarProfVinculados(ulong posgraduacao_id);
        Task<List<ProfessorCadastrado>> PesquisarPorNome(ulong posgraduacao_id, string nome);

        Task<bool> deletar(ulong id, ulong posid);

        SolucaoMecanica calcularNota(XElement root, double indiceh, string nome ,ulong posgraducao_id);

        Task<Posgraduacao_Professor> verifcarUsuarioCadastrado(ulong professorId, ulong posgraducaoId);

        ProfessorCadastrado SalvarStatus(ulong professor_id, ulong posgraduacao_id, string status);

        UploadXML uploadXML(XElement root);

        public SolucaoMecanica cadastrarProfessorPosgraduacao(double indiceh, List<string> listdpregistro, List<string> listpcregistro, List<string> listissn, UploadXML uploadXML);

    }
}

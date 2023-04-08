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
        Posgraduacao verificarProfessorVinculado(ulong professor_id);
        Task<bool> deletar(ulong id, ulong posid);
        ProfessorCadastrado SalvarStatus(ulong professor_id, ulong posgraduacao_id, string status);
        UploadXML uploadXML(XElement root);
        public SolucaoMecanica cadastrarProfessorPosgraduacao(double indiceh, List<string> listdpregistro, List<string> listpcregistro, List<string> listissn, UploadXML uploadXML);

    }
}

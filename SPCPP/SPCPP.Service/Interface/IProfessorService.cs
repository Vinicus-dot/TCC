using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;


namespace SPCPP.Service.Interface
{
    public interface IProfessorService
    {
        Task<bool> Create(ProfessorRequest professorRequest);

        List<Professor> Listar();

        Professor PesquisarProfessor(ulong id);

        string Excluir(ulong id);
        string GetParametro(string nome_parametro);
        Task<bool> Atualizar(Professor professor);

    }
}

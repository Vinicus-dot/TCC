using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;


namespace SPCPP.Service.Interface
{
    public interface IProfessorService
    {
        Task<bool> Create(ProfessorRequest professorRequest);

        List<Professor> Listar();

        Professor PesquisarProfessor(ulong id);

        bool Excluir(ulong id);

        Task<bool> Atualizar(Professor professor);

    }
}

using SPCPP.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Repository.Interface
{
    public interface  IProfessorRepository : IGenericRepository<Professor>
    {
        Professor PesquisarProfessor(ulong id);

        Task<bool> Apagar(ulong id);


    }
}

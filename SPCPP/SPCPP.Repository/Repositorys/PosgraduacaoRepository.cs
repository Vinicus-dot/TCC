using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Repository.Repositorys
{
    public class PosgraduacaoRepository : GenericRepository<Posgraduacao, ApplicationDbContext>, IPosgraduacaoRepository
    {
        public PosgraduacaoRepository(ApplicationDbContext db) : base(db)
        {
        }



    }
}

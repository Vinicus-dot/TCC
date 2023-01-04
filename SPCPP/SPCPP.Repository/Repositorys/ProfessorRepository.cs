using com.sun.xml.@internal.bind.v2.model.core;
using javax.swing.table;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Repository.Repositorys
{
    public class ProfessorRepository: GenericRepository<Professor, ApplicationDbContext>, IProfessorRepository
    {
        public ProfessorRepository(ApplicationDbContext db) : base(db)
        {


        }
        public Professor PesquisarProfessor(ulong id)
        {
            try
            {

                Professor professor = _db.Professores.FirstOrDefault(p => p.user_id == id);

                if (professor == null)
                    throw new Exception("Não foi encontrar o professor");

                return professor;
            }
            catch (Exception)
            {
                throw;

            }
        }

        public async Task<bool> Apagar(ulong id)
        {
            try
            {

                Professor professor = PesquisarProfessor(id);

                if (professor == null)
                    return false;

                try
                {
                    _db.Professores.Remove(professor);
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;

                }
                return true;
            }
            catch (Exception)
            {
                throw;

            }
            
        }




    }
}

using com.sun.xml.@internal.bind.v2.model.core;
using java.awt;
using jdk.nashorn.@internal.ir;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Repository.Repositorys
{
    public class Posgraduacao_ProfessorRepository : GenericRepository<Posgraduacao_Professor, ApplicationDbContext>, IPosgraduacao_ProfessorRepository
    {
        public Posgraduacao_ProfessorRepository(ApplicationDbContext db) : base(db)
        {

        }

        public List<Professor> ListarProfVinculados(ulong posgraducao_id)
        {
            var parametros = new MySqlParameter[]
                 {
                        new MySqlParameter("@posgraducao_id",posgraducao_id),
                 };
            string sql = @"select  p.*  from professor p 
                                        right join posgraduacao_professor pp on p.user_id = pp.professor_id 
                                        left join  usuario u on u.id=pp.professor_id 
                                        where pp.posgraducao_id = @posgraducao_id;";

            
             var lista = _db.Professores.FromSqlRaw(sql, parametros).ToList();

            return lista;
        }

        public List<Professor> PesquisarPorNome(ulong posgraducao_id, string nome)
        {
            var parametros = new MySqlParameter[]
                 {
                        new MySqlParameter("@posgraducao_id",posgraducao_id)
                       
                 };


            string sql = @"select  p.*  from professor p 
                                        right join posgraduacao_professor pp on p.user_id = pp.professor_id 
                                        left join  usuario u on u.id=pp.professor_id 
                                        where pp.posgraducao_id = @posgraducao_id and p.Cnome like ";

            sql += "'%" + nome + "%';";

            var lista = _db.Professores.FromSqlRaw(sql, parametros).ToList();

            return lista;
        }
    }
}

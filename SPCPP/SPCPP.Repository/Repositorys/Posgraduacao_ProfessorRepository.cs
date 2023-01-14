
using Dapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SPCPP.Model.DbContexts;
using SPCPP.Model.Models;
using SPCPP.Repository.Interface;


namespace SPCPP.Repository.Repositorys
{
    public class Posgraduacao_ProfessorRepository : GenericRepository<Posgraduacao_Professor, ApplicationDbContext>, IPosgraduacao_ProfessorRepository
    {
        private ContextSPCPP _contextSPCPP = new();
        public Posgraduacao_ProfessorRepository(ApplicationDbContext db) : base(db)
        {

        }

        public List<Professor> ListarProfVinculados(ulong posgraduacao_id)
        {
            var parametros = new MySqlParameter[]
                 {
                        new MySqlParameter("@posgraduacao_id",posgraduacao_id),
                 };
            string sql = @"select  p.*  from professor p 
                                        right join posgraduacao_professor pp on p.user_id = pp.professor_id 
                                        left join  usuario u on u.id=pp.professor_id 
                                        where pp.posgraduacao_id = @posgraduacao_id;";


            var lista = _db.Professores.FromSqlRaw(sql, parametros).ToList();

            return lista;
        }

        public List<Professor> PesquisarPorNome(ulong posgraduacao_id, string nome)
        {
            var parametros = new MySqlParameter[]
                 {
                        new MySqlParameter("@posgraduacao_id",posgraduacao_id)

                 };

            string sql = @"select  p.*  from professor p 
                                        right join posgraduacao_professor pp on p.user_id = pp.professor_id 
                                        left join  usuario u on u.id=pp.professor_id 
                                        where pp.posgraduacao_id = @posgraduacao_id and p.Cnome like ";

            sql += "'%" + nome + "%';";

            var lista = _db.Professores.FromSqlRaw(sql, parametros).ToList();

            return lista;
        }

        public async Task<bool> deletar(ulong id, ulong posid)
        {
            try
            {
                string sql = $@"DELETE FROM posgraduacao_professor WHERE professor_id = { id} and posgraduacao_id= {posid};";

                _contextSPCPP.GetConnection();

                var result = await _contextSPCPP.Connection.ExecuteAsync(sql);
                
                return result != 0 ? true : false;

            }
            catch(Exception )
            {
                return false;
            }
        }
    }
}

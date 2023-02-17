using Dapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Configuration;
using SPCPP.Model.DbContexts;
using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
using sun.nio.ch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Repository.Repositorys
{
        public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TEntity : class where TContext : ApplicationDbContext
        {
            readonly public ApplicationDbContext _db;
            private ContextSPCPP _contextSPCPP = new();
            public GenericRepository(ApplicationDbContext db)
            {
                _db = db;
            }

            public string GetParametro(string nome_parametro)
            {
                string valor_parametro = string.Empty;
                try
                {
                    string like= "'%"+nome_parametro +"%'";
                    string sql = $@" select valor_parametro from parametros where nome_parametro like {like}; ";
                    _contextSPCPP.GetConnection();

                    valor_parametro =  _contextSPCPP.Connection.QueryFirstOrDefaultAsync<string>(sql).Result;
                }
                catch(Exception )
                {
                    throw;
                }
                return valor_parametro;
            }
            public async Task<bool> Cadastrar(TEntity objeto)
            {
                try
                {
                    _db.Add(objeto);
                    _db.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;

                }
            }

            public async Task<bool> Editar(TEntity objeto)
            {
                try
                {
                    _db.Update(objeto);
                    _db.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;

                }
            }

            public async Task<bool> Excluir(ulong id)
            {


                try
                {
                    var objeto = PesquisarPorId(id);
                    _db.Remove(objeto);
                    _db.SaveChanges();
                    return true;

                }
                catch
                {
                    return false;

                }



            }

            public List<TEntity> Listar()
            {
                return _db.Set<TEntity>().ToList();
            }

            public TEntity PesquisarPorId(ulong id)
            {
                return _db.Find<TEntity>(id);
            }



        }
}

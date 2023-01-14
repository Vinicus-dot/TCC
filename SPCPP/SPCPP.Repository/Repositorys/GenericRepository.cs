using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Configuration;
using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
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
            
            public GenericRepository(ApplicationDbContext db)
            {
                _db = db;
            }

        public string Conection(string nomeConection)
        {
            string connectionString = string.Empty;
            try
            {

                IConfigurationRoot configuration = new ConfigurationBuilder()
                          .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                          .AddJsonFile("appsettings.json")
                        .Build();
                connectionString = configuration.GetConnectionString(nomeConection);

            }
            catch (Exception)
            {

            }
            return connectionString;
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Repository.Interface
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        string GetParametro(string nome_parametro);
        List<TEntity> Listar();

        Task<bool> Cadastrar(TEntity objeto);

        TEntity PesquisarPorId(ulong id);

        Task<bool> Editar(TEntity objeto);

        Task<bool> Excluir(ulong id);
    }

}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Models
{
    public class PaginaList<T> : List<T>
    {
        public int  PaginaInicial { get; set; }
        public int TotalPaginas { get; set; }


        public PaginaList(List<T> items, int count , int paginainicial, int totalpagina )
        {   
            PaginaInicial = paginainicial;
            TotalPaginas = (int)Math.Ceiling(count/(double)totalpagina);
            
            this.AddRange(items);
        }

        public bool PaginaAnterior => PaginaInicial > 1;

        public bool ProximaPagina => PaginaInicial < TotalPaginas;
       
        //uso geral
        public static PaginaList<T> Create (List<T> conteudo, int paginainicial, int totalpagina)
        {
            var count = conteudo.Count;

            var items = conteudo.Skip((paginainicial-1)*totalpagina).Take(totalpagina).ToList();

            return new PaginaList<T>(items, count, paginainicial, totalpagina ); 
        }
       


    }
}

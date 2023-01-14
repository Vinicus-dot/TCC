﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
using SPCPP.Repository.Repositorys;
using SPCPP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Service.Services
{
    public class Posgraduacao_ProfessorService : IPosgraduacao_ProfessorService
    {

        private readonly IPosgraduacao_ProfessorRepository _posgraduacao_ProfessorRepository;
        

        public Posgraduacao_ProfessorService (IPosgraduacao_ProfessorRepository posgraduacao_ProfessorRepository)
        {
            _posgraduacao_ProfessorRepository = posgraduacao_ProfessorRepository;
           
        }

        public Task<bool> Incluir(ulong posgraduacao_id , User usuario)
        {

            try
            {            
               
                Posgraduacao_Professor posgraduacao_Professor = new Posgraduacao_Professor();

                posgraduacao_Professor.posgraduacao_id = posgraduacao_id;
                posgraduacao_Professor.professor_id = usuario.Id;
                posgraduacao_Professor.DataCadastro = DateTime.Now;
                



                return _posgraduacao_ProfessorRepository.Cadastrar(posgraduacao_Professor);
            }
            catch (Exception)
            {

                throw;
            }


        }

        public List<Professor> ListarProfVinculados(ulong posgraduacao_id)
        {


            try
            {
                return _posgraduacao_ProfessorRepository.ListarProfVinculados(posgraduacao_id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Professor> PesquisarPorNome(ulong posgraduacao_id ,string nome )
        {


            try
            {
                return _posgraduacao_ProfessorRepository.PesquisarPorNome(posgraduacao_id, nome );
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> deletar(ulong id, ulong posid)
        {

            try
            {

                return _posgraduacao_ProfessorRepository.deletar(id,posid);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

using com.sun.xml.@internal.bind.v2.model.core;
using java.awt;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using SPCPP.Repository.Interface;
using SPCPP.Repository.Repositorys;
using SPCPP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SPCPP.Service.Services
{
    public class Posgraduacao_ProfessorService : IPosgraduacao_ProfessorService
    {

        private readonly IPosgraduacao_ProfessorRepository _posgraduacao_ProfessorRepository;
        

        public Posgraduacao_ProfessorService (IPosgraduacao_ProfessorRepository posgraduacao_ProfessorRepository)
        {
            _posgraduacao_ProfessorRepository = posgraduacao_ProfessorRepository;
           
        }

        public Task<bool> Incluir(ulong posgraduacao_id , User usuario, SolucaoMecanica notas)
        {

            try
            {            
               
                Posgraduacao_Professor posgraduacao_Professor = new Posgraduacao_Professor();

                posgraduacao_Professor.posgraduacao_id = posgraduacao_id;
                posgraduacao_Professor.professor_id = usuario.Id;
                posgraduacao_Professor.DataCadastro = DateTime.Now;
                posgraduacao_Professor.nota = notas.nota;
                posgraduacao_Professor.A1 = notas.A1;
                posgraduacao_Professor.A2 = notas.A2;
                posgraduacao_Professor.A3 = notas.A3;
                posgraduacao_Professor.A4 = notas.A4;
                posgraduacao_Professor.DP = notas.DP;
                posgraduacao_Professor.PC = notas.PC;
                posgraduacao_Professor.PQ = notas.PQ;
                posgraduacao_Professor.indiceH = notas.indiceH;


                return _posgraduacao_ProfessorRepository.Cadastrar(posgraduacao_Professor);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public Task<List<ProfessorCadastrado>> ListarProfVinculados(ulong posgraduacao_id)
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

        public Task<List<ProfessorCadastrado>> PesquisarPorNome(ulong posgraduacao_id ,string nome )
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

        public SolucaoMecanica calcularNota(XElement root, string nome , ulong posgraducao_id)
        {
            try
            {

                return _posgraduacao_ProfessorRepository.calcularNota(root,nome , posgraducao_id);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public Posgraduacao_Professor verifcarUsuarioCadastrado(ulong professorId, ulong posgraducaoId)
        {
            try
            {

                return _posgraduacao_ProfessorRepository.verifcarUsuarioCadastrado(professorId, posgraducaoId).Result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

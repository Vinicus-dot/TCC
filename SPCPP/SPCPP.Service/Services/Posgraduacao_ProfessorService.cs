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
        private readonly IProfessorService _professorService;

        public Posgraduacao_ProfessorService (IPosgraduacao_ProfessorRepository posgraduacao_ProfessorRepository, IProfessorService professorService)
        {
            _posgraduacao_ProfessorRepository = posgraduacao_ProfessorRepository;
            _professorService = professorService;
           
        }

        public Task<bool> Incluir(ulong posgraduacao_id , User usuario, SolucaoMecanica notas)
        {

            try
            {            
               
                Posgraduacao_Professor posgraduacao_Professor = new Posgraduacao_Professor();

                posgraduacao_Professor.posgraduacao_id = posgraduacao_id;
                posgraduacao_Professor.professor_id = usuario.Id;
                posgraduacao_Professor.DataCadastro = DateTime.Now;
                posgraduacao_Professor.status = "Aguardando Avaliação";
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

        public SolucaoMecanica calcularNota(XElement root, double indiceh,string nome , ulong posgraducao_id)
        {
            try
            {

                return _posgraduacao_ProfessorRepository.calcularNota(root,indiceh,nome , posgraducao_id);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public UploadXML uploadXML(XElement root)
        {
            try
            {

                return _posgraduacao_ProfessorRepository.uploadXML(root);
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
        
        public ProfessorCadastrado SalvarStatus(ulong professor_id, ulong posgraduacao_id, string status)
        {
            try
            {
                if (string.IsNullOrEmpty(status))
                    throw new Exception("Selecione um status!");

                return _posgraduacao_ProfessorRepository.SalvarStatus(professor_id,  posgraduacao_id,  status);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public double cadastrarProfessorPosgraduacao(double indiceh, bool pq, List<string> listdpregistro, List<string> listpcregistro, List<string> listissn, string uploadXML, User? usuario)
        {
            try
            {
                UploadXML? uploadXML1 = System.Text.Json.JsonSerializer.Deserialize<UploadXML>(uploadXML);

                if (indiceh <= 0)
                    throw new Exception("É necessario informar o Indice-H!");

                if (indiceh >= 200)
                    throw new Exception("O Indice-H está muito alto, procure um administrador!");

                if (usuario?.Perfil != Model.Enums.PerfilEnum.Docente)
                    throw new Exception("Seu tipo de Perfil não pode cadastrar em Pós Gradução!");

                string[] name = uploadXML1.nome.Trim().ToLower().Split(' ');

                string usuarionome = usuario.Nome.Trim().ToLower();
                
                if (_professorService.PesquisarProfessor(usuario.Id).numero_identificador.Trim() != uploadXML1.numero_identificador.Trim())
                    throw new Exception("O Campo numero identificador do XML não condiz com o perfil logado!");

                uploadXML1.pq = pq;

                SolucaoMecanica notas = _posgraduacao_ProfessorRepository.cadastrarProfessorPosgraduacao(indiceh, listdpregistro, listpcregistro, listissn, uploadXML1);


                Posgraduacao_Professor posgraduacao_Professor = new Posgraduacao_Professor();

                posgraduacao_Professor.posgraduacao_id = uploadXML1.posgraduacao_id;
                posgraduacao_Professor.professor_id = usuario.Id;
                posgraduacao_Professor.DataCadastro = DateTime.Now;
                posgraduacao_Professor.status = "Aguardando Avaliação";
                posgraduacao_Professor.nota = notas.nota;
                posgraduacao_Professor.A1 = notas.A1;
                posgraduacao_Professor.A2 = notas.A2;
                posgraduacao_Professor.A3 = notas.A3;
                posgraduacao_Professor.A4 = notas.A4;
                posgraduacao_Professor.DP = notas.DP;
                posgraduacao_Professor.PC = notas.PC;
                posgraduacao_Professor.PQ = notas.PQ;
                posgraduacao_Professor.indiceH = notas.indiceH;


                if (!_posgraduacao_ProfessorRepository.Cadastrar(posgraduacao_Professor).Result)
                    throw new Exception("Falha ao salvar Professor na Pós-graduação!");
                

                return posgraduacao_Professor.nota;
            }
            catch (Exception )
            {

                throw;
            }
        }
    }
}

﻿using com.sun.org.apache.bcel.@internal.generic;
using java.awt;
using SPCPP.Model.Enums;
using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using SPCPP.Repository.Interface;
using SPCPP.Repository.Repositorys;
using SPCPP.Service.Interface;
using sun.awt.geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Service.Services
{
    public class ProfessorService : IProfessorService
    {
        private readonly IProfessorRepository _professorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPosgraduacao_ProfessorRepository _posgraduacao_ProfessorRepository;

        public ProfessorService(IProfessorRepository alunoRepository, IUserRepository userRepository, IPosgraduacao_ProfessorRepository posgraduacao_ProfessorRepository)
        {
            _professorRepository = alunoRepository;
            _userRepository = userRepository;
            _posgraduacao_ProfessorRepository = posgraduacao_ProfessorRepository;
        }


        public Task<bool> Create(ProfessorRequest professorRequest)
        {

            try
            {

                User usuario = new User();
                Professor professor = new Professor();

                usuario.Nome = professorRequest.Nome;
                usuario.Login = professorRequest.Login;
                usuario.Email = professorRequest.Email;
                usuario.Perfil = PerfilEnum.Docente;
                usuario.Senha = professorRequest.Senha;
                usuario.DataCadastro = DateTime.Now;
                usuario.SetSenhaHash();

                if (_userRepository.Cadastrar(usuario).Result)
                    usuario = _userRepository.BuscarPorLogin(usuario.Login);
                else
                    throw new Exception("Não foi possivel cadastrar o usuário/professor");

                professor.numero_identificador = professorRequest.numero_identificador;
                professor.siape = professorRequest.Login;
                professor.Cnome = professorRequest.Cnome;
                professor.Email = professorRequest.Email;
                professor.Data_nasc = professorRequest.Data_nasc;
               

                professor.user_id = usuario.Id;

  
                return _professorRepository.Cadastrar(professor);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Professor> Listar()
        {


            try
            {
                List<Professor> professor = _professorRepository.Listar();



                return professor;
            }
               
            catch (Exception)
            {

                throw;
            }
        }

        public Professor PesquisarProfessor(ulong id)
        {


            try
            {
                return _professorRepository.PesquisarProfessor(id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string Excluir(ulong id)
        {


            try
            {
                Professor professor = _professorRepository.PesquisarProfessor(id);
                if (professor == null)
                    throw new Exception("Erro Professor não encontrado!");

                Posgraduacao posgraduacao = _posgraduacao_ProfessorRepository.verificarProfessorVinculado(professor.user_id);

                if (posgraduacao != null)
                    throw new Exception($"Professor vinculado a Pós Graduação {posgraduacao.nome}");

                if (_professorRepository.Apagar(professor.user_id).Result && _userRepository.Excluir(professor.user_id).Result)
                    return professor.Cnome;

                return string.Empty;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> Atualizar(Professor professor)
        {


            try
            {
                return _professorRepository.Editar(professor);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetParametro(string nome_parametro)
        {


            try
            {
                return _professorRepository.GetParametro(nome_parametro);
            }
            catch (Exception)
            {

                throw;
            }
        }



    }
}

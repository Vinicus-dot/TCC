using SPCPP.Model.Models;
using SPCPP.Repository.Interface;
using SPCPP.Repository.Repositorys;
using SPCPP.Service.Interface;


namespace SPCPP.Service.Services
{
    public class PosgraduacaoService : IPosgraduacaoService
    {
        private readonly IPosgraduacaoRepository _posgraduacaoRepository;
        public PosgraduacaoService(IPosgraduacaoRepository posgraduacaoRepository)
        {
            _posgraduacaoRepository = posgraduacaoRepository;
        }

        public Task<bool> Create(Posgraduacao posgraduacao)
        {

            try
            {
                posgraduacao.DataCadastro = DateTime.Now;
                
                return _posgraduacaoRepository.Cadastrar(posgraduacao);
            }
            catch (Exception)
            {

                throw;
            }


        }

        public List<Posgraduacao> Listar()
        {


            try
            {
                return _posgraduacaoRepository.Listar();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Posgraduacao PesquisarPorId(ulong id)
        {


            try
            {
                return _posgraduacaoRepository.PesquisarPorId(id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> Atualizar(Posgraduacao posgraduacao)
        {


            try
            {
                posgraduacao.DataAtualizacao = DateTime.Now;
                return _posgraduacaoRepository.Editar(posgraduacao);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Task<bool> Excluir(ulong id)
        {

            try
            {
                
                return _posgraduacaoRepository.Excluir(id);
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
                return  _posgraduacaoRepository.GetParametro(nome_parametro);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

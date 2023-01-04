using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SPCPP.Model.Helper;
using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using SPCPP.Service.Interface;
using SPCPP.Service.Services;

namespace SPCPP.Web.Controllers
{
    public class Posgraduacao_ProfessorController : Controller
    {
        private readonly IPosgraduacaoService _posgraduacaoService;
        private readonly IPosgraduacao_ProfessorService _posgraduacao_ProfessorService;
        private readonly ISessao _sessao;

        public Posgraduacao_ProfessorController(IPosgraduacao_ProfessorService posgraduacao_ProfessorService, ISessao sessao, IPosgraduacaoService posgraduacaoService)
        {
            _posgraduacao_ProfessorService = posgraduacao_ProfessorService;
            _sessao = sessao;
            _posgraduacaoService = posgraduacaoService;
        }


        public async Task<JsonResult> Incluir(ulong posgraducao_id)
        {
            try
            {
                string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");
                if (string.IsNullOrEmpty(sessaoUsuario))
                    return Json(new { sucesso = false, mensagem = "Usuario não encontrado!" });

                User usuario = JsonConvert.DeserializeObject<User>(sessaoUsuario);

                if (usuario.Perfil == Model.Enums.PerfilEnum.Admin)
                    return Json(new { sucesso = false, mensagem = "Seu tipo de Perfil não pode cadastrar em Pós Gradução!" });

                bool valido = _posgraduacao_ProfessorService.Incluir(posgraducao_id, usuario).Result;


                return Json(new { sucesso = true, valido = valido });

            }
            catch (Exception ex)
            {

                return Json(new { sucesso = false, mensagem = ex.Message });
            }

        }
        public IActionResult ListarProfVinculados(ulong id, int? numeroPagina, string? nome)
        {
            try
            {
                int totalpagina = 5;
                Posgraduacao posgraducao = _posgraduacaoService.PesquisarPorId(id);
                TempData["Posgraduacao"] = posgraducao.nome;

                List<Professor> professores = new List<Professor>();
                if (!string.IsNullOrEmpty(nome))
                {
                    professores = _posgraduacao_ProfessorService.PesquisarPorNome(id,nome);
                    
                }
                else
                {
                    professores = _posgraduacao_ProfessorService.ListarProfVinculados(id);

                }

               
                

                return View(PaginaList<Professor>.Create(professores, numeroPagina ?? 1, totalpagina ,id));

            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex;
                return View();
            }

        }
        public IActionResult Pesquisar(ulong id, int? numeroPagina, string? nome)
        {
            try
            {
                int totalpagina = 5;
                Posgraduacao posgraducao = _posgraduacaoService.PesquisarPorId(id);
                TempData["Posgraduacao"] = posgraducao.nome;

                List<Professor> professores = new List<Professor>();
                if (!string.IsNullOrEmpty(nome))
                {
                    professores = _posgraduacao_ProfessorService.PesquisarPorNome(id, nome);

                }


                return PartialView(PaginaList<Professor>.Create(professores, numeroPagina ?? 1, totalpagina, id));

            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex;
                return PartialView();
            }

        }
    }
}

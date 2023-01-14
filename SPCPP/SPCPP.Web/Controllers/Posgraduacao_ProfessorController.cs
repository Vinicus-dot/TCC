using javax.annotation.processing;
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
        public IActionResult ListarProfVinculados(ulong id, int? pagina, string? Filter, string Ordenar)
        {
            try
            {
                ViewBag.Filter = Filter;
                int totalpagina = 5;
                Posgraduacao posgraduacao = _posgraduacaoService.PesquisarPorId(id);
                TempData["Posgraduacao"] = posgraduacao.nome.ToUpper();
                ViewBag.PosId = posgraduacao.id;
                List<Professor> professores = new List<Professor>();
                if (!string.IsNullOrEmpty(Filter))
                {
                    professores = _posgraduacao_ProfessorService.PesquisarPorNome(id, Filter);
                    
                }
                else
                {
                    professores = _posgraduacao_ProfessorService.ListarProfVinculados(id);

                }
                ViewBag.Id = id;
                ViewBag.OrdenarPg = Ordenar;
                ViewBag.CnomeParm = String.IsNullOrEmpty(Ordenar) ? "cnome" : "";
                ViewBag.EmailParm = Ordenar == "email" ? "email_desc" : "email";
                ViewBag.Data_NascParm = Ordenar == "data_nasc" ? "data_nasc_desc" : "data_nasc";
                ViewBag.Data_exoneracaoParm = Ordenar == "data_exoneracao" ? "data_exoneracao_desc" : "data_exoneracao";
                ViewBag.Data_saidaParm = Ordenar == "data_saida" ? "data_saida_desc" : "data_saida";
                ViewBag.Data_aposentadoriaParm = Ordenar == "data_aposentadoria" ? "data_aposentadoria_desc" : "data_aposentadoria";
                ViewBag.Carga_atualParm = Ordenar == "carga_atual" ? "carga_atual_desc" : "carga_atual";
                ViewBag.StatusParm = Ordenar == "status" ? "status_desc" : "status";
                ViewBag.SiapeParm = Ordenar == "siape" ? "siape_desc" : "siape";


                switch (Ordenar)
                {
                    case "cnome":
                        professores = professores.OrderByDescending(s => s.Cnome).ToList();
                        break;
                    case "email":
                        professores = professores.OrderBy(s => s.Email).ToList();
                        break;
                    case "email_desc":
                        professores = professores.OrderByDescending(s => s.Email).ToList();
                        break;
                    case "data_nasc":
                        professores = professores.OrderBy(s => s.Data_nasc).ToList();
                        break;
                    case "data_nasc_desc":
                        professores = professores.OrderByDescending(s => s.Data_nasc).ToList();
                        break;
                    case "data_exoneracao":
                        professores = professores.OrderBy(s => s.Data_exoneracao).ToList();
                        break;
                    case "data_exoneracao_desc":
                        professores = professores.OrderByDescending(s => s.Data_exoneracao).ToList();
                        break;
                    case "data_saida":
                        professores = professores.OrderBy(s => s.Data_saida).ToList();
                        break;
                    case "data_saida_desc":
                        professores = professores.OrderByDescending(s => s.Data_saida).ToList();
                        break;
                    case "data_aposentadoria":
                        professores = professores.OrderBy(s => s.Data_aposentadoria).ToList();
                        break;
                    case "data_aposentadoria_desc":
                        professores = professores.OrderByDescending(s => s.Data_aposentadoria).ToList();
                        break;
                    case "carga_atual":
                        professores = professores.OrderBy(s => s.Carga_atual).ToList();
                        break;
                    case "carga_atual_desc":
                        professores = professores.OrderByDescending(s => s.Carga_atual).ToList();
                        break;
                    
                    case "status":
                        professores = professores.OrderBy(s => s.Status).ToList();
                        break;
                    case "status_desc":
                        professores = professores.OrderByDescending(s => s.Status).ToList();
                        break;
                    
                    case "siape":
                        professores = professores.OrderBy(s => s.siape).ToList();
                        break;
                    case "siape_desc":
                        professores = professores.OrderByDescending(s => s.siape).ToList();
                        break;

                    default:
                        professores = professores.OrderBy(s => s.Cnome).ToList();
                        break;
                }

                return View(PaginaList<Professor>.Create(professores, pagina ?? 1, totalpagina));

            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex;
                return View();
            }

        }

        public async Task<JsonResult> Delete(ulong id, ulong posid)
        {
            try
            {

                bool valido = _posgraduacao_ProfessorService.deletar(id, posid).Result ;


                return Json(new { sucesso = true, valido = valido });

            }
            catch (Exception ex)
            {

                return Json(new { sucesso = false, mensagem = ex.Message });
            }

        }

    }
}

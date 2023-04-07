using java.awt;
using javax.annotation.processing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SPCPP.Model.Filters;
using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using SPCPP.Service.Interface;
using SPCPP.Service.Services;
using static com.sun.net.httpserver.Authenticator;

namespace SPCPP.Web.Controllers
{
    [PaginaRestritaAvaliador]
    public class ProfessorController : Controller
    {
        private readonly IProfessorService _professorService;
        public ProfessorController(IProfessorService professorService)
        {
            _professorService = professorService;
        }
        public IActionResult Index(string Ordenar , string Filter, int? pagina , string? pesquisar)
        {
            try
            {
                string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");
                User usuario = JsonConvert.DeserializeObject<User>(sessaoUsuario);

                ViewBag.Perfil = usuario.Perfil;

                int totalpagina = Convert.ToInt32(_professorService.GetParametro("NUMERO_PAGINATION"));
                List<Professor> professores = _professorService.Listar();

                ViewBag.OrdenarPg = Ordenar;
                ViewBag.CnomeParm = String.IsNullOrEmpty(Ordenar) ? "cnome" : "";
                ViewBag.EmailParm = Ordenar == "email" ? "email_desc" : "email";
                ViewBag.Data_NascParm = Ordenar == "data_nasc" ? "data_nasc_desc" : "data_nasc";
                ViewBag.Data_exoneracaoParm = Ordenar == "data_exoneracao" ? "data_exoneracao_desc" : "data_exoneracao";
                ViewBag.Data_saidaParm = Ordenar == "data_saida" ? "data_saida_desc" : "data_saida";
                ViewBag.Data_aposentadoriaParm = Ordenar == "data_aposentadoria" ? "data_aposentadoria_desc" : "data_aposentadoria";                
                ViewBag.SiapeParm = Ordenar == "siape" ? "siape_desc" : "siape";
                ViewBag.NLattexParm =  Ordenar == "nlattex" ? "nlattex_desc" : "nlattex";

                if (pesquisar != null) pagina = 1; else pesquisar = Filter;
                ViewBag.Filter = pesquisar;
                if (!String.IsNullOrEmpty(pesquisar))
                    professores = professores.Where(s => s.Cnome.Contains(pesquisar) || s.Email.Contains(pesquisar)).ToList();

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
                    case "nlattex":
                        professores = professores.OrderBy(s => s.numero_identificador).ToList();
                        break;
                    case "nlattex_desc":
                        professores = professores.OrderByDescending(s => s.numero_identificador).ToList();
                        break;
                    case "data_aposentadoria":
                        professores = professores.OrderBy(s => s.Data_aposentadoria).ToList();
                        break;
                    case "data_aposentadoria_desc":
                        professores = professores.OrderByDescending(s => s.Data_aposentadoria).ToList();
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

        public IActionResult Create()
        {
            string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");
            User usuariologado = JsonConvert.DeserializeObject<User>(sessaoUsuario);
            ViewBag.Perfil = usuariologado.Perfil;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProfessorRequest professorRequest)
        {
            try
            {
                string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");
                User usuariologado = JsonConvert.DeserializeObject<User>(sessaoUsuario);
                ViewBag.Perfil = usuariologado.Perfil;

                if (_professorService.Create(professorRequest).Result )
                {
                    TempData["MensagemSucesso"] = "Professor cadastrado com sucesso";
                    return RedirectToAction("Index");
                }


                return View(professorRequest);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possivel cadastrar o professor, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }

        }

        public IActionResult Delete(ulong id)
        {
            try
            {
                string nome = _professorService.Excluir(id);

                if (string.IsNullOrEmpty(nome))
                    throw new Exception("Falha em deletar professor!");

                return Json(new { success = true , message  = $"Sucesso em deletar o professor {nome}"});
               
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }

        }


        public IActionResult Editar(ulong id)
        {
            string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");
            User usuariologado = JsonConvert.DeserializeObject<User>(sessaoUsuario);
            ViewBag.Perfil = usuariologado.Perfil;

            Professor professor = _professorService.PesquisarProfessor(id);
            return View(professor);
        }

        [HttpPost]
        public IActionResult Editar(Professor professor)
        {
            try
            {
                string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");
                User usuariologado = JsonConvert.DeserializeObject<User>(sessaoUsuario);
                ViewBag.Perfil = usuariologado.Perfil;

                if (_professorService.Atualizar(professor).Result )
                {

                    TempData["MensagemSucesso"] = "Professor atualizado com sucesso";
                    return RedirectToAction("Index");

                }

                return View(professor);
            }
            catch (Exception erro)
            {

                TempData["MensagemErro"] = $"Ops, não foi possivel atualizar o Professor tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }


        }
    }
}
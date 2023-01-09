using javax.annotation.processing;
using Microsoft.AspNetCore.Mvc;
using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using SPCPP.Service.Interface;
using SPCPP.Service.Services;

namespace SPCPP.Web.Controllers
{
    public class ProfessorController : Controller
    {
        private readonly IProfessorService _professorService;
        public ProfessorController(IProfessorService professorService)
        {
            _professorService = professorService;
        }
        public IActionResult Index(string Ordenar , int? numeroPagina , string? pesquisar)
        {
            try
            {
                ViewBag.Nome = pesquisar;
                int totalpagina = 5;
                List<Professor> professores = _professorService.Listar();

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

                return View(PaginaList<Professor>.Create(professores, numeroPagina ?? 1, totalpagina));
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex;
                return View();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProfessorRequest professorRequest)
        {
            try
            {

                if (ModelState.IsValid &&  _professorService.Create(professorRequest).Result )
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

        public async Task<JsonResult> Delete(ulong id)
        {
            try
            {

                var professor = _professorService.PesquisarProfessor(id);
                if (professor == null)
                    return Json(new { sucesso = false, valido = false });

                bool valido = _professorService.Excluir(professor.user_id);

                return Json(new { sucesso = true, valido = valido });

            }
            catch (Exception ex)
            {

                return Json(new { sucesso = false, mensagem = ex.Message });
            }

        }


        public IActionResult Editar(ulong id)
        {
            Professor professor = _professorService.PesquisarProfessor(id);
            return View(professor);
        }

        [HttpPost]
        public IActionResult Editar(Professor professor)
        {
            try
            {
      
                if ( ModelState.IsValid && _professorService.Atualizar(professor).Result )
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
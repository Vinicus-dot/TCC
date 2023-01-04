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
        public IActionResult Index()
        {
            List<Professor> professor = _professorService.Listar();

            return View(professor);
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
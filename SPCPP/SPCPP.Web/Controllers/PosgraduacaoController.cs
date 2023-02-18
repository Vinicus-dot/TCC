using com.sun.xml.@internal.bind.v2.model.core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SPCPP.Model.Filters;
using SPCPP.Model.Models;
using SPCPP.Service.Interface;
using SPCPP.Service.Services;
using System.Xml.Linq;

namespace SPCPP.Web.Controllers
{
    //[PaginaParaUsuarioLogado]
    public class PosgraduacaoController : Controller
    {
        private readonly IPosgraduacaoService _posgraduacaoService;
        public PosgraduacaoController(IPosgraduacaoService posgraduacaoService)
        {
            _posgraduacaoService = posgraduacaoService;
        }

        public ViewResult Index(string Ordenar, string Filter, string pesquisar, int? pagina)
        {
            try
            {
                ViewBag.OrdernarPg = Ordenar;
                ViewBag.NameParm = String.IsNullOrEmpty(Ordenar) ? "nome" : "";
                ViewBag.DateParm = Ordenar == "data" ? "data_desc" : "data";
                ViewBag.NameDoCursoParm = Ordenar == "nomeCurso" ? "nomeCurso_desc" : "nomeCurso";
                ViewBag.CampusDoCursoParm = Ordenar == "campusDoCurso" ? "campusDoCurso_desc" : "campusDoCurso";
                ViewBag.DescricaoParm = Ordenar == "descricao" ? "descricao_desc" : "descricao";
                ViewBag.EditalParm = Ordenar == "edital" ? "edital_desc" : "edital";
                ViewBag.DateAttParm = Ordenar == "dateAtt" ? "dateAtt_desc" : "dateAtt";

                if (pesquisar != null) pagina = 1; else pesquisar = Filter;

                ViewBag.Filter = pesquisar;

                List<Posgraduacao> posgraduacao = _posgraduacaoService.Listar();

                if (!String.IsNullOrEmpty(pesquisar))
                    posgraduacao = posgraduacao.Where(s => s.nome.Contains(pesquisar) || s.nome_curso.Contains(pesquisar)).ToList();

                switch (Ordenar)
                {
                    case "nome":
                        posgraduacao = posgraduacao.OrderByDescending(s => s.nome).ToList();
                        break;
                    case "data":
                        posgraduacao = posgraduacao.OrderBy(s => s.DataCadastro).ToList();
                        break;
                    case "data_desc":
                        posgraduacao = posgraduacao.OrderByDescending(s => s.DataCadastro).ToList();
                        break;
                    case "nomeCurso":
                        posgraduacao = posgraduacao.OrderBy(s => s.nome_curso).ToList();
                        break;
                    case "nomeCurso_desc":
                        posgraduacao = posgraduacao.OrderByDescending(s => s.nome_curso).ToList();
                        break;
                    case "campusDoCurso":
                        posgraduacao = posgraduacao.OrderBy(s => s.nome_curso).ToList();
                        break;
                    case "campusDoCurso_desc":
                        posgraduacao = posgraduacao.OrderByDescending(s => s.nome_curso).ToList();
                        break;
                    case "descricao":
                        posgraduacao = posgraduacao.OrderBy(s => s.descricao).ToList();
                        break;
                    case "descricao_desc":
                        posgraduacao = posgraduacao.OrderByDescending(s => s.descricao).ToList();
                        break;
                    case "edital":
                        posgraduacao = posgraduacao.OrderBy(s => s.edital).ToList();
                        break;
                    case "edital_desc":
                        posgraduacao = posgraduacao.OrderByDescending(s => s.edital).ToList();
                        break;
                    case "dateAtt":
                        posgraduacao = posgraduacao.OrderBy(s => s.DataAtualizacao).ToList();
                        break;
                    case "dateAtt_desc":
                        posgraduacao = posgraduacao.OrderByDescending(s => s.DataAtualizacao).ToList();
                        break;

                    default:
                        posgraduacao = posgraduacao.OrderBy(s => s.nome).ToList();
                        break;
                }

                int totalpagina = Convert.ToInt32(_posgraduacaoService.GetParametro("NUMERO_PAGINATION"));
                return View(PaginaList<Posgraduacao>.Create(posgraduacao, pagina ?? 1, totalpagina));
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex;
                return View();
            }

        }

        public ActionResult Pesquiar(ulong id, int? numeroPagina, string? nome)
        {
            try
            {
                int totalpagina = 5;
                List<Posgraduacao> posgraduacao = _posgraduacaoService.Listar();


                return PartialView(PaginaList<Posgraduacao>.Create(posgraduacao, numeroPagina ?? 1, totalpagina));

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
        public IActionResult Create(Posgraduacao posgraduacao)
        {
            try
            {

                if (ModelState.IsValid && _posgraduacaoService.Create(posgraduacao).Result)
                {
                    TempData["MensagemSucesso"] = "Pos Graduação cadastrado com sucesso";
                    return RedirectToAction("Index");
                }


                return View(posgraduacao);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possivel cadastrar a Pos Graduação, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }


        public IActionResult Editar(ulong id)
        {
            Posgraduacao posgraduacao = _posgraduacaoService.PesquisarPorId(id);
            return View(posgraduacao);
        }

        [HttpPost]
        public IActionResult Editar(Posgraduacao posgraduacao)
        {
            try
            {

                if (ModelState.IsValid && _posgraduacaoService.Atualizar(posgraduacao).Result)
                {

                    TempData["MensagemSucesso"] = "Pos Graduação atualizada com sucesso";
                    return RedirectToAction("Index");

                }
                TempData["MensagemErro"] = "Não conseguimos atulizar esta Pós Graduação!";
                return View(posgraduacao);
            }
            catch (Exception erro)
            {

                TempData["MensagemErro"] = $"Ops, não foi possivel atualizar esta Pos Graduação tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }


        }

        public async Task<JsonResult> Delete(ulong id)
        {
            try
            {

                bool valido = _posgraduacaoService.Excluir(id).Result;


                return Json(new { sucesso = true, valido = valido });

            }
            catch (Exception ex)
            {

                return Json(new { sucesso = false, mensagem = ex.Message });
            }

        }      

    }
}

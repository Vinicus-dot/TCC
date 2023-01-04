using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SPCPP.Model.Filters;
using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using SPCPP.Service.Interface;
using SPCPP.Service.Services;

namespace SPCPP.Web.Controllers
{
    [PaginaParaUsuarioLogado]
    public class PosgraduacaoController : Controller
    {
        private readonly IPosgraduacaoService _posgraduacaoService;
        public PosgraduacaoController(IPosgraduacaoService posgraduacaoService)
        {
            _posgraduacaoService = posgraduacaoService;
        }
  
        public IActionResult Index()
        {
            List<Posgraduacao> posgraduacao = _posgraduacaoService.Listar();
            return View(posgraduacao);
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

                if (ModelState.IsValid &&  _posgraduacaoService.Create(posgraduacao).Result)
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

                if (ModelState.IsValid &&  _posgraduacaoService.Atualizar(posgraduacao).Result )
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

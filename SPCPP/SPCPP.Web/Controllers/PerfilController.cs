using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SPCPP.Model.Filters;
using SPCPP.Model.Helper;
using SPCPP.Model.Models;
using SPCPP.Service.Interface;

namespace SPCPP.Web.Controllers
{
    [PaginaParaUsuarioLogado]
    public class PerfilController : Controller
    {
        private readonly IPerfilService _perfilService;
        private readonly ISessao _sessao;
        public PerfilController(IPerfilService perfilService, ISessao sessao)
        {
            _perfilService = perfilService;
            _sessao = sessao;
        }

        public IActionResult Editar()
        {
            

            string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");

            User usuario = JsonConvert.DeserializeObject<User>(sessaoUsuario);

            ViewBag.Perfil = usuario.Perfil;

            return View(usuario);
        }
        [HttpPost]
        public IActionResult Editar(User usuario)
        {

            try
            {
                if (_perfilService.Atualizar(usuario).Result )
                {
                    TempData["MensagemSucesso"] = "Usúario atualizado com sucesso";
                    _sessao.RemoverSessaoDoUsuario();
                }
                return RedirectToAction("Index", "Login");
            } 
            catch(Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possivel alterar seu perfil, tente novamente, detalhe do erro: {erro.Message}";
                return View(usuario);
            }
        }


    }
}

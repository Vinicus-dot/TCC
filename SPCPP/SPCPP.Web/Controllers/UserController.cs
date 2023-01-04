using Microsoft.AspNetCore.Mvc;
using SPCPP.Model.Filters;
using SPCPP.Model.Models;
using SPCPP.Service.Interface;



namespace SPCPP.Web.Controllers
{
    [PaginaRestritaSomenteAdmin]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            List<User> usuarios = _userService.BuscarTodos();

            return View(usuarios);
        }
        public IActionResult Criar()
        {
            return View();
        }
        public IActionResult Editar(ulong id)
        {
            User usuario = _userService.PesquisarPorId(id);
            return View();
        }
        public IActionResult Deletar(ulong id)
        {
            User usuario = _userService.PesquisarPorId(id);
            return View(usuario);
        }

        public IActionResult DeletarConfirmacao(ulong id)
        {
            try
            {
                Task<bool>  deletado =  _userService.Deletar(id);    
                if (deletado.Result)
                {
                    TempData["MensagemSucesso"] = "Usuário apagado com sucesso!";
                }
                else
                {
                    TempData["MensagemSucesso"] = "Ops, não conseguimos apagar seu usuário!";
                }

                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemSucesso"] = $"Ops, não conseguimos apagar seu usuário, mais detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public IActionResult Criar(User usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Task<bool> result = _userService.Adicionar(usuario);
                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso";
                    return RedirectToAction("Index");
                }

                return View(usuario);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possivel cadastrar seu usuario, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public IActionResult Editar(UsuarioSemSenhaModal usuarioSemSenhaModel)
        {
            try
            {
                User usuario = null;

                if (ModelState.IsValid)
                {
                    usuario = new User()
                    {
                        Id = usuarioSemSenhaModel.Id,
                        Nome = usuarioSemSenhaModel.Nome,
                        Login = usuarioSemSenhaModel.Login,
                        Email = usuarioSemSenhaModel.Email,
                        Perfil = (Model.Enums.PerfilEnum)usuarioSemSenhaModel.Perfil
                        

                    };

                    Task<bool> result = _userService.Atualizar(usuario);
                    TempData["MensagemSucesso"] = "Usúario atualizado com sucesso";
                    return RedirectToAction("Index");

                }
                return View(usuario);
            }
            catch (Exception erro)
            {

                TempData["MensagemErro"] = $"Ops, não foi possivel atualizar seu usúario tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }


        }
    }
}

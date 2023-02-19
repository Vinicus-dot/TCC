using java.awt;
using Microsoft.AspNetCore.Mvc;
using SPCPP.Model.Filters;
using SPCPP.Model.Models;
using SPCPP.Service.Interface;



namespace SPCPP.Web.Controllers
{
    //[PaginaRestritaSomenteAdmin]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index(string Ordenar, string Filter, int? pagina, string? pesquisar)
        {
            try
            {
                ViewBag.OrdernarPg = Ordenar;
                ViewBag.NameParm = String.IsNullOrEmpty(Ordenar) ? "nome" : "";
                ViewBag.DateParm = Ordenar == "data" ? "data_desc" : "data";
                ViewBag.DateAttParm = Ordenar == "dataAtt" ? "dataAtt_desc" : "dataAtt";
                ViewBag.PerfilParm = Ordenar == "perfil" ? "perfil_desc" : "perfil";
                ViewBag.EmailParm = Ordenar == "email" ? "email_desc" : "email";
                ViewBag.LoginParm = Ordenar == "login" ? "login_desc" : "login";

                if (pesquisar != null) pagina = 1; else pesquisar = Filter;

                ViewBag.Filter = pesquisar;

                List<User> usuarios = _userService.BuscarTodos();

                if (!String.IsNullOrEmpty(pesquisar))
                    usuarios = usuarios.Where(s => s.Nome.Contains(pesquisar) || s.Email.Contains(pesquisar)).ToList();
                
                switch (Ordenar)
                {
                    case "nome":
                        usuarios = usuarios.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "data":
                        usuarios = usuarios.OrderBy(s => s.DataCadastro).ToList();
                        break;
                    case "data_desc":
                        usuarios = usuarios.OrderByDescending(s => s.DataCadastro).ToList();
                        break; 
                    case "dateAtt":
                        usuarios = usuarios.OrderBy(s => s.DataAtualizacao).ToList();
                        break;
                    case "dateAtt_desc":
                        usuarios = usuarios.OrderByDescending(s => s.DataAtualizacao).ToList();
                        break;
                    case "login":
                        usuarios = usuarios.OrderBy(s => s.Login).ToList();
                        break;
                    case "login_desc":
                        usuarios = usuarios.OrderByDescending(s => s.Login).ToList();
                        break;
                    case "email":
                        usuarios = usuarios.OrderBy(s => s.Email).ToList();
                        break;
                    case "email_desc":
                        usuarios = usuarios.OrderByDescending(s => s.Email).ToList();
                        break;
                    case "perfil":
                        usuarios = usuarios.OrderBy(s => s.Perfil).ToList();
                        break;
                    case "perfil_desc":
                        usuarios = usuarios.OrderByDescending(s => s.Perfil).ToList();
                        break;
                    default:
                        usuarios = usuarios.OrderBy(s => s.Nome).ToList();
                        break;
                }

                int totalpagina = Convert.ToInt32(_userService.GetParametro("NUMERO_PAGINATION"));
                return View(PaginaList<User>.Create(usuarios, pagina ?? 1, totalpagina));
            }
            catch(Exception ex)
            {
                TempData["MensagemErro"] = ex;
                return View();
            }
            
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
            try { 

                
                bool deletado = _userService.Deletar(id);
                



                return Json(new { success = deletado , message = "Usuário deletado com sucesso!" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
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

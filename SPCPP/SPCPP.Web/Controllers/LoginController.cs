
using Microsoft.AspNetCore.Mvc;
using SPCPP.Model.Helper;
using SPCPP.Model.Models;
using SPCPP.Service.Interface;


namespace SPCPP.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISessao _sessao;
        private readonly IEmail _email;
        private readonly ICookieHelper _cookieHelper;
      

        public LoginController(IUserService usuarioRepositorio, ISessao sessao, IEmail email, ICookieHelper cookieHelper)
        {
            _userService = usuarioRepositorio;
            _sessao = sessao;
            _email = email;
            _cookieHelper = cookieHelper;
        }
        public IActionResult Index()
        {
            UserCookieData userCookieData = _cookieHelper.GetUserCookie("rememberMe");

            if(userCookieData !=null  && !string.IsNullOrEmpty(userCookieData.Login) && !string.IsNullOrEmpty(userCookieData.Senha))
            {
                LoginModel loginModel = new LoginModel();
                loginModel.Login = userCookieData.Login;
                loginModel.Senha = userCookieData.Senha;
                
                return Entrar(loginModel);
            }

            if (_sessao.BuscarSessaoDoUsuario() != null)
                return RedirectToAction("Index", "Home");

            return View();
        }
        public IActionResult RedefinirSenha()
        {
            return View();
        }
        public IActionResult Sair()
        {
            _sessao.RemoverSessaoDoUsuario();

            _cookieHelper.RemoveUserCookie("rememberMe");
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public IActionResult Entrar(LoginModel loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User usuario = _userService.BuscarPorLogin(loginModel.Login);

                    if (usuario != null)
                    {
                        if (usuario.SenhaValida(loginModel.Senha))
                        {

                            _sessao.CriarSesaoDoUsuario(usuario);


                            if (loginModel.lembrar)
                            {
                                var userCookieData = new UserCookieData
                                {
                                    UserId = usuario.Id,
                                    Login = loginModel.Login,
                                    Senha = loginModel.Senha,
                                    ExpirationTime = DateTime.UtcNow.AddDays(7)
                                };

                                _cookieHelper.SetUserCookie(userCookieData, "rememberMe");
   
                            }

                            return RedirectToAction("Index", "Home");
                        }
                        TempData["MensagemErro"] = $"Senha do usúario é inválida, tente novamente.";
                    }

                    TempData["MensagemErro"] = $"Usúario e/ou senha inválido(s). Por favor, tente novamente.";
                }

                return View("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos realizar seu login, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult EnviarLinkParaRedefinirSenha(RedefinirSenhaModel redefinirSenhaModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User usuario = _userService.BuscarPorEmailElogin(redefinirSenhaModel.Email, redefinirSenhaModel.Login);

                    if (usuario != null)
                    {
                        string novaSenha = usuario.GerarNovaSenha();
                        string mensagem = $"Sua nova senha é: {novaSenha}";

                        bool emailEnviado = _email.Enviar(usuario.Email, "Sistema de Cadastro de Professores na Pos Gradução - Nova Senha", mensagem);
                        if (emailEnviado)
                        {
                            _userService.Atualizar(usuario);
                            TempData["MensagemSucesso"] = $"Enviamos para seu e-mail cadastrado uma nova senha.";
                        }else
                        {
                            TempData["MensagemErro"] = $"Não conseguimos enviar e-mail. Por Favor, tente novamente";
                        }
                        
                        return RedirectToAction("Index", "Login");

                    }

                    TempData["MensagemErro"] = $"Não conseguimos redefinir sua senha. Por Favor, verifique os dados informados.";
                }

                return View("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos redefinir sua senha, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}

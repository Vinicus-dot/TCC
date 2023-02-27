using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SPCPP.Model.Filters;
using SPCPP.Model.Models;
using SPCPP.Web.Models;
using System.Diagnostics;

namespace SPCPP.Web.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Logado = true;           
            string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");

            User usuario = new User();
            if (string.IsNullOrEmpty(sessaoUsuario))
                ViewBag.Logado = false;
            else
            usuario = JsonConvert.DeserializeObject<User>(sessaoUsuario);

            ViewBag.Perfil = usuario.Perfil;
            return View(usuario);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
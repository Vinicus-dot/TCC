using Microsoft.AspNetCore.Mvc;
using SPCPP.Model.Filters;

namespace SPCPP.Web.Controllers
{

    [PaginaParaUsuarioLogado]
    public class RestritoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}

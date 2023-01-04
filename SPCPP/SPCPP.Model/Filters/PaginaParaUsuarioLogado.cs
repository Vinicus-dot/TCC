
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using SPCPP.Model.Models;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using RedirectToRouteResult = Microsoft.AspNetCore.Mvc.RedirectToRouteResult;

namespace SPCPP.Model.Filters
{
    public class PaginaParaUsuarioLogado : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            string sessaoUsuario = context.HttpContext.Session.GetString("sessaoUsuarioLogado");
            if (string.IsNullOrEmpty(sessaoUsuario))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });

            }
            else
            {
                User usuario = JsonConvert.DeserializeObject<User>(sessaoUsuario);

                if (usuario == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });

                }
            }

            base.OnActionExecuted(context);
        }
    }
}

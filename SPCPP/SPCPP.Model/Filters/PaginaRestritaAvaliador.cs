using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SPCPP.Model.Enums;
using SPCPP.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCPP.Model.Filters
{
    public class PaginaRestritaAvaliador : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessaoUsuario = context.HttpContext.Session.GetString("sessaoUsuarioLogado");
            if (string.IsNullOrEmpty(sessaoUsuario))
            {
                context.Result = new RedirectToRouteResult(new { controller = "Login", action = "Index" });
            }
            else
            {
                var usuario = JsonConvert.DeserializeObject<User>(sessaoUsuario);
                if (usuario == null || (usuario.Perfil != PerfilEnum.Admin && usuario.Perfil != PerfilEnum.Avaliador))
                {
                    context.Result = new RedirectToRouteResult(new { controller = "Restrito", action = "Index" });
                }
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var sessaoUsuario = context.HttpContext.Session.GetString("sessaoUsuarioLogado");
            if (string.IsNullOrEmpty(sessaoUsuario))
            {
                context.Result = new RedirectToRouteResult(new { controller = "Login", action = "Index" });
            }
            else
            {
                var usuario = JsonConvert.DeserializeObject<User>(sessaoUsuario);
                if (usuario == null || (usuario.Perfil != PerfilEnum.Admin && usuario.Perfil != PerfilEnum.Avaliador))
                {
                    context.Result = new RedirectToRouteResult(new { controller = "Restrito", action = "Index" });
                }
            }

            base.OnActionExecuted(context);
        }

    }
}

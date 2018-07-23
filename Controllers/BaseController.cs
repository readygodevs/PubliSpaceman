using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlanogramaGen.Models;
using System.Web.Routing;

namespace PlanogramaGen.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest()) { return; }

            string actionName = filterContext.ActionDescriptor.ActionName.ToUpper();
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper();

            UsuarioLogin usrLogin = (UsuarioLogin)Session["usr"];
            if (usrLogin == null && (controllerName != "HOME") || (controllerName == "HOME" && actionName != "LOGIN"))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Login"
                }));
            }
            else if (usrLogin != null && (controllerName == "HOME" && actionName == "LOGIN"))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Planograma",
                    action = "Index"
                }));
            }

            ViewBag.Usuario = usrLogin;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlanogramaGen.Models;

namespace PlanogramaGen.Controllers
{
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        tb_usuarios_model modelo = new tb_usuarios_model();
        wcfLoginDominio.Login wcfDominio = new wcfLoginDominio.Login();

        public ActionResult ObtenerUsuarios()
        {
            return PartialView(modelo.obtenerUsuarios());
        }

        public ActionResult GuardarUsuario(string usuario, string tipo)
        {
            try
            {
                //var usrsDominio= wcfDominio.ObtenerTodosUsuarios().ToList().Where(a=>a.User==usuario);
                //if (usrsDominio.Count() == 0) throw new Exception("Usuario incorrecto. El usuario no existe en el dominio.");
                modelo.guardarUsuario(new Datos.tb_usuarios { Login = usuario, Perfil = tipo });
                return Json(new { Error = false, Mensaje = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Mensaje = ex.Message });
            }
        }

        public ActionResult GuardarRutas(string publicacion, string imagenes)
        {
            try
            {
                modelo.guardarRutas(publicacion, imagenes);
                return Json(new { Error = false, Mensaje = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Mensaje = ex.Message });
            }
        }
        public ActionResult EliminarUsuario(int idusr)
        {
            try
            {
                modelo.eliminarUsuario(idusr);
                return Json(new { Error = false, Mensaje = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Mensaje = ex.Message });
            }
        }

        //public ActionResult ObtenerUsuario(int id_usr)
        //{
        //    try
        //    {
        //        var usuario = modelo.obtenerUsuario(id_usr);
        //        wcfDominio.Autenticar()
        //        return Json(new { Error = false, usuario = usuario.Login, });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Error = true, Mensaje = ex.Message });
        //    }
        //}
    }
}
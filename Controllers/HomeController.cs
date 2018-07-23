using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlanogramaGen.Models;
using System.Web.Hosting;
using System.Drawing;

namespace PlanogramaGen.Controllers
{
    public class HomeController : BaseController
    {

        tb_usuarios_model usuarioModel = new tb_usuarios_model();

        public ActionResult Login()
        {
            
                //HtmlToPdf conv = new HtmlToPdf();
                //conv.BasePath = HostingEnvironment.MapPath("~/");
                //conv.PageInfo.PageFormat = ePageFormat.A4;
                //conv.PageInfo.PageOrientation = ePageOrientation.Portrait;
                //conv.OpenHTML("<p> hello world </p><img src='" + @"\\192.168.100.22\Datos\Kiosko\PLANOGRAMAS\RutasPublicadorPlanogramas\imagenes\ABARROTES\904121.1" + "' />");
                //conv.SavePDF(HostingEnvironment.MapPath(@"~/test4.pdf"));

                //var myColor = Color.FromArgb(255-(-256));


                //string color_code = "-256";
                //byte r = (byte)((Int16.Parse(color_code) >> 0) & 255);
                //byte g = (byte)((Int16.Parse(color_code) >> 8) & 255);
                //byte b = (byte)((Int16.Parse(color_code) >> 16) & 255);


                //string color= "RGB(" + r + "," + g + "," + b + ")";



                //            using (NetworkShareAccesser.Access("192.168.100.22", "GOCSA", "devkiosko", "Super.2017xx"))
                //          {

                //    //Byte[] res = null;
                //    using (MemoryStream ms = new MemoryStream())
                //    {
                //        var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(
                //            "<p> hello world </p><img src='" + @"\\192.168.100.22\Datos\Kiosko\PLANOGRAMAS\RutasPublicadorPlanogramas\imagenes\ABARROTES\904121.1" + "' />"
                //            , PdfSharp.PageSize.A4);
                //        pdf.Save(HostingEnvironment.MapPath(@"~/test3.pdf"));
                //        //res = ms.ToArray();
                //    }

                //    new IronPdf.HtmlToPdf().RenderHtmlAsPdf(
                //"<p> hello world </p><img src='" + @"file:\\192.168.100.22\Datos\Kiosko\PLANOGRAMAS\RutasPublicadorPlanogramas\imagenes\ABARROTES\904121.1" + "' />")
                //.SaveAs(HostingEnvironment.MapPath(@"~/test.pdf"));

                //new IronPdf.HtmlToPdf().RenderHtmlAsPdf(
                //"<p> hello world </p><img src='" + @"C:\Users\JOSE ANTONIO\Desktop\imagenes_planograma\7501040005855.4" + "' />")
                //.SaveAs(HostingEnvironment.MapPath(@"~/test2.pdf"));

                //new NReco.PdfGenerator.HtmlToPdfConverter().GeneratePdf("<p> hello world </p><img src='" + @"\\192.168.100.22\Datos\Kiosko\PLANOGRAMAS\RutasPublicadorPlanogramas\imagenes\ABARROTES\904121.1" + "' />",
                //    "", HostingEnvironment.MapPath(@"~/test2.pdf"));

                //        }



                return View();
        }
        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session["usr"] = null;
            return RedirectToAction("Login");
        }
        public ActionResult Index()
        {
            //try
            //{
            //using (NetworkShareAccesser.Access("192.168.100.22", "GOCSA", "devkiosko", "Super.2017xx"))
            //{
            //    System.Drawing.Image img_stream =
            //            System.Drawing.Image.FromFile(@"\\192.168.100.22\Datos\Kiosko\PLANOGRAMAS\imagenes_planogramas\904121.1");
            //}

            //}
            //catch (Exception e)
            //{

            //}

            return View();
        }

        public ActionResult Navegador_Usrs()
        {
            return PartialView(usuarioModel.obtenerUsuarios());
        }

        [HttpPost]
        public ActionResult ValidarUsr(string user, string pass)
        {
            try
            {
                UsuarioLogin usuarioSesion = new UsuarioLogin
                {
                    Usuario = user,
                    Nombre = "devkiosko"
                };
                using (wcfLoginDominio.Login wcfLogin = new wcfLoginDominio.Login())
                {
                    wcfLoginDominio.Usuario usrDominio = wcfLogin.Autenticar(user, pass);
                    usuarioSesion.Nombre = usrDominio.Nombre;

                    if (!usrDominio.EsValido) throw new Exception("El usuario de dominio y/o contraseña son incorrectos.");
                }
                string perfil = usuarioModel.validarUsuario(user);
                if (string.IsNullOrEmpty(perfil))
                {
                    throw new Exception("El usuario no tiene acceso al sistema. Consulte al administrador.");
                }
                usuarioSesion.Perfil = perfil;
                Session["usr"] = usuarioSesion;
                return Json(new { Error = false, Mensaje = "" });
            }
            catch (Exception ex)
            {
                Session["usr"] = null;
                return Json(new { Error = true, Mensaje = ex.Message });
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlanogramaGen.Datos;
using PlanogramaGen.Models;
using System.Web.Hosting;
using IronPdf;

namespace PlanogramaGen.Controllers
{
    public class PlanogramaController : BaseController
    {
        planogramas_model model = new planogramas_model();
        tb_usuarios_model model_usr = new tb_usuarios_model();

        // GET: Planograma
        public ActionResult Index()
        {
            List<sp_obtener_categorias_Result> list_categorias = new List<sp_obtener_categorias_Result>(); //{ "(Seleccionar todo)" };
            try
            {
                list_categorias.AddRange(model.ObtenerCategorias());
                ViewBag.Categorias = list_categorias;
                var usr = Session["usr"] as UsuarioLogin;
                if (usr.Perfil == "Admin")
                {
                    ReglaNegocioModel reglaModel = new ReglaNegocioModel();
                    ViewBag.RutaPub = reglaModel.obtenerRegla("URLExporta");
                    ViewBag.RutaImg = reglaModel.obtenerRegla("URLTomaImg");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            //ViewBag.Usuario = usr;
            return View();
        }
        public ActionResult CerrarSesion()
        {
            Session["usr"] = null;
            return RedirectToAction("Login", "Home");
        }

        [ValidateInput(false)]
        public ActionResult ArbolZonificacion()
        {
            return PartialView("_ArbolZonificacion", model.ObtenerZonificacion());
        }

        public ActionResult VisualizarPlanograma(string planograma)
        {
            PlanogramaGenEntities ctx = new PlanogramaGenEntities();
            //obtener usuario de dominio
            var objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRRED");
            if (objREN == null) throw new Exception("No existe la regla de negocio [USRRED]. Avise al área de sistemas.");
            string usr_dominio = objREN.Regla;

            //obtener pass usuario de dominio
            objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRPASS");
            if (objREN == null) throw new Exception("No existe la regla de negocio [USRPASS]. Avise al área de sistemas.");
            string pass_usr_dominio = objREN.Regla;

            objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "URLExporta");
            if (objREN == null) throw new Exception("No existe la regla de negocio [URLExporta]. Avise al área de sistemas.");
            string urlExporta = objREN.Regla;

            using (NetworkShareAccesser.Access("192.168.100.22", "GOCSA", usr_dominio, pass_usr_dominio))
            {
                return File(urlExporta + @"\" + planograma, "application/pdf");
            }
        }

        [HttpPost]
        public ActionResult BuscarPlanograma(string tiendas, string categorias)
        {
            try
            {
                PlanogramaEncabezado encabezado = model.ObtenerPlanograma(tiendas, categorias);
                PlanogramaGenEntities ctx = new PlanogramaGenEntities();
                //obtener usuario de dominio
                var objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRRED");
                if (objREN == null) throw new Exception("No existe la regla de negocio [USRRED]. Avise al área de sistemas.");
                string usr_dominio = objREN.Regla;

                //obtener pass usuario de dominio
                objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRPASS");
                if (objREN == null) throw new Exception("No existe la regla de negocio [USRPASS]. Avise al área de sistemas.");
                string pass_usr_dominio = objREN.Regla;

                objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "URLExporta");
                if (objREN == null) throw new Exception("No existe la regla de negocio [URLExporta]. Avise al área de sistemas.");
                string urlExporta = objREN.Regla;

                using (NetworkShareAccesser.Access("192.168.100.22", "GOCSA", usr_dominio, pass_usr_dominio))
                {
                    if (!System.IO.File.Exists(urlExporta + @"\" + encabezado.PLANOGRAM + ".pdf"))
                    {
                        throw new Exception("Archivo inexistente");
                    }
                }
                return Json(new { Error = false, Mensaje = String.Format("Planograma {0} - {1}", encabezado.CATEGORIA, encabezado.PLAZA), Planograma = encabezado.PLANOGRAM + ".pdf" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Mensaje = string.Format("No existe planograma {0} - {1}", categorias, tiendas) });
            }
        }

        [HttpPost]
        public ActionResult ActualizarPlanogramas(List<string> tiendas, List<string> categorias)
        {
            string tienda = "";
            string categoria = "";
            try
            {
                foreach (string item_tda in tiendas)
                {
                    foreach (string item_cat in categorias)
                    {
                        List<sp_obtener_planograma_Result> productos = model.ObtenerProductosPlanograma(item_tda, item_cat);

                        if (productos.Count == 0)
                        {
                            throw new Exception("No existe información para generar el planograma " + categorias[0] + " Tienda: " + tiendas[0]);
                        }

                        List<sp_obtener_segmentos_labels_Result> segmentos = model.ObtenerSegmentos(productos.First().PLN_ID);

                        tienda = productos.First().PLAZA;
                        categoria = productos.First().CATEGORIA;

                        bool landscape = false;
                        string html = "";
                        GeneradorPlanograma generadorHtml = new GeneradorPlanograma();


                        html = generadorHtml.Generar_2(productos, segmentos, ref landscape);


                        var pdfNReco = new NReco.PdfGenerator.HtmlToPdfConverter();
                        pdfNReco.Size = NReco.PdfGenerator.PageSize.A4;
                        pdfNReco.Orientation = NReco.PdfGenerator.PageOrientation.Portrait;
                        if (landscape)
                        {
                            pdfNReco.Orientation = NReco.PdfGenerator.PageOrientation.Landscape;
                        }
                        pdfNReco.LowQuality = true;
                        DateTime fechaActual = DateTime.Now;
                        string fecha = fechaActual.ToString("dddd") + ", " + fechaActual.ToString("dd") + " de " + fechaActual.ToString("MMMM") + " de " + fechaActual.ToString("yyyy");

                        var itemProducto = productos.First();
                        pdfNReco.PageHeaderHtml = "<div>Page: <span class='page'></span></div>";
                        pdfNReco.PageFooterHtml = "<div>" +
                            "<span style='color:red;font:20px arial'>" + fecha + "</span><br/>" +
                            "<span style='color:blue;font:20px arial'>" + itemProducto.CATEGORIA +
                            ";tienda " + itemProducto.PLAZA + ";" + itemProducto.PLANOGRAM + "</span><br/>" +
                            "</div>";

                        //var ironPDF = new IronPdf.HtmlToPdf();
                        //ironPDF.PrintOptions.DPI = 300;
                        //ironPDF.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;// System.Drawing.PaperKind.A4;
                        //ironPDF.PrintOptions.EnableJavaScript = true;
                        //if (landscape) ironPDF.PrintOptions.PaperOrientation = PdfPrintOptions.PdfPaperOrientation.Landscape;
                        ////ironPDF.PrintOptions.AllowScreenCss = false;
                        ////ironPDF.PrintOptions.Header = "{page} of {total-pages}";
                        ////ironPDF.PrintOptions.GrayScale = true;

                        //obtener ruta de publicacion
                        Datos.PlanogramaGenEntities ctx = new PlanogramaGenEntities();
                        var objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "URLExporta");
                        if (objREN == null) throw new Exception("No existe la regla de negocio [URLExporta]. Avise al área de sistemas.");
                        string urlExporta = objREN.Regla;

                        //obtener usuario de dominio
                        objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRRED");
                        if (objREN == null) throw new Exception("No existe la regla de negocio [USRRED]. Avise al área de sistemas.");
                        string usr_dominio = objREN.Regla;

                        //obtener pass usuario de dominio
                        objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRPASS");
                        if (objREN == null) throw new Exception("No existe la regla de negocio [USRPASS]. Avise al área de sistemas.");
                        string pass_usr_dominio = objREN.Regla;


                        using (NetworkShareAccesser.Access("192.168.100.22", "GOCSA", usr_dominio, pass_usr_dominio))
                        {
                            //urlExporta = HostingEnvironment.MapPath(@"~\planogramas");
                            
                            pdfNReco.GeneratePdf(html, "", urlExporta + @"\" + productos[0].PLANOGRAM + ".pdf");
                            pdfNReco = null;
                            //ironPDF.RenderHtmlAsPdf(html).SaveAs(urlExporta + @"\" + productos[0].PLANOGRAM + ".pdf");
                        }

                        try
                        {
                            var usr = (UsuarioLogin)Session["usr"];
                            model.Guardar(usr.Usuario, productos, segmentos);
                        }
                        catch (Exception ex)
                        {
                            using (NetworkShareAccesser.Access("192.168.100.22", "GOCSA", usr_dominio, pass_usr_dominio))
                            {

                                System.IO.File.Delete(urlExporta + @"\" + productos[0].PLANOGRAM + ".pdf");
                            }


                            //escribir error de BD en log
                            ReadyGoUtility.Utilities.EscribirErrorLog(categorias[0] + " Tienda: " + tiendas[0] + " PLN_ID:" + productos.First().PLN_ID + ". " + ex.InnerException.InnerException.Message);
                            //if (innerException.Number == numero_que_quieras)
                            //{
                            //    // Tratar la excepción como quieras
                            //}


                            throw new Exception("Error al guardar en BD el planograma " + categorias[0] + " Tienda: " + tiendas[0] + " PLN_ID:" + productos.First().PLN_ID);
                        }
                    }
                }
                return Json(new { Error = false, Mensaje = "" });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Cannot generate PDF: Exit with code 1, due to unknown error. (exit code: 1)")
                {//el planograma está abierto
                    return Json(new
                    {
                        Error = true,
                        Mensaje =
                        string.Format("El planograma {0} Tienda {1} está siendo utilizado por otro proceso, " +
                        "verifique que se encuentre cerrado o vuelva a intentar más tarde.", categoria, tienda)
                    });
                }
                else if(ex.Message.ToLower().Contains("no existe esta conexión de red"))
                {
                    return Json(new { Error = true, Mensaje = string.Format("Planograma {0} Tienda {1}. Verifique su conexión de red, existe intermitencia.", categoria, tienda) });
                }
                else if (ex.Message.ToLower().Contains("referencia a objeto no establecida"))
                {
                    return Json(new { Error = true, Mensaje = string.Format("Planograma {0} Tienda {1}. Actualice su navegador y vuelva a ingresar. Planograma no generado.", categoria, tienda) });
                }
                else
                {
                    return Json(new { Error = true, Mensaje = string.Format("Planograma {0} Tienda {1}. " + ex.Message, categoria, tienda) });
                }
            }
        }

        [HttpPost]
        public ActionResult ActualizarPlanogramas2(List<string> tiendas, List<string> categorias)
        {
            string tienda = "";
            string categoria = "";
            string error = "";
            try
            {
                foreach (string item_tda in tiendas)
                {
                    foreach (string item_cat in categorias)
                    {
                        List<sp_obtener_planograma_Result> productos = model.ObtenerProductosPlanograma(item_tda, item_cat);

                        if (productos.Count == 0)
                        {
                            error += "\r\n\r\n" + "No existe información para generar el planograma " + categorias[0] + " Tienda: " + tiendas[0];
                            continue;
                        }

                        List<sp_obtener_segmentos_labels_Result> segmentos = model.ObtenerSegmentos(productos.First().PLN_ID);

                        tienda = productos.First().PLAZA;
                        categoria = productos.First().CATEGORIA;

                        bool landscape = false;
                        GeneradorPlanograma generadorHtml = new GeneradorPlanograma();
                        string html = generadorHtml.Generar_2(productos, segmentos, ref landscape);



                        var pdfNReco = new NReco.PdfGenerator.HtmlToPdfConverter();

                        pdfNReco.Size = NReco.PdfGenerator.PageSize.A4;

                        pdfNReco.Orientation = NReco.PdfGenerator.PageOrientation.Portrait;
                        if (landscape)
                        {
                            pdfNReco.Orientation = NReco.PdfGenerator.PageOrientation.Landscape;
                        }
                        pdfNReco.LowQuality = true;

                        //var ironPDF = new IronPdf.HtmlToPdf();
                        //ironPDF.PrintOptions.DPI = 300;
                        //ironPDF.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;// System.Drawing.PaperKind.A4;
                        //ironPDF.PrintOptions.EnableJavaScript = true;
                        //ironPDF.PrintOptions.JpegQuality = 50;
                        //if (landscape) ironPDF.PrintOptions.PaperOrientation = PdfPrintOptions.PdfPaperOrientation.Landscape;
                        ////ironPDF.PrintOptions.AllowScreenCss = false;
                        ////ironPDF.PrintOptions.Header = "{page} of {total-pages}";
                        ////ironPDF.PrintOptions.GrayScale = true;



                        DateTime fechaActual = DateTime.Now;
                        string fecha = fechaActual.ToString("dddd") + ", " + fechaActual.ToString("dd") + " de " + fechaActual.ToString("MMMM") + " de " + fechaActual.ToString("yyyy");

                        var itemProducto = productos.First();
                        //pdfNReco.PageFooterHtml = "<div>" +
                        //    "<span style='color:red;font:20px arial'>" + fecha + "</span><br/>" +
                        //    "<span style='color:blue;font:20px arial'>" + itemProducto.CATEGORIA +
                        //    ";tienda " + itemProducto.PLAZA + ";" + itemProducto.PLANOGRAM + "</span><br/>" +
                        //    "</div>";


                        //obtener ruta de publicacion
                        Datos.PlanogramaGenEntities ctx = new PlanogramaGenEntities();
                        var objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "URLExporta");
                        if (objREN == null) error += "\r\n\r\n" + "No existe la regla de negocio [URLExporta]. Avise al área de sistemas.";
                        string urlExporta = objREN.Regla;

                        //obtener usuario de dominio
                        objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRRED");
                        if (objREN == null) error += "\r\n\r\n" + "No existe la regla de negocio [USRRED]. Avise al área de sistemas.";
                        string usr_dominio = objREN.Regla;

                        //obtener pass usuario de dominio
                        objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRPASS");
                        if (objREN == null) error += "\r\n\r\n" + "No existe la regla de negocio [USRPASS]. Avise al área de sistemas.";
                        string pass_usr_dominio = objREN.Regla;



                        //var footer = new SimpleHeaderFooter();
                        //footer.CenterText = "<div>" +
                        //    "<span style='color:red;font:20px arial'>" + fecha + "</span><br/>" +
                        //    "<span style='color:blue;font:20px arial'>" + itemProducto.CATEGORIA +
                        //    ";TIENDA " + itemProducto.PLAZA + ";" + itemProducto.PLANOGRAM + "</span><br/>" +
                        //    "</div>";
                        //ironPDF.PrintOptions.Footer = footer;

                        using (NetworkShareAccesser.Access("192.168.100.22", "GOCSA", usr_dominio, pass_usr_dominio))
                        {
                            //urlExporta = HostingEnvironment.MapPath(@"~\planogramas");
                            pdfNReco.GeneratePdf(html, "", urlExporta + @"\" + productos[0].PLANOGRAM + ".pdf");
                            //ironPDF.RenderHtmlAsPdf(html).SaveAs(urlExporta + @"\" + productos[0].PLANOGRAM + "_sharp.pdf");
                        }

                        try
                        {
                            //model.Guardar(productos, segmentos);
                        }
                        catch (Exception ex)
                        {
                            using (NetworkShareAccesser.Access("192.168.100.22", "GOCSA", usr_dominio, pass_usr_dominio))
                            {
                                System.IO.File.Delete(urlExporta + @"\" + productos[0].PLANOGRAM + ".pdf");
                            }


                            //escribir error de BD en log
                            ReadyGoUtility.Utilities.EscribirErrorLog(categorias[0] + " Tienda: " + tiendas[0] + " PLN_ID:" + productos.First().PLN_ID + ". " + ex.InnerException.InnerException.Message);
                            //if (innerException.Number == numero_que_quieras)
                            //{
                            //    // Tratar la excepción como quieras
                            //}


                            error += "\r\n\r\n" + "Error al guardar en BD el planograma " + categorias[0] + " Tienda: " + tiendas[0] + " PLN_ID:" + productos.First().PLN_ID;
                            continue;
                        }
                    }
                }
                return Json(new { Error = false, Mensaje = "" });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Cannot generate PDF: Exit with code 1, due to unknown error. (exit code: 1)")
                {//el planograma está abierto
                    return Json(new
                    {
                        Error = true,
                        Mensaje =
                        string.Format("El planograma {0} Tienda {1} está siendo utilizado por otro proceso, " +
                        "verifique que se encuentre cerrado o vuelva a intentar más tarde.", categoria, tienda)
                    });
                }
                else
                {
                    return Json(new { Error = true, Mensaje = ex.Message });
                }
            }
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartialUsr()
        {
            return PartialView("_GridViewPartialUsr", model_usr.obtenerUsuarios());
        }

    }
}
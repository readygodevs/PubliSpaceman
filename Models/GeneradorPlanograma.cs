using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanogramaGen.Datos;
using System.Web.Hosting;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace PlanogramaGen.Models
{
    public class GeneradorPlanograma //:IDisposable
    {
        public double constMultiplicadorDescr = 1.2;
        public double widthMaxPanelPortrait = 1200; //maxima anchura para panel de mueble en pixeles

        //colocar encabezado de documento html
        public string headers_const = "<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" +
                "<meta charset='utf-8' /><meta name='viewport' content='width=device-width, initial-scale=1.0' />" +
                "<link href='" + HostingEnvironment.MapPath(@"~/Mentor/css/tablas_planogramas.css") + "' rel='stylesheet' />" +
                "<script src='" + HostingEnvironment.MapPath(@"~/Mentor/js/jquery.min.js") + "'></script>" +
                "<script src='" + HostingEnvironment.MapPath(@"~/Mentor/js/tablas_planogramas.js") + "'></script>" +
                "<script src='" + HostingEnvironment.MapPath(@"~/Mentor/js/jquery.textfill.min.js") + "'></script>" +
                "<script type='text/javascript'>jQuery(document).ready(function ($) { });</script>" +
                "</head><body>";

        Datos.PlanogramaGenEntities ctx = new PlanogramaGenEntities();

        public string Generar_2(List<sp_obtener_planograma_Result> productos, List<sp_obtener_segmentos_labels_Result> segmentos_labels, ref bool landscape)
        {


            //obtener ruta de imagenes
            var objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "URLTomaImg");
            if (objREN == null) throw new Exception("No existe la regla de negocio [URLTomaImg]. Avise al área de sistemas.");
            string urlTomaIMG = objREN.Regla;

            //obtener usuario de dominio
            objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRRED");
            if (objREN == null) throw new Exception("No existe la regla de negocio [USRRED]. Avise al área de sistemas.");
            string usr_dominio = objREN.Regla;

            //obtener pass usuario de dominio
            objREN = ctx.ReglasNegocio.FirstOrDefault(a => a.RENClave == "USRPASS");
            if (objREN == null) throw new Exception("No existe la regla de negocio [USRPASS]. Avise al área de sistemas.");
            string pass_usr_dominio = objREN.Regla;

            string planograma_html = headers_const;

            using (NetworkShareAccesser.Access("192.168.100.22", "GOCSA", usr_dominio, pass_usr_dominio))
            {
                foreach (int segmento in productos.Select(a => a.SEGMENT).OrderBy(a => a).Distinct())
                {
                    //obtener medidas de mueble
                    var productos_segmento = productos.Where(a => a.SEGMENT == segmento).ToList();

                    var datos_segmento = segmentos_labels.FirstOrDefault(a => a.SEGMENT == segmento &&
                                              a.TYPE == 8);

                    double restar_margen_derecho = 0;

                    if (segmento > 1)
                    {
                        restar_margen_derecho = productos_segmento.OrderBy(a => a.X_CENEFA).ToList()
                            [0].X_CENEFA ?? 0;
                    }

                    //obtener margen derecho
                    double margen_derecho = (-1 * productos_segmento.Where(a => a.X_CENEFA <= 0).Min(a => a.X_CENEFA)) ?? 0;
                    //margen_derecho = string.IsNullOrEmpty(margen_derecho) ? "0" : margen_derecho;

                    double? width_segmento = productos_segmento.OrderByDescending(a => a.SEGMENT_WIDTH_PIXEL).First()
                                            .SEGMENT_WIDTH_PIXEL;//.ToString().Replace(",", ".");

                    double? height_segmento = datos_segmento.HEIGHT_PIXEL;// (productos_segmento.Max(a => a.SEGMENT_HEIGHT_PIXEL + a.Y_CENEFA + a.Y_PROD).Value);//.ToString().Replace(",", ".");
                                                                          //if (datos_segmento.HEIGHT_PIXEL > height_segmento)
                                                                          //{
                                                                          //    height_segmento = datos_segmento.HEIGHT_PIXEL; // (segmentos_labels.FirstOrDefault(a => a.SEGMENT == segmento)                                                                                                                                          //??

                    //}
                    double? max_height = productos_segmento.Where(a => a.Y_CENEFA == productos_segmento.Max(b => b.Y_CENEFA)).Max(a => a.PRODUCT_HEIGHT_PIXEL + a.Y_CENEFA);
                    if (height_segmento < max_height) height_segmento = max_height + 50;


                    string class_pagina = "pagina";

                    double? divisor = 1;
                    double? multiplicador = constMultiplicadorDescr;


                    if (width_segmento * multiplicador > 800 && segmento == 1)//if (width_segmento >= 600 || productos_segmento.Max(a => a.X_PROD) >= 600)
                    {
                        class_pagina = "pagina_h";
                        landscape = true;
                    }

                    if (class_pagina == "pagina")
                    {
                        while (height_segmento * divisor > 1700)
                        {
                            divisor = divisor - 0.05;
                        }

                        var y_max = productos_segmento.Where(a => a.Y_CENEFA == productos_segmento.Max(b => b.Y_CENEFA)).Max(a => a.PRODUCT_HEIGHT_PIXEL + a.Y_CENEFA);
                        while (y_max * divisor > 1700)
                        {
                            divisor = divisor - 0.05;
                        }

                        y_max = segmentos_labels.Where(a => a.TYPE == 0 && a.SEGMENT == segmento).Max(a => a.YPIX);
                        while (y_max * divisor > 1700)
                        {
                            divisor = divisor - 0.05;
                        }

                        y_max = segmentos_labels.Where(a => a.TYPE == 13 && a.SEGMENT == segmento).Max(a => a.YPIX);
                        while (y_max * divisor > 1700)
                        {
                            divisor = divisor - 0.05;
                        }

                        while (height_segmento * multiplicador > 1700)
                        {
                            multiplicador = multiplicador - 0.1;
                        }

                        y_max = segmentos_labels.Where(a => a.TYPE == 0 && a.SEGMENT == segmento).Max(a => a.YPIX);
                        while (y_max * multiplicador > 1700)
                        {
                            multiplicador = multiplicador - 0.1;
                        }

                        y_max = segmentos_labels.Where(a => a.TYPE == 13 && a.SEGMENT == segmento).Max(a => a.YPIX);
                        while (y_max * multiplicador > 1700)
                        {
                            multiplicador = multiplicador - 0.1;
                        }
                        //si el ancho del segmento es mayor a 600px se dejarán en otra hoja la descripción
                        //de productos
                        //___________________________________________--
                        if (width_segmento >= 600)
                        {
                            while (width_segmento * divisor > 1050)
                            {
                                divisor = divisor - 0.05;
                            }
                            while (width_segmento * multiplicador > 1050)
                            {
                                multiplicador = multiplicador - 0.1;
                            }
                        }
                        else
                        {
                            //while (width_segmento * divisor > 500)
                            //{
                            //    divisor = divisor - 0.05;
                            //}
                            //while (width_segmento * multiplicador > 700)
                            while (((width_segmento * divisor) + (width_segmento * multiplicador))
                                >=
                                widthMaxPanelPortrait)
                            {
                                multiplicador = multiplicador - 0.05;
                            }
                        }
                        //____________________________________________--
                    }
                    else
                    {
                        while (height_segmento * divisor > 1100)
                        {
                            divisor = divisor - 0.05;
                        }

                        var y_max = productos_segmento.Max(a => a.Y_PROD);
                        while (y_max * divisor > 1100)
                        {
                            divisor = divisor - 0.05;
                        }

                        y_max = segmentos_labels.Where(a => a.TYPE == 0 && a.SEGMENT == segmento).Max(a => a.YPIX);
                        while (y_max * divisor > 1100)
                        {
                            divisor = divisor - 0.05;
                        }

                        y_max = segmentos_labels.Where(a => a.TYPE == 13 && a.SEGMENT == segmento).Max(a => a.YPIX);
                        while (y_max * divisor > 1100)
                        {
                            divisor = divisor - 0.05;
                        }

                        while (height_segmento * multiplicador > 1100)
                        {
                            multiplicador = multiplicador - 0.05;
                        }

                        y_max = segmentos_labels.Where(a => a.TYPE == 0 && a.SEGMENT == segmento).Max(a => a.YPIX);
                        while (y_max * multiplicador > 1100)
                        {
                            multiplicador = multiplicador - 0.05;
                        }

                        y_max = segmentos_labels.Where(a => a.TYPE == 13 && a.SEGMENT == segmento).Max(a => a.YPIX);
                        while (y_max * multiplicador > 1100)
                        {
                            multiplicador = multiplicador - 0.05;
                        }

                        while (width_segmento * divisor > 1400)
                        {
                            divisor = divisor - 0.05;
                        }

                        while (width_segmento * multiplicador > 1400)
                        {
                            multiplicador = multiplicador - 0.05;
                        }
                    }

                    planograma_html += "<div class='" + class_pagina + "'>" +
                        "<div class='panel' style='" +
                        "width:" + (width_segmento * divisor).ToString().Replace(",", ".") + "px;" +
                        "background-color:" + retornar_color(datos_segmento.COLOUR.ToString()) + ";" +
                        "left:" + (width_segmento <= 400 ? "100" : ((margen_derecho + (datos_segmento.XPIX) - restar_margen_derecho) * divisor).ToString().Replace(",", ".")) + "px;" +
                        //"bottom:" + ((1650 - (productos_segmento.Max(a => a.Y_PROD + a.PRODUCT_HEIGHT_PIXEL))) / 2).ToString().Replace(",", ".") + "px;" +
                        "height:" + (height_segmento * divisor).ToString().Replace(",", ".") + "px;'>";

                    string div_descripciones = "<div class='panel' style='" +
                        (class_pagina == "pagina" ?
                        (((width_segmento * divisor) + (width_segmento * multiplicador) >= widthMaxPanelPortrait) ?
                        "left:" + ((margen_derecho + (datos_segmento.XPIX) - restar_margen_derecho) * multiplicador).ToString().Replace(",", ".") + "px;"
                        :
                        "right:" + (width_segmento <= 400 ? "100" : ((margen_derecho + (datos_segmento.XPIX) - restar_margen_derecho) * multiplicador).ToString().Replace(",", ".")) + "px;"
                        //"margin-left:" + (
                        //productos_segmento.Max(a => a.X_CENEFA + a.SEGMENT_WIDTH_PIXEL + 50) -
                        //restar_margen_derecho).ToString().Replace(",", ".") + "px;"
                        )
                        : "left:" + ((margen_derecho + (datos_segmento.XPIX) - restar_margen_derecho) * multiplicador).ToString().Replace(",", ".") + "px;")
                        +
                        // +
                        "width:" + (width_segmento * multiplicador).ToString().Replace(",", ".") + "px;" +
                        "background-color:" + retornar_color(datos_segmento.COLOUR.ToString()) + ";" +
                        //"bottom:" + (((1650 - (productos_segmento.Max(a => a.Y_PROD + a.PRODUCT_HEIGHT_PIXEL))) * multiplicador) / 2).ToString().Replace(",", ".") + "px;" +
                        "height:" + (height_segmento * multiplicador).ToString().Replace(",", ".") + "px;'>";

                    short consecutivo = 0;

                    //colocar los productos en su posicion
                    foreach (var item_planograma in productos_segmento)
                    {
                        //leer dimensiones de imagen
                        //System.Drawing.Image img_stream = null;
                        string[] pathImage = new string[] { };

                        //System.Drawing.Image img_stream =
                        //        System.Drawing.Image.FromFile(@"\\192.168.100.22\Datos\Kiosko\PLANOGRAMAS\imagenes_planogramas\904121.1");

                        try
                        {
                            pathImage = Directory.GetFiles(urlTomaIMG,//Directory.GetFiles(HostingEnvironment.MapPath(@"~\imagenes_planograma\"),
                            item_planograma.PRODUCT_ID +
                            (item_planograma.MERCH_STYLE == 2 ? "c" : "") +
                            "." +
                            (
                            new short?[] { 8, 20, 11 }.Contains(item_planograma.ORIENTATION) ? "2" :
                            (new short?[] { 4, 16, 9 }.Contains(item_planograma.ORIENTATION) ? "9" : "1")
                            ), SearchOption.AllDirectories);
                            //img_stream = System.Drawing.Image.FromFile(pathImage[0]);

                            if (pathImage.Length == 0)
                            {
                                //img_stream = System.Drawing.Image.FromFile(urlTomaIMG + @"\" + "box.1");
                                if (item_planograma.ORIENTATION == 4)
                                {
                                    pathImage = Directory.GetFiles(urlTomaIMG,//Directory.GetFiles(HostingEnvironment.MapPath(@"~\imagenes_planograma\"),
                            item_planograma.PRODUCT_ID +
                            (item_planograma.MERCH_STYLE == 2 ? "c" : "") +
                            ".3", SearchOption.AllDirectories);

                                    if (pathImage.Length == 0)
                                    {
                                        pathImage = Directory.GetFiles(urlTomaIMG,//Directory.GetFiles(HostingEnvironment.MapPath(@"~\imagenes_planograma\"),
                            item_planograma.PRODUCT_ID +
                            (item_planograma.MERCH_STYLE == 2 ? "c" : "") +
                            ".2", SearchOption.AllDirectories);

                                        if (pathImage.Length == 0)
                                        {

                                            pathImage = new string[] { urlTomaIMG + @"\" + "box.1" };
                                        }
                                    }
                                }
                                else
                                {
                                    pathImage = new string[] { urlTomaIMG + @"\" + "box.1" };
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            pathImage = new string[] { urlTomaIMG + @"\" + "box.1" };
                        }
                        //OBTENER MARGEN DERECHO
                        //*********************************************
                        double? left_producto = 0;
                        if (item_planograma.TOTAL_FACINGS > 1)
                        {

                            double? siguiente_punto = item_planograma.SEGMENT_WIDTH_PIXEL; //si el producto no tiene otro producto a la derecha se toma el ancho del segmento

                            var nextProduct = productos_segmento.FirstOrDefault(a => a.Y_CENEFA == item_planograma.Y_CENEFA &&
                            a.FIXEL_ID == item_planograma.FIXEL_ID &&
                                //a.Y_PROD == item_planograma.Y_PROD &&
                                a.X_PROD > item_planograma.X_PROD);

                            if (nextProduct != null)
                            {
                                siguiente_punto = nextProduct.X_PROD;
                            }

                            double? diferencia = siguiente_punto -
                                (((item_planograma.TOTAL_FACINGS * item_planograma.PRODUCT_WIDTH_PIXEL)) + item_planograma.X_PROD);
                            if (item_planograma.SPREAD_PRODUCTS == 4) //
                            {
                                if (diferencia > 0)
                                {
                                    left_producto = (diferencia / (item_planograma.TOTAL_FACINGS)); //- 1));
                                                                                                    //if (siguiente_punto != item_planograma.SEGMENT_WIDTH_PIXEL) 
                                                                                                    //left_producto = left_producto / 2;
                                                                                                    //left_producto -= 2;
                                }
                            }
                        }

                        //*********************************************

                        //agregar item
                        for (short indexH = 0; indexH < item_planograma.TOTAL_FACINGS; indexH++)
                        {
                            for (short indexV = 0; indexV < item_planograma.VERTICAL; indexV++)
                            {
                                double? width_img = item_planograma.PRODUCT_WIDTH_PIXEL;
                                double? height_img = item_planograma.PRODUCT_HEIGHT_PIXEL;

                                //AJUSTAR ANCHO DEL PRODUCTO A ACOMODO DE PLANOGRAMA
                                //*********************************************
                                var nextProductX = productos_segmento.FirstOrDefault(a => a.Y_CENEFA == item_planograma.Y_CENEFA &&
                                a.Y_PROD == item_planograma.Y_PROD &&
                                a.X_PROD > item_planograma.X_PROD);
                                if (nextProductX != null)
                                {
                                    if (item_planograma.X_PROD + width_img > nextProductX.X_PROD)
                                    {
                                        double? diferencia = nextProductX.X_PROD - item_planograma.X_PROD;
                                        if (width_img - diferencia >= 50) goto ignorarWidth;
                                        width_img = diferencia;
                                    }
                                }
                                //*********************************************

                                ignorarWidth:

                                //AJUSTAR ALTURA DEL PRODUCTO A ACOMODO DE PLANOGRAMA
                                //*********************************************
                                var nextProductY = productos_segmento.Where(a => a.Y_CENEFA > item_planograma.Y_CENEFA && a.FIXEL_TYPE == 0 &&
                                (((a.X_PROD + a.X_CENEFA) <= (item_planograma.X_PROD + item_planograma.X_CENEFA)
                                &&
                                (a.X_PROD + a.PRODUCT_WIDTH_PIXEL + a.X_CENEFA) > (item_planograma.X_PROD + item_planograma.X_CENEFA))
                                ||
                                ((a.X_PROD + a.X_CENEFA) >= (item_planograma.X_PROD + item_planograma.X_CENEFA)
                                &&
                                (a.X_PROD + a.X_CENEFA) <=
                                (item_planograma.X_PROD + width_img + item_planograma.X_CENEFA)
                                )
                                )).OrderBy(a => a.Y_CENEFA).FirstOrDefault();
                                if (nextProductY != null)
                                {
                                    if (item_planograma.Y_CENEFA + height_img > nextProductY.Y_CENEFA)
                                    {
                                        double? diferencia = nextProductY.Y_CENEFA - item_planograma.Y_CENEFA - nextProductY.SEGMENT_HEIGHT_PIXEL;
                                        if (height_img - diferencia >= 50) goto ignorarHeight;
                                        height_img = diferencia;

                                    }
                                }
                                //*********************************************

                                ignorarHeight:

                                if (!(new double?[] { 1, 11, 3, 9 }).Contains(item_planograma.ORIENTATION))
                                {
                                    planograma_html += @"<img class='" +
                                (item_planograma.VERTICAL > 1 && indexV == 0 ? item_planograma.PRODUCT_ID + "_" + item_planograma.VERTICAL : (item_planograma.VERTICAL > 1 ? "v" : "h")) + "' " +
                                "title='" + item_planograma.PROD_NAME.Replace("'", "\"") + " " + string.Format("{0:0.00}", item_planograma.PRODUCT_WIDTH_REAL).Replace(",", ".") + "cm' " +
                                "id='" + item_planograma.PRODUCT_ID + "_" + consecutivo + "' " +
                                "style='bottom:" + (((
                                            (item_planograma.FIXEL_TYPE == 0 ? item_planograma.SEGMENT_HEIGHT_PIXEL : 0) //sumamos el alto de la charola si se trata de una cenefa
                                            + item_planograma.Y_PROD + (height_img * indexV)) + item_planograma.Y_CENEFA) * divisor).ToString().Replace(",", ".") + "px;" +
                                "left:" + (((
                                            //(indexH > 0 ? 
                                            ((indexH) * left_producto
                                            //) : 0
                                            ) +
                                item_planograma.X_CENEFA + item_planograma.X_PROD +
                                (width_img * indexH)) - restar_margen_derecho) * divisor).ToString().Replace(",", ".") + "px;" +
                                "width:" + (width_img * divisor).ToString().Replace(",", ".") + "px;" +
                                "height:" + (height_img * divisor).ToString().Replace(",", ".") + "px;" +
                                "z-index:" + item_planograma.Z_PROD.ToString().Replace(",", ".") + ";" +
                                "' src='" + pathImage[0] + "' />";
                                }
                                else
                                {
                                    planograma_html +=
                                        "<div style='position:absolute;" +
                                        "bottom:" + (((
                                            (item_planograma.FIXEL_TYPE == 0 ? item_planograma.SEGMENT_HEIGHT_PIXEL : 0) //sumamos el alto de la charola si se trata de una cenefa
                                            + item_planograma.Y_PROD + (height_img * indexV)) + item_planograma.Y_CENEFA) * divisor).ToString().Replace(",", ".") + "px;" +
                                        "left:" + (((
                                        //(indexH > 0 ? 
                                        ((indexH) * left_producto
                                            //) : 0
                                            )
                                        +
                                item_planograma.X_CENEFA + item_planograma.X_PROD +
                                (width_img * indexH)) - restar_margen_derecho) * divisor).ToString().Replace(",", ".") + "px;" +
                                        "height:" + (height_img * divisor).ToString().Replace(",", ".") + "px;" +
                                        "width:" + (width_img * divisor).ToString().Replace(",", ".") + "px;'>" +
                                        "<div style='height:" + (width_img * divisor).ToString().Replace(",", ".") + "px;" +
                                        "width:" + (height_img * divisor).ToString().Replace(",", ".") + "px;" +
                                        ((new double?[] { 11, 3 }).Contains(item_planograma.ORIENTATION) ?
                                        "-webkit-transform: rotate(270deg) translateX(-100%);-moz-transform: rotate(270deg) translateX(-100%);-o-transform: rotate(270deg) translateX(-100%);-ms-transform: rotate(270deg) translateX(-100%);transform: rotate(270deg) translateX(-100%);"
                                        :

                                        "-webkit-transform: rotate(90deg) translateY(-100%);-moz-transform: rotate(90deg) translateY(-100%);-o-transform: rotate(90deg) translateY(-100%);-ms-transform: rotate(90deg) translateY(-100%);transform: rotate(90deg) translateY(-100%);"
                                        ) +
                                        "position: relative;" +
                                        "-webkit-transform-origin: 0 0;-moz-transform-origin: 0 0;-o-transform-origin: 0 0;-ms-transform-origin: 0 0;transform-origin: 0 0;'>" +
                                        @"<img class='" +
                                (item_planograma.VERTICAL > 1 && indexV == 0 ? item_planograma.PRODUCT_ID + "_" + item_planograma.VERTICAL : (item_planograma.VERTICAL > 1 ? "v" : "h")) + "' " +
                                "title='" + item_planograma.PROD_NAME.Replace("'", "\"") + " " + string.Format("{0:0.00}", item_planograma.PRODUCT_WIDTH_REAL).Replace(",", ".") + "cm' " +
                                "id='" + item_planograma.PRODUCT_ID + "_" + consecutivo + "' " +
                                "style='z-index:" + item_planograma.Z_PROD.ToString().Replace(",", ".") + ";" +
                                "height: 100%;position: relative;width: 100%;" +
                                "' src='" + pathImage[0] + "' /></div></div>";
                                }
                                string descrProd = item_planograma.PROD_NAME.Replace("'", "\"") + " " + string.Format("{0:0.00}", item_planograma.PRODUCT_WIDTH_REAL).Replace(",", ".") + "cm";

                                div_descripciones += "<div style='" +
                                    (item_planograma.UNITS_CASE_DEEP == 0 ? "background-color:gray;" : "") +
                        "width:" + (width_img * multiplicador).ToString().Replace(",", ".") + "px;" +
                        "height:" + (height_img * multiplicador).ToString().Replace(",", ".") + "px;" +
                        "bottom:" + (((
                                    (item_planograma.FIXEL_TYPE == 0 ? item_planograma.SEGMENT_HEIGHT_PIXEL : 0) //sumamos el alto de la charola si se trata de una cenefa
                                    + item_planograma.Y_PROD + (height_img * indexV)) +
                                    item_planograma.Y_CENEFA) * multiplicador).ToString().Replace(",", ".") + "px;" +
                        "left:" + (((
                        //(indexH > 0 ? 
                        ((indexH) * left_producto
                                            //) : 0
                                            ) +
                                item_planograma.X_CENEFA + item_planograma.X_PROD +
                                (width_img * indexH)) - restar_margen_derecho) * multiplicador).
                                ToString().Replace(",", ".") + "px;" +
                                "z-index:" + item_planograma.Z_PROD.ToString().Replace(",", ".") + ";' " +
                        " class='descripcion'>" +
                        (item_planograma.VERTICAL > 1 ? "" :
                        (((height_img > width_img) ?
                        "<div class='fitText rotate' style='height:" + (width_img * multiplicador).ToString().Replace(",", ".") +
                        "px;width:" + (height_img * multiplicador).ToString().Replace(",", ".") + "px;'><span style='height:" +
                        (width_img * multiplicador).ToString().Replace(",", ".") + "px;" +
                        "width:" + (height_img * multiplicador).ToString().Replace(",", ".") + "px;'>" + descrProd + "</span></div>"
                        :
                        "<div class='fitText' style='" +
                        "width:" + (width_img * multiplicador).ToString().Replace(",", ".") + "px;" +
                        "height:" + (height_img * multiplicador).ToString().Replace(",", ".") + "px;'><span>" + descrProd + "</span></div>"))) +
                        "</div>";

                                if (item_planograma.VERTICAL > 1 && indexV == 0)
                                {
                                    div_descripciones += "<div style='" +
                                        "z-index:" + (item_planograma.Z_PROD + 10) + " ;" +
                        "background: none;" +
                        "border: none;" +
                        "width:" + (width_img * multiplicador).ToString().Replace(",", ".") + "px;" +
                        "height:" + (height_img * multiplicador * item_planograma.VERTICAL).ToString().Replace(",", ".") + "px;" +
                        "bottom:" + (((
                                    (item_planograma.FIXEL_TYPE == 0 ? item_planograma.SEGMENT_HEIGHT_PIXEL : 0) //sumamos el alto de la charola si se trata de una cenefa
                                    + item_planograma.Y_PROD + (height_img * indexV)) +
                                    item_planograma.Y_CENEFA) * multiplicador).ToString().Replace(",", ".") + "px;" +
                        "left:" + (((
                        //(indexH > 0 ? 
                        ((indexH) * left_producto
                                            //) : 0
                                            ) +
                                item_planograma.X_CENEFA + item_planograma.X_PROD +
                                (width_img * indexH)) - restar_margen_derecho) * multiplicador).
                                ToString().Replace(",", ".") + "px;'" +
                        " class='descripcion'>" +
                        (height_img * item_planograma.VERTICAL * multiplicador >=
                        width_img * multiplicador
                        ?
                        "<div class='fitText rotate' style=';height:" + (width_img * multiplicador).ToString().Replace(",", ".") +
                            "px;width:" + (height_img * multiplicador * item_planograma.VERTICAL).ToString().Replace(",", ".") + "px;'><span>" + descrProd + "</span></div>"
                        :
                        "<div class='fitText' style='width:" + (width_img * multiplicador).ToString().Replace(",", ".") +
                            "px;height:" + (height_img * multiplicador * item_planograma.VERTICAL).ToString().Replace(",", ".") + "px;'><span>" + descrProd + "</span></div>") +
                        "</div>";
                                }

                            }
                        }


                        consecutivo++;
                    }

                    //colocar cenefas y/o gancheras
                    foreach (var cenefa in productos_segmento.Select(a => a.FIXEL_ID).Distinct())
                    {
                        var datos_cenefa = productos_segmento.FirstOrDefault(a => a.FIXEL_ID == cenefa);
                        if (datos_cenefa == null) continue;

                        planograma_html += "<div class='cenefa' style='" +
                            "bottom:" + (datos_cenefa.Y_CENEFA * divisor).ToString().Replace(",", ".") + "px;" +
                            "left:" + ((datos_cenefa.X_CENEFA - restar_margen_derecho) * divisor).ToString().Replace(",", ".") + "px;" +
                            "height:" + (datos_cenefa.SEGMENT_HEIGHT_PIXEL * divisor).ToString().Replace(",", ".") + "px;" +
                            "width:" + (datos_cenefa.SEGMENT_WIDTH_PIXEL * divisor).ToString().Replace(",", ".") + "px;" +
                            "background-color:" + retornar_color(datos_cenefa.COLOUR_CENEFA.ToString()) + ";" +
                            "'></div>";

                        div_descripciones += "<div class='cenefa fitText' style='" +
                            "bottom:" + (datos_cenefa.Y_CENEFA * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "left:" + ((datos_cenefa.X_CENEFA - restar_margen_derecho) * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "height:" + (datos_cenefa.SEGMENT_HEIGHT_PIXEL * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "width:" + (datos_cenefa.SEGMENT_WIDTH_PIXEL * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "background-color:" + retornar_color(datos_cenefa.COLOUR_CENEFA.ToString()) + ";" +
                            "font-family:monospace;" +
                            "'><span>" + datos_cenefa.LABEL_CENEFA + "</span></div>";
                    }

                    //colocar cajas de texto
                    foreach (var texto in segmentos_labels.Where(a => a.TYPE == 13 && a.SEGMENT == segmento))
                    {
                        planograma_html += "<div class='texto' style='" +
                            "bottom:" + (texto.YPIX * divisor).ToString().Replace(",", ".") + "px;" +
                            "left:" + (((texto.XPIX) - restar_margen_derecho) * divisor).ToString().Replace(",", ".") + "px;" +
                            "height:" + (texto.HEIGHT_PIXEL * divisor).ToString().Replace(",", ".") + "px;" +
                            "width:" + ((texto.WIDTH_PIXEL) * divisor).ToString().Replace(",", ".") + "px;" +
                            "background-color:" + retornar_color((texto.COLOUR ?? 0).ToString()) + ";" +
                            "color:#000" +
                            "'><label class='fitText' style='" +
                              "font-family:" + texto.FONT_NAME + ";" +
                              //"font-size:" + texto.SIZE_VAL.ToString().Replace(",", ".") + "px;" +
                              "'><span>" + texto.TEXTFIELD + "<span></label></div>";

                        div_descripciones += "<div class='texto' style='" +
                            "bottom:" + ((texto.YPIX) * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "left:" + (((texto.XPIX) - restar_margen_derecho) * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "height:" + (texto.HEIGHT_PIXEL * multiplicador).ToString().Replace(",", ".") + "px;" +
                              "width:" + (texto.WIDTH_PIXEL * multiplicador).ToString().Replace(",", ".") + "px;" +
                              "background-color:" + retornar_color((texto.COLOUR ?? 0).ToString()) + ";" +
                              "color:#000" +
                              "'><label class='fitText' style='" +
                              "font-family:" + texto.FONT_NAME + ";" +
                              //"font-size:" + (texto.SIZE_VAL).ToString().Replace(",", ".") + "px;" +
                              "'><span>" + texto.TEXTFIELD + "<span></label></div>";
                    }

                    //colocar cenefas sin productos
                    foreach (var cenefa in segmentos_labels.Where(a => a.TYPE == 0 && a.SEGMENT == segmento))
                    {
                        planograma_html += "<div class='cenefa' style='" +
                            "bottom:" + (cenefa.YPIX * divisor).ToString().Replace(",", ".") + "px;" +
                            "left:" + ((cenefa.XPIX * divisor) - restar_margen_derecho).ToString().Replace(",", ".") + "px;" +
                            "height:" + (cenefa.HEIGHT_PIXEL * divisor).ToString().Replace(",", ".") + "px;" +
                            "width:" + (cenefa.WIDTH_PIXEL * divisor).ToString().Replace(",", ".") + "px;" +
                            "background-color:" + retornar_color(cenefa.COLOUR.ToString()) + ";" +
                            "'></div>";

                        div_descripciones += "<div class='cenefa' style='" +
                            "bottom:" + ((cenefa.YPIX) * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "left:" + (((cenefa.XPIX) - restar_margen_derecho) * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "height:" + (cenefa.HEIGHT_PIXEL * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "width:" + ((cenefa.WIDTH_PIXEL) * multiplicador).ToString().Replace(",", ".") + "px;" +
                            "background-color:" + retornar_color(cenefa.COLOUR.ToString()) + ";" +
                            "font-family:monospace;" +
                            "font-size:15px;" +
                            "'>" + cenefa.LABEL + "</div>";
                    }

                    if (((width_segmento * divisor) + (width_segmento * multiplicador) >= widthMaxPanelPortrait) || class_pagina == "pagina_h")
                    { //si el panel de productos supero el ancho maximo, se colocara individualmente en la pagina del documento
                        planograma_html += "</div>" + //cerrar panel
                        "</div>" + //cerrar pagina
                        "<div class='" + class_pagina + "'>" + //crear pagina para descripciones
                            div_descripciones + "</div>" + //cerrar panel
                            "</div>";//cerrar pagina
                    }
                    else
                    {
                        planograma_html += "</div>" +
                         div_descripciones + "</div>" +
                            "</div>";
                    }

                }
            }
            planograma_html += "</body></html>";
            return planograma_html;
        }

        public string retornar_color(string color_code)
        {

            byte r = (byte)(int.Parse(color_code) >> 0);
            byte g = (byte)(int.Parse(color_code) >> 8);
            byte b = (byte)(int.Parse(color_code) >> 16);


            //color = System.Configuration.ConfigurationManager.AppSettings["cenefa_" + color_code];

            //if (color == null)
            //{
            //    color = System.Configuration.ConfigurationManager.AppSettings["cenefa_14806254"];
            //}


            return "RGB(" + r + "," + g + "," + b + ")";
        }

        //public void Dispose()
        //{
        //    ctx = null;
        //    headers_const = null;
        //    //this.Dispose();
        //}
    }
}

//planograma_html += "<div class='content-image' " +
//                            "id='" + item_planograma.PRODUCT_ID + "_" + consecutivo + "' " +
//                            "style='bottom:" + (item_planograma.Y_PROD + item_planograma.Y_CENEFA).ToString().Replace(",", ".") + "px;" +
//                            "left:" + item_planograma.X_PROD.ToString().Replace(",", ".") + "px;" +
//                            "width:" + (item_planograma.PRODUCT_WIDTH_PIXEL* item_planograma.TOTAL_FACINGS).ToString().Replace(",", ".") + "px;" +
//                            "height:" + (item_planograma.PRODUCT_HEIGHT_PIXEL* item_planograma.VERTICAL).ToString().Replace(",", ".") + "px;" +
//                            "background: url(" + pathImage[0].Replace(@"\", "/") + ");" +
//                            "background-size: " + item_planograma.PRODUCT_WIDTH_PIXEL.ToString().Replace(",", ".") + "px " +
//                            item_planograma.PRODUCT_HEIGHT_PIXEL.ToString().Replace(",", ".") + "px;" +
//                            "-webkit-background-size: " + item_planograma.PRODUCT_WIDTH_PIXEL.ToString().Replace(",", ".") + "px " +
//                            item_planograma.PRODUCT_HEIGHT_PIXEL.ToString().Replace(",", ".") + "px;" +
//                            "-moz-background-size:" + item_planograma.PRODUCT_WIDTH_PIXEL.ToString().Replace(",", ".") + "px " +
//                            item_planograma.PRODUCT_HEIGHT_PIXEL.ToString().Replace(",", ".") + "px;" +
//                            "-o-background-size:" + item_planograma.PRODUCT_WIDTH_PIXEL.ToString().Replace(",", ".") + "px " +
//                            item_planograma.PRODUCT_HEIGHT_PIXEL.ToString().Replace(",", ".") + "px;" +
//                            "background-repeat:repeat;background-position: -5px -5px;'" +
//                            "></div>";
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanogramaGen.Datos;
namespace PlanogramaGen.Models
{



    public class planogramas_model : IDisposable
    {
        public void Dispose()
        {
            contexto = null;
            this.Dispose();
        }

        PlanogramaGenEntities contexto = new PlanogramaGenEntities();

        public PlanogramaEncabezado ObtenerPlanograma(string tienda, string categoria)
        {
            return contexto.PlanogramaEncabezado.First(a => a.PLAZA.Substring(0, 4) == tienda && a.CATEGORIA == categoria);
        }

        public List<sp_arbol_zonificacion_Result> ObtenerZonificacion()
        {
            return contexto.sp_arbol_zonificacion().ToList();
        }

        public List<sp_obtener_planograma_Result> ObtenerProductosPlanograma(string tienda, string categoria)
        {
            return contexto.sp_obtener_planograma(tienda, categoria).ToList();
        }

        public List<sp_obtener_segmentos_labels_Result> ObtenerSegmentos(int pln_id)
        {
            return contexto.sp_obtener_segmentos_labels(pln_id).ToList();
        }

        public List<sp_obtener_categorias_Result> ObtenerCategorias()
        {
            return contexto.sp_obtener_categorias().ToList();
        }

        public void Guardar(string usuario, List<sp_obtener_planograma_Result> productos, List<sp_obtener_segmentos_labels_Result> segmento)
        {
            var firstProducto = productos.First();

            //eliminar versión de planograma anterior
            contexto.sp_eliminar_planograma(firstProducto.PLN_ID);

            //encabezado
            PlanogramaEncabezado encabezado = new PlanogramaEncabezado
            {
                CATEGORIA = firstProducto.CATEGORIA,
                PLANOGRAM = firstProducto.PLANOGRAM,
                PLAZA = firstProducto.PLAZA,
                PLN_ID = firstProducto.PLN_ID,
                DATE_CREATE = DateTime.Now,
                USER = usuario
            };
            contexto.PlanogramaEncabezado.Add(encabezado);

            //productos
            foreach (var item in productos)
            {
                PlanogramaProductos producto = new PlanogramaProductos
                {
                    FIXEL_ID = item.FIXEL_ID,
                    MERCH_STYLE = item.MERCH_STYLE ?? 0,
                    ORIENTATION = item.ORIENTATION ?? 0,
                    PLN_ID = item.PLN_ID,
                    PRODUCT_HEIGHT = item.PRODUCT_HEIGHT ?? 0,
                    PRODUCT_HEIGHT_PIXEL = item.PRODUCT_HEIGHT_PIXEL ?? 0,
                    PRODUCT_ID = item.PRODUCT_ID,
                    PRODUCT_WIDTH = item.PRODUCT_WIDTH ?? 0,
                    PRODUCT_WIDTH_PIXEL = item.PRODUCT_WIDTH_PIXEL ?? 0,
                    PRODUCT_WIDTH_REAL = item.PRODUCT_WIDTH_REAL ?? 0,
                    PROD_NAME = item.PROD_NAME,
                    TOTAL_FACINGS = item.TOTAL_FACINGS ?? 0,
                    UNITS_CASE_DEEP = item.UNITS_CASE_DEEP ?? 0,
                    VERTICAL = item.VERTICAL ?? 0,
                    X_PROD = item.X_PROD ?? 0,
                    Y_PROD = item.Y_PROD ?? 0
                };
                contexto.PlanogramaProductos.Add(producto);
            }

            //cenefas
            foreach (var item in productos.Select(a => new
            {
                PLN_ID = a.PLN_ID,
                FIXEL_ID = a.FIXEL_ID,
                X_CENEFA = a.X_CENEFA,
                Y_CENEFA = a.Y_CENEFA,
                COLOUR_CENEFA = a.COLOUR_CENEFA,
                SEGMENT_WIDTH = a.SEGMENT_WIDTH,
                SEGMENT_WIDTH_PIXEL = a.SEGMENT_WIDTH_PIXEL,
                SEGMENT_HEIGHT = a.SEGMENT_HEIGHT,
                SEGMENT_HEIGHT_PIXEL = a.SEGMENT_HEIGHT_PIXEL,
                LABEL_CENEFA = a.LABEL_CENEFA,
                FIXEL_TYPE = a.FIXEL_TYPE,
                SEGMENT = a.SEGMENT
            }).Distinct())
            {
                PlanogramaCenefas cenefa = new PlanogramaCenefas
                {
                    PLN_ID = item.PLN_ID,
                    FIXEL_ID = item.FIXEL_ID,
                    X_CENEFA = item.X_CENEFA ?? 0,
                    Y_CENEFA = item.Y_CENEFA ?? 0,
                    COLOUR_CENEFA = item.COLOUR_CENEFA ?? 0,
                    FIXEL_TYPE = item.FIXEL_TYPE ?? 0,
                    LABEL_CENEFA = item.LABEL_CENEFA,
                    SEGMENT = item.SEGMENT ?? 0,
                    SEGMENT_HEIGHT = item.SEGMENT_HEIGHT ?? 0,
                    SEGMENT_HEIGHT_PIXEL = item.SEGMENT_HEIGHT_PIXEL ?? 0,
                    SEGMENT_WIDTH = item.SEGMENT_WIDTH ?? 0,
                    SEGMENT_WIDTH_PIXEL = item.SEGMENT_WIDTH_PIXEL ?? 0
                };
                contexto.PlanogramaCenefas.Add(cenefa);
            }

            //objetos
            foreach (var item in segmento)
            {
                PlanogramaObjetos objeto = new PlanogramaObjetos
                {
                    COLOUR = item.COLOUR ?? 0,
                    COLOURISCLEAR = item.COLOURISCLEAR ?? 0,
                    FILL_COLOR = item.FILL_COLOR ?? 0,
                    FIXEL_ID = item.FIXEL_ID,
                    FONT_NAME = item.FONT_NAME,
                    HEIGHT = item.HEIGHT ?? 0,
                    HEIGHT_PIXEL = item.HEIGHT_PIXEL ?? 0,
                    LABEL = item.LABEL,
                    SEGMENT = item.SEGMENT ?? 0,
                    SIZE_VAL = Convert.ToInt16(item.SIZE_VAL ?? 0),
                    TEXTFIELD = item.TEXTFIELD,
                    TYPE = item.TYPE ?? 0,
                    WIDTH = item.WIDTH ?? 0,
                    WIDTH_PIXEL = item.WIDTH_PIXEL ?? 0,
                    XPIX = item.XPIX ?? 0,
                    YPIX = item.YPIX ?? 0,
                    PLN_ID = firstProducto.PLN_ID
                };
                contexto.PlanogramaObjetos.Add(objeto);
            }
            contexto.SaveChanges();
        }


    }
}
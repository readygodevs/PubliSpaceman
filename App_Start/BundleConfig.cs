using System.Web;
using System.Web.Optimization;

namespace PlanogramaGen
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                                        "~/Maquetado/js/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Maquetado/js/app.js",
                "~/Maquetado/js/chosen.jquery.min.js",
                "~/Maquetado/js/toast.js",
                //"~/Maquetado/js/datatables.min.js",
                "~/Maquetado/js/jquery.datatable.min.js",
                "~/Maquetado/js/sweetalert.min.js",
                "~/Maquetado/js/dataTables.bootstrap.min.js",
                "~/Maquetado/js/timer.jquery.min.js"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
            //                            ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Maquetado/css/app.css",
                      "~/Maquetado/css/chosen.min.css",
                      "~/Maquetado/css/datatable.min.css",
                      "~/Maquetado/css/ionicons.css",
                      "~/Maquetado/css/sweetalert.css",
                      "~/Maquetado/css/toast.css"));
        }
    }
}

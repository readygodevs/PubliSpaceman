using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanogramaGen.Datos;

namespace PlanogramaGen.Models
{
    public class ReglaNegocioModel
    {
        PlanogramaGenEntities contexto = new PlanogramaGenEntities();

        public string obtenerRegla(string clave)
        {
            return contexto.ReglasNegocio.First(a => a.RENClave == clave).Regla;
        }
    }
}
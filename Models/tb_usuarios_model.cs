using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanogramaGen.Datos;
using System.Data.Entity.Core.Objects;

namespace PlanogramaGen.Models
{
    public class UsuarioLogin
    {
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Perfil { get; set; }
    }

    public class tb_usuarios_model : IDisposable
    {
        public void Dispose()
        {
            contexto = null;
        }

        PlanogramaGenEntities contexto = new PlanogramaGenEntities();

        public string validarUsuario(string login)
        {
            return contexto.sp_validar_usuario(login).First();
        }

        public List<tb_usuarios> obtenerUsuarios()
        {
            return contexto.tb_usuarios.ToList();
        }

        public void guardarUsuario(tb_usuarios usr)
        {
            contexto.tb_usuarios.Add(usr);
            contexto.SaveChanges();
        }

        public void guardarRutas(string pub, string img)
        {
            contexto.ReglasNegocio.First(a => a.RENClave == "URLExporta").Regla = pub;
            contexto.ReglasNegocio.First(a => a.RENClave == "URLTomaImg").Regla = img;
            contexto.SaveChanges();
        }

        public void eliminarUsuario(int idusr)
        {
            contexto.tb_usuarios.Remove(contexto.tb_usuarios.First(a => a.IdUsuario == idusr));
            contexto.SaveChanges();
        }

        public tb_usuarios obtenerUsuario(int id_usr)
        {
            return contexto.tb_usuarios.FirstOrDefault(a => a.IdUsuario == id_usr) ?? new tb_usuarios();
        }
    }
}
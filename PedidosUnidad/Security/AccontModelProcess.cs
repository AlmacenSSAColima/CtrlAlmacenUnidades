using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PedidosUnidad.Security;
using PedidosUnidad.Models.DBPedido;

namespace PedidosUnidad.Security
{
    public class AccontModelProcess
    {
        public AccontModelProcess()
        {
        }

        public CurrentUser find(string username)
        {
            //CONSULTA POR NOMBRE DE USURIO. SI SE REQUIERE AQUI VA LA CONSULTA A LA BD PARA TRAER INFO DEL USUARIO
            CurrentUser mdl = new CurrentUser();
            mdl.id_user = 1;
            mdl.name_user = username;
            mdl.login = true;
            mdl.rol = "admin";
            return mdl;
        }

        public CurrentUser login(string username, string password)
        {
            CurrentUser mdl = new CurrentUser();
            PedidoEntity db = new PedidoEntity();

            try
            {
                //CONSULTAR EN BD QUERY
                usuarios UserDatos = db.usuarios.Where(u => u.user_name == username && u.password == password).FirstOrDefault();
                if (UserDatos != null)
                //if (username == "admin" && password == "1234")
                {
                    mdl.id_user = UserDatos.id;
                    mdl.name_user = UserDatos.user_name;
                    mdl.full_name = UserDatos.nombre_usuario;
                    mdl.id_rol = UserDatos.rol ?? -1;
                    mdl.rol = UserDatos.nombre_rol;
                    mdl.id_unidad = UserDatos.unidad ?? -1;
                    mdl.nom_unidad = UserDatos.nombre_unidad;
                    mdl.login = true;
                    mdl.tipo = UserDatos.tipo_usuario ?? 0;
                }
            }
            catch (Exception e)
            {

            }


            return mdl;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PedidosUnidad.Models.DBPedido;
using PedidosUnidad.Models.DBAlmacen;
using System.Web.Mvc;

namespace PedidosUnidad.Models
{
    public class RepoUsuarioPermisos
    {
        private PedidoEntity db;
        private AlmacenEntity dban;

        public List<usuarios> getUsuarios()
        {
            db = new PedidoEntity();
            List<usuarios> lista = new List<usuarios>();
            try
            {
                lista = db.usuarios.ToList();
            }
            catch(Exception e)
            {
                lista = new List<usuarios>();
            }


            return lista;
        }

        public usuarios getUsuario(int id)
        {
            db = new PedidoEntity();
            usuarios RegistroUsuario = new usuarios();
            try
            {
                RegistroUsuario = db.usuarios.SingleOrDefault(u => u.id == id);
            }
            catch(Exception e)
            {
                RegistroUsuario = new usuarios();
            }
            
            return RegistroUsuario;
        }

        public List<roles> getRolesPerfil()
        {
            db = new PedidoEntity();
            List<roles> registros = new List<roles>();
            try
            {
                registros = db.roles.ToList();
            }
            catch (Exception e)
            {
                registros = new List<roles>();
            }

            return registros;
        }

        public List<PermisosClass> getPermisos(int id_rol)
        {
            List<PermisosClass> permisos = new List<PermisosClass>();
            db = new PedidoEntity();

            List<modules> modulosFunciones = new List<modules>();
            modulosFunciones = db.modules.ToList();

            List<roles_modules_privileges> asignados = new List<roles_modules_privileges>();
            asignados = (from r in db.roles_modules_privileges.ToList() where r.roles_id == id_rol select r).ToList();

            foreach (var item in modulosFunciones)
            {
                PermisosClass per = new PermisosClass();
                //per.id = asignado.idroles_module_privileges;
                per.id_modulo = item.idmodules;
                per.id_rol = id_rol;
                per.nombre = item.namemodules;
                per.ruta = item.menuurl;

                roles_modules_privileges asignado = asignados.SingleOrDefault(r => r.modules_idmodules == item.idmodules);
                if (asignado != null)
                    per.permiso = true;

                permisos.Add(per);
            }

            return permisos;
        }

        public ResultClass savePermisos(int id_rol, string ids)
        {
            ResultClass mdl = new ResultClass();
            try
            {
                db = new PedidoEntity();
                List<roles_modules_privileges> asignados = new List<roles_modules_privileges>();
                asignados = (from r in db.roles_modules_privileges.ToList() where r.roles_id == id_rol select r).ToList();
                if(asignados.Count >0)
                {
                    db.roles_modules_privileges.RemoveRange(asignados);
                    db.SaveChanges();
                }
                
                if(!string.IsNullOrWhiteSpace(ids))
                {
                    string[] ar_ids = ids.Split(',');
                    foreach (var item in ar_ids)
                    {
                        string id = item;
                        roles_modules_privileges permiso = new roles_modules_privileges();
                        permiso.idroles_module_privileges = 0;
                        permiso.roles_id = id_rol;
                        permiso.modules_idmodules = Convert.ToInt32(id);
                        permiso.create_privilege = true;
                        permiso.read_privilege = true;
                        permiso.delete_privilege = true;
                        permiso.update_privilege = true;
                        db.roles_modules_privileges.Add(permiso);
                        db.SaveChanges();

                    }
                }
                
                mdl.exito = true;
                mdl.msg = "Proceso correcto";
            }
            catch(Exception e) {
                mdl.exito = false;
                mdl.msg = "Ocurrio un error de excepción";
            }

            return mdl;
        }

        public SelectList getUnidades()
        {
            dban = new AlmacenEntity();
            List<CA_CENTROS> lista = new List<CA_CENTROS>();
            try
            {
                lista = dban.CA_CENTROS.ToList();
                return new SelectList(lista, "centro", "descrip");
            }
            catch (Exception e)
            {
                lista = new List<CA_CENTROS>().OrderByDescending(a=>a.DESCRIP).ToList();
                return new SelectList(lista, "centro", "descrip");
            }
        }

        public SelectList getRoles()
        {
            db = new PedidoEntity();
            List<roles> lista = new List<roles>();
            try
            {
                lista = db.roles.ToList().OrderByDescending(a => a.nombre_rol).ToList();
                return new SelectList(lista, "id", "nombre_rol");
            }
            catch (Exception e)
            {
                lista = new List<roles>();
                return new SelectList(lista, "id", "nombre_rol");
            }
        }

        public ResultClass cambiarPassword(int id, string pass, string newPass)
        {
            ResultClass mdl = new ResultClass();
            db = new PedidoEntity();

            usuarios usuario = db.usuarios.SingleOrDefault(a => a.id == id);
            if (usuario.password == pass)
            {
                usuario.password = newPass;
                db.SaveChanges();
                mdl.exito = true;
                mdl.msg = "El password se ha establecido correctamente. En el siguiente ingreso deberá usar su nueva contraseña.";
            }
            else
            {
                mdl.exito = false;
                mdl.msg = "El password es incorrecto";
            }

            return mdl;
        }
    }
}
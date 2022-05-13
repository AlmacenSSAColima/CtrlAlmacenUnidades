using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PedidosUnidad.Models;
using PedidosUnidad.Models.DBPedido;
using PedidosUnidad.Security;

namespace PedidosUnidad.Controllers
{
    public class UsuariosController : Controller
    {
        public RepoUsuarioPermisos repo;

        // GET: Usuarios
        [XAuthorizeAtribute]
        public ActionResult Index()
        {
            repo = new RepoUsuarioPermisos();
            List<usuarios> listaUsuarios = repo.getUsuarios();
            return View(listaUsuarios);
        }

        [XAuthorizeAtribute]
        public ActionResult DatosUsuario(int id)
        {
            usuarios RegistroUsuario = null;
            repo = new RepoUsuarioPermisos();
            ViewBag.SelectListUnidades = repo.getUnidades();
            ViewBag.SelectListRoles = repo.getRoles();

            if (id == 0)
            {
                RegistroUsuario = new usuarios();
            }
            else
            {
                RegistroUsuario = repo.getUsuario(id);
            }

            return View(RegistroUsuario);
        }

        [XAuthorizeAtribute]
        public ActionResult Perfiles()
        {
            repo = new RepoUsuarioPermisos();
            List<roles> registros = repo.getRolesPerfil();
            return View(registros);
        }

        public ActionResult Permisos(int id)
        {
            repo = new RepoUsuarioPermisos();
            List<PermisosClass> modulos = repo.getPermisos(id); 
            return PartialView(modulos);
        }

        [HttpPost]
        public JsonResult SavePermisos(FormCollection frm)
        {
            string rol = frm["form_rol_select"].ToString();
            int id_rol = Convert.ToInt32(rol);
            string ids = (frm["ids_modulos"] ?? "").ToString();
            ResultClass data = new ResultClass();
            repo = new RepoUsuarioPermisos();
            data = repo.savePermisos(id_rol, ids);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FechasPedido()
        {
            return View();
        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [XAuthorizeAtribute]
        [HttpPost]
        public JsonResult ChangePassword(FormCollection frm)
        {
            int id_usuario = SessionPersister.CurrentUser.id_user;
            string pass = frm["Passusers"];
            string newPass = frm["newPassusers"];

            RepoUsuarioPermisos obj = new RepoUsuarioPermisos();
            ResultClass mdlR = new ResultClass();// 
            mdlR = obj.cambiarPassword(id_usuario, pass, newPass);
            return Json(mdlR, JsonRequestBehavior.AllowGet);
        }

    }

}
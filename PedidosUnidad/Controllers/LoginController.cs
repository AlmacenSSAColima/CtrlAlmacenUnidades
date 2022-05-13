using PedidosUnidad.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PedidosUnidad.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            SessionPersister.CurrentUser = new CurrentUser();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel LModel)
        {
            //VALIDAR FORMULARIO
            if (string.IsNullOrEmpty(LModel.loginusers) || string.IsNullOrEmpty(LModel.passusers))
            {
                ViewBag.Error = "Usuario no valido";
                return View("Index");
            }

            //CONSULTAR EN BD QUERY
            AccontModelProcess obj = new AccontModelProcess();
            CurrentUser mdl = obj.login(LModel.loginusers, LModel.passusers);
            if (!mdl.login)
            {
                ViewBag.Error = "Error en el nombre de usuario o contraseña";
                return View("Index");
            }

            mdl.navegador = LModel.navegador;
            SessionPersister.CurrentUser = mdl;

            return RedirectToAction("Index", "Home");
        }
       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PedidosUnidad.Security;
using PedidosUnidad.Models.DBPedido;
using PedidosUnidad.Models;

namespace PedidosUnidad.Controllers
{
    public class HomeController : Controller
    {
        private PedidoEntity db = new PedidoEntity();

        [XAuthorizeAtribute]
        public ActionResult Index()
        {
            //RepoPedidos elRGrande = new RepoPedidos();
            //List<InsumoClassSIAA> listado = elRGrande.getReporteDescripcionesCVE();
            //List<ReportInsumos> listado = elRGrande.getGeneral();

            //CuadroGeneralModel mdl = elRGrande.getDistribucionMaterial();
            //return View("Indesx_des", listado);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Denied()
        {
            ViewBag.Message = "";
            return View();
        }

        public ActionResult PedidoDenied() {
            return View();
        }

        public ActionResult Iconos()
        {
            return View();
        }

        public ActionResult Componentes()
        {
            return View();
        }

        //[ChildActionOnly]
        public ActionResult LoadMenu()
        {
            int id_usr = SessionPersister.CurrentUser.id_rol;
            List<modules_temp> menu = db.Database.SqlQuery<modules_temp>("SP_Load_Menu @UserID =" + id_usr.ToString()).ToList();

            //List<modules_temp> menuT = (from a in db.modules_temp where a.users_idusers == id_usr  select a).ToList();
            return PartialView(menu);
        }

        [XAuthorizeAtribute]
        public ActionResult MonitoreoUnidades()
        {
            RepoCuadros repo = new RepoCuadros();
            UnidadesMonitoreoConsultaClass mdl = repo.GetMonitoreoUnidades();

            return View(mdl);
        }

    }
}
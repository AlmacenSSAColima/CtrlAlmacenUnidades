using PedidosUnidad.Models;
using PedidosUnidad.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PedidosUnidad.Controllers
{
    public class ProgramasController : Controller
    {
        // GET: Programas
        public ActionResult Index()
        {
            return View();
        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult Consulta()
        {
            AddInsumoForm mdl = new AddInsumoForm();
            return View(mdl);
        }

        [XAuthorizeAtribute]
        [HttpPost]
        public ActionResult Consulta(FormCollection frm)
        {
             RepoProgramasInfo repo = new RepoProgramasInfo();
            string Lote = frm["buscar_item"].ToString();
            List<infoProgramaClass>  mdl = repo.getInfoByLote(Lote);
            return View("ItemsConsulta", mdl);
        }


    }
}
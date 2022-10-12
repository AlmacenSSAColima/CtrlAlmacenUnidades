using PedidosUnidad.Models;
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

        [HttpGet]
        public ActionResult Consulta()
        {
            AddInsumoForm mdl = new AddInsumoForm();
            return View(mdl);
        }

        [HttpPost]
        public ActionResult Consulta(FormCollection frm)
        {
            RepoProgramasInfo repo = new RepoProgramasInfo();
            string Lote = frm["buscar_item"].ToString();
            repo.getInfoByLote(Lote);
            return View("ItemsConsulta");
        }


    }
}
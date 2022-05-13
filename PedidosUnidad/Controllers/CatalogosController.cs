using PedidosUnidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PedidosUnidad.Controllers
{
    public class CatalogosController : Controller
    {
        // GET: Catalogos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Catalogo(int tipo_insumo, bool controlado)
        {
            RepoEntradas repo = new RepoEntradas();
            List<InsumoClassSIAA> mdl_list = repo.getLstInsumos(tipo_insumo, controlado);
            AddInsumoForm mdl = new AddInsumoForm();
            mdl.clavesCat = mdl_list;
            mdl.cantidad_claves = mdl_list.Count();
            mdl.tipo_insumo = repo.getTipoInsumo(tipo_insumo);
            return View(mdl);
        }
    }
}
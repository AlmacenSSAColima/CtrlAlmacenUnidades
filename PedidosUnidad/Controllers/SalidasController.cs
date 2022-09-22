using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PedidosUnidad.Models;
using PedidosUnidad.Models.DBConcentradora;
using PedidosUnidad.Security;

namespace PedidosUnidad.Controllers
{
    public class SalidasController : Controller
    {
        // GET: Salidas (Listado de Salidas)
        [XAuthorizeAtribute]
        public ActionResult Index()
        {
            RepoSalidas repoSalidas = new RepoSalidas();
            SalidasClassSIAA mdlSalidas = new SalidasClassSIAA();
            mdlSalidas = repoSalidas.getSalidas(SessionPersister.CurrentUser.id_unidad);
            int hoy_ = DateTime.Now.Year;
            return View(mdlSalidas);

        }

        [XAuthorizeAtribute]
        public ActionResult Salida(int? folio, int? anio)
        {
            RepoSalidas repoSalidas = new RepoSalidas();
            //CLASE MAESTRA
            MaDeSalidasClassSIAA mdlMaDeSalidas = new MaDeSalidasClassSIAA();
            //CLASE DETALLE
            List<DeSalidasClassSIAA> detSalida = new List<DeSalidasClassSIAA>();

            mdlMaDeSalidas.maSalidas.PEDIDO = 0;
            mdlMaDeSalidas.maSalidas.ANIO = 2019; //ESTE DATO LO DEBE TOMAR DEL FOLIADOR
            mdlMaDeSalidas.maSalidas.FECHA_PEDIDO = DateTime.Now;

            mdlMaDeSalidas.maSalidas.TIPO_PEDIDO = 1; //TIPO DE SALIDA 1 = NORMAL


            ViewBag.TipoSalida = repoSalidas.getTipoSalida();
            ViewBag.UnidadSolicito = repoSalidas.getUnidadSolicito();


            //CARGAR MODELO CON ENTRADA SELECCIONADA (EDITANDO) DE LO CONTRARIO MANTENER LA CLASE INICIALIZADA PARA NUEVO REGISTRO
            if (folio != null)
            {
                mdlMaDeSalidas = repoSalidas.getSalida(folio ?? 0, anio ?? 0, SessionPersister.CurrentUser);
                detSalida = mdlMaDeSalidas.deSalidas;
            }

            //MANTENER DETALLE EN VARIABLE DE SESION PARA CONTROL Y MANEJO EN PROCESO
            Session["detalle_insumos"] = detSalida;

            return View(mdlMaDeSalidas);
        }




        [HttpGet]
        public ActionResult DetalleSalida()
        {
            List<DeSalidasClassSIAA> mdl = new List<DeSalidasClassSIAA>();
            mdl = Session["detalle_insumos"] as List<DeSalidasClassSIAA>;

            return View(mdl);
        }

        [HttpGet]
        public ActionResult AddInsumoSalida(int tipo_insumo)
        {
            RepoSalidas repo = new RepoSalidas();
            List<InsumoClassSIAA> mdl = repo.getLstInsumos(tipo_insumo);
            return View(mdl);
        }

        [HttpPost]
        public JsonResult AddInsumoSalida(FormCollection frm)
        {
            RepoSalidas repo = new RepoSalidas();
            List<DeSalidasClassSIAA> detalle_ = Session["detalle_insumos"] as List<DeSalidasClassSIAA>;

            string pk = frm["ins_pk"].ToString();
            string tipo = frm["ins_tipo"].ToString();
            string grupo = frm["ins_grupo"].ToString();
            string clave = frm["ins_clave"].ToString();
            string presentacion = frm["ins_presentacion"].ToString();
            string cantidad = frm["ins_solicitado_cap"].ToString();
            string cant_surt = frm["ins_surtida_cap"].ToString();

            string descripcion_cve = frm["ins_des"].ToString();
            string descripLarga_cve = frm["ins_des_la"].ToString();

            DeSalidasClassSIAA item_insumo = new DeSalidasClassSIAA();
            item_insumo.desalidas.pk_articulos = pk;
            item_insumo.desalidas.ANIO = Convert.ToInt16(DateTime.Now.Year);
            item_insumo.desalidas.TIPO = Convert.ToInt16(tipo);
            item_insumo.desalidas.GRUPO = Convert.ToInt16(grupo);
            item_insumo.desalidas.CLAVE = Convert.ToInt16(clave);
            item_insumo.desalidas.presentacion = Convert.ToInt16(presentacion);
            item_insumo.desalidas.CANTIDAD = Convert.ToInt16(cantidad);
            item_insumo.desalidas.CANT_SURT = Convert.ToInt16(cant_surt);
            item_insumo.desalidas.id_centro = SessionPersister.CurrentUser.id_unidad;
            item_insumo.insumo = descripcion_cve;
            item_insumo.descLarga = descripLarga_cve;

            ReturnModelClass mdl = new ReturnModelClass();
            mdl = repo.addInsumo(ref detalle_, item_insumo);
            Session["detalle_insumos"] = detalle_;


            return Json(mdl, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarCVE(string pk)
        {
            List<DeSalidasClassSIAA> detalle_ = Session["detalle_insumos"] as List<DeSalidasClassSIAA>;
            DeSalidasClassSIAA rowElimiar = detalle_.SingleOrDefault(a => a.desalidas.pk_articulos == pk);

            RepoSalidas repo = new RepoSalidas();
            ReturnModelClass rmodel = repo.deleteInsumo(ref detalle_, rowElimiar);

            return Json(rmodel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLotes(string pk)
        {
            RepoSalidas repo = new RepoSalidas();
            List<vwDispCad> rmodel = repo.getCadDisp(pk, SessionPersister.CurrentUser.id_unidad);

            return Json(rmodel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ReserveLotes(string pk, string pk_cad, int cantR)
        {
            string Proceso = "A";
            int Usr = SessionPersister.CurrentUser.id_user;
            RepoSalidas repo = new RepoSalidas();
            ReturnModelClass rmodel = repo.apartaLote(Proceso, pk, pk_cad, cantR, SessionPersister.CurrentUser.id_unidad, SessionPersister.CurrentUser.id_user);

            return Json(rmodel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FreeLotes(string pk, string pk_cad, int cantR)
        {
            string Proceso = "E";
            int Usr = SessionPersister.CurrentUser.id_user;
            RepoSalidas repo = new RepoSalidas();
            ReturnModelClass rmodel = repo.apartaLote(Proceso, pk, pk_cad, cantR, SessionPersister.CurrentUser.id_unidad, SessionPersister.CurrentUser.id_user);

            return Json(rmodel, JsonRequestBehavior.AllowGet);
        }



    }
}
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
    public class EntradasController : Controller
    {
        // GET: Entradas (Listado de entradas)
        [XAuthorizeAtribute]
        public ActionResult Index()
        {
            RepoEntradas repoEntradas = new RepoEntradas();
            EntradasClassSIAA mdlEntradas = new EntradasClassSIAA();
            mdlEntradas = repoEntradas.getEntradas(SessionPersister.CurrentUser.id_unidad);
            int hoy_ = DateTime.Now.Year;
            return View(mdlEntradas);

        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult Entrada(int? folio, int? anio, int ? tipoInsumo)
        {
            RepoEntradas repoEntradas = new RepoEntradas();
            //CLASE MAESTRA
            MaDeEntradasClassSIAA mdlMaDeEntradas = new MaDeEntradasClassSIAA();
            //CLASE DETALLE
            List<DeEntradasClassSIAA> detEntrada = new List<DeEntradasClassSIAA>();

            mdlMaDeEntradas.maEntradas.FOLIO = 0;
            mdlMaDeEntradas.maEntradas.ANIO = DateTime.Now.Year; ; //ESTE DATO LO DEBE TOMAR DEL FOLIADOR
            mdlMaDeEntradas.maEntradas.FECHA = DateTime.Now;
            mdlMaDeEntradas.maEntradas.TIPO_ENTRADA = 1; //TIPO DE ENTRADA 1 = NORMAL
            mdlMaDeEntradas.maEntradas.TIPO_DOCTO = 6; //TIPO VALE 6 = NORMAL
            mdlMaDeEntradas.cvproveedor = "1|1|300"; //PROOVEDOR ALMACEN CENTRAL
            mdlMaDeEntradas.tipo_insumo = tipoInsumo ?? 0;
            mdlMaDeEntradas.maEntradas.TIPO_INSUMOS = tipoInsumo ?? 0;

            ViewBag.TipoEntrada = repoEntradas.getTipoEntrada();
            ViewBag.TipoDocumento = repoEntradas.getTipoDocumento();
            ViewBag.TipoLicitacion = repoEntradas.getTipoLicitacion();
            ViewBag.CentroRequisito = repoEntradas.getCentroRequisito();
            ViewBag.Programa = repoEntradas.getPrograma();
            ViewBag.Fuente = repoEntradas.getFuente();
            ViewBag.Proveedor = repoEntradas.getProveedor();

            //CARGAR MODELO CON ENTRADA SELECCIONADA (EDITANDO) DE LO CONTRARIO MANTENER LA CLASE INICIALIZADA PARA NUEVO REGISTRO
            if (folio != null)
            {
                mdlMaDeEntradas = repoEntradas.getEntrada(folio ?? 0, anio ?? 0, SessionPersister.CurrentUser);
                mdlMaDeEntradas.tipo_insumo = mdlMaDeEntradas.maEntradas.TIPO_INSUMOS ?? 0;
                detEntrada = mdlMaDeEntradas.deEntradas;
            }

            //MANTENER DETALLE EN VARIABLE DE SESION PARA CONTROL Y MANEJO EN PROCESO
            Session["detalle_insumos"] = detEntrada;

            return View(mdlMaDeEntradas);
        }

        [HttpPost]
        public JsonResult Entrada(FormCollection frm, MaDeEntradasClassSIAA mdl)
        {
            RepoEntradas repoEntradas = new RepoEntradas();
            mdl.deEntradas = Session["detalle_insumos"] as List<DeEntradasClassSIAA>;

            ReturnModelClass mdl_retorno = new ReturnModelClass();
            if (mdl.maEntradas.FOLIO == 0)
                mdl_retorno = repoEntradas.saveEntrada(ref mdl, SessionPersister.CurrentUser);
            else
                mdl_retorno = repoEntradas.editEntrada(ref mdl, SessionPersister.CurrentUser);


            mdl.exito = mdl_retorno.exito;
            mdl.msg = mdl_retorno.msg;

            return Json(mdl, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult DetalleEntrada()
        {
            //Estos tipos no aplica el IVA
            int[] arrayTipo = { 0, 10, 30, 40, 41 };

            //Modelo para enviar a la vista
            AddInsumoForm mdlView = new AddInsumoForm();

            List<DeEntradasClassSIAA> mdl = new List<DeEntradasClassSIAA>();
            mdl = Session["detalle_insumos"] as List<DeEntradasClassSIAA>; 

            mdlView.clavesAgregadas = mdl;

            decimal total_sin_iva = 0;
            decimal total_con_iva = 0;
            decimal iva_ = 0;
            
            foreach (var item in mdl)
            {
                decimal costo = item.deentradas.COSTO;
                int cantidad = item.deentradas.CANTIDAD ?? 1;

                decimal total = costo * cantidad;
                total_sin_iva = total_sin_iva + total;                   
            }

            //Validar si tipo de insumo esta dentro de los qe no aplica IVA
            if (arrayTipo.Contains(mdl[0].deentradas.TIPO))
               iva_ = 0;            
            else 
               iva_ = total_sin_iva * Convert.ToDecimal(0.16);

            
            total_con_iva = total_sin_iva + iva_;

            //SIN FORMATO TIPO DECIMAL
            mdlView.d_total_sin_iva = total_sin_iva;
            mdlView.d_total_con_iva = total_con_iva;
            mdlView.d_iva_ = iva_;
            //CON FORMATO PARA PRESENTAR
            mdlView.total_sin_iva = String.Format("{0:0,0.00}", total_sin_iva);
            mdlView.total_con_iva = String.Format("{0:0,0.00}", total_con_iva);
            mdlView.iva_ = String.Format("{0:0,0.00}", iva_);

            return View(mdlView);
        }

        [HttpGet]
        public ActionResult AddInsumoEntrada(int tipo_insumo, bool controlado)
        {
            RepoEntradas repo = new RepoEntradas();
            List<InsumoClassSIAA> mdl_list = repo.getLstInsumos(tipo_insumo, controlado);
            AddInsumoForm mdl = new AddInsumoForm();
            mdl.clavesCat = mdl_list;
            mdl.cantidad_claves = mdl_list.Count();
            mdl.tipo_insumo = repo.getTipoInsumo(tipo_insumo);
            mdl.clavesAgregadas = Session["detalle_insumos"] as List<DeEntradasClassSIAA>;
            return View(mdl);
        }

        [HttpPost]
        public JsonResult AddInsumoEntrada(FormCollection frm)
        {
            RepoEntradas repo = new RepoEntradas();
            List<DeEntradasClassSIAA> detalle_ = Session["detalle_insumos"] as List<DeEntradasClassSIAA>;

            int editando_insumo = Convert.ToInt32(frm["ins_editando_insumo"].ToString()); 
            string pk = frm["ins_pk"].ToString();
            string tipo = frm["ins_tipo"].ToString();
            string grupo = frm["ins_grupo"].ToString();
            string clave = frm["ins_clave"].ToString();
            string presentacion = frm["ins_presentacion"].ToString();
            string cantidad = frm["ins_cantidad_cap"].ToString();
            string costo = frm["ins_costo_cap"].ToString();

            string descripcion_cve = frm["ins_des"].ToString();
            string descripLarga_cve = frm["ins_des_la"].ToString();
            string descripPresentacion = frm["ins_presentacion_descrip"].ToString();

            string lotes_cve = frm["ins_fechas_lotes"].ToString();

            DeEntradasClassSIAA item_insumo = new DeEntradasClassSIAA();
            item_insumo.deentradas.PK_ARTICULOS = pk;
            item_insumo.deentradas.ANIO = Convert.ToInt16(DateTime.Now.Year);
            item_insumo.deentradas.TIPO = Convert.ToInt16(tipo);
            item_insumo.deentradas.GRUPO = Convert.ToInt16(grupo);
            item_insumo.deentradas.CLAVE = Convert.ToInt16(clave);
            item_insumo.deentradas.PRESENTACION = Convert.ToInt16(presentacion);
            item_insumo.deentradas.CANTIDAD = Convert.ToInt16(cantidad);
            item_insumo.deentradas.COSTO = Convert.ToDecimal(costo);
            item_insumo.deentradas.id_centro = SessionPersister.CurrentUser.id_unidad;
            item_insumo.insumo = descripcion_cve;
            item_insumo.descLarga = descripLarga_cve;
            item_insumo.pres = descripPresentacion;

            List<DE_EntradasCAD> de_lotes_cve = new List<DE_EntradasCAD>();
            string[] caducidades = lotes_cve.Split(',');
            foreach (var item in caducidades)
            {
                string[] lot_ = item.Split('~');
                DE_EntradasCAD de_en = new DE_EntradasCAD();

                de_en.PK_ARTICULOS = item_insumo.deentradas.PK_ARTICULOS;
                de_en.Lote = lot_[0].ToString();
                de_en.Caducidad = Convert.ToDateTime(lot_[1].ToString());
                de_en.pk_caducidades = lot_[1].ToString(); // Este es temporal por usar la propiedad en tipo cadena para maniobras con Java script
                //de_en.f_caducidad = lot_[1].ToString();
                de_en.Cantidad = Convert.ToInt32(lot_[2].ToString());
                de_lotes_cve.Add(de_en);
            }

            item_insumo.deEntradasCad = de_lotes_cve;

            ReturnModelClass mdl = new ReturnModelClass();
            mdl = repo.addInsumo(ref detalle_, item_insumo, editando_insumo);
            Session["detalle_insumos"] = detalle_;


            return Json(mdl, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarCVE(string pk)
        {
            List<DeEntradasClassSIAA> detalle_ = Session["detalle_insumos"] as List<DeEntradasClassSIAA>;
            DeEntradasClassSIAA rowElimiar = detalle_.SingleOrDefault(a => a.deentradas.PK_ARTICULOS == pk);

            RepoEntradas repo = new RepoEntradas();
            ReturnModelClass rmodel = repo.deleteInsumo(ref detalle_, rowElimiar);

            return Json(rmodel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditarCVE(string pk)
        {
            List<DeEntradasClassSIAA> detalle_ = Session["detalle_insumos"] as List<DeEntradasClassSIAA>;
            DeEntradasClassSIAA rowEDitar = detalle_.SingleOrDefault(a => a.deentradas.PK_ARTICULOS == pk);

            //RepoEntradas repo = new RepoEntradas();
            //ReturnModelClass rmodel = repo.deleteInsumo(ref detalle_, rowElimiar);

            return Json(rowEDitar, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AfectarEntrada(int Folio, int Anio, int Id_Centro)
        {
            string Proceso = "A";
            int Usr = SessionPersister.CurrentUser.id_user;
            RepoEntradas repo = new RepoEntradas();
            ReturnModelClass rmodel = repo.afectarEntrada(Proceso, Folio, Anio, Id_Centro, Usr);

            return Json(rmodel, JsonRequestBehavior.AllowGet);
        }


    }
}
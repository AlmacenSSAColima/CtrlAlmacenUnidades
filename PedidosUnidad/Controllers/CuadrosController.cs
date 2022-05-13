using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using PedidosUnidad.Models;
using PedidosUnidad.Security;
using ClosedXML.Excel;

namespace PedidosUnidad.Controllers
{
    public class CuadrosController : Controller
    {
        // GET: Cuadros
        public ActionResult Index()
        {
            return View();
        }
        
        [XAuthorizeAtribute]
        public ActionResult CMedicamento()
        {

            CurrentUser user = SessionPersister.CurrentUser;
            if (user.navegador == "Movil" && (user.id_rol == 1 || user.id_rol == 3))
            {
                // /Existencia/MonitorInsumos?tipo = 1

                //return View(cuadroMdl);
                return RedirectToAction("MonitorInsumos", "Existencia", new { tipo = 1 });
            }
            else
            {
                RepoCuadros rep = new RepoCuadros();
                CuadroGeneralModel cuadroMdl = rep.getCuadroMedicamento();
                return View(cuadroMdl);
            }

            //return View(cuadroMdl);
        }

        [XAuthorizeAtribute]
        public ActionResult CMaterialCuracion()
        {
            CurrentUser user = SessionPersister.CurrentUser;
            if (user.navegador == "Movil" && (user.id_rol == 1 || user.id_rol == 3))
                {
                    // /Existencia/MonitorInsumos?tipo = 2
                  return RedirectToAction("MonitorInsumos", "Existencia", new { tipo = 2 });
                }
            else
            {
            RepoCuadros rep = new RepoCuadros();
            CuadroGeneralModel cuadroMdl = rep.getCuadroMaterialCuracion();                
            return View(cuadroMdl);
            }
    }

        public ActionResult getInsumo(string cve)
        {
            RepoCuadros rep = new RepoCuadros();
            RowsCuadroGeneralModel cuadroMdl = new RowsCuadroGeneralModel();
            int anio = 2019;
            string[] cveT = cve.Split('.');
            int tipo = Convert.ToInt32(cveT[0]);

            //MATERIAL DE CURACION DE LO CONTRARIO MEDICAMENTO
            if(tipo == 60 || tipo == 70 || tipo == 80)
                cuadroMdl = rep.getInsumoMaterialCuracion(anio,cve);
            else
                cuadroMdl = rep.getInsumoMedicamento(anio, cve);

            return View(cuadroMdl);
        }

        public JsonResult getSalidas(string pk_)
        {
            RepoCuadros rep = new RepoCuadros();
            
            List<SalidasClass> data = new List<SalidasClass>();
            data = rep.getSalidasInsumo(pk_);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getEntradas(string pk_)
        {
            RepoCuadros rep = new RepoCuadros();

            List<EntradasClass> data = new List<EntradasClass>();
            data = rep.getEntradasInsumo(pk_);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getConsumoUltimoMes(string pk_)
        {
            RepoCuadros repo = new RepoCuadros();

            TotalUltimoPedidoClass mdl = repo.getTotalUltimaSalida(pk_);
            return Json(mdl, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getEntradasByAnioCve(string pk_)
        {
            RepoCuadros repo = new RepoCuadros();
            TotalUltimoPedidoClass mdl = new TotalUltimoPedidoClass();
            int Total = repo.getEntradasByAnio(pk_);
            return Json(Total, JsonRequestBehavior.AllowGet);
        }

        public ActionResult reportPedidoMedicamento(int tipo)
        {
            RepoCuadros repC = new RepoCuadros();
            List<ProyeccionPedidoClass> mdl = new List<ProyeccionPedidoClass>();
            //STRING NOMBRE DE ARCHIVO
            string name_file = "";
            //MEDICAMENTO O MATERIA DE CURACION
            if (tipo == 1)
            {
                mdl = repC.getPedidoMedicamento();
                name_file = "Pedido_Medicamento";
            }
                
            else
            {
                mdl = repC.getPedidoMaterialCuracion();
                name_file = "Pedido_Material";
            }
                

            var gv = new GridView();
            gv.DataSource = mdl;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Proyeccion_" + name_file + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

            return View(mdl);
        }
    }
}
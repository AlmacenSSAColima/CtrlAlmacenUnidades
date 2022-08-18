using ClosedXML.Excel;
using PedidosUnidad.Models;
using PedidosUnidad.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PedidosUnidad.Controllers
{
    public class ExistenciaController : Controller
    {
        // GET: Existencia
        [XAuthorizeAtribute]
        public ActionResult Index()
        {
            return View();
        }

        [XAuthorizeAtribute]
        public ActionResult ExistenciaPFMI()
        {
            
            CuadroMonitoreoClass mdl = new CuadroMonitoreoClass();
            RepoCuadros repo = new RepoCuadros();

            mdl = repo.GetTodoCuadroPFMI(2);
            return View(mdl);
        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult ExistenciaDia()
        {
            RepoUtilerias objUtils = new RepoUtilerias(); 
            
            RepoExistencias objExistencias = new RepoExistencias();  

            CurrentUser user = SessionPersister.CurrentUser;
            ExistenciaDiaria mdl_existencia = objExistencias.getExistencia(user, null, null);
            ViewBag.SelectListMovimietos = objUtils.getSelectListTiposMov(null);

            //VALIDAR SI TIENE UN CPM ASIGNADO
            if (!mdl_existencia.tieneCPM)
            {
                return View("SinCPM");
            }

            return View(mdl_existencia);
        }

        [XAuthorizeAtribute]
        [HttpPost]
        public JsonResult ExistenciaDia(FormCollection frm)
        {
            PedidoModel mdl = new PedidoModel();
            string editando = frm["flag_edit"].ToString();
            mdl.editando = Convert.ToBoolean(editando);
            mdl.id_pedido = Convert.ToInt32(frm["id_pedido"].ToString());
            mdl.id_unidad = Convert.ToInt32(frm["id_unidad"].ToString());
            mdl.fecha = frm["fecha_afectar"].ToString(); 

            string input_pks = frm["valores_pks"].ToString();
            string[] pks = input_pks.Split(',');

            foreach (var item in pks)
            {
                string pk = item;//frm["pk_" + item].ToString();
                string existencias = frm["exist_" + item].ToString();
                string cpm = frm["cpm_" + item].ToString();
                string cve = frm["cve_" + item].ToString(); 
                string tipo_mov = frm["mov_" + item].ToString();
                string des_mov = frm["desMov_" + item].ToString();

                existencias = string.IsNullOrEmpty(existencias) ? "0" : existencias;
                CPMrowModel row = new CPMrowModel();
                row.pk = pk;
                row.clave = cve;
                row.cpm = Convert.ToInt32(cpm);
                row.existencia = Convert.ToInt32(existencias);
                row.tipo_mov = Convert.ToInt32(tipo_mov);
                row.des_mov = des_mov;
                mdl.articulos.Add(row);
            }

            CurrentUser user = SessionPersister.CurrentUser;
            RepoExistencias objE = new RepoExistencias();

            ResultClass Rmdl = new ResultClass();
            if (mdl.editando)
                Rmdl = objE.editExistencia(mdl, user);
            else
                Rmdl = objE.saveExistencia(mdl, user);

            return Json(Rmdl, JsonRequestBehavior.AllowGet);
        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult Report(int tipo)
        {
            RepoUtilerias objUtils = new RepoUtilerias();

            RepoExistencias objExistencias = new RepoExistencias();

            CurrentUser user = SessionPersister.CurrentUser;
            ExistenciaDiaria mdl_existencia = objExistencias.getExistencia(user, null, null);

            if (tipo == 1)
            {
                //MEDICAMENTO TIPO = 1
                mdl_existencia.Rows = (from r in mdl_existencia.Rows where r.tipo == 1 select r).ToList();
                mdl_existencia.tipo_listado = "Medicamento";

            }
            else
            {
                //MEDICAMENTO TIPO = 2
                mdl_existencia.Rows = (from r in mdl_existencia.Rows where r.tipo == 2 select r).ToList();
                mdl_existencia.tipo_listado = "Material de curación";
            }


            //VALIDAR SI TIENE UN CPM ASIGNADO
            if (!mdl_existencia.tieneCPM)
            {
                return View("SinCPM");
            }

            //return View("Report",mdl_existencia);

            var pdfResult = new Rotativa.ViewAsPdf("Report", mdl_existencia)
            {
                FileName = mdl_existencia.tipo_listado + "Report.pdf",
                PageMargins = { Left = 10, Bottom = 15, Right = 10, Top = 10 },
                CustomSwitches =
            "--footer-right \" " + "  Page: [page]/[toPage]\"" +
          " --footer-font-size \"9\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };
            return pdfResult;

        }

        public ActionResult ReporteExistencias(int tipo)
        {
            RepoUtilerias objUtils = new RepoUtilerias();

            RepoExistencias objExistencias = new RepoExistencias();

            CurrentUser user = SessionPersister.CurrentUser;
            ReporteExistenciaClass mdl_existencia = objExistencias.reporteExistencias(user);

            //return View(mdl_existencia);
            var pdfResult = new Rotativa.ViewAsPdf("ReporteExistencias", mdl_existencia)
            {
                FileName = "EXISTENCIAS " + mdl_existencia.unidad + ".pdf",
                PageMargins = { Left = 10, Bottom = 15, Right = 10, Top = 10 },
                CustomSwitches =
            "--footer-right \" " + "  Page: [page]/[toPage]\"" +
          " --footer-font-size \"9\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };
            return pdfResult;
        }

        [XAuthorizeAtribute]
        [HttpPost]
        public ActionResult ReadExistenciasDoc(FormCollection frm)
        {
            ResultClass Rmdl = new ResultClass();
            List<DocExistenciasClass> mdlExis = new List<DocExistenciasClass>();

            //PROCESAR ARCHIVO DE EXISTENCIAS
            string[] lines = System.IO.File.ReadAllLines(@"C:\archivo_cvs.txt");
            System.Console.WriteLine("Contenido del archivo = ");
            foreach (string line in lines)
            {
                DocExistenciasClass rowLine = new DocExistenciasClass();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    string[] dts = line.Split(',');
                    rowLine.pk = dts[0].ToString();
                    rowLine.existencia = Convert.ToInt32(dts[1].ToString());
                    mdlExis.Add(rowLine);
                }                
            }

            //--------------------
            PedidoModel mdl = new PedidoModel();
            string editando = frm["flag_edit"].ToString();
            mdl.editando = Convert.ToBoolean(editando);
            mdl.id_pedido = Convert.ToInt32(frm["id_pedido"].ToString());
            mdl.id_unidad = Convert.ToInt32(frm["id_unidad"].ToString());
            mdl.fecha = frm["fecha_afectar"].ToString();

            //string input_pks = frm["valores_pks"].ToString();
            //string[] pks = input_pks.Split(',');

            foreach (var item in mdlExis)
            {
                string pk = item.pk;
                string existencias = item.existencia.ToString();
                string cpm = frm["cpm_" + item].ToString();
                string cve = frm["cve_" + item].ToString();
                string tipo_mov = frm["mov_" + item].ToString();
                string des_mov = frm["desMov_" + item].ToString();

                existencias = string.IsNullOrEmpty(existencias) ? "0" : existencias;
                CPMrowModel row = new CPMrowModel();
                row.pk = pk;
                row.clave = cve;
                row.cpm = Convert.ToInt32(cpm);
                row.existencia = Convert.ToInt32(existencias);
                row.tipo_mov = Convert.ToInt32(tipo_mov);
                row.des_mov = des_mov;
                mdl.articulos.Add(row);
            }

            CurrentUser user = SessionPersister.CurrentUser;
            RepoExistencias objE = new RepoExistencias();

            if (mdl.editando)
                Rmdl = objE.editExistencia(mdl, user);
            else
                Rmdl = objE.saveExistencia(mdl, user);



            return Json(Rmdl, JsonRequestBehavior.AllowGet);
        }

        [XAuthorizeAtribute]
        public ActionResult MonitorInsumos(int tipo)
        {
            CuadroMonitoreoClass mdl = new CuadroMonitoreoClass();
            RepoCuadros repo = new RepoCuadros();

            mdl = repo.GetCuadroMonitoreo(tipo);


            return View("MonitorMovil", mdl);
        }

        [XAuthorizeAtribute]
        public ActionResult MonitorTodosInsumos(int tipo)
        {
            CuadroMonitoreoClass mdl = new CuadroMonitoreoClass();
            RepoCuadros repo = new RepoCuadros();

            mdl = repo.GetTodoCuadroMonitoreo(tipo);


            return View("MonitorMovil", mdl);
        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult GetSolicitadoVsSurtido(string cve, string unidad)
        {
            RepoCuadros repo = new RepoCuadros();
            List<RowRepoInsumos> mdl =  repo.getSolicitudVSsurtido(cve, unidad);

            return View(mdl);
        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult ExistenciaDiaGerente(int tipo)
        {
            RepoUtilerias objUtils = new RepoUtilerias();

            RepoExistencias objExistencias = new RepoExistencias();

            CurrentUser user = SessionPersister.CurrentUser;
            user.tipo = tipo;
            ExistenciaDiaria mdl_existencia = objExistencias.getExistencia(user, null, null);
            ViewBag.SelectListMovimietos = objUtils.getSelectListTiposMov(null);

            //VALIDAR SI TIENE UN CPM ASIGNADO
            if (!mdl_existencia.tieneCPM)
            {
                return View("SinCPM");
            }

            return View("ExistenciaDia", mdl_existencia);
        }

        [XAuthorizeAtribute]
        public ActionResult ExistenciasByUnidad(int tipo, int unidad_)
        {
            CurrentUser user = SessionPersister.CurrentUser;

            RepoExistencias objExistencias = new RepoExistencias();
            ExistenciaDiaria mdl_existencia = objExistencias.getExistencia(user, tipo, unidad_);
            mdl_existencia.nom_unidad = objExistencias.GetInicialesUnidad_(unidad_);

            //VALIDAR SI TIENE UN CPM ASIGNADO
            if (!mdl_existencia.tieneCPM)
            {
                return View("SinCPM");
            }

            return View(mdl_existencia);
        }

        [XAuthorizeAtribute]
        public ActionResult MenuExistenciasEnAlmacen()
        {
            return View();
        }

        [XAuthorizeAtribute]
        public ActionResult ExistenciasEnALmacenByTipo(int tipo_insumo)
        {
            List<InsumoClassSIAA> mdl_list = new List<InsumoClassSIAA>();
            RepoExistencias repo = new RepoExistencias();
            RepoEntradas repo_ = new RepoEntradas();
            bool incluir_lotes = false;
            mdl_list = repo.GetExistenciasDeAlmacenByTipo(tipo_insumo, incluir_lotes);

            AddInsumoForm mdl = new AddInsumoForm();
            mdl.clavesCat = mdl_list;
            mdl.cantidad_claves = mdl_list.Count();
            mdl.tipo_insumo = repo_.getTipoInsumo(tipo_insumo);
            mdl.tipo_int = tipo_insumo;

            return View(mdl);
        }

        public ActionResult GetLotesByPkInsumo(string pk)
        {
            RepoExistencias repo = new RepoExistencias();

            List<LotesInfoClass> mdl = repo.GetLotesByPkInsumo(pk);

            
            return View(mdl);
        }

        public JsonResult Reporte(int tipo_insumo)
        {
            List<InsumoClassSIAA> mdl_list = new List<InsumoClassSIAA>();
            RepoExistencias repo = new RepoExistencias();
            RepoEntradas repo_ = new RepoEntradas();
            bool incluir_lotes = true;
            mdl_list = repo.GetExistenciasDeAlmacenByTipo(tipo_insumo, incluir_lotes);

            AddInsumoForm mdl = new AddInsumoForm();
            mdl.clavesCat = mdl_list;
            mdl.cantidad_claves = mdl_list.Count();
            string desc_tipo_insumo = repo_.getTipoInsumo(tipo_insumo);

            string nameDocto = desc_tipo_insumo;



            //Creating DataTable  
            DataTable dt = new DataTable();
            
            //Setiing Table Name  
            dt.TableName = nameDocto;

            dt.Columns.Add("no", typeof(string));
            dt.Columns.Add("clave", typeof(string));
            dt.Columns.Add("descripcion", typeof(string));
            dt.Columns.Add("datos", typeof(string));
            dt.Columns.Add("cantidades", typeof(int));
            //Add Rows in DataTable  
            int no_c = 1;
            string pk_curso = "pk_init";
            int no_row = 2;

            //List<rows_cab> rows_cab = new List<rows_cab>();
            //int[] myNum;
            List<int> rows_cab = new List<int>();
            foreach (var item in mdl_list)
            {
                //SI ES DIFERENTE ES RENGLON PRINCIPAL
                if (pk_curso != item.pk)
                {
                    //TOTAL DE EXISTENCIAS EN ALMACEN
                    dt.Rows.Add(no_c, item.clave_txt,item.descripcion,"Existencias en almacén:", item.existencia);                    
                    rows_cab.Add(no_row);
                    no_row = no_row + 1;

                    //TOTAL DE EXISTENCIAS EN ALMACEN MENOS LAS QUE ESTAN EN PROCESO
                    int SumaEnProceso = mdl_list.Where(m => m.pk == item.pk).Sum(x => x.En_proceso);
                    dt.Rows.Add(no_c, item.clave_txt, item.descripcion, "Existencias menos en proceso:", (item.existencia - SumaEnProceso));
                    //rows_cab.Add(no_row);
                    no_row = no_row + 1;

                    //CONSECUTIVO
                    no_c = no_c + 1;
                }

                string programa_d = ( string.IsNullOrEmpty(item.Programa) ? "" : (", Programa: " + item.Programa));
                string en_proceso = (item.En_proceso <= 0 ? "" : ", En proceso: "+ item.En_proceso);
                string en_unidades = (string.IsNullOrEmpty(item.CS_Apartados) ? "" : " (" + item.CS_Apartados + ")"); 
                string datos_lote = "Lote: " + item.lote + ", Cad: "+ item.caducidad + en_proceso + en_unidades + programa_d;
            
                dt.Rows.Add((no_c-1).ToString(), item.clave_txt, item.descripcion, datos_lote, item.Existencia_lote);

                pk_curso = item.pk;
                no_row = no_row + 1;

            }




            dt.AcceptChanges();

            //DataTable dt = getData();
            //Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();
            string f_ = String.Format("{0:ddMMyy}", DateTime.Now);
            string fileName = "Existencias_" + desc_tipo_insumo + "_" + f_;



            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet  
                wb.Worksheets.Add(dt);

                //var workbook = new XLWorkbook();
                var worksheet = wb.Worksheets.First(); //workbook.Worksheets.Add("Sheet 1");

                //CABECERO (TITULOS)
                worksheet.Range("A1:E1").Style
                 .Font.SetFontSize(13)
                 .Font.SetBold(true)
                 .Font.SetFontColor(XLColor.White)
                 .Fill.SetBackgroundColor(XLColor.Gray);

                //TODO FONDO BLANCO
                string mat = "A2:E" + no_row.ToString();
                worksheet.Range(mat).Style.Fill.SetBackgroundColor(XLColor.White);

                worksheet.Columns("C").Width = 80;
                worksheet.Columns("D").Width = 50;

                for (int i = 1; i < no_row; i++)
                {
                    
                    //LINEAS DE DATOS DE INSUMO Y TOTAL EXISTENCIA ALMACEN
                    if (rows_cab.Contains(i))
                    {
                        string n_r = i.ToString();//rows_cab[i].ToString();
                        string rango_cel = "A" + n_r + ":E" + n_r;
                        worksheet.Range(rango_cel).Style
                            .Font.SetBold(true)
                            .Border.SetTopBorder(XLBorderStyleValues.Medium);

                        worksheet.Cell("E" + n_r.ToString()).Style.Font.SetFontSize(15);
                    }
                    else
                    {
                        //RENGLONES DE LOTES Y CANTIDADES
                        string rango_cel = "A" + i + ":C" + i;
                        worksheet.Range(rango_cel).Style.Font.SetFontColor(XLColor.White);
                    }
                    
                }



                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.Position = 0;
                    TempData[handle] = memoryStream.ToArray();
                }
            }

            //return new JsonResult()
            //{
            //    Data = new { FileGuid = handle, FileName = fileName }
            //};

            return Json(new { FileGuid = handle, FileName = fileName }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName + ".xls");
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HiQPdf;
using PedidosUnidad.Models;
using PedidosUnidad.Security;
using PedidosUnidad.Utils;
using PedidosUnidad.Models.DBPedido;
using QRCoder;

namespace PedidosUnidad.Controllers
{
    public class PedidoController : Controller
    {
        private ConvertHtmlToString GenerarPDF { get; set; }

        public PedidoController()
        {
            if (GenerarPDF == null)
            {
                GenerarPDF = new ConvertHtmlToString();
                GenerarPDF.TituloSistema = "";
            }
        }

        // GET: Pedido
        [XAuthorizeAtribute]
        public ActionResult Index()
        {
            CurrentUser user = SessionPersister.CurrentUser;
            RepoPedidos objP = new RepoPedidos();
            CPMpedidosModel mdl = objP.getPedidos(user.id_unidad, user.tipo);
            mdl.tipo_pedido = user.tipo;
            if (TempData["mdlReturn"] != null)
            {
                ResultClass mdlR = TempData["mdlReturn"] as ResultClass;
                mdl.show_msg = true;
                mdl.exito = mdlR.exito;
                mdl.msg = mdlR.msg;
            }
            return View(mdl);
        }

        [HttpGet]
        public ActionResult MesesOrdinario( bool controlado)
        {
            CurrentUser user = SessionPersister.CurrentUser;
            int id_unidad = user.id_unidad; 
            int tipo = user.tipo;

            RepoPedidos objP = new RepoPedidos();
            List<mesesCls> meses =  objP.getMesesOrdinario(id_unidad, tipo, controlado);
            return View(meses);
        }

        public JsonResult MesesExtraordinario(bool controlado)
        {
            CurrentUser user = SessionPersister.CurrentUser;
            int id_unidad = user.id_unidad;
            int tipo = user.tipo;

            ResultDatosClass mdl = new ResultDatosClass();

            RepoPedidos objP = new RepoPedidos();
            bool flag  = objP.getOrdinarioMes(id_unidad, tipo, controlado);

            mdl.flag = flag;
            mdl.tipo = tipo; // 1 MEDICAMENTO, 2 MATERIAL 
            mdl.pedido = 2;   //  1 ORDINARIO, 2 EXTRAORDINARIO 
            mdl.mes = DateTime.Now.Month;
            mdl.controlado = controlado;

            return Json(mdl, JsonRequestBehavior.AllowGet);
        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult Pedido(int tipo, int pedido, int mes, bool controlado )
        {
            //tipo = 1 MEDICAMENTO, 2 MATERIAL pedido  1 ORDINARIO, 2 EXTRAORDINARIO  mes = 3 no mes,  controlado True/False

            RepoUtilerias objUtils = new RepoUtilerias();
            bool sePuedeRealizarPedido = objUtils.sePuedeRealizarPedido();
            sePuedeRealizarPedido = true;
            //SI ESTA DENTRO DE LAS FECHAS PERMITIDAS PARA REALIZAR PEDIDO
            if (sePuedeRealizarPedido)
            {
                RepoPedidos objPedidos = new RepoPedidos();
                CurrentUser user = SessionPersister.CurrentUser;
                PedidoModel mdl = new PedidoModel();
                mdl.mes_ordinario = mes; //numero de mes seleccionado para ordinario
                mdl.controlado = controlado; //flag para saber si seran medicamentos controlados

                //mdl = objPedidos.getPedidoByExistencia(user.id_unidad, tipo);
                mdl.tipo_pedido = tipo; // TIPO DE PEDIDO
                if(pedido == 1)
                    mdl.pedido = "ORDINARIO";
                else
                    mdl.pedido = "EXTRAORDINARIO";

                //VALIDAR SI TIENE UN CPM ASIGNADO
                mdl.tieneCPM = true; //POR DE FAULT EN TRUE PARA QUE SEA ABIERTO  Y NO SOLO A UN CUADRO ASIGNADO.
                if (!mdl.tieneCPM)
                {
                    return View("SinCPM");
                }

                //SI YA TIENE UNPEDIDO EN CURSO PARA EL MES CORRESPONDIENTE, MOSTRAR INFORMACION DE QUE YA EXISTE
                if (mdl.existe_pedido)
                {
                    return View("ExistePedido", mdl);
                }

                //ExistePedidoModel existePedido = objUtils.existePedido(user.id_unidad);

                //SI EXISTE PEDIDO OBTENER LOS DATOS DEL QUE ESTAEN CURSO
                //if (existePedido.flag)
                //    mdl = objPedidos.getPedido(existePedido.pedido.id, existePedido.pedido.id_unidad);
                //else
                //    mdl = objPedidos.getCPMunidad(user.id_unidad);

                return View(mdl);
            }
            else {
                //DE LO CONTRARIO ACCESO DENEGADO
                return RedirectToAction("PedidoDenied", "Home");
            }

            
        }

        [XAuthorizeAtribute]
        [HttpPost]
        public JsonResult Pedido(FormCollection frm)
        {
            PedidoModel mdl = new PedidoModel();
            string editando = frm["flag_edit"].ToString();
            mdl.editando = Convert.ToBoolean(editando);
            mdl.id_pedido = Convert.ToInt32(frm["id_pedido"].ToString());
            mdl.id_unidad = Convert.ToInt32(frm["id_unidad"].ToString());
            mdl.tipo_pedido = Convert.ToInt32(frm["tipo_pedido"].ToString());
            mdl.tipo_guardado= Convert.ToInt32(frm["tipo_guardado"].ToString());
            mdl.pedido = frm["pedido"];
            mdl.controlado = Convert.ToBoolean(frm["controlado"].ToString());
            mdl.mes_ordinario = Convert.ToInt32(frm["mes_ordinario"].ToString());

            string input_pks = frm["valores_pks"].ToString();
            string[] pks = input_pks.Split(',');

            int con_cero = 0;
            foreach(var item in pks)
            {
             
                string presentacion_ = frm["presentacion_insumo_" + item];
                string pk = item;//frm["pk_" + item].ToString();
                string cpm = frm["cpm_" + item].ToString();
                string existencias = frm["exist_" + item].ToString();
                string solicita = frm["pedi_" + item].ToString();
                string origen = frm["origen_" + item].ToString();
                string cve = frm["cve_" + item].ToString();
                string descrip = frm["desc_" + item].ToString();
                string presentacion = frm["presentacion_insumo_" + item].ToString();
                string subtipo_insumo = frm["subtipo_insumo_" + item].ToString();

                int cantidad_solicitada = Convert.ToInt32(solicita);
                if (cantidad_solicitada <= 0)
                {
                    con_cero = con_cero + 1;
                }

                CPMrowModel row = new CPMrowModel();
                row.pk = pk;
                row.cpm = Convert.ToInt32(cpm);
                row.existencia = Convert.ToInt32(existencias);
                row.solicita = Convert.ToInt32(solicita);
                row.clave = cve;
                row.descripcion = descrip;
                row.origen = origen;
                row.presentacion = presentacion;
                row.subtipo_insumo = subtipo_insumo;
                mdl.articulos.Add(row); 

                
            }

            //VALIDAR QUE TODAS LA CLAVES TENGAN UN NUMERO MAYO A 0 A SOLICITAR
            if (con_cero > 0)
            {
                mdl.exito = false;
                mdl.msg = "Existen "+ con_cero + " claves con cantidad en cero";
            }
            else
            {
                //
                CurrentUser user = SessionPersister.CurrentUser;
                RepoPedidos objP = new RepoPedidos();

                if (mdl.editando)
                    mdl = objP.editPedido(mdl, user);
                else
                    mdl = objP.savePedido(mdl, user);

                //1 => pedido enviado
                if (mdl.tipo_guardado == 1)
                    Session["result_pedido"] = mdl;
            }

           

            
            return Json(mdl, JsonRequestBehavior.AllowGet);
            //return View("ResultPedido", mdl);
        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult ResultPedido()
        {
            PedidoModel mdl = Session["result_pedido"] as PedidoModel;
            Session["result_pedido"] = mdl;
            return View("ResultPedido", mdl);
        }

        public ActionResult ReportePedido(int id, int unidad)
        {
            CurrentUser user = SessionPersister.CurrentUser;
            RepoPedidos objP = new RepoPedidos();
            PedidoModel mdl = objP.getPedido(id, unidad);
            mdl.descrip_unidad = user.nom_unidad;

            string txtQRCode = mdl.folio;
            mdl.QR = this.GetQR(txtQRCode);

            //DATOS DE FIRMANTES ENCARGADOS DE LA UNIDAD
            configuracion_unidades dataUnidad = objP.getConfigUnidad(unidad);

            if(user.tipo == 1)
                mdl.encargado = dataUnidad.encargado_medicamentos;
            if(user.tipo == 2)
                mdl.encargado = dataUnidad.encargado_curacion;

            mdl.administrador = dataUnidad.administrador;
            mdl.director = dataUnidad.director;
            //---

            //PedidoModel mdl = Session["result_pedido"] as PedidoModel;
            Session["result_pedido"] = mdl;
            return View("ReportePedido", mdl);
        }

        public JsonResult GetReportePedido(int id, int unidad)
        {
            RepoPedidos objP = new RepoPedidos();
            CurrentUser user = SessionPersister.CurrentUser;

            PedidoModel mdl_ = objP.getPedido(id, unidad);

            PedidoModel mdl = mdl_; // Session["result_pedido"] as PedidoModel;
            string fileName = "Pedido_" + mdl.folio;
            //Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();

            //Generar QR
            string txtQRCode = mdl.folio;
            mdl.QR = this.GetQR(txtQRCode);

            //Domicilio de unidad (datos)
            DatosUnidadClass DomicilioUnidad = objP.getDomicilioUnidad(user.id_unidad);

            mdl.domicilio_unidad = DomicilioUnidad.direccion;
            mdl.cp_unidad = DomicilioUnidad.codpost;
            mdl.rfc_unidad = DomicilioUnidad.rfc;
            mdl.tel_unidad = DomicilioUnidad.tel;
            mdl.estado_unidad = DomicilioUnidad.Entidad;
            mdl.municipio_unidad = DomicilioUnidad.Municipio;

            //DATOS DE FIRMANTES ENCARGADOS DE LA UNIDAD            
            configuracion_unidades dataUnidad = objP.getConfigUnidad(user.id_unidad);

            if (user.tipo == 1)
                mdl.encargado = dataUnidad.encargado_medicamentos;
            if (user.tipo == 2)
                mdl.encargado = dataUnidad.encargado_curacion;

            mdl.administrador = dataUnidad.administrador;
            mdl.director = dataUnidad.director;
            //---

            GenerarPDF.TituloSistema = "PEDIDO: " + mdl.folio;
            string fecha = String.Format("{0:ddd dd MMM yyyy HH:mm}", mdl.fecha_envio);
            GenerarPDF.Fecha = fecha.ToUpper() + " HRS";
            byte[] PDF = GenerarPDF.GenerarPDF("ReportePedido", mdl, this.ControllerContext, PdfPageSize.Letter, PdfPageOrientation.Portrait, 10, 10, 10, 10);
            //return File(PDF, "application/pdf", "ResguardoIndividualdeBienesPDF" + DateTime.Now.ToShortDateString());


            using (MemoryStream memoryStream = new MemoryStream(PDF))
            {
                //wb.SaveAs(memoryStream);
                // Stream stream = new MemoryStream(byteArray);
                memoryStream.Position = 0;
                TempData[handle] = memoryStream.ToArray();
            }

            return Json(new { FileGuid = handle, FileName = fileName }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Reporte(int tipo_insumo)
        {
            CurrentUser user = SessionPersister.CurrentUser;
            RepoPedidos objP = new RepoPedidos();

            PedidoModel mdl = Session["result_pedido"] as PedidoModel;
            string fileName = "Pedido_" + mdl.folio;
            //Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();

            //Generar QR
            string txtQRCode = mdl.folio;
            mdl.QR = this.GetQR(txtQRCode);

            //Domicilio de unidad (datos)
            DatosUnidadClass DomicilioUnidad =  objP.getDomicilioUnidad(user.id_unidad);

            mdl.domicilio_unidad = DomicilioUnidad.direccion;
            mdl.cp_unidad = DomicilioUnidad.codpost;
            mdl.rfc_unidad = DomicilioUnidad.rfc;
            mdl.tel_unidad = DomicilioUnidad.tel;
            mdl.estado_unidad = DomicilioUnidad.Entidad;
            mdl.municipio_unidad = DomicilioUnidad.Municipio;

            //DATOS DE FIRMANTES ENCARGADOS DE LA UNIDAD            
            configuracion_unidades dataUnidad = objP.getConfigUnidad(user.id_unidad);

            if (user.tipo == 1)
                mdl.encargado = dataUnidad.encargado_medicamentos;
            if (user.tipo == 2)
                mdl.encargado = dataUnidad.encargado_curacion;

            mdl.administrador = dataUnidad.administrador;
            mdl.director = dataUnidad.director;
            //---

            GenerarPDF.TituloSistema = "PEDIDO: " + mdl.folio;
            string fecha = String.Format("{0:ddd dd MMM yyyy HH:mm}", mdl.fecha_envio);
            GenerarPDF.Fecha = fecha.ToUpper() + " HRS";
            byte[] PDF = GenerarPDF.GenerarPDF("ReportePedido", mdl, this.ControllerContext, PdfPageSize.Letter, PdfPageOrientation.Portrait, 10, 10, 10, 10);
            //return File(PDF, "application/pdf", "ResguardoIndividualdeBienesPDF" + DateTime.Now.ToShortDateString());


            using (MemoryStream memoryStream = new MemoryStream(PDF))
            {
                //wb.SaveAs(memoryStream);
               // Stream stream = new MemoryStream(byteArray);
                memoryStream.Position = 0;
                TempData[handle] = memoryStream.ToArray();
            }

            return Json(new { FileGuid = handle, FileName = fileName }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReporteVale()
        {
            PedidoModel mdl = Session["result_vale"] as PedidoModel;
            string fileName = "Vale_" + mdl.folio;
            //Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();

            GenerarPDF.TituloSistema = "VALE: " + mdl.folio;
            string fecha = String.Format("{0:ddd dd MMM yyyy HH:mm}", mdl.fecha);
            GenerarPDF.Fecha = fecha.ToUpper();// + " HRS";
            byte[] PDF = GenerarPDF.GenerarPDF("ReporteVale", mdl, this.ControllerContext, PdfPageSize.Letter, PdfPageOrientation.Portrait, 10, 13, 10, 10);
            //return File(PDF, "application/pdf", "ResguardoIndividualdeBienesPDF" + DateTime.Now.ToShortDateString());


            using (MemoryStream memoryStream = new MemoryStream(PDF))
            {
                memoryStream.Position = 0;
                TempData[handle] = memoryStream.ToArray();
            }

            return Json(new { FileGuid = handle, FileName = fileName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteVale_()
        {
            PedidoModel mdl = Session["result_vale"] as PedidoModel;
            return View("ReporteVale", mdl);
        }

        public JsonResult ReporteNegativa()
        {
            PedidoModel mdl = Session["result_vale"] as PedidoModel;
            string fileName = "Vale_" + mdl.folio;
            //Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();

            GenerarPDF.TituloSistema = "VALE: " + mdl.folio;
            string fecha = String.Format("{0:ddd dd MMM yyyy HH:mm}", mdl.fecha);
            GenerarPDF.Fecha = fecha.ToUpper();// + " HRS";
            byte[] PDF = GenerarPDF.GenerarPDF("ReporteNegativa", mdl, this.ControllerContext, PdfPageSize.Letter, PdfPageOrientation.Portrait, 10, 13, 10, 10);
            //return File(PDF, "application/pdf", "ResguardoIndividualdeBienesPDF" + DateTime.Now.ToShortDateString());


            using (MemoryStream memoryStream = new MemoryStream(PDF))
            {
                memoryStream.Position = 0;
                TempData[handle] = memoryStream.ToArray();
            }

            return Json(new { FileGuid = handle, FileName = fileName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReporteNegativa_()
        {
            PedidoModel mdl = Session["result_vale"] as PedidoModel;
            return View("ReporteNegativa", mdl);
        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                //return File(data, "application/vnd.ms-excel", fileName + ".xls");
                return File(data, "application/pdf", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }


        [HttpGet]
        public ActionResult AddInsumo(bool controlado)
        {
            CurrentUser user = SessionPersister.CurrentUser;
            //int tipo = user.tipo;
            int tipo_insumo = user.tipo;

            RepoEntradas repo = new RepoEntradas();
            List<InsumoClassSIAA> mdl_list = repo.getLstInsumos(tipo_insumo, controlado);
            AddInsumoForm mdl = new AddInsumoForm();
            mdl.clavesCat = mdl_list;
            mdl.cantidad_claves = mdl_list.Count();
            mdl.tipo_insumo = repo.getTipoInsumo(tipo_insumo);
            mdl.clavesAgregadas = new List<DeEntradasClassSIAA>();
            return View(mdl);
        }

        [XAuthorizeAtribute]
        [HttpGet]
        public ActionResult GetPedido(int id, int unidad)
        {
            RepoPedidos objP = new RepoPedidos();
            PedidoModel mdl = objP.getPedido(id, unidad);
            return View("Pedido",mdl);
        }

        [XAuthorizeAtribute]
        [HttpPost]
        public ActionResult ConfirmarPedido(FormCollection frm)
        {
            int id = Convert.ToInt32(frm["id_pedido"]);
            int id_unidad = Convert.ToInt32(frm["id_unidad"]);

            RepoPedidos objP = new RepoPedidos();
            ResultClass mdlR = objP.confirmPedido(id, id_unidad);
            TempData["mdlReturn"] = mdlR;
            return RedirectToAction("Index");
        }

        public ActionResult ExistePedido()
        {
            return View();
        }

        public JsonResult GetDatosInsumo(string pk)
        {
            RepoPedidos repo = new RepoPedidos();
            DatosInsumosClass mdl = repo.getDatosInsumos(pk);

            return Json(mdl, JsonRequestBehavior.AllowGet);
        }

        [XAuthorizeAtribute]
        public ActionResult DetallePedido(int id, int unidad, int estatus)
        {
            //VALIDAR QUE EL USUARIO LOGUEADO SEA DE LA UNIDAD QUE DESEA VER EL DETALLE
            CurrentUser user = SessionPersister.CurrentUser;
            if(user.id_unidad != unidad)
            {
                return RedirectToAction("Index");
            }
            else
            {
                //SI ESTA EN ESTATUS 1 O 2 TOMAR DE LA BD CPM PEDIDOS
                if (estatus != 3)
                {
                    RepoPedidos objP = new RepoPedidos();
                    PedidoModel mdl_ = objP.getPedido(id, unidad);

                    //Generar QR
                    string txtQRCode = mdl_.folio;
                    mdl_.QR = this.GetQR(txtQRCode);

                    return View(mdl_);
                }
                else
                {
                    //ESTATUS 3 , YA GENERO PEDIDO TIPO ALMACEN CON FOLIO
                    
                    RepoPedidos objP = new RepoPedidos();
                    PedidoModel mdl_ = objP.getPedido(id, unidad);
                    //GET FOLIO DE AMACEN DE PEDIDO
                    string[] valores_solio = mdl_.folio_almacen.Split('/');
                    int folio = 527;//Convert.ToInt32(valores_solio[0]);
                    int anio = 2022;// Convert.ToInt32(valores_solio[1]);

                    //TOMAR REGISTRO DE LA BD ALMACEN
                    PedidoModel mdl_almacen = objP.getValeAlmacen(anio,folio);

                    

                    //Generar QR
                    string txtQRCode = mdl_.folio;
                    mdl_almacen.QR = this.GetQR(txtQRCode);
                    mdl_almacen.folio = mdl_.folio;
                    mdl_almacen.descrip_tipo_pedido = mdl_.descrip_tipo_pedido;
                    mdl_almacen.pedido = mdl_.pedido;
                    mdl_almacen.id_pedido = mdl_.id_pedido;
                    mdl_almacen.id_unidad = mdl_.id_unidad;

                    //Domicilio de unidad (datos)
                    //mdl_almacen.domicilio_unidad = mdl_.domicilio_unidad;
                    //mdl_almacen.cp_unidad = mdl_.cp_unidad;
                    //mdl_almacen.rfc_unidad = mdl_.rfc_unidad;
                    //mdl_almacen.tel_unidad = mdl_.tel_unidad;
                    //mdl_almacen.estado_unidad = mdl_.estado_unidad;
                    //mdl_almacen.municipio_unidad = mdl_.municipio_unidad;

                    Session["result_vale"] = mdl_almacen;
                    return View("DetallePedidoAlmacen", mdl_almacen);
                }
            }
            
            
        }

        [XAuthorizeAtribute]
        public ActionResult NegativaPedido(int id, int unidad)
        {
            RepoPedidos objP = new RepoPedidos();
            PedidoModel mdl_ = objP.getPedido(id, unidad);
            //GET FOLIO DE AMACEN DE PEDIDO
            string[] valores_solio = mdl_.folio_almacen.Split('/');
            int folio = 527;//Convert.ToInt32(valores_solio[0]);
            int anio = 2022;// Convert.ToInt32(valores_solio[1]);

            //TOMAR REGISTRO DE LA BD ALMACEN
            PedidoModel mdl_almacen = objP.getNegativaAlmacen(anio, folio);



            //Generar QR
            string txtQRCode = mdl_.folio;
            mdl_almacen.QR = this.GetQR(txtQRCode);
            mdl_almacen.folio = mdl_.folio;
            mdl_almacen.descrip_tipo_pedido = mdl_.descrip_tipo_pedido;
            mdl_almacen.pedido = mdl_.pedido;            

            Session["result_vale"] = mdl_almacen;
            return View(mdl_almacen);
        }


        public byte[] GetQR(string txtQRCode)
        {

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtQRCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            //System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
            //imgBarCode.Height = 150;
            //imgBarCode.Width = 150;
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                   return ms.ToArray();
                    //imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }
        }

        public ActionResult negativaUnidad(int id, int anio)
        {
            CurrentUser user = SessionPersister.CurrentUser;
            RepoPedidos objP = new RepoPedidos();
            //PedidoModel mdl_ = objP.getPedido(id, unidad);
            ////GET FOLIO DE AMACEN DE PEDIDO
            ////string[] valores_folio = id.Split('/');
            ////int folio = Convert.ToInt32(valores_folio[0]);
            ////int anio = Convert.ToInt32(valores_folio[1]);

            //TOMAR REGISTRO DE LA BD ALMACEN
            PedidoModel mdl_almacen = objP.getValeAlmacen(anio, id);



            //Generar QR
            string txtQRCode = mdl_almacen.folio_almacen;
            mdl_almacen.QR = this.GetQR(txtQRCode);
            mdl_almacen.folio = mdl_almacen.folio_almacen;
            mdl_almacen.descrip_tipo_pedido = "NEGATIVAS";
            mdl_almacen.pedido = "";
            mdl_almacen.id_pedido = 1;
            mdl_almacen.id_unidad = user.id_unidad;

            //Domicilio de unidad (datos)
            //mdl_almacen.domicilio_unidad = mdl_.domicilio_unidad;
            //mdl_almacen.cp_unidad = mdl_.cp_unidad;
            //mdl_almacen.rfc_unidad = mdl_.rfc_unidad;
            //mdl_almacen.tel_unidad = mdl_.tel_unidad;
            //mdl_almacen.estado_unidad = mdl_.estado_unidad;
            //mdl_almacen.municipio_unidad = mdl_.municipio_unidad;

            Session["result_vale"] = mdl_almacen;
            return View("DetallePedidoAlmacen", mdl_almacen);
        }

        public ActionResult negativaUnidadReporte(int id, int unidad)
        {
            RepoPedidos objP = new RepoPedidos();
            //PedidoModel mdl_ = objP.getPedido(id, unidad);
            //GET FOLIO DE AMACEN DE PEDIDO
            //string[] valores_solio = mdl_.folio_almacen.Split('/');
            int folio = 527;//Convert.ToInt32(valores_solio[0]);
            int anio = 2022;// Convert.ToInt32(valores_solio[1]);

            //TOMAR REGISTRO DE LA BD ALMACEN
            PedidoModel mdl_almacen = objP.getNegativaAlmacen(anio, folio);



            //Generar QR
            string txtQRCode = mdl_almacen.folio_almacen;
            mdl_almacen.QR = this.GetQR(txtQRCode);
            mdl_almacen.folio = mdl_almacen.folio_almacen;
            mdl_almacen.descrip_tipo_pedido = "Negativa";
            mdl_almacen.pedido = "1";

            Session["result_vale"] = mdl_almacen;
            return View("NegativaPedido",mdl_almacen);
        }
    }
}
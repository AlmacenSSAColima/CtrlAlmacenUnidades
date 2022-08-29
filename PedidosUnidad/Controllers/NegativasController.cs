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
    public class NegativasController : Controller
    {

        private ConvertHtmlToString GenerarPDF { get; set; }

        public NegativasController()
        {
            if (GenerarPDF == null)
            {
                GenerarPDF = new ConvertHtmlToString();
                GenerarPDF.TituloSistema = "";
            }
        }

        // GET: Negativas
        [XAuthorizeAtribute]
        public ActionResult Index()
        {
            // GET: Pedido

                CurrentUser user = SessionPersister.CurrentUser;
                RepoPedidos objP = new RepoPedidos();
                PedidosUnidadModelAlmCentral mdl = objP.getPedidosUnidadAlmCentral(user.id_unidad);
                if (TempData["mdlReturn"] != null)
                {
                    ResultClass mdlR = TempData["mdlReturn"] as ResultClass;
                    mdl.show_msg = true;
                    mdl.exito = mdlR.exito;
                    mdl.msg = mdlR.msg;
                }
                return View(mdl);
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
            return View("DetallePedidoAlmacen",mdl_almacen);
        }

        public ActionResult negativaUnidadReporte(int id, int anio)
        {
            RepoPedidos objP = new RepoPedidos();
            ////PedidoModel mdl_ = objP.getPedido(id, unidad);
            ////GET FOLIO DE AMACEN DE PEDIDO
            ////string[] valores_solio = mdl_.folio_almacen.Split('/');
            //int folio = 527;//Convert.ToInt32(valores_solio[0]);
            //int anio = 2022;// Convert.ToInt32(valores_solio[1]);

            //TOMAR REGISTRO DE LA BD ALMACEN
            PedidoModel mdl_almacen = objP.getNegativaAlmacen(anio, id);



            //Generar QR
            string txtQRCode = mdl_almacen.folio_almacen;
            mdl_almacen.QR = this.GetQR(txtQRCode);
            mdl_almacen.folio = mdl_almacen.folio_almacen;
            mdl_almacen.descrip_tipo_pedido = "Negativa";
            mdl_almacen.pedido = "1";

            Session["result_vale"] = mdl_almacen;
            return View("NegativaPedido", mdl_almacen);
        }

        public JsonResult ReporteNegativa()
        {
            PedidoModel mdl = Session["result_vale"] as PedidoModel;
            string fileName = "Negativa_Vale_" + mdl.folio;
            //Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();

            GenerarPDF.TituloSistema = "NEGATIVA DEL VALE: " + mdl.folio;
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


    }


}
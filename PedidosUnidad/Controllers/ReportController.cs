using PedidosUnidad.Models;
using PedidosUnidad.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using ClosedXML.Excel;
using System.Data;

namespace PedidosUnidad.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report

        [HttpGet]
        [XAuthorizeAtribute]
        public ActionResult Index()
        {
            RepoPedidos objPedidos = new RepoPedidos();
            CurrentUser user = SessionPersister.CurrentUser;
            PedidoModel mdl = new PedidoModel();
            mdl = objPedidos.getCPMunidad(user.id_unidad, 0, false);

            RepoExistencias objExistencias = new RepoExistencias();
            ExistenciaDiaria mdl_existencia = objExistencias.getExistencia(user, null, null);

            //MEDICAMENTO TIPO = 1
            mdl_existencia.Rows = (from r in mdl_existencia.Rows where r.tipo == 1 select r).ToList();

            return View(mdl_existencia);
        }

        [XAuthorizeAtribute]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Export(string GridHtml)
        {
            Font blackFont = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.WHITE);
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 20f, 20f);

                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();


                byte[] bytes = stream.ToArray();
                using (MemoryStream stream_ = new MemoryStream())
                {
                    PdfReader reader = new PdfReader(bytes);
                    using (PdfStamper stamper = new PdfStamper(reader, stream_))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase(i.ToString(), blackFont), 568f, 15f, 0);
                        }
                    }
                    bytes = stream_.ToArray();
                }


                string pdf = String.Format("data:application/pdf;base64,{0}", Convert.ToBase64String(stream.ToArray()));
                //string pdf = String.Format("data:application/pdf;base64,{0}", Convert.ToBase64String(bytes));
                ViewData["StreamPDF"] = pdf;
                //return View("PDFViewer");

                return File(stream.ToArray(), "application/pdf", "Grid_1.pdf");
            }
        }

        [XAuthorizeAtribute]
        public ActionResult Reporte()
        {
            RepoUtilerias repo = new RepoUtilerias();
            ViewBag.Unidades = repo.getSelectListUnidades(null);
            return View();
        }

        [HttpPost]
        public JsonResult Reporte(FormCollection frm)
        {
            string f_inicial = frm["fe_inicial"].ToString();
            string f_final = frm["fe_final"].ToString();
            int tipo = Convert.ToInt32(frm["tipo_insumo"].ToString());
            string unidad = frm["unidad_sol"].ToString();
            if (unidad == "")
                unidad = "0";

            string tipo_descrip = "";
            //if (tipo == 1)
            //    tipo_descrip = "Medicamento";
            //if (tipo == 2)
            //    tipo_descrip = "M_Curacion";
            //if (tipo == 3)
            //    tipo_descrip = "M_Curacion";

            //1 medicamento, 2 curacion, 3 laboratorio, 4 radiografico 
            switch (tipo)
            {
                case 1:
                    tipo_descrip = "Medicamento";
                    break;
                case 2:
                    tipo_descrip = "M_Curacion";
                    break;
                case 3:
                    tipo_descrip = "Laboratorio";
                    break;
                case 4:
                    tipo_descrip = "Radiografico";
                    break;
                case 5:
                    tipo_descrip = "Reporte";
                    break;
                case 6:
                    tipo_descrip = "Reporte";
                    break;
                case 7:
                    tipo_descrip = "Reporte";
                    break;
                case 8:
                    tipo_descrip = "Reporte";
                    break;
                default:
                    tipo_descrip = "Reporte";
                    break;
            }


            int centro = Convert.ToInt32(unidad);

            int option = Convert.ToInt32(frm["optionsRadios"].ToString());

            RepoReportes r = new RepoReportes();
            List<ReporteSolicitadoVSEntregado> mdl = r.getRegitros(tipo, centro, f_inicial, f_final, option);


            //Creating DataTable  
            DataTable dt = new DataTable();
            //Setiing Table Name  
            dt.TableName = tipo_descrip;

            if (option == 1)
            {
                dt.Columns.Add("pk_centro", typeof(int));
                dt.Columns.Add("centro", typeof(string));
                dt.Columns.Add("anio", typeof(int));
                dt.Columns.Add("mes", typeof(int));
            }

            if (option == 2)
            {
                dt.Columns.Add("anio", typeof(int));
                dt.Columns.Add("mes", typeof(int));
            }

            //if (option == 3)
            
            //Add Columns              
            //dt.Columns.Add("pk_articulos", typeof(string));
            
            dt.Columns.Add("clave_t", typeof(string));
            dt.Columns.Add("descripcion", typeof(string));
            dt.Columns.Add("solicitado", typeof(int));
            dt.Columns.Add("surtido", typeof(int));

            //Add Rows in DataTable  
            
            if (option == 1)
            {
                foreach (var item in mdl)
                {
                    dt.Rows.Add(item.pk_centro, item.centro, item.anio, item.mes, item.clave_t, item.descripcion, item.solicitado, item.surtido);
                }
            }

            if (option == 2)
            {
                foreach (var item in mdl)
                {
                    dt.Rows.Add(item.anio, item.mes, item.clave_t, item.descripcion, item.solicitado, item.surtido);
                }
            }
            if (option == 3)
            {
                foreach (var item in mdl)
                {
                    dt.Rows.Add( item.clave_t, item.descripcion, item.solicitado, item.surtido);
                }
            }



            dt.AcceptChanges();

            //DataTable dt = getData();

            //Name of File  
            //string fileName = "Sample.xlsx";
            //using (XLWorkbook wb = new XLWorkbook())
            //{
            //    //Add DataTable in worksheet  
            //    wb.Worksheets.Add(dt);
            //    using (MemoryStream stream = new MemoryStream())
            //    {
            //        wb.SaveAs(stream);
            //        //Return xlsx Excel File  
            //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            //    }
            //}


            // Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();
            string f_ = String.Format("{0:ddMMyy}",DateTime.Now);
            string fileName = tipo_descrip+"_"+ f_;



            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet  
                wb.Worksheets.Add(dt);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.Position = 0;
                    TempData[handle] = memoryStream.ToArray();                    
                }
            }

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = fileName }
            };

        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName+".xls");
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
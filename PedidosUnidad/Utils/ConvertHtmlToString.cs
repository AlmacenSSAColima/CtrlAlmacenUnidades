using HiQPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PedidosUnidad.Models;

namespace PedidosUnidad.Utils
{
    public class ConvertHtmlToString
    {

        public String TituloSistema { get; set; }
        public String PiePagina { get; set; }
        public String NombreCompleto { get; set; }

        public String Fecha { get; set; }
        private const String HIQPDF_SERIAL = "s/va4uPX-1f/a0cHS-wcqFnYOT-gpOHk4qL-h5OAgp2C-gZ2KioqK";
        private void SetHeader(PdfDocumentControl htmlToPdfDocument)
        {
            // enable header display
            htmlToPdfDocument.Header.Enabled = true;
            // set header height
            htmlToPdfDocument.Header.Height = 50;
            float pdfPageWidth =
                htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                        htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;
            float headerWidth = pdfPageWidth - htmlToPdfDocument.Margins.Left
                        - htmlToPdfDocument.Margins.Right;
            float headerHeight = htmlToPdfDocument.Header.Height;
            PdfHtml headerHtml = new PdfHtml(0, 0, "http://localhost:1579/Html/Header");
            headerHtml.FitDestHeight = true;
            htmlToPdfDocument.Header.Layout(headerHtml);

            //PdfRectangle borderRectangle = new PdfRectangle(1, 1, headerWidth - 2, headerHeight - 2);
            //borderRectangle.LineStyle.LineWidth = 0.5f;
            ////borderRectangle.ForeColor = System.Drawing.Color.Navy;
            //htmlToPdfDocument.Header.Layout(borderRectangle);
        }
        public string RenderRazorViewToString(string viewName, object model, ControllerContext ctx)
        {
            ctx.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ctx, viewName);
                var viewContext = new ViewContext(ctx, viewResult.View, ctx.Controller.ViewData, ctx.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ctx, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        private HtmlToPdf GetHtmlToPdf(float Bottom, float Top, float Left, float Right)
        {
            HiQPdf.HtmlToPdf docpdf = new HtmlToPdf();
            docpdf.Document.Margins.Bottom = Bottom;
            docpdf.Document.Margins.Top = Top;
            docpdf.Document.Margins.Left = Left;
            docpdf.Document.Margins.Right = Right;            
            docpdf.SerialNumber = HIQPDF_SERIAL;
            return docpdf;
        }
        public byte[] GenerarPDF(string viewName, object model, ControllerContext ctx, PdfPageSize pageSize, PdfPageOrientation orientation, float bottom, float top, float left, float right, footer footer = null, bool hasHeader = false)
        {
            if (footer == null)
            {
                footer = new footer();
                footer.hasFooter = true;
            }
            String View = RenderRazorViewToString(viewName, model, ctx);
            HtmlToPdf docpdf = GetHtmlToPdf(bottom, top, left, right);
            docpdf.Document.PageSize = pageSize == null ? PdfPageSize.A4 : pageSize;
            docpdf.Document.PageOrientation = orientation == null ? PdfPageOrientation.Portrait : orientation;
            if (footer.hasFooter)
                SetFooter(docpdf.Document);
            //SetFooterFirmantes(docpdf.Document, footer);
            if (hasHeader)
                SetHeader(docpdf.Document);
            byte[] archivo = docpdf.ConvertHtmlToPdfDocument(View, "").WriteToMemory();
            return archivo;
        }

        public byte[] GenerarPDF_Blanco(string viewName, object model, ControllerContext ctx)
        {
            String View = RenderRazorViewToString(viewName, model, ctx);
            HtmlToPdf docpdf = GetHtmlToPdf(0, 0, 0, 0);
            byte[] archivo = docpdf.ConvertHtmlToPdfDocument(View, "").WriteToMemory();
            return archivo;
        }


        public byte[] GenerarPDF_Horizontal(string viewName, object model, ControllerContext ctx, PdfPageSize pageSize, PdfPageOrientation orientation, float bottom, float top, float left, float right, bool hasFooter = true, bool hasHeader = false)
        {
            String View = RenderRazorViewToString(viewName, model, ctx);
            HiQPdf.HtmlToPdf docpdf = GetHtmlToPdf(bottom, top, left, right);
            docpdf.Document.PageOrientation = PdfPageOrientation.Landscape;
            SetFooter(docpdf.Document);
            if (hasFooter)
                SetFooter(docpdf.Document);
            if (hasHeader)
                SetHeader(docpdf.Document);
            byte[] archivo = docpdf.ConvertHtmlToPdfDocument(View, "").WriteToMemory();
            return archivo;
        }

        private void SetFooter(PdfDocumentControl htmlToPdfDocument)
        {
            // enable footer display
            htmlToPdfDocument.Footer.Enabled = true;

            // set footer height
            htmlToPdfDocument.Footer.Height = 15;
            // set footer background color
            htmlToPdfDocument.Footer.BackgroundColor = System.Drawing.Color.White;

            float pdfPageWidth = htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                    htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;

            float footerWidth = pdfPageWidth - htmlToPdfDocument.Margins.Left -
                        htmlToPdfDocument.Margins.Right;
            float footerHeight = htmlToPdfDocument.Footer.Height;

            // layout HTML in footer
            if (String.IsNullOrEmpty(TituloSistema)) TituloSistema = "";
            if (String.IsNullOrEmpty(PiePagina)) PiePagina = "{0} {1} {2}";
            if (String.IsNullOrEmpty(NombreCompleto)) NombreCompleto = "";
            if (String.IsNullOrEmpty(Fecha)) Fecha = "";
            
            PdfHtml footerHtml = new PdfHtml(5, 0,
                    String.Format(PiePagina, TituloSistema, NombreCompleto, Fecha), null);
            footerHtml.FitDestHeight = true;
            htmlToPdfDocument.Footer.Layout(footerHtml);

            // add page numbering
            System.Drawing.Font pageNumberingFont =
                            new System.Drawing.Font(
                            new System.Drawing.FontFamily("Times New Roman"),
                            7, System.Drawing.GraphicsUnit.Point);
            PdfText pageNumberingText = new PdfText(footerWidth - 100, 3,
                            "Página {CrtPage} de {PageCount}", pageNumberingFont);
            pageNumberingText.HorizontalAlign = PdfTextHAlign.Center;
            pageNumberingText.EmbedSystemFont = true;
            pageNumberingText.ForeColor = System.Drawing.Color.Gray;
            htmlToPdfDocument.Footer.Layout(pageNumberingText);
        }
        private void SetFooterFirmantes(PdfDocumentControl htmlToPdfDocument, footer footer)
        {
            // enable footer display
            htmlToPdfDocument.Footer.Enabled = true;

            // set footer height
            htmlToPdfDocument.Footer.Height = footer.height;
            // set footer background color
            //htmlToPdfDocument.Footer.BackgroundColor = System.Drawing.Color.WhiteSmoke;

            float pdfPageWidth =
                htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                        htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;
            float headerWidth = pdfPageWidth - htmlToPdfDocument.Margins.Left
                        - htmlToPdfDocument.Margins.Right;
            float headerHeight = htmlToPdfDocument.Header.Height;

            PdfHtml footerHtml = new PdfHtml(0, 0, footer.url);
            footerHtml.FitDestHeight = true;
            htmlToPdfDocument.Footer.Layout(footerHtml);

            // add page numbering
            if (footer.paginado)
            {
                System.Drawing.Font pageNumberingFont =
                            new System.Drawing.Font(
                            new System.Drawing.FontFamily("Times New Roman"),
                            11, System.Drawing.GraphicsUnit.Point);
                PdfText pageNumberingText = new PdfText(headerWidth - 1100, headerHeight + 35,
                                "Pág: {CrtPage}", pageNumberingFont);
                pageNumberingText.HorizontalAlign = PdfTextHAlign.Center;
                pageNumberingText.EmbedSystemFont = true;
                pageNumberingText.ForeColor = System.Drawing.Color.Black;
                htmlToPdfDocument.Footer.Layout(pageNumberingText);
            }
        }

    }
}
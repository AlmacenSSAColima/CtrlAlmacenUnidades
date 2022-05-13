using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PedidosUnidad.Controllers
{
    public class PDFUtilsController : Controller
    {
        // GET: PDFUtils
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Header()
        {
            return View("ReportHeader");
        }
        public ActionResult Footer()
        {
            return View("ReportFooter");
        }
    }
}
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
    }
}
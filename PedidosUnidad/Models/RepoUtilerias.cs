using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PedidosUnidad.Models.DBPedido;
using PedidosUnidad.Models.DBAlmacen;

namespace PedidosUnidad.Models
{
    public class RepoUtilerias
    {
        public bool sePuedeRealizarPedido() {
            bool flag = false;
            try
            {
                PedidoEntity dbPedido = new PedidoEntity();
                parametros_sys p_fe_1 = dbPedido.parametros_sys.SingleOrDefault(a => a.id == 1);
                parametros_sys p_fe_2 = dbPedido.parametros_sys.SingleOrDefault(a => a.id == 2);

                DateTime fe_1 = Convert.ToDateTime(p_fe_1.valor + " 00:00:00");
                DateTime fe_2 = Convert.ToDateTime(p_fe_2.valor + " 23:59:59");

                DateTime hoy = DateTime.Now;

                if ((hoy >= fe_1) && (hoy <= fe_2))
                    flag = true;
                else
                    flag = false;
            }
            catch(Exception e) {
                flag = false;
            }

            return flag;
        }

        public ExistePedidoModel existePedido(int id_unidad)
        {
            ExistePedidoModel mdl = new ExistePedidoModel();
            try {
                PedidoEntity dbPedido = new PedidoEntity();
                List<ma_pedido> pedido = (from a in dbPedido.ma_pedido where a.id_unidad == id_unidad && a.pedido_confirmado == false select a).ToList();
                if (pedido.Count > 0)
                {
                    mdl.flag = true;
                    mdl.pedido = pedido.First();
                }
                else {
                    mdl.pedido = new ma_pedido();
                }
            }
            catch (Exception e) {
            }
            return mdl;
        }

        //Get SelectList de Cat TiposEnt
        public SelectList getSelectListTiposMov(int? id)
        {
            AlmacenEntity dbAlm = new AlmacenEntity();
            List<CA_TIPOSENT> lista = dbAlm.CA_TIPOSENT.ToList();
            if (id == null)
            {
                return new SelectList(lista, "TIPO", "DESCRIP");
            }
            else
            {
                return new SelectList(lista, "TIPO", "DESCRIP", id);
            }
        }

        public SelectList getSelectListUnidades(int? id)
        {
            var InClause = new int[] { 90000, 52, 29, 110000, 80000, 100000, 120000, 10051, 180000, 51, 130000, 081103, 100036, 120040 };
            AlmacenEntity dbAlm = new AlmacenEntity();
            List<CA_CENTROS> lista = (from c in dbAlm.CA_CENTROS where InClause.Contains(c.CENTRO) select c).ToList().OrderBy(c=> c.DESCRIP).ToList();
           
            if (id == null)
            {
                return new SelectList(lista, "CENTRO", "DESCRIP");
            }
            else
            {
                return new SelectList(lista, "CENTRO", "DESCRIP", id);
            }
        }


    }
}
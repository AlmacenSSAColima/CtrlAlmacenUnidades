using PedidosUnidad.Models.DBAlmacen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PedidosUnidad.Models
{
    public class RepoProgramasInfo
    {
        public List<infoProgramaClass> getInfoByLote(string lote)
        {
            List<infoProgramaClass> lista = new List<infoProgramaClass>();
            AlmacenEntity dbA = new AlmacenEntity();
            StringBuilder QueryString = new StringBuilder();

            QueryString.Append("select 'Entrada' Tipo_Mov, '' Unidad, de_entradascad.No_Entrada Num_Mov, de_entradascad.Anio, ma_articulos.Tipo, ma_articulos.Grupo, ma_articulos.clave Clave,ma_articulos.PRESENTACION Presentacion, Cantidad, Fecha, convert(varchar(10),fecha,103) Fecha_Mov, de_entradascad.lote Lote,convert(varchar(10),de_entradascad.caducidad,103) Caducidad, dc.costo Costo ");
            QueryString.Append("from de_entradascad ");
            QueryString.Append("inner join ma_entradas on de_entradascad.no_entrada = ma_entradas.folio and de_entradascad.anio = ma_entradas.anio ");
            QueryString.Append("inner join ma_articulos on de_entradascad.PK_ARTICULOS = ma_articulos.PK_ARTICULOS ");
            QueryString.Append("left join de_caducidad dc on de_entradascad.pk_articulos = dc.pk_articulos and de_entradascad.pk_caducidades = dc.pk_caducidades ");
            QueryString.Append("where de_entradascad.lote like '%"+ lote + "%' ");
            QueryString.Append("union select 'Ajuste / ' + TA Tipo_Mov, '' Unidad, No_Ajuste Num, de_ajustescad.Anio, ma_articulos.Tipo, ma_articulos.Grupo, ma_articulos.clave Clav_Med, ma_articulos.PRESENTACION, cantidad, fecha, convert(varchar(10), fecha, 103) fecha2, de_ajustescad.lote, convert(varchar(10), de_ajustescad.caducidad, 103) caducidad, dc.costo ");
            QueryString.Append("from de_ajustescad inner join ma_ajustes on de_ajustescad.no_ajuste = ma_ajustes.folio and de_ajustescad.anio = ma_ajustes.anio  inner join ma_articulos on de_ajustescad.PK_ARTICULOS = ma_articulos.PK_ARTICULOS left join de_caducidad dc on de_ajustescad.pk_articulos = dc.pk_articulos and de_ajustescad.pk_caducidades = dc.pk_caducidades ");
            QueryString.Append("where de_ajustescad.lote like '%" + lote + "%' ");
            QueryString.Append("union select 'Vale' Tipo_Mov, cc.descrip Unidad, de_pedidoscad.Pedido Num, de_pedidoscad.Anio, ma_articulos.Tipo , ma_articulos.Grupo, ma_articulos.clave Clav_Med, ma_articulos.PRESENTACION, cantidad, fecha_pedido fecha, convert(varchar(10), fecha_pedido, 103) fecha2, de_pedidoscad.lote, convert(varchar(10), de_pedidoscad.caducidad, 103) caducidad, dc.costo ");
            QueryString.Append("from de_pedidoscad inner join ma_pedidos on de_pedidoscad.pedido = ma_pedidos.pedido and de_pedidoscad.anio = ma_pedidos.anio  inner join ma_articulos on de_pedidoscad.PK_ARTICULOS = ma_articulos.PK_ARTICULOS left join de_caducidad dc on de_pedidoscad.pk_articulos = dc.pk_articulos and de_pedidoscad.pk_caducidades = dc.pk_caducidades  left join ca_centros cc on ma_pedidos.centro_sol = cc.centro ");
            QueryString.Append("where de_pedidoscad.lote like '%" + lote + "%' ORDER BY 5,6,7,8,12,13,10,3,4 ");


            try {
                
                lista = dbA.Database.SqlQuery<infoProgramaClass>(QueryString.ToString()).ToList();

            }
            catch (Exception e)
            { 
            
            }

            return lista;
        }

        
    }

    public class infoProgramaClass
    { 
        public string Tipo_Mov { get; set; }
        public string Unidad { get; set; }
        public int Num_Mov { get; set; }//No_Entrada
        public Int16 Anio { get; set; }
        public Int16 Tipo { get; set; }
        public Int16 Grupo { get; set; }
        public Int16 Clave { get; set; }
        public int Presentacion { get; set; }
        public int Cantidad { get; set; }
        public string Fecha_Mov { get; set; }
        public string Lote { get; set; }
        public string Caducidad { get; set; }
        public decimal Costo { get; set; }
    }
}
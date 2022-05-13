using PedidosUnidad.Models.DBAlmacen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PedidosUnidad.Models
{
    public class RepoReportes
    {
        public List<ReporteSolicitadoVSEntregado> getRegitros(int tipo, int centro, string f_1, string f_2, int option)
        {
            List<ReporteSolicitadoVSEntregado> list_ = new List<ReporteSolicitadoVSEntregado>();

            try {
                string QueryString = "";
                if (option == 1)
                    QueryString = this.getOptionPorUnidad( tipo,  centro,  f_1, f_2);
                if (option == 2)
                    QueryString = this.getOptionGeneralMes(tipo, centro, f_1, f_2);
                if (option == 3)
                    QueryString = this.getOptionGeneralTotal(tipo, centro, f_1, f_2);




                AlmacenEntity dbAlm = new AlmacenEntity();
                list_ = dbAlm.Database.SqlQuery<ReporteSolicitadoVSEntregado>(QueryString.ToString()).ToList();

            }
            catch (Exception e) {
                string msg = e.Message;
            }

            return list_;
        }

        public string getOptionPorUnidad(int tipo, int centro, string f_1, string f_2)
        {
            string fecha_1 = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(f_1));
            string fecha_2 = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(f_2));

            StringBuilder QueryString = new StringBuilder();
            QueryString.Append("select ");
            QueryString.Append("pk_centro, centro, pk_articulos, anio, mes, clave_t, descripcion, sum(solicitado) solicitado, sum(surtido) surtido ");

            QueryString.Append("from( select mp.centro_sol pk_centro, cc.descrip centro, dp.pk_articulos, YEAR(mp.fecha_pedido) anio, MONTH(mp.fecha_pedido) mes, ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(dp.tipo as varchar(3))))) + cast(dp.tipo as varchar(3)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(dp.grupo as varchar(3))))) + cast(dp.grupo as varchar(3)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(dp.clave as varchar(4))))) + cast(dp.clave as varchar(4)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(dp.presentacion as varchar(2))))) + cast(dp.presentacion as varchar(2)))) clave_t, ");
            QueryString.Append("ma.descripcion, ");
            QueryString.Append("dp.cantidad solicitado, ");
            QueryString.Append("dp.cant_surt surtido ");
            QueryString.Append("from almacen_central_nvo.dbo.ma_pedidos mp ");
            QueryString.Append("left ");
            QueryString.Append("join almacen_central_nvo.dbo.de_pedidos dp on dp.pedido = mp.pedido and dp.anio = mp.anio ");
            QueryString.Append("left join almacen_central_nvo.dbo.ma_articulos ma on ma.pk_articulos = dp.pk_articulos ");
            QueryString.Append("left join almacen_central_nvo.dbo.ca_centros cc on mp.centro_sol = cc.centro ");
            QueryString.Append("where dp.clave > 0 and mp.afectado = 1 ");

            if (tipo == 1)
                QueryString.Append("and dp.Tipo < 60 ");
            else
                QueryString.Append("and dp.Tipo in (60, 130, 531, 537) ");

            QueryString.Append("and mp.fecha_pedido BETWEEN '" + fecha_1 + " 00:00:01' AND '" + fecha_2 + " 23:59:59' ");
            if (centro == 0)
                QueryString.Append("and mp.centro_sol in (90000, 52, 29, 110000, 80000, 100000, 120000, 10051, 180000, 51, 130000, 081103, 100036, 120040) ");
            else
                QueryString.Append("and mp.centro_sol in (" + centro + ") ");

            QueryString.Append(")f ");
            QueryString.Append("group by pk_centro, centro, pk_articulos, clave_t, descripcion, mes, anio ");
            QueryString.Append("order by anio, mes, centro, descripcion ");

            return QueryString.ToString();
        }


        public string getOptionGeneralMes(int tipo, int centro, string f_1, string f_2)
        {
            string fecha_1 = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(f_1));
            string fecha_2 = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(f_2));

            StringBuilder QueryString = new StringBuilder();
            QueryString.Append(" select pk_articulos, clave_t, descripcion, anio, mes,");
            QueryString.Append(" sum(solicitado) solicitado, sum(surtido) surtido");
            QueryString.Append(" from(");
            QueryString.Append(" select dp.pk_articulos,");
            QueryString.Append("  ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(dp.tipo as varchar(3))))) + cast(dp.tipo as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(dp.grupo as varchar(3))))) + cast(dp.grupo as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(dp.clave as varchar(4))))) + cast(dp.clave as varchar(4)))) + '-' + ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(dp.presentacion as varchar(2))))) + cast(dp.presentacion as varchar(2)))) clave_t,");
            QueryString.Append("  ma.descripcion, dp.cantidad solicitado, dp.cant_surt surtido,");
            QueryString.Append("  YEAR(mp.fecha_pedido) anio, MONTH(mp.fecha_pedido) mes");
            QueryString.Append("  from almacen_central_nvo.dbo.ma_pedidos mp");
            QueryString.Append("  left join almacen_central_nvo.dbo.de_pedidos dp on dp.pedido = mp.pedido and dp.anio = mp.anio");
            QueryString.Append("  left join almacen_central_nvo.dbo.ma_articulos ma on ma.pk_articulos = dp.pk_articulos");
            QueryString.Append("  where dp.clave > 0 and mp.afectado = 1");
            if (tipo == 1)
                QueryString.Append(" and dp.Tipo < 60 ");
            else
                QueryString.Append(" and dp.Tipo in (60, 130, 531, 537) ");

            QueryString.Append(" and mp.fecha_pedido BETWEEN '" + fecha_1 + " 00:00:01' AND '" + fecha_2 + " 23:59:59' ");

            if (centro == 0)
                QueryString.Append(" and mp.centro_sol in (90000, 52, 29, 110000, 80000, 100000, 120000, 10051, 180000, 51, 130000, 081103, 100036, 120040) ");
            else
                QueryString.Append(" and mp.centro_sol in (" + centro + ") ");

            QueryString.Append(" )f ");
            QueryString.Append(" group by pk_articulos, clave_t, descripcion, mes, anio ");
            QueryString.Append(" order by  anio, mes, descripcion ");
            return QueryString.ToString();
        }

        public string getOptionGeneralTotal(int tipo, int centro, string f_1, string f_2)
        {
            string fecha_1 = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(f_1));
            string fecha_2 = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(f_2));

            StringBuilder QueryString = new StringBuilder();
            QueryString.Append(" select pk_articulos, clave_t, descripcion,");
            QueryString.Append(" sum(solicitado) solicitado, sum(surtido) surtido");
            QueryString.Append(" from(");
            QueryString.Append(" select dp.pk_articulos,");
            QueryString.Append(" ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(dp.tipo as varchar(3))))) + cast(dp.tipo as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(dp.grupo as varchar(3))))) + cast(dp.grupo as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(dp.clave as varchar(4))))) + cast(dp.clave as varchar(4)))) + '-' + ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(dp.presentacion as varchar(2))))) + cast(dp.presentacion as varchar(2)))) clave_t,");
            QueryString.Append(" ma.descripcion, dp.cantidad solicitado, dp.cant_surt surtido");
            QueryString.Append(" from almacen_central_nvo.dbo.ma_pedidos mp");
            QueryString.Append(" left join almacen_central_nvo.dbo.de_pedidos dp on dp.pedido = mp.pedido and dp.anio = mp.anio");
            QueryString.Append(" left join almacen_central_nvo.dbo.ma_articulos ma on ma.pk_articulos = dp.pk_articulos");
            QueryString.Append(" where dp.clave > 0 and mp.afectado = 1");
            if (tipo == 1)
                QueryString.Append(" and dp.Tipo < 60 ");
            else
                QueryString.Append(" and dp.Tipo in (60, 130, 531, 537) ");

            QueryString.Append(" and mp.fecha_pedido BETWEEN '" + fecha_1 + " 00:00:01' AND '" + fecha_2 + " 23:59:59' ");

            if (centro == 0)
                QueryString.Append(" and mp.centro_sol in (90000, 52, 29, 110000, 80000, 100000, 120000, 10051, 180000, 51, 130000, 081103, 100036, 120040) ");
            else
                QueryString.Append(" and mp.centro_sol in (" + centro + ") ");

            QueryString.Append(" )f ");
            QueryString.Append(" group by pk_articulos, clave_t, descripcion");
            QueryString.Append(" order by descripcion");
            return QueryString.ToString();
        }
    }
}
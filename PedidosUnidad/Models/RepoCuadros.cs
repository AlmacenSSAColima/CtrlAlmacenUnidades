using PedidosUnidad.Models.DBPedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using PedidosUnidad.Models.DBAlmacen;

namespace PedidosUnidad.Models
{
    public class RepoCuadros
    {
        PedidoEntity dbPedido = null;

        public RepoCuadros()
        {
            dbPedido = new PedidoEntity();
        }

        public CuadroGeneralModel getCuadroMedicamento()
        {
            int anio = 2019;
            int quitar = 0;
            CuadroGeneralModel cuadroMdl = new CuadroGeneralModel();
            try
            {
                //cuadroMdl.rowsCuadro = dbPedido.Database.SqlQuery<RowsCuadroGeneralModel>("dbo.spGetCuadroMedicamento " + anio + "," + quitar).ToList();//.Take(531).ToList();
                cuadroMdl.rowsCuadro = dbPedido.Database.SqlQuery<RowsCuadroGeneralModel>("dbo.spGetExistenciasAlmacenMed " + anio + "," + quitar).ToList();
                cuadroMdl.total_claves = cuadroMdl.rowsCuadro.Count();

                cuadroMdl.total_consolidada = (from r in cuadroMdl.rowsCuadro where r.origen_consolidada == true select r).ToList().Count();
                cuadroMdl.total_dimesa = (from r in cuadroMdl.rowsCuadro where r.origen_dimesa == true select r).ToList().Count();
                cuadroMdl.total_desiertas = (from r in cuadroMdl.rowsCuadro where r.origen_desierta == true select r).ToList().Count();
                cuadroMdl.total_primer_nivel = (from r in cuadroMdl.rowsCuadro where r.nivel_1 == true select r).ToList().Count();
                cuadroMdl.total_segundo_nivel = (from r in cuadroMdl.rowsCuadro where r.nivel_2 == true select r).ToList().Count();

            }
            catch(Exception e) {
                cuadroMdl = new CuadroGeneralModel();
            }
            

            return cuadroMdl;
        }

        public CuadroGeneralModel getCuadroMaterialCuracion()
        {
            int anio = 2019;
            int quitar = 0;
            CuadroGeneralModel cuadroMdl = new CuadroGeneralModel();
            try
            {
                cuadroMdl.rowsCuadro = dbPedido.Database.SqlQuery<RowsCuadroGeneralModel>("dbo.spGetCuadroMaterialCuracion " + anio + "," + quitar).ToList();//.Take(531).ToList();
                //cuadroMdl.rowsCuadro = dbPedido.Database.SqlQuery<RowsCuadroGeneralModel>("dbo.spGetExistenciasAlmacenCur " + anio + "," + quitar).ToList();
                cuadroMdl.total_claves = cuadroMdl.rowsCuadro.Count();

                cuadroMdl.total_consolidada = (from r in cuadroMdl.rowsCuadro where r.origen_consolidada == true select r).ToList().Count();
                cuadroMdl.total_dimesa = (from r in cuadroMdl.rowsCuadro where r.origen_dimesa == true select r).ToList().Count();
                cuadroMdl.total_desiertas = (from r in cuadroMdl.rowsCuadro where r.origen_desierta == true select r).ToList().Count();
                cuadroMdl.total_primer_nivel = (from r in cuadroMdl.rowsCuadro where r.nivel_1 == true select r).ToList().Count();
                cuadroMdl.total_segundo_nivel = (from r in cuadroMdl.rowsCuadro where r.nivel_2 == true select r).ToList().Count();

            }
            catch (Exception e)
            {

            }


            return cuadroMdl;
        }
        

        public RowsCuadroGeneralModel getInsumoMedicamento(int anio, string cve) {
            RowsCuadroGeneralModel mdl = new RowsCuadroGeneralModel();

            try
            {
                List< RowsCuadroGeneralModel> row = dbPedido.Database.SqlQuery<RowsCuadroGeneralModel>("dbo.spGetMedicamentoByClave " + anio + ", '" + cve + "'").ToList();
                //si no esta con esa clave posiblemente es de los registros que tienen equivalencia en el campo cve_sistema en almacen
                if (row.Count == 0)
                {
                    string query = " select cve_sist from cuadro_general_material_curacion where cve = '" + cve + "'";//060.066.1052.03
                    try
                    {
                        var register = dbPedido.Database.SqlQuery<string>(query.ToString()).FirstOrDefault();
                        cve = register.ToString();
                        row = dbPedido.Database.SqlQuery<RowsCuadroGeneralModel>("dbo.spGetMedicamentoByClave " + anio + ", '" + cve + "'").ToList();
                        
                    }
                    catch(Exception ex) 
                    {

                        //SI CACHO. INDICA QUE TAL VEZ ES DEL MA ARTICULOS DIRECTAMENTE
                        string[] cveT = cve.Split('.');
                        int tipo = Convert.ToInt32(cveT[0]);
                        int grupo = Convert.ToInt32(cveT[1]);
                        int clave = Convert.ToInt32(cveT[2]);
                        int presentacion = Convert.ToInt32(cveT[3]);

                        string query_ma = "  select * from ma_articulos where TIPO = "+tipo+ " and GRUPO = " + grupo + " and CLAVE = " + clave + " and PRESENTACION = " + presentacion + " ";
                        AlmacenEntity dbAlmac = new AlmacenEntity();
                        List<ma_articulos> insumos = dbAlmac.Database.SqlQuery<ma_articulos>(query_ma.ToString()).ToList();
                        ma_articulos insumo = insumos[0];


                        RowsCuadroGeneralModel row_m = new RowsCuadroGeneralModel();
                        row_m.cve_sist = insumo.TIPO.ToString("000") +"."+ insumo.TIPO.ToString("000") + "." + insumo.CLAVE.ToString("0000") + "." + (insumo.PRESENTACION ?? 0).ToString("00");
                        row_m.descrip_almacen = insumo.DESCRIPCION;
                        row_m.pk = insumo.PK_ARTICULOS;
                        row_m.existencia_almacen = insumo.EXISTENCIA;

                        row.Add(row_m);
                    }
                    
                    //registros = db.Database.SqlQuery<DatosCliente>(query.ToString()).ToList();

                }
                foreach (var item in row)
                {
                    mdl = item;
                }
            }
            catch(Exception e)
            {

            }


            return mdl;
        }

        public RowsCuadroGeneralModel getInsumoMaterialCuracion(int anio, string cve)
        {
            RowsCuadroGeneralModel mdl = new RowsCuadroGeneralModel();

            try
            {
                List<RowsCuadroGeneralModel> row = dbPedido.Database.SqlQuery<RowsCuadroGeneralModel>("dbo.spGetMaterialCuByClave " + anio + ", '" + cve + "'").ToList();
                        
                foreach (var item in row)
                {
                    mdl = item;
                }

                //si no esta con esa clave posiblemente es de los registros que tienen equivalencia en el campo cve_sistema en almacen
                if (mdl.pk == "0" || mdl.pk == null)
                {
                    string query = " select cve_sist from cuadro_general_material_curacion where cve = '" + cve + "'";//060.066.1052.03
                    try
                    {
                        var register = dbPedido.Database.SqlQuery<string>(query.ToString()).FirstOrDefault();
                        cve = register.ToString();
                        row = dbPedido.Database.SqlQuery<RowsCuadroGeneralModel>("dbo.spGetMaterialCuByClave " + anio + ", '" + cve + "'").ToList();

                        foreach (var item in row)
                        {
                            mdl = item;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception e)
            {

            }


            return mdl;
        }

        //OBTENER LA SALIDAS DE UN INSUMO POR PK
        public List<SalidasClass> getSalidasInsumo(string pk)
        {
            List<SalidasClass> mdl = new List<SalidasClass>();
            try
            {
                //GET FECHA PARA CALCULAR UN RANGO DE UN ANIO A TRAS
                DateTime hoy = DateTime.Now;
                DateTime desde = hoy.AddMonths(-12);
                int anio = (desde.Year);
                int mes = (desde.Month);
                DateTime anio_back = new DateTime(anio,mes, 1);
                string endF = String.Format("{0:yyyyMMdd}", hoy);
                string startF = String.Format("{0:yyyyMMdd}", anio_back);
                //'20190101', '20190630'
                mdl = dbPedido.Database.SqlQuery<SalidasClass>("dbo.spGetSalidasByClave '" + pk + "', '"+ startF + "', '"+ endF + "'").ToList();

            }
            catch(Exception e) {

            }
            return mdl;
        }

        //OBTENER LA ENTRADAS DE UN INSUMO POR PK ...
        public List<EntradasClass> getEntradasInsumo(string pk)
        {
            List<EntradasClass> mdl = new List<EntradasClass>();
            try
            {
                //GET FECHA PARA CALCULAR UN RANGO DE UN ANIO A TRAS
                DateTime hoy = DateTime.Now;
                DateTime desde = hoy.AddMonths(-11);
                int anio = (desde.Year);
                int mes = (desde.Month);
                DateTime anio_back = new DateTime(anio, mes, 1);
                string endF = String.Format("{0:yyyyMMdd}", hoy);
                string startF = String.Format("{0:yyyyMMdd}", anio_back);
                //'20190101', '20190630'
                mdl = dbPedido.Database.SqlQuery<EntradasClass>("dbo.spGetEntradasByClave '" + pk + "', '" + startF + "', '" + endF + "'").ToList();
            }
            catch(Exception e)
            {

            }
            return mdl;
        }

        //OBTENER EL ULTIMO CONSUMO Y SURTIMIENTO DEL MES ANTERIOR A LA FECHA ACTUAL
        public TotalUltimoPedidoClass getTotalUltimaSalida(string pk)
        {
            TotalUltimoPedidoClass mdl = new TotalUltimoPedidoClass();
            try {
                DateTime hoy = DateTime.Now;
                DateTime mesAnterior = hoy.AddMonths(-1);
                int anio = mesAnterior.Year;
                int mes = mesAnterior.Month;
                List<TotalUltimoPedidoClass> mdl_temp = new List<TotalUltimoPedidoClass>();
                mdl_temp = dbPedido.Database.SqlQuery<TotalUltimoPedidoClass>("dbo.spGetPedidosVSsurtido "+anio+", "+mes+", '"+ pk +"'").ToList();
                //execute spGetPedidosVSsurtido 2019, 8, 655
                if (mdl_temp.Count > 0)
                {
                    mdl = mdl_temp.First();
                    string MesLetra = String.Format("{0:MMMM}", mesAnterior);
                    mdl.mes = MesLetra.ToUpper();
                }                    
                else
                {
                    mdl.solicitado = -1;
                    mdl.surtido = -1;
                }
                    
            }
            catch(Exception e)
            {
                mdl.solicitado = -1;
                mdl.surtido = -1;
            }

            return mdl;
        }

        public List<ProyeccionPedidoClass> getPedidoMedicamento()
        {
            List<ProyeccionPedidoClass> mdl = new List<ProyeccionPedidoClass>();
            int anio = 2019;//DateTime.Now.Year;
            try
            {
                mdl = dbPedido.Database.SqlQuery<ProyeccionPedidoClass>("dbo.spGetProyeccionPedidoMedicamento " + anio).ToList();
                foreach (ProyeccionPedidoClass item in mdl)
                {
                    if (item.solicitar < 0)
                        item.solicitar = 0;
                }
            }
            catch (Exception e)
            {

            }
            return mdl;
        }

        public List<ProyeccionPedidoClass> getPedidoMaterialCuracion()
        {
            List<ProyeccionPedidoClass> mdl = new List<ProyeccionPedidoClass>();
            int anio = 2019;//DateTime.Now.Year;
            try
            {
                mdl = dbPedido.Database.SqlQuery<ProyeccionPedidoClass>("dbo.spGetProyeccionPedidoMaterial " + anio).ToList();
                foreach (ProyeccionPedidoClass item in mdl)
                {
                    if (item.solicitar < 0)
                        item.solicitar = 0;
                }
            }
            catch (Exception e)
            {

            }
            return mdl;
        }

        public int getEntradasByAnio(string pk)
        {
            int total = 0;
            int anio = DateTime.Now.Year; 
            try {
                List<int> row = dbPedido.Database.SqlQuery<int>("dbo.spGetTotalEntradasByAnio " + anio + ", '" + pk + "'").ToList();

                total = row[0];
            }
            catch(Exception e)
            {
                total = 0;
            }
            return total;
        }

        
             public CuadroMonitoreoClass GetCuadroMonitoreo(int tipo)
        {
            CuadroMonitoreoClass mdl = new CuadroMonitoreoClass();
            try
            {
                if (tipo == 1)
                {
                    mdl.rows = dbPedido.Database.SqlQuery<RowCuadroMonitoreo>("dbo.SP_PROG_MED").ToList();
                }
                else
                {
                    mdl.rows = dbPedido.Database.SqlQuery<RowCuadroMonitoreo>("dbo.SP_PROG_CUR").ToList();
                }

                mdl.tipo = tipo;
                mdl.total_claves = mdl.rows.Count();
            }
            catch (Exception e)
            {

            }

            return mdl;
        }
        public CuadroMonitoreoClass GetTodoCuadroMonitoreo(int tipo)
        {
            CuadroMonitoreoClass mdl = new CuadroMonitoreoClass();
            try
            {
                if (tipo == 1)
                {
                    mdl.rows = dbPedido.Database.SqlQuery<RowCuadroMonitoreo>("dbo.SP_PROG_MED_ALMACEN").ToList();                    
                }
                else
                {
                    mdl.rows = dbPedido.Database.SqlQuery<RowCuadroMonitoreo>("dbo.SP_PROG_CUR_ALMACEN").ToList();
                }

                mdl.tipo = tipo;
                mdl.total_claves = mdl.rows.Count();
            }
            catch(Exception e) {
            
            }

            return mdl;
        }

        public CuadroMonitoreoClass GetTodoCuadroPFMI(int tipo)
        {
            CuadroMonitoreoClass mdl = new CuadroMonitoreoClass();
            try
            {
                mdl.rows = dbPedido.Database.SqlQuery<RowCuadroMonitoreo>("dbo.SP_PROG_PFMI").ToList();


                mdl.tipo = tipo;
                mdl.total_claves = mdl.rows.Count();
            }
            catch (Exception e)
            {

            }

            return mdl;
        }

        public UnidadesMonitoreoConsultaClass GetMonitoreoUnidades()
        {
            UnidadesMonitoreoConsultaClass mdl = new UnidadesMonitoreoConsultaClass();
            try
            {

               mdl.rows = dbPedido.Database.SqlQuery<RowsUnidadesMonitoreo>("dbo.SP_ULT_CAP_UNIDADES").ToList();
                foreach(var item in mdl.rows)
                {
                    if (string.IsNullOrEmpty(item.ult_fech_med))
                    {
                        item.ult_fech_med = "Sin Captura";
                        item.ult_hr_med = "--";
                    }
                    else
                    {
                        item.ult_fech_med = String.Format("{0:dd MMMM yyyy}", Convert.ToDateTime(item.ult_fech_med));
                        item.ult_hr_med = item.ult_hr_med + " hrs";
                    }

                    if (string.IsNullOrEmpty(item.ult_fech_cur))
                    {
                        item.ult_fech_cur = "Sin Captura";
                        item.ult_hr_cur = "--";
                    }
                    else
                    {
                        item.ult_fech_cur = String.Format("{0:dd MMMM yyyy}", Convert.ToDateTime(item.ult_fech_cur));
                        item.ult_hr_cur = item.ult_hr_cur + " hrs";
                    }

                    if (item.id_unidad == 90000) //HRU
                    { 
                        mdl.ult_fec_hru_me = item.ult_fech_med;
                        mdl.ult_hr_hru_me = item.ult_hr_med;
                        mdl.ult_fec_hru_ma = item.ult_fech_cur;
                        mdl.ult_hr_hru_ma = item.ult_hr_cur;
                    }
                    if (item.id_unidad == 29) //HGT
                    {
                       
                        mdl.ult_fec_hgt_me =  item.ult_fech_med;
                        mdl.ult_hr_hgt_me = item.ult_hr_med;
                        mdl.ult_fec_hgt_ma = item.ult_fech_cur;
                        mdl.ult_hr_hgt_ma = item.ult_hr_cur;
                    }
                    if (item.id_unidad == 52) //HGM
                    {                      

                        mdl.ult_fec_hgm_me = item.ult_fech_med;
                        mdl.ult_hr_hgm_me = item.ult_hr_med;
                        mdl.ult_fec_hgm_ma = item.ult_fech_cur;
                        mdl.ult_hr_hgm_ma = item.ult_hr_cur;
                    }
                    if (item.id_unidad == 110000) //HGI
                    {
                        mdl.ult_fec_hgi_me = item.ult_fech_med;
                        mdl.ult_hr_hgi_me = item.ult_hr_med;
                        mdl.ult_fec_hgi_ma = item.ult_fech_cur;
                        mdl.ult_hr_hgi_ma = item.ult_hr_cur;
                    }
                    if (item.id_unidad == 130000) //HMI
                    {
                        mdl.ult_fec_hmi_me = item.ult_fech_med;
                        mdl.ult_hr_hmi_me = item.ult_hr_med;
                        mdl.ult_fec_hmi_ma = item.ult_fech_cur;
                        mdl.ult_hr_hmi_ma = item.ult_hr_cur;
                    }
                    if (item.id_unidad == 180000) //IEC
                    {
                        mdl.ult_fec_iec_me = item.ult_fech_med;
                        mdl.ult_hr_iec_me = item.ult_hr_med;
                        mdl.ult_fec_iec_ma = item.ult_fech_cur;
                        mdl.ult_hr_iec_ma =  item.ult_hr_cur;

                    }
                    if (item.id_unidad == 10051) //CEH
                    {
                        mdl.ult_fec_ceh_me = item.ult_fech_med;
                        mdl.ult_hr_ceh_me = item.ult_hr_med;
                        mdl.ult_fec_ceh_ma = item.ult_fech_cur;
                        mdl.ult_hr_ceh_ma = item.ult_hr_cur;
                    }
                    if (item.id_unidad == 80000) //J1
                    {
                        mdl.ult_fec_j1_me = item.ult_fech_med;
                        mdl.ult_hr_j1_me = item.ult_hr_med;
                        mdl.ult_fec_j1_ma = item.ult_fech_cur;
                        mdl.ult_hr_j1_ma = item.ult_hr_cur;
                    }
                    if (item.id_unidad == 100000) //J2
                    {
                        mdl.ult_fec_j2_me = item.ult_fech_med;
                        mdl.ult_hr_j2_me = item.ult_hr_med;
                        mdl.ult_fec_j2_ma = item.ult_fech_cur;
                        mdl.ult_hr_j2_ma = item.ult_hr_cur;
                    }
                    if (item.id_unidad == 120000) //J3
                    {
                        mdl.ult_fec_j3_me = item.ult_fech_med;
                        mdl.ult_hr_j3_me = item.ult_hr_med;
                        mdl.ult_fec_j3_ma = item.ult_fech_cur;
                        mdl.ult_hr_j3_ma = item.ult_hr_cur;
                    }
                }


            }
            catch (Exception e)
            {

            }
            return mdl;
        }

        public List<RowRepoInsumos> getSolicitudVSsurtido(string cve, string unidad)
        {
            AlmacenEntity dbA = new AlmacenEntity();
            List<RowRepoInsumos> listadin = new List<RowRepoInsumos>();

            try
            {
                ReportInsumos insumo = new ReportInsumos();
                string[] dts_cve = cve.Split('.');
                int tipo = Convert.ToInt32(dts_cve[0]);
                int grupo = Convert.ToInt32(dts_cve[1]);
                int clave = Convert.ToInt32(dts_cve[2]);
                int presentacion = Convert.ToInt32(dts_cve[3]);

                //GET FECHA PARA CALCULAR UN RANGO DE UN ANIO A TRAS
                DateTime hoy = DateTime.Now;
                DateTime desde = hoy.AddMonths(-12);
                int anio = (desde.Year);
                int mes = (desde.Month);
                DateTime anio_back = new DateTime(anio, mes, 1);
                string endF = String.Format("{0:yyyyMMdd}", hoy);
                string startF = String.Format("{0:yyyyMMdd}", anio_back);


                StringBuilder QueryString = new StringBuilder();
                QueryString.Append("select ");
                QueryString.Append(" pk_articulos as pk, ");
                QueryString.Append(" anio,");
                QueryString.Append(" mes, ");
                QueryString.Append(" mes_nombre, ");
                QueryString.Append(" clave_t as clave, ");
                QueryString.Append(" descripcion, ");
                QueryString.Append("cast (sum(solicitado) as varchar) solicitado, ");
                QueryString.Append("cast(sum(surtido) as varchar)surtido ");

                QueryString.Append("from( ");
                QueryString.Append("select ");
                QueryString.Append("dp.pk_articulos, ");
                QueryString.Append("MONTH(mp.fecha_pedido) mes, ");
                QueryString.Append("DATENAME(month, mp.fecha_pedido)mes_nombre, ");
                QueryString.Append("YEAR(mp.fecha_pedido) anio, ");
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
                QueryString.Append("where dp.clave > 0 and mp.afectado = 1 ");
                //QueryString.Append("and mp.anio = 2019 ");

                QueryString.Append("and dp.Tipo = " + tipo);
                QueryString.Append("and dp.grupo = " + grupo);
                QueryString.Append("and dp.clave = " + clave);
                QueryString.Append("and dp.presentacion = " + presentacion);


                QueryString.Append("and mp.fecha_pedido between '"+ startF + "' and '"+ endF + "' ");
                QueryString.Append("and mp.centro_sol in ("+ unidad + ") ");
                QueryString.Append(")f ");
                QueryString.Append("group by pk_articulos, clave_t, descripcion, anio, mes, mes_nombre ");
                QueryString.Append("order by anio desc, mes desc ");

                listadin = dbA.Database.SqlQuery<RowRepoInsumos>(QueryString.ToString()).ToList();
              

            }
            catch (Exception e)
            {
                listadin = new List<RowRepoInsumos>();
            }

            return listadin;
        }
    }
}
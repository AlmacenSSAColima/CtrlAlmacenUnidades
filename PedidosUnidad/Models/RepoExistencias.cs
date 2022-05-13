using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using PedidosUnidad.Models.DBAlmacen;
using PedidosUnidad.Models.DBPedido;
using PedidosUnidad.Security;

namespace PedidosUnidad.Models
{
    public class RepoExistencias
    {
        public ExistenciaDiaria getExistencia(CurrentUser user, int? tipo_, int? unidad_ )
        {
            int tipo = user.tipo;
            int unidad = user.id_unidad;
            if (tipo_ != null && unidad_ != null)
            {
                tipo = tipo_ ?? -1;
                unidad = unidad_ ?? -1;
            }
                

            ExistenciaDiaria mdl = new ExistenciaDiaria();
            
            //DIA ACTUAL
            DateTime fecha_hoy = DateTime.Now;
            string dia_hoy_ = DateTime.Now.ToString("dddd").ToUpper();

            mdl.dia_en_curso = String.Format("{0:dddd d MMMM yyyy}", fecha_hoy); 
            mdl.fecha_afectar = fecha_hoy.ToShortDateString();
            mdl.id_unidad = unidad;
            mdl.tipo = tipo_ ?? -1;
            mdl.editando = false;
            

            //DIAS A RESTAR PARA LLEGAR A UN DOMINGO ANTERIOR
            int dias = 0;
            switch (dia_hoy_)
            {
                case "LUNES":
                    dias = 1;
                    break;
                case "MARTES":
                    dias = 2;
                    break;
                case "MIÉRCOLES":
                    dias = 3;
                    break;
                case "JUEVES":
                    dias = 4;
                    break;
                case "VIERNES":
                    dias = 5;
                    break;
                case "SÁBADO":
                    dias = 6;
                    break;
                case "DOMINGO":
                    dias = 7;
                    break;
                default:
                    dias = 0;
                    break;
            }

            //
            string f_ini = fecha_hoy.AddDays(-dias).ToShortDateString();
            string f_end = fecha_hoy.AddDays(-dias).AddDays(7).ToShortDateString();
            mdl.semana = "SEMANA " + f_ini + " AL " + f_end;
            mdl.f_desde = f_ini;
            mdl.f_hasta = f_end;

            //DIAS EN NUMEROS
            DateTime f_orgigen = fecha_hoy.AddDays(-dias);
            mdl.d_1 = f_orgigen.Day.ToString("00");
            
            mdl.d_2 = f_orgigen.AddDays(1).Day.ToString("00");
            mdl.d_3 = f_orgigen.AddDays(2).Day.ToString("00");
            mdl.d_4 = f_orgigen.AddDays(3).Day.ToString("00");
            mdl.d_5 = f_orgigen.AddDays(4).Day.ToString("00");
            mdl.d_6 = f_orgigen.AddDays(5).Day.ToString("00");
            mdl.d_7 = f_orgigen.AddDays(6).Day.ToString("00");
            mdl.d_8 = f_orgigen.AddDays(7).Day.ToString("00");

            PedidoEntity dbPedido = new PedidoEntity();
            List<PedidoModel> listExistencias = new List<PedidoModel>();
            try
            {
                RepoPedidos objPedidos = new RepoPedidos();
                PedidoModel cpmOriginal = new PedidoModel();
                cpmOriginal = objPedidos.getCPMunidad(unidad, tipo, false);
                mdl.tieneCPM = cpmOriginal.tieneCPM;
                //SI NO CONTIENE CPM ASIGNADO RETORNAR EL MODELO PARA MOSTRAR MENSAJE.
                if (!cpmOriginal.tieneCPM)
                    return mdl;

                mdl.no_claves = cpmOriginal.claves_total;
                mdl.claves_curacion = cpmOriginal.claves_curacion;
                mdl.claves_medicamento = cpmOriginal.claves_medicamento;

                PedidoModel cpm_temp = new PedidoModel();
                cpm_temp = objPedidos.getCPMunidad(unidad, tipo, false);
                //8 DIAS PARA PODER TENER SIEMPRE UN COMODIN DE REFERENCIA Y NO INICIE EN BLANCO O EN CERO EJ: DOMINGO A DOMINGO
                for (int i=1; i<=8; i++)
                {
                    //SABER SI YA LLEGAMOS AL DIA ACTUAL
                    if (dias >= 0)
                    {
                        //CALCULAR FECHA A OBTENER (FECHA ACTUAL - LOS DIAS CORRESPONDIENTES PARA COMENZAR EN DOMINGO)
                        DateTime fecha_r = fecha_hoy.AddDays(-dias);
                        string anio = fecha_r.Year.ToString("0000");
                        string mes = fecha_r.Month.ToString("00");
                        string dia = fecha_r.Day.ToString("00");
                        string f_init = anio + mes + dia;
                        //CONSULTAR Y REVISAR SI EXISTE REGISTRO 
                        string query = "select * from ma_inventario_dia where id_unidad = "+ unidad + " and fecha = '" + f_init + "' and tipo = "+ tipo;
                        List<ma_inventario_dia> lista = dbPedido.Database.SqlQuery<ma_inventario_dia>(query).ToList();
                        
                        //SI HAY REGISTRO ASIGNAR EXISTENCIAS A LA CLASE PARA MOSTRAR EN GRID DE CAPTURA
                        if (lista.Count > 0)
                        {
                            //OBTENER EL REGISTRO DE LA LISTA
                            ma_inventario_dia regiDia = lista.First();

                            if (dias != 0) //|| mdl.ultimo_dia_captura == "S/C" 
                                mdl.ultimo_dia_captura = (regiDia.fecha ?? DateTime.Now).ToShortDateString() + " "+ (regiDia.hora ?? "");

                            //OBTENER EL DETALLE
                            List<det_inventario_articulo> ExistenciasDelDia = (from r in dbPedido.det_inventario_articulo where r.id_unidad == regiDia.id_unidad && r.id_inv == regiDia.id select r).ToList();
                            PedidoModel cpm_list_ = new PedidoModel();
                            cpm_list_ = objPedidos.getCPMunidad(unidad, tipo, false);//cpmOriginal.articulos;
                            //cpm_temp = cpm;
                            foreach (var item_ in cpm_list_.articulos)
                            {

                                
                                List<det_inventario_articulo> rowse = (from r in ExistenciasDelDia where r.pk_articulo == item_.pk select r).ToList();
                                det_inventario_articulo row = null;                               

                                if (rowse.Count > 0)
                                {
                                    item_.existencia = 0;
                                    row = rowse.First();
                                }
                                    

                                if (row != null)
                                {
                                    item_.existencia = row.existencias ?? 0;
                                    item_.tipo_mov = row.tipo_mov ?? 1;
                                    item_.des_mov = row.observaciones;
                                }
                            }
                            //DIA 1,2,3,4,5,6,7,8
                            cpm_list_.dia = i;
                            cpm_list_.actual = false;
                            //SI DIAS ES IGUAL A CERO QUIERE DECIR QUE YA LLEGO AL DIA ACTUAL
                            if (dias == 0)
                            {
                                cpm_list_.actual = true;
                                mdl.id_ = regiDia.id;
                                mdl.editando = true;
                            }                             

                            mdl.DiasExistencia.Add(cpm_list_);
                            cpm_temp = cpm_list_;
                        }//SI NO HAY REGISTRO DE ESE DIA, LLENAR CLASE CON LA VUELTA ANTERIOR Y MUESTRE LAS MISMA EXISTENCIAS DE UN DIA ANTES(EN CASO DE EXISTIR)
                        else
                        {
                            PedidoModel cp_ = new PedidoModel();
                            
                            //SI ES EL PRIMER DIA DEL GRID Y NO HAY REGISTRO EN ESA FECHA, NI DATOS DE UNA VUELTA ANTERIOR
                            //BUSCAR EL ULTIMO DIA CUALQUIERA QUE SEA EN EL CUAL SE REGISTRO SI ES QUE EXISTE PARA QUE TENGAN UNA BASE DE CAPTURA.
                            if (i == 1)
                            {
                                //CUALQUIER REGISTRO MENOR A LA FECHA DEL DIA INICIAL DEL GRID, PARA QUE NO SALGAN LAS EXISTENCIAS DE ESA CAPTURA, EN CASO DE SER LA UNICA.
                                DateTime fecha_act_hoy = DateTime.Now;
                                string anioNow = fecha_act_hoy.Year.ToString("0000");
                                string mesNow = fecha_act_hoy.Month.ToString("00");
                                string diaNow = fecha_act_hoy.Day.ToString("00");
                                string f_now = anioNow + mesNow + diaNow;

                                string query_u = "select * from ma_inventario_dia where id_unidad = " + unidad + " and fecha < '"+ f_init + "' and tipo = " + tipo +" order by id desc";
                                List<ma_inventario_dia> registros_f = dbPedido.Database.SqlQuery<ma_inventario_dia>(query_u).ToList();
                                //SI EXISTEN REGISTRO TOMAR EL ULTIMO CAPTURADO PARA MOSTRARLO COMO REFERENCIA EN EXISTENCIAS.
                                if (registros_f.Count > 0)
                                {
                                    ma_inventario_dia regiEseDia = registros_f.First();
                                    mdl.ultimo_dia_captura = (regiEseDia.fecha ?? DateTime.Now).ToShortDateString() + " " + (regiEseDia.hora ?? "");

                                    //OBTENER EL DETALLE DE ESE DIA ENCONTRADO
                                    List<det_inventario_articulo> ExistenciasDeEseDia = (from r in dbPedido.det_inventario_articulo where r.id_unidad == regiEseDia.id_unidad && r.id_inv == regiEseDia.id select r).ToList();

                                    PedidoModel cpm_ultimo_dia = new PedidoModel();
                                    cpm_ultimo_dia = objPedidos.getCPMunidad(unidad, tipo, false);

                                    foreach (var item_ in cpm_ultimo_dia.articulos)
                                    {
                                        item_.existencia = 0;
                                        List<det_inventario_articulo> rowse_det = (from r in ExistenciasDeEseDia where r.pk_articulo == item_.pk select r).ToList();
                                        det_inventario_articulo row_det = null;
                                        if (rowse_det.Count > 0)
                                            row_det = rowse_det.First();

                                        if (row_det != null)
                                        {
                                            item_.existencia = row_det.existencias ?? 0;
                                            item_.tipo_mov = row_det.tipo_mov ?? 1;
                                            item_.des_mov = row_det.observaciones;
                                        }
                                    }
                                    //DIA 1,2,3,4,5,6,7,8
                                    cpm_ultimo_dia.dia = i;
                                    cpm_ultimo_dia.actual = false;

                                    mdl.DiasExistencia.Add(cpm_ultimo_dia);
                                    cpm_temp = cpm_ultimo_dia;
                                }//DELO CONTRARIO, SI NO EXISTE REGISTRO ALGUNO, (NUNCA SE AH CAPTURADO INICIAR CON CEROS)
                                else {
                                    cp_.articulos = cpm_temp.articulos;
                                    //DIA 1,2,3,4,5,6,7,8
                                    cp_.dia = i;
                                    cp_.actual = false;
                                    //SI DIAS ES IGUAL A CERO QUIERE DECIR QUE YA LLEGO AL DIA ACTUAL
                                    if (dias == 0)
                                        cp_.actual = true;
                                }
                            }//SI NO ES EL DIA UNO, TOMAR LA ULTIMA EXISTENCIA DE LA VUELTA ANTERIOR
                            else {
                                cp_.articulos = cpm_temp.articulos;
                                //DIA 1,2,3,4,5,6,7,8
                                cp_.dia = i;
                                cp_.actual = false;
                                //SI DIAS ES IGUAL A CERO QUIERE DECIR QUE YA LLEGO AL DIA ACTUAL
                                if (dias == 0)
                                    cp_.actual = true;
                            }                     
                            

                            mdl.DiasExistencia.Add(cp_);
                        }
                    }
                    else {
                        PedidoModel diaVacio = new PedidoModel();
                        diaVacio = objPedidos.getCPMunidad(unidad, tipo, true);//cpmOriginal.articulos;
                        //DIA 1,2,3,4,5,6,7,8
                        diaVacio.dia = i;
                        diaVacio.actual = false;
                        mdl.DiasExistencia.Add(diaVacio);
                    }
                    

                    dias = dias - 1;
                }
            }
            catch (Exception e)
            {

            }           


            List<CPMrowModel> rowsDeTable = new List<CPMrowModel>();
            foreach (var item in mdl.DiasExistencia.OrderBy(a=> a.dia))
            {

                if (item.dia == 1)
                {                    
                    foreach (var it in item.articulos)
                    {

                            CPMrowModel row = new CPMrowModel();
                            row.pk = it.pk;
                            row.clave = it.clave;
                            row.cpm = it.cpm;
                            row.descripcion = it.descripcion;
                            row.presentacion = it.presentacion;
                            row.D1 = it.existencia;
                            row.A1 = item.actual;
                            row.tipo = it.tipo;
                            row.origen = it.origen;
                            row.consolidada = it.consolidada;
                            row.dimesa = it.dimesa;
                            row.max_dimesa = it.max_dimesa;
                        
                            row.programa = it.programa;
                            row.desierta = it.desierta;
                            row.controlado = it.controlado;
                            row.maximo = it.maximo;
                            row.minimo = it.minimo;
                            row.tipo_mov_1 = it.tipo_mov;
                            row.des_mov_1 = it.des_mov;
                            rowsDeTable.Add(row);                      
                    }
                }

                if (item.dia == 2)
                {
                    foreach (var it in item.articulos)
                    {
                        
                        CPMrowModel row = rowsDeTable.SingleOrDefault(a => a.pk == it.pk);
                        row.D2 = it.existencia;
                        row.A2 = item.actual;
                        row.tipo_mov_2 = it.tipo_mov;
                        row.des_mov_2 = it.des_mov;
                    }
                }

                if (item.dia == 3)
                {
                    foreach (var it in item.articulos)
                    {

                        CPMrowModel row = rowsDeTable.SingleOrDefault(a => a.pk == it.pk);
                        row.D3 = it.existencia;
                        row.A3 = item.actual;
                        row.tipo_mov_3 = it.tipo_mov;
                        row.des_mov_3 = it.des_mov;
                    }
                }
                if (item.dia == 4)
                {
                    foreach (var it in item.articulos)
                    {
                        
                        CPMrowModel row = rowsDeTable.SingleOrDefault(a => a.pk == it.pk);
                        row.D4 = it.existencia;
                        row.A4 = item.actual;
                        row.tipo_mov_4 = it.tipo_mov;
                        row.des_mov_4 = it.des_mov;
                    }
                }
                if (item.dia == 5)
                {
                    foreach (var it in item.articulos)
                    {
                        
                        CPMrowModel row = rowsDeTable.SingleOrDefault(a => a.pk == it.pk);
                        row.D5 = it.existencia;
                        row.A5 = item.actual;
                        row.tipo_mov_5 = it.tipo_mov;
                        row.des_mov_5 = it.des_mov;
                    }
                }
                if (item.dia == 6)
                {
                    foreach (var it in item.articulos)
                    {
                       
                        CPMrowModel row = rowsDeTable.SingleOrDefault(a => a.pk == it.pk);
                        row.pk = it.pk;
                        row.clave = it.clave;
                        row.descripcion = it.descripcion;
                        row.D6 = it.existencia;
                        row.A6 = item.actual;
                        row.tipo_mov_6 = it.tipo_mov;
                        row.des_mov_6 = it.des_mov;
                    }
                }
                if (item.dia == 7)
                {
                    foreach (var it in item.articulos)
                    {
                        
                        CPMrowModel row = rowsDeTable.SingleOrDefault(a => a.pk == it.pk);
                        row.pk = it.pk;
                        row.clave = it.clave;
                        row.descripcion = it.descripcion;
                        row.D7 = it.existencia;
                        row.A7 = item.actual;
                        row.tipo_mov_7 = it.tipo_mov;
                        row.des_mov_7 = it.des_mov;


                    }
                }
                if (item.dia == 8)
                {
                    foreach (var it in item.articulos)
                    {
                       
                        CPMrowModel row = rowsDeTable.SingleOrDefault(a => a.pk == it.pk);
                        row.pk = it.pk;
                        row.clave = it.clave;
                        row.descripcion = it.descripcion;
                        row.presentacion = it.presentacion;
                        row.D8 = it.existencia;
                        row.A8 = item.actual;
                        row.tipo_mov_8 = it.tipo_mov;
                        row.des_mov_8 = it.des_mov;

                    }
                }
            }

            mdl.Rows = rowsDeTable.OrderBy(b=> b.tipo).ThenBy(c=> c.descripcion).ToList();

            mdl.ultimo_dia_captura = this.GetUltimaFechaCaptura(unidad, tipo);

            return mdl;

        }


        //SAVE EXISTENCIA DEL DIA
        public ResultClass saveExistencia(PedidoModel mdl, CurrentUser user)
        {
            ResultClass Rmdl = new ResultClass();
            try
            {
                PedidoEntity dbPedido = new PedidoEntity();

                DateTime fecha_a = Convert.ToDateTime(mdl.fecha);
                ma_inventario_dia headExis = new ma_inventario_dia();
                headExis.id_unidad = user.id_unidad;
                headExis.id = this.getMaxId(user.id_unidad);
                headExis.fecha = fecha_a;
                headExis.mes = fecha_a.Month;
                headExis.anio = fecha_a.Year;
                headExis.tipo = user.tipo;
                headExis.hora = String.Format("{0:HH:mm:ss}", DateTime.Now);

                dbPedido.ma_inventario_dia.Add(headExis);
                dbPedido.SaveChanges();

                int id_ = 1;
                foreach (var item in mdl.articulos)
                {
                    try
                    {
                        det_inventario_articulo det = new det_inventario_articulo();
                        det.id_inv = headExis.id;
                        det.id_unidad = headExis.id_unidad;
                        det.id = id_;
                        det.pk_articulo = item.pk;
                        det.cve = item.clave;
                        det.cpm = item.cpm;
                        det.existencias = item.existencia;
                        det.entradas = 0;
                        det.tipo = user.tipo;
                        det.tipo_mov = item.tipo_mov;
                        det.observaciones = item.des_mov;
                        dbPedido.det_inventario_articulo.Add(det);
                        dbPedido.SaveChanges();
                        id_++;
                    }
                    catch(Exception e)
                    {

                    }
                }
                Rmdl.editando = true;
                Rmdl.exito = true;
                Rmdl.id_ = headExis.id;
                Rmdl.msg = "Se guardo correctamente";
            }
            catch (Exception e)
            {
                Rmdl.exito = false;
                Rmdl.msg = "Se presento un error";
            }

            return Rmdl;
        }

        //EDIT EXISTENCIA DEL DIA
        public ResultClass editExistencia(PedidoModel mdl, CurrentUser user)
        {
            ResultClass Rmdl = new ResultClass();
            try
            {
                PedidoEntity dbPedido = new PedidoEntity();

                //GET CABECERO
                ma_inventario_dia cabecero = dbPedido.ma_inventario_dia.SingleOrDefault(a => a.id == mdl.id_pedido && a.id_unidad == mdl.id_unidad && a.tipo == user.tipo);
                
                if (cabecero != null)
                {
                    //ACTUALIZAR HORA
                    cabecero.hora = String.Format("{0:HH:mm:ss}", DateTime.Now);
                    dbPedido.SaveChanges();

                    //CLEAN DETALLE
                    bool Rexito = this.initDetalleExistencia(mdl.id_pedido, mdl.id_unidad);
                    if (Rexito)
                    {
                        int id_ = 1;
                        foreach (var item in mdl.articulos)
                        {
                            try
                            {
                                det_inventario_articulo det = new det_inventario_articulo();
                                det.id_inv = cabecero.id;
                                det.id_unidad = cabecero.id_unidad;
                                det.id = id_;
                                det.pk_articulo = item.pk;
                                det.cve = item.clave;
                                det.cpm = item.cpm;
                                det.existencias = item.existencia;
                                det.entradas = 0;
                                det.observaciones = item.des_mov;
                                det.tipo = user.tipo;
                                det.tipo_mov = item.tipo_mov;
                                dbPedido.det_inventario_articulo.Add(det);
                                dbPedido.SaveChanges();
                                id_++;
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }

                    Rmdl.editando = true;
                    Rmdl.exito = true;
                    Rmdl.id_ = cabecero.id;
                    Rmdl.msg = "Se guardo correctamente";

                }                

            }
            catch (Exception e)
            {
                Rmdl.exito = false;
                Rmdl.msg = "Se presento un error";
            }

            return Rmdl;
        }

        //GET MAX ID EXISTENCIA
        public int getMaxId(int unidad)
        {
            PedidoEntity dbPedido = new PedidoEntity();
            int id = 0;
            try
            {
                id = (from r in dbPedido.ma_inventario_dia where r.id_unidad == unidad select r).ToList().Max(a => a.id);
                id = id + 1;
            }
            catch
            {
                id = 1;
            }
            return id;
        }

        //LIMPIAR DETALLE DE EXISTENCIA 
        public bool initDetalleExistencia(int id, int id_unidad)
        {
            PedidoEntity dbPedido = new PedidoEntity();
            bool exito = false;
            try
            {
                List<det_inventario_articulo> detalle_pedido = (from r in dbPedido.det_inventario_articulo where r.id_inv == id && r.id_unidad == id_unidad select r).ToList();
                dbPedido.det_inventario_articulo.RemoveRange(detalle_pedido);
                dbPedido.SaveChanges();
                exito = true;
            }
            catch (Exception e)
            {
                exito = false;
            }
            
            return exito;
        }

        public ReporteExistenciaClass reporteExistencias(CurrentUser user)
        {
            ReporteExistenciaClass mdl = new ReporteExistenciaClass();
            List<rowReportExistencia> rowsL = new List<rowReportExistencia>();

            try
            {
                int anio = 2019;//DateTime.Now.Year;//
                string tipo_pro = " < 60 ";
                if (user.tipo == 2)
                    tipo_pro = " >= 60 ";

                ma_inventario_dia registro = new ma_inventario_dia();
                PedidoEntity dbPedido = new PedidoEntity();

                List<ma_inventario_dia> maRegistros = (from r in dbPedido.ma_inventario_dia where r.id_unidad == user.id_unidad && r.tipo == user.tipo select r).OrderByDescending(a => a.id).ToList();
                if (maRegistros.Count() > 0)
                {

                    registro = maRegistros.First();
                    mdl.tipo_insumo = user.tipo;
                    mdl.unidad = user.nom_unidad;
                    mdl.ultima_captura = (registro.fecha ?? DateTime.Now).ToShortDateString() + " " + registro.hora;

                    StringBuilder QueryString = new StringBuilder();

                    QueryString.Append("select ");
                    QueryString.Append("m.articulos_pk as pk, ");
                    QueryString.Append("ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(d.tipo as varchar(3))))) + cast(d.tipo as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(d.grupo as varchar(3))))) + cast(d.grupo as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(d.clave as varchar(4))))) + cast(d.clave as varchar(4)))) + '-' + ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(d.presentacion as varchar(4))))) + cast(d.presentacion as varchar(4)))) clave , ");
                    QueryString.Append("d.descripcion ");
                    QueryString.Append(", m.origen ");
                    QueryString.Append(", m.desc_programa ");
                    QueryString.Append(", cast(m.desierta as bit) desierta");
                    QueryString.Append(", cast(ISNULL(m.controlado, 0) as bit) controlado ");
                    QueryString.Append(", ISNULL(m.max_consolidada, 0) maximo ");
                    QueryString.Append(", ISNULL(m.min_consolidada, 0) minimo ");
                    QueryString.Append(", m.consolidada consolidada ");
                    QueryString.Append(", m.dimesa dimesa ");
                    QueryString.Append(", m.max_dimesa max_dimesa ");
                    QueryString.Append(", (up.descrip + ' ' + cast(unidades as varchar) + ' ' + um.descrip) presentacion,");
                    QueryString.Append("cast(m.cpm as varchar) cpm ");
                    QueryString.Append(", cast(det.existencias as varchar) existencias ");
                    QueryString.Append("from cpm_unidades m ");
                    QueryString.Append("left join ma_articulos d on m.articulos_pk = d.pk_articulos ");
                    QueryString.Append("left join ca_unidadesp up on d.uni_pres = up.unidad ");
                    QueryString.Append("left join ca_unidadesm um on d.uni_med = um.unidad ");
                    QueryString.Append("left join[pedidos_cpm].[dbo].[det_inventario_articulo] det ");

                    QueryString.Append("on m.articulos_pk COLLATE Modern_Spanish_CI_AS = det.pk_articulo COLLATE Modern_Spanish_CI_AS ");
                    QueryString.Append("and  m.centro_pk = det.id_unidad and det.id_inv = " + registro.id);
                    QueryString.Append("where m.centro_pk = " + registro.id_unidad + " and m.anio = " + anio + " and m.activo = 1 and d.tipo " + tipo_pro);
                    QueryString.Append("order by d.tipo ");
                    AlmacenEntity dbA = new AlmacenEntity();
                    List<rowReportExistencia> lista = dbA.Database.SqlQuery<rowReportExistencia>(QueryString.ToString()).ToList();

                    rowsL = lista;
                    mdl.rowsInsumos = rowsL;
                }
            }
            catch (Exception e)
            {

            }

           

            return mdl;
        }

        public string GetUltimaFechaCaptura(int unidad, int tipo)
        {
            string fecha_hora = "S/C";
            StringBuilder QueryString = new StringBuilder();
            PedidoEntity dbPedido = new PedidoEntity();
            try
            {
                QueryString.Append("select isnull(cast(max(fecha) as varchar), '') ult_fech_med from dbo.ma_inventario_dia where id_unidad = "+unidad+" and tipo ="+tipo);
                List<string> row = dbPedido.Database.SqlQuery<string>(QueryString.ToString()).ToList();
                string fecha = row.First();

                QueryString = new StringBuilder();
                QueryString.Append("select m.hora from dbo.ma_inventario_dia m where m.id_unidad = " + unidad + " and tipo = " + tipo);
                QueryString.Append("and fecha = (select max(fecha) from dbo.ma_inventario_dia where id_unidad = " + unidad + " and tipo = " + tipo +" )");
                List<string> row_ = dbPedido.Database.SqlQuery<string>(QueryString.ToString()).ToList();
                string hrs = row_.First();

                fecha_hora = String.Format("{0:dd MMMM yyyy}", Convert.ToDateTime(fecha)) + " " + hrs +" hrs";
            }
            catch (Exception e)
            {
                fecha_hora = "S/C";
            }

            return fecha_hora;
        }

        public string GetInicialesUnidad_(int id_unidad)
        {
            string unidad_ = "";
            switch (id_unidad)
            {
                case 29:
                    unidad_ = "HGT";
                    break;
                case 52:
                    unidad_ = "HGM";
                    break;
                case 130000:
                    unidad_ = "HMI";
                    break;
                case 110000:
                    unidad_ = "HGI";
                    break;
                case 90000:
                    unidad_ = "HRU";
                    break;
                case 180000:
                    unidad_ = "IEC";
                    break;
                case 10051:
                    unidad_ = "CEH";
                    break;
                case 80000:
                    unidad_ = "J 1";
                    break;
                case 100000:
                    unidad_ = "J 2";
                    break;
                case 120000:
                    unidad_ = "J 3";
                    break;
                default:
                    unidad_ = "***";
                    break;
            }

            return unidad_;
        }

        //------ EXISTENCIAS DEL ALMACEN
        public List<InsumoClassSIAA> GetExistenciasDeAlmacenByTipo(int tipo, bool incluir_lotes)
        {
            List<InsumoClassSIAA> mdl = new List<InsumoClassSIAA>();
            AlmacenEntity dbAlmacen = new AlmacenEntity();
            StringBuilder QueryString = new StringBuilder();
            try {
                QueryString.Append("select ma.pk_articulos pk, ");
                if (incluir_lotes)
                {
                    QueryString.Append("lot.Lote lote, ");
                    QueryString.Append("convert(varchar(10), lot.Caducidad, 103)caducidad, ");
                    QueryString.Append("lot.Cant_Ent,  ");
                    QueryString.Append("lot.Cant_Sal, ");
                    QueryString.Append("(lot.Cant_Ent - lot.Cant_Sal) Existencia_lote, ");
                    QueryString.Append("lot.cant_aparta En_proceso, ");
                    QueryString.Append("cp.descrip Programa, ");
                }
                QueryString.Append("ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.tipo, 0) as varchar(3))))) + cast(isnull(ma.tipo, 0) as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.grupo, 0) as varchar(3))))) + cast(isnull(ma.grupo, 0) as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(isnull(ma.clave, 0) as varchar(4))))) + cast(isnull(ma.clave, 0) as varchar(4)))) + '-' + ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(isnull(ma.presentacion, 0) as varchar(4))))) + cast(isnull(ma.presentacion, 0) as varchar(4)))) clave_txt, ");
                QueryString.Append("convert(int, ma.existencia) existencia, ");
                QueryString.Append("convert(int, ma.tipo) tipo, ");
                QueryString.Append("convert(int, ma.grupo) grupo, ");
                QueryString.Append("convert(int, ma.clave) clave, ");
                QueryString.Append("convert(int, ma.presentacion) presentacion, ");
                QueryString.Append("ma.descripcion, ");
                QueryString.Append("m.desc_larga descLarga, ");
                QueryString.Append("(up.descrip + ' ' + cast(unidades as varchar) + ' ' + um.descrip) presentacion_txt ");
                QueryString.Append("from ma_articulos ma ");
                QueryString.Append("left join ma_articulos_desc_larga_tabla_corta m on ma.pk_articulos = m.pk_articulos ");
                QueryString.Append("left join ca_unidadesp up on ma.uni_pres = up.unidad ");
                QueryString.Append("left join ca_unidadesm um on ma.uni_med = um.unidad ");
                if (incluir_lotes)
                {
                    QueryString.Append("left join de_caducidad lot on (ma.pk_articulos = lot.pk_articulos and ma.existencia > 0) ");
                    QueryString.Append("left join ca_programas_salud cp on lot.no_prog = cp.no_prog ");
                }
                    

                switch (tipo)
                {
                    case 1:
                        QueryString.Append("where ma.tipo in(10,20,30,40,41) and ma.activo=1 ");
                        break;
                    case 2:
                        QueryString.Append("where ma.tipo in(60,130,531,537) and ma.activo=1 ");
                        break;
                    case 3:
                        QueryString.Append("where ma.tipo in(50,80,533,535,553) and ma.activo=1 ");
                        break;
                    case 4:
                        QueryString.Append("where ma.tipo in(70) and ma.activo=1 ");
                        break;
                    case 5:
                        QueryString.Append("where ma.tipo in(120) and ma.activo=1 ");
                        break;
                    case 6:
                        QueryString.Append("where ma.tipo in(100) and ma.activo=1 ");
                        break;
                    case 7:
                        QueryString.Append("where ma.tipo in(107) and ma.activo=1 ");
                        break;
                    case 8:
                        QueryString.Append("where ma.tipo not in(10,20,30,40,41,50,60,80,70,120,100,107,130,531,537,533,535,553) and ma.activo=1 ");
                        break;
                    default:
                        QueryString.Append("where ma.tipo in(10,20,30,40,41) and ma.activo=1 ");
                        break;
                }

                //INCLUIR LOTES Y SOLO CLAVES CON EXISTENCIA
                if (incluir_lotes)
                {
                    QueryString.Append("and ma.existencia > 0 ");
                    QueryString.Append("and(lot.Cant_Ent - lot.Cant_Sal) <> 0 ");
                }

                    //QueryString.Append("order by ma.tipo ");
                    QueryString.Append("order by ma.descripcion ");

                mdl = dbAlmacen.Database.SqlQuery<InsumoClassSIAA>(QueryString.ToString()).ToList();
            }
            catch (Exception e) { 
            
            }

            return mdl;
        }

        public List<LotesInfoClass> GetLotesByPkInsumo(string pk)
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();
            StringBuilder QueryString = new StringBuilder();
            List<LotesInfoClass> mdl = new List<LotesInfoClass>();
            try
            {
                
                QueryString.Append("select " +
                    "ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.tipo, 0) as varchar(3))))) + cast(isnull(ma.tipo, 0) as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.grupo, 0) as varchar(3))))) + cast(isnull(ma.grupo, 0) as varchar(3)))) + '-' + ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(isnull(ma.clave, 0) as varchar(4))))) + cast(isnull(ma.clave, 0) as varchar(4)))) + '-' + ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(isnull(ma.presentacion, 0) as varchar(4))))) + cast(isnull(ma.presentacion, 0) as varchar(4)))) Clave_txt, " +
                    "Lote,convert(varchar(10), Caducidad, 103) Caducidad, Cant_Ent, Cant_Sal,(Cant_Ent - Cant_Sal) Existencia,descrip Programa, cant_aparta En_proceso ");
                QueryString.Append("from de_caducidad de ");
                QueryString.Append("left join ca_programas_salud on de.no_prog = ca_programas_salud.no_prog ");
                QueryString.Append("left join ma_articulos ma on de.pk_articulos = ma.pk_articulos ");
                QueryString.Append("Where de.pk_articulos = '" + pk + "' ");
                QueryString.Append("and(Cant_Ent - Cant_Sal) <> 0 ");
                QueryString.Append("Order by Caducidad desc ");

                mdl = dbAlmacen.Database.SqlQuery<LotesInfoClass>(QueryString.ToString()).ToList();
            }
            catch(Exception e) {
                mdl = new List<LotesInfoClass>();
            }

            return mdl;
        }

    }
}
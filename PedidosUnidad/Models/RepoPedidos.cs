using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using PedidosUnidad.Models;
using PedidosUnidad.Models.DBAlmacen;
using PedidosUnidad.Models.DBPedido;
using PedidosUnidad.Security;

namespace PedidosUnidad.Models
{
    public class RepoPedidos
    {
        PedidoEntity dbPedido = null;
        public RepoPedidos()
        {
            dbPedido = new PedidoEntity();
        }

        public PedidosUnidadModelAlmCentral getPedidosUnidadAlmCentral(int unidad)
        {
            //unidad = 90000;
            AlmacenEntity dbA = new AlmacenEntity();
            PedidosUnidadModelAlmCentral mdl = new PedidosUnidadModelAlmCentral();

            StringBuilder QueryString = new StringBuilder();
            QueryString.Append("SELECT (CAST(pedido AS VARCHAR)+'/'+CAST(anio AS VARCHAR(4))) pedido ");
            QueryString.Append(", CONVERT(VARCHAR(10),FECHA_PEDIDO,103) fecha ");
            QueryString.Append(", ISNULL(OBSERVA,'') obs ");
            QueryString.Append(", pedido pedido_ori ");
            QueryString.Append(", CAST(anio AS int) anio_ori  ");
            QueryString.Append("FROM ma_pedidos ");
            QueryString.Append("WHERE CENTRO_SOL = " + unidad);
            QueryString.Append(" AND TIPO_PEDIDO != 9 ");
            QueryString.Append(" AND ANIO = 2022 ");
            QueryString.Append(" AND AFECTADO = 1 ");
            QueryString.Append(" ORDER BY anio DESC, fecha_pedido desc, pedido desc ");


            try
            {
                List<PedidoModelAlmCentral> lista = dbA.Database.SqlQuery<PedidoModelAlmCentral>(QueryString.ToString()).ToList();

                mdl.pedidos = lista;

            }
            catch (Exception e)
            {

            }

            return mdl;
        }







        //GET PEDIDOS CPM
        public CPMpedidosModel getPedidos(int id_unidad, int tipo_p)
        {
            CPMpedidosModel mdl = new CPMpedidosModel();
            try
            {
                List<ma_pedido> list = new List<ma_pedido>();
                if (id_unidad > 0)
                    list = (from r in dbPedido.ma_pedido where r.id_unidad == id_unidad && r.tipo_pedido == tipo_p select r).OrderByDescending(a => a.id).ToList();
                else
                    list = (from r in dbPedido.ma_pedido select r).ToList().OrderByDescending(a => a.fecha_registro).ToList();

                foreach (var item in list)
                {
                    PedidoModel pedido = new PedidoModel();
                    pedido.folio = item.folio;
                    pedido.id_pedido = item.id;
                    pedido.id_unidad = item.id_unidad;
                    pedido.descrip_unidad = item.descrip_unidad;
                    pedido.anio = item.anio.ToString();
                    pedido.mes = item.mes.ToString();
                    pedido.fecha = (item.fecha_registro ?? DateTime.Now).ToShortDateString();
                    pedido.claves_total = item.claves_total ?? 0;
                    pedido.claves_pedidas = item.claves_solicitadas ?? 0;
                    pedido.confirmado = item.pedido_confirmado ?? false;
                    pedido.estatus = item.estatus ?? 0;
                    pedido.folio_almacen = item.folio_almacen;
                    pedido.controlado = item.controlado ?? false;

                    if (item.tipo_pedido == 1)
                        pedido.descrip_tipo_pedido = "Medicamento";
                    if (item.tipo_pedido == 2)
                        pedido.descrip_tipo_pedido = "Curación";

                    pedido.pedido = item.pedido;
                    pedido.mes_des = this.getNameMes(item.mes ?? 1);

                    mdl.pedidos.Add(pedido);
                }

            }
            catch (Exception e)
            {

            }

            return mdl;
        }

        //GET CPM
        public PedidoModel getCPMunidad(int unidad, int tipo, bool conExistenciaCero)
        {
            //unidad = 90000;
            int anio = 2019;//DateTime.Now.Year;//
            AlmacenEntity dbA = new AlmacenEntity();
            PedidoModel mdl = new PedidoModel();

            StringBuilder QueryString = new StringBuilder();
            QueryString.Append("select m.articulos_pk as pk, m.centro_pk as centro, ");
            QueryString.Append("CASE ");
            QueryString.Append("WHEN d.Tipo < 60 ");
            QueryString.Append("THEN 1 ");
            QueryString.Append("ELSE 2 ");
            QueryString.Append("END as tipo ");
            QueryString.Append(", ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(d.tipo as varchar(3))))) + cast(d.tipo as varchar(3)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(d.grupo as varchar(3))))) + cast(d.grupo as varchar(3)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(d.clave as varchar(4))))) + cast(d.clave as varchar(4)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(d.presentacion as varchar(4))))) + cast(d.presentacion as varchar(4)))) clave ");
            QueryString.Append(", d.descripcion, m.cpm ");

            if (conExistenciaCero)
                QueryString.Append(", 0 existencia ");
            else
                QueryString.Append(", m.cpm existencia ");

            QueryString.Append(", m.origen ");
            QueryString.Append(", m.desc_programa ");
            QueryString.Append(", cast(m.desierta as bit) desierta");
            //QueryString.Append(", cast(CASE WHEN m.origen IS NULL THEN 1 ELSE 0 END as bit) as desierta"); 
            QueryString.Append(", cast(ISNULL(m.controlado, 0) as bit) controlado ");
            QueryString.Append(", ISNULL(m.max_consolidada, 0) maximo ");
            QueryString.Append(", ISNULL(m.min_consolidada, 0) minimo ");
            QueryString.Append(", m.consolidada consolidada ");
            QueryString.Append(", m.dimesa dimesa ");
            QueryString.Append(", m.max_dimesa max_dimesa ");
            QueryString.Append(",(up.descrip + ' ' + cast(unidades as varchar) + ' ' + um.descrip) presentacion ");
            QueryString.Append("from cpm_unidades m ");
            QueryString.Append("left ");
            QueryString.Append("join ma_articulos d on m.articulos_pk = d.pk_articulos ");
            QueryString.Append("left join ca_unidadesp up on d.uni_pres = up.unidad ");
            QueryString.Append("left join ca_unidadesm um on d.uni_med = um.unidad ");
            QueryString.Append("where m.centro_pk = " + unidad + " and m.anio = " + anio + " and m.activo = 1 ");
            //QueryString.Append("and d.Tipo < 60 --MEDICAMENTO ");
            //QueryString.Append("and d.Tipo in (60, 130, 531, 537) --MATERIAL DE CURACION ");

            QueryString.Append("order by d.tipo ");
            try
            {
                List<CPMrowModel> lista = dbA.Database.SqlQuery<CPMrowModel>(QueryString.ToString()).ToList();
                if (tipo == 0)//TODOS
                    mdl.articulos = lista;

                if (tipo == 1) //MEDICAMENTO
                    mdl.articulos = (from r in lista where r.tipo == 1 select r).ToList();

                if (tipo == 2) //MATERIAL DE CURACION
                    mdl.articulos = (from r in lista where r.tipo == 2 select r).ToList();

                if (lista.Count > 0) //SI TIENE UN CONFIGURACION CPM ASIGNADA
                {
                    mdl.tieneCPM = true;

                    mdl.claves_total = mdl.articulos.Count();
                    mdl.claves_medicamento = (from r in lista where r.tipo == 1 select r).ToList().Count();
                    mdl.claves_curacion = (from r in lista where r.tipo == 2 select r).ToList().Count();

                    //MEDICAMENTO
                    if (tipo == 1)
                    {
                        mdl.claves_total = (from r in lista where r.tipo == 1 select r).ToList().Count();
                        mdl.claves_medicamento = (from r in lista where r.tipo == 1 select r).ToList().Count();
                        mdl.claves_curacion = 0;
                    }
                    if (tipo == 2)
                    {
                        mdl.claves_total = (from r in lista where r.tipo == 2 select r).ToList().Count();
                        mdl.claves_medicamento = 0;
                        mdl.claves_curacion = (from r in lista where r.tipo == 2 select r).ToList().Count();
                    }

                }




            }
            catch (Exception e)
            {

            }

            return mdl;
        }

        //GET PEDIDO
        public PedidoModel getPedido(int id, int id_unidad)
        {
            PedidoModel mdl = new PedidoModel();

            try
            {
                //GET CPM UNIDADES
                //mdl = this.getCPMunidad(id_unidad, 0, true);

                //GET CABECERO
                ma_pedido pedido = dbPedido.ma_pedido.SingleOrDefault(a => a.id == id && a.id_unidad == id_unidad);
                mdl.id_pedido = pedido.id;
                mdl.id_unidad = pedido.id_unidad;
                mdl.descrip_unidad = pedido.descrip_unidad;
                mdl.editando = true;
                mdl.tipo_pedido = pedido.tipo_pedido ?? 0;
                mdl.pedido = pedido.pedido;
                mdl.folio = pedido.folio;
                mdl.folio_almacen = pedido.folio_almacen;
                mdl.fecha = String.Format("{0:dd/MM/yyyy}", pedido.fecha_envio);
                mdl.mes = String.Format("{0:MMMM}", pedido.fecha_envio).ToUpper();
                mdl.anio = (pedido.fecha_envio ?? DateTime.Now).Year.ToString();

                //AQUI SE TENDRIA QUE AGREGAR MAS TIPOS COMO PAPELERIA, LABORATORIO ETC. 
                if (mdl.tipo_pedido == 1)
                    mdl.descrip_tipo_pedido = "MEDICAMENTO";
                if (mdl.tipo_pedido == 2)
                    mdl.descrip_tipo_pedido = "MATERIAL DE CURACIÓN";

                mdl.fecha_envio = pedido.fecha_envio ?? DateTime.Now;

                 //GET DETALLE
                List<det_pedido> detalle = (from r in dbPedido.det_pedido where r.id_pedido == id && r.id_unidad == id_unidad select r).ToList();
                foreach (var item in detalle)
                {
                    CPMrowModel row = new CPMrowModel();
                    row.pk = item.PK;
                    row.centro = 0;
                    row.clave = item.cve;
                    row.descripcion = item.descripcion;
                    row.cpm = 0;
                    row.existencia = 0;
                    row.solicita = item.solicitadas ?? 0;
                    row.origen = item.origen;
                    row.tipo = item.tipo ?? 0;
                    row.programa = "";
                    row.desierta = false;
                    row.controlado = false;
                    row.presentacion = item.presentacion;
                    row.subtipo_insumo = item.subtipo_insumo;

                    mdl.articulos.Add(row);
                }



                //foreach (var item in mdl.articulos)
                //{
                //    det_pedido row = (from r in detalle where r.PK == item.pk select r).First();
                //    if (row != null)
                //    {
                //        item.solicita = row.solicitadas ?? 0;
                //        item.existencia = row.existencias ?? 0;
                //    }
                //}
            }
            catch (Exception e)
            {

            }

            return mdl;
        }

        //SAVE PEDIDO
        public PedidoModel savePedido(PedidoModel mdl, CurrentUser user)
        {
            try
            {
                ma_pedido pedido = new ma_pedido();
                pedido.id = this.getMaxId(user.id_unidad);
                pedido.id_unidad = user.id_unidad;
                pedido.anio = DateTime.Now.Year;
                pedido.mes = mdl.mes_ordinario; //DateTime.Now.Month;
                pedido.tipo_pedido = mdl.tipo_pedido;
                pedido.pedido = mdl.pedido;
                pedido.descrip_unidad = user.nom_unidad;
                pedido.claves_total = mdl.articulos.Count();
                pedido.claves_solicitadas = (from r in mdl.articulos where r.solicita > 0 select r).ToList().Count();
                pedido.id_usuario = user.id_user;
                pedido.nombre_usuario = user.name_user;
                pedido.fecha_registro = DateTime.Now;
                pedido.fecha_actualiza = DateTime.Now;
                pedido.fecha_confirmado = null;
                pedido.pedido_confirmado = false;
                pedido.folio = pedido.anio + "/" + (pedido.mes ?? 0).ToString("00") + pedido.id_unidad.ToString("000000") + pedido.id.ToString("00"); //MES_UNIDAD_CONSECUTIVO
                pedido.estatus = mdl.tipo_guardado; //0 = TRABAJANDO EN UNIDAD, 1 = PEDIDO ENVIADO, 2 PEDIDO EN PROCEOS, 3 LISTO PARA SER ENVIADO 
                pedido.controlado = mdl.controlado;
                

                if (mdl.tipo_guardado == 1)
                {
                    pedido.fecha_envio = DateTime.Now;
                }

                dbPedido.ma_pedido.Add(pedido);
                dbPedido.SaveChanges();

                if (mdl.tipo_pedido == 1)
                    mdl.descrip_tipo_pedido = "MEDICAMENTO";
                if (mdl.tipo_pedido == 2)
                    mdl.descrip_tipo_pedido = "MATERIAL DE CURACIÓN";

                mdl.id_pedido = pedido.id;
                mdl.id_unidad = pedido.id_unidad;
                mdl.folio = pedido.folio;


                int id_ = 1;
                foreach (var item in (from r in mdl.articulos where r.solicita > 0 select r).ToList())
                {
                    try
                    {
                        det_pedido det = new det_pedido();
                        det.id_pedido = pedido.id;
                        det.id_unidad = pedido.id_unidad;
                        det.id = id_;
                        det.PK = item.pk;
                        det.cve = item.clave;
                        det.descripcion = item.descripcion;
                        det.cpm = item.cpm;
                        det.origen = item.origen;
                        det.existencias = item.existencia;
                        det.solicitadas = item.solicita;
                        det.fecha = DateTime.Now;
                        det.tipo = mdl.tipo_pedido;
                        det.presentacion = item.presentacion;
                        det.subtipo_insumo = item.subtipo_insumo;
                        dbPedido.det_pedido.Add(det);
                        dbPedido.SaveChanges();
                        id_++;
                    }
                    catch
                    {

                    }

                }

                mdl.claves_total = pedido.claves_total ?? 0;
                mdl.claves_pedidas = pedido.claves_solicitadas ?? 0;
                mdl.folio = pedido.anio + "/" + pedido.id_unidad.ToString("000000") + pedido.id.ToString("00") + (pedido.mes ?? 0).ToString("00");
                mdl.descrip_unidad = pedido.descrip_unidad;

                //DATOS DE YA ESTA ENVIANDO EL PEDIDO AL ALMACEN (MOSTRAR EN UNA PANTALLA)
                if (mdl.tipo_guardado == 1)
                {
                    mdl.fecha = String.Format("{0:dd/MM/yyyy}", pedido.fecha_envio);
                    mdl.mes = String.Format("{0:MMMM}", pedido.fecha_envio).ToUpper();
                    mdl.fecha_envio = pedido.fecha_envio ?? DateTime.Now;

                    //mdl.fecha = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss}", pedido.fecha_registro);
                    mdl.dia_no = (pedido.fecha_envio ?? DateTime.Now).Day.ToString("00");
                    mdl.dia_des = String.Format("{0:dddd}", pedido.fecha_envio);
                    mdl.mes_des = String.Format("{0:MMMM}", pedido.fecha_envio);
                    mdl.anio = (pedido.fecha_envio ?? DateTime.Now).Year.ToString();
                    mdl.hrs_des = String.Format("{0:HH:mm}", pedido.fecha_envio);
                }



                mdl.exito = true;
                mdl.msg = "Pedido " + mdl.folio + " guardado correctamente.";
            }
            catch (Exception e)
            {
                mdl.exito = false;
                mdl.msg = "Ocurrio un erro al momento de guardar el pedido";
            }

            return mdl;
        }

        public PedidoModel editPedido(PedidoModel mdl, CurrentUser user)
        {
            try
            {

                //GET CABECERO
                ma_pedido pedido = dbPedido.ma_pedido.SingleOrDefault(a => a.id == mdl.id_pedido && a.id_unidad == mdl.id_unidad);
                pedido.claves_total = mdl.articulos.Count();
                pedido.claves_solicitadas = (from r in mdl.articulos where r.solicita > 0 select r).ToList().Count();
                pedido.fecha_actualiza = DateTime.Now;
                pedido.estatus = mdl.tipo_guardado; //0 = TRABAJANDO EN UNIDAD, 1 = PEDIDO ENVIADO, 2 PEDIDO EN PROCEOS, 3 LISTO PARA SER ENVIADO 

                if (mdl.tipo_guardado == 1)
                {
                    pedido.fecha_envio = DateTime.Now;
                }
                dbPedido.SaveChanges();

                //CLEAN DETALLE
                bool Rexito = this.initDetallePedido(mdl.id_pedido, mdl.id_unidad);
                if (Rexito)
                {
                    int id_ = 1;
                    foreach (var item in mdl.articulos)
                    {
                        try
                        {
                            det_pedido det = new det_pedido();
                            det.id_pedido = pedido.id;
                            det.id_unidad = pedido.id_unidad;
                            det.id = id_;
                            det.PK = item.pk;
                            det.cve = item.clave;
                            det.descripcion = item.descripcion;
                            det.cpm = item.cpm;
                            det.existencias = item.existencia;
                            det.solicitadas = item.solicita;
                            det.fecha = DateTime.Now;
                            det.tipo = mdl.tipo_pedido;
                            det.presentacion = item.presentacion;
                            det.subtipo_insumo = item.subtipo_insumo;

                            dbPedido.det_pedido.Add(det);
                            dbPedido.SaveChanges();
                            id_++;
                        }
                        catch (Exception e)
                        {

                        }
                    }


                    mdl.claves_total = pedido.claves_total ?? 0;
                    mdl.claves_pedidas = pedido.claves_solicitadas ?? 0;
                    mdl.folio = pedido.anio + "/" + pedido.id_unidad.ToString("000000") + pedido.id.ToString("00") + (pedido.mes ?? 0).ToString("00");
                    mdl.descrip_unidad = pedido.descrip_unidad;
                    mdl.pedido = pedido.pedido;

                    //DATOS DE YA ESTA ENVIANDO EL PEDIDO AL ALMACEN (MOSTRAR EN UNA PANTALLA)
                    if (mdl.tipo_guardado == 1)
                    {
                        mdl.fecha = String.Format("{0:dd/MM/yyyy}", pedido.fecha_envio);
                        mdl.mes = String.Format("{0:MMMM}", pedido.fecha_envio).ToUpper();
                        mdl.fecha_envio = pedido.fecha_envio ?? DateTime.Now;

                        //mdl.fecha = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss}", pedido.fecha_registro);
                        mdl.dia_no = (pedido.fecha_envio ?? DateTime.Now).Day.ToString("00");
                        mdl.dia_des = String.Format("{0:dddd}", pedido.fecha_envio);
                        mdl.mes_des = String.Format("{0:MMMM}", pedido.fecha_envio);
                        mdl.anio = (pedido.fecha_envio ?? DateTime.Now).Year.ToString();
                        mdl.hrs_des = String.Format("{0:HH:mm}", pedido.fecha_envio);
                    }

                    if (mdl.tipo_pedido == 1)
                        mdl.descrip_tipo_pedido = "MEDICAMENTO";
                    if (mdl.tipo_pedido == 2)
                        mdl.descrip_tipo_pedido = "MATERIAL DE CURACIÓN";

                    mdl.exito = true;
                    mdl.msg = "Pedido " + mdl.folio + " guardado correctamente.";


                }

            }
            catch (Exception e)
            {
                mdl.exito = false;
                mdl.msg = "Ocurrio un erro al momento de guardar el pedido";
            }

            return mdl;
        }

        public PedidoModel existeOrdinarioParaMesEncurso(int id_unidad, int tipo)
        {
            PedidoModel mdl = new PedidoModel();

            try
            {
                int anio = DateTime.Now.Year;
                int mes = DateTime.Now.Month;
                int claves_pedidas = 0;

                //VERIFICAR QUE NO EXISTA UN PEDIDO PARA EL MES EN CURSO.
                List<ma_pedido> pedidos = (from r in dbPedido.ma_pedido where r.id_unidad == id_unidad && r.mes == mes && r.anio == anio && r.tipo_pedido == tipo && r.pedido == "ORDINARIO" select r).ToList();
                //SI EXISTE YA UN PEDIDO PARA EL MES EN CURSO DE LA UNIDAD CORRESPONDIENTE. NO PROCEDE
                if (pedidos.Count > 0)
                {
                    ma_pedido ped = pedidos.First();
                    mdl.existe_pedido = true;
                    mdl.tieneCPM = true;
                    mdl.fecha = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss}", ped.fecha_registro);
                    mdl.folio = ped.folio;
                }
            }
            catch (Exception e)
            {

            }

            return mdl;
        }

        //GET PEDIDO BY EXISTENCIAS DIARIA ()
        //GET PEDIDO
        public PedidoModel getPedidoByExistencia(int id_unidad, int tipo)
        {
            PedidoModel mdl = new PedidoModel();

            try
            {
                int anio = DateTime.Now.Year;
                int mes = DateTime.Now.Month;
                int claves_pedidas = 0;

                //VERIFICAR QUE NO EXISTA UN PEDIDO PARA EL MES EN CURSO.
                List<ma_pedido> pedidos = (from r in dbPedido.ma_pedido where r.id_unidad == id_unidad && r.mes == mes && r.anio == anio && r.tipo_pedido == tipo && r.pedido == "ORDINARIO" select r).ToList();
                //SI EXISTE YA UN PEDIDO PARA EL MES EN CURSO DE LA UNIDAD CORRESPONDIENTE. NO PROCEDE
                if (pedidos.Count > 0)
                {
                    ma_pedido ped = pedidos.First();
                    mdl.existe_pedido = true;
                    mdl.tieneCPM = true;
                    mdl.fecha = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss}", ped.fecha_registro);
                    mdl.folio = ped.folio;
                }
                else
                {
                    //DE LO CONTRARIO REALIZAR PROCESO PARA CRUZAR EXISTENCIAS DIARA CON PEDIDO.

                    //GET CPM UNIDADES
                    mdl = this.getCPMunidad(id_unidad, tipo, true);

                    //GET CABECERO EXISTENCIAS
                    string query_u = "select * from ma_inventario_dia where id_unidad = " + id_unidad + " order by id desc";
                    List<ma_inventario_dia> registros_f = dbPedido.Database.SqlQuery<ma_inventario_dia>(query_u).ToList();
                    //SI EXISTEN REGISTRO TOMAR EL ULTIMO CAPTURADO.
                    if (registros_f.Count > 0)
                    {
                        ma_inventario_dia regiEseDia = registros_f.First();
                        string ultimo_dia_captura = (regiEseDia.fecha ?? DateTime.Now).ToShortDateString();

                        mdl.id_unidad = id_unidad;//pedido.id_unidad;
                        mdl.id_pedido = 1;//pedido.id;                    
                        mdl.editando = false;

                        //OBTENER EL DETALLE DE ESE DIA ENCONTRADO
                        List<det_inventario_articulo> ExistenciasDeEseDia = (from r in dbPedido.det_inventario_articulo where r.id_unidad == regiEseDia.id_unidad && r.id_inv == regiEseDia.id select r).ToList();
                        List<CPMrowModel> articulos_a_pedir = new List<CPMrowModel>();
                        foreach (var item in mdl.articulos)
                        {
                            det_inventario_articulo row = (from r in ExistenciasDeEseDia where r.pk_articulo == item.pk select r).First();
                            if (row != null)
                            {
                                //item.existencia = ((row.existencias ?? 0) == 0) ? item.cpm : row.existencias ?? 0;
                                item.existencia = row.existencias ?? 0;

                                int soli = item.cpm - item.existencia;
                                item.solicita = (soli < 0) ? 0 : soli;

                                if (item.solicita > 0)
                                {
                                    mdl.claves_pedidas = claves_pedidas + 1;
                                    claves_pedidas = claves_pedidas + 1;
                                    articulos_a_pedir.Add(item);
                                }


                            }
                        }

                        mdl.articulos = new List<CPMrowModel>();
                        mdl.articulos = articulos_a_pedir;
                    }
                    else
                    {
                        foreach (var item in mdl.articulos)
                        {
                            item.existencia = item.cpm;
                            item.solicita = item.cpm - item.existencia;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            //ORDENAR POR LOS QUE SE PIDEN MAS
            mdl.articulos = mdl.articulos.OrderByDescending(b => b.solicita).ToList();
            return mdl;
        }


        //CONFIRM PEDIDO
        public ResultClass confirmPedido(int id, int id_unidad)
        {
            ResultClass mdl = new ResultClass();
            try
            {
                ma_pedido pedido = dbPedido.ma_pedido.SingleOrDefault(a => a.id == id && a.id_unidad == id_unidad);
                pedido.pedido_confirmado = true;
                dbPedido.SaveChanges();
                mdl.exito = true;
                mdl.msg = "Pedido enviado correctamente";
            }
            catch (Exception e)
            {
                mdl.exito = false;
                mdl.msg = "Ocurrio un error, el pedido no pudo ser enviado";
            }
            return mdl;
        }

        //LIMPIAR DETALLE DE PEDIDO 
        public bool initDetallePedido(int id, int id_unidad)
        {
            bool exito = false;
            try
            {
                List<det_pedido> detalle_pedido = (from r in dbPedido.det_pedido where r.id_pedido == id && r.id_unidad == id_unidad select r).ToList();
                dbPedido.det_pedido.RemoveRange(detalle_pedido);
                dbPedido.SaveChanges();
                exito = true;
            }
            catch (Exception e)
            {
                exito = false;
            }
            return exito;
        }

        //GET MAX ID PEDIDO
        public int getMaxId(int unidad)
        {
            int id = 0;
            try
            {
                id = (from r in dbPedido.ma_pedido where r.id_unidad == unidad select r).ToList().Max(a => a.id);
                id = id + 1;
            }
            catch
            {
                id = 1;
            }
            return id;
        }

        public configuracion_unidades getConfigUnidad(int id_unidad)
        {
            configuracion_unidades datos = dbPedido.configuracion_unidades.SingleOrDefault(a=> a.id_unidad == id_unidad);
            return datos;
        }

        //GET DATOS DE INSUMO PARA AGREGAR 
        public DatosInsumosClass getDatosInsumos(string pk)
        {
            DatosInsumosClass mdl = new DatosInsumosClass();
            //get datos de ma articulos por pk
            StringBuilder QueryString = new StringBuilder();
            QueryString.Append("select ");
            QueryString.Append("PK_ARTICULOS pk, m.DESCRIPCION descripcion, (up.descrip + ' ' + cast(unidades as varchar) + ' ' + um.descrip) presentacion, EXISTENCIA existencia_almacen, 0 existencia_unidad, ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(m.tipo as varchar(3))))) + cast(m.tipo as varchar(3)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(m.grupo as varchar(3))))) + cast(m.grupo as varchar(3)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(m.clave as varchar(4))))) + cast(m.clave as varchar(4)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(m.presentacion as varchar(4))))) + cast(m.presentacion as varchar(4)))) cve, ");
            QueryString.Append("ISNULL(m.id_categoria, 0) sub_tipo_insumo, "); 
            QueryString.Append("ISNULL(cain.descripcion, 'NA')  descrip_tipo_insumo ");
            QueryString.Append("from dbo.ma_articulos m ");
            QueryString.Append("left join ca_unidadesp up on m.uni_pres = up.unidad ");
            QueryString.Append("left join ca_unidadesm um on m.uni_med = um.unidad ");
            QueryString.Append("left join ca_categoria_insumo cain on m.id_categoria = cain.id ");
            QueryString.Append("where PK_ARTICULOS = '"+ pk + "' ");

            AlmacenEntity dbA = new AlmacenEntity();
            List<DatosInsumosClass> lista = dbA.Database.SqlQuery<DatosInsumosClass>(QueryString.ToString()).ToList();
            mdl = lista.First(); 
            //get existencias

            return mdl;
        }

        //OBTENER VALE DEL PEDIDO DE BD ALMACEN
        public PedidoModel getValeAlmacen(int anio, int folio)
        {
            PedidoModel mdl = new PedidoModel();
            AlmacenEntity dbA = new AlmacenEntity();

            try {
                StringBuilder QueryString = new StringBuilder();
                QueryString.Append("exec Get_Vale " + folio + "," + anio);
                List<valeAlmacen> vale = dbA.Database.SqlQuery<valeAlmacen>(QueryString.ToString()).ToList();


                if (vale.Count() > 0)
                {
                    mdl.descrip_unidad = vale[0].centro_solicitante;
                    //mdl.descrip_tipo_pedido = vale[0].tipo_vale;
                    mdl.descrip_tipo_vale = vale[0].tipo_vale;
                    //mdl.pedido = vale[0].categoria;
                    mdl.fecha = vale[0].fecha_vale;
                    mdl.mes = vale[0].mes.ToUpper();
                    mdl.anio = vale[0].anio;
                    mdl.folio_almacen = vale[0].no_vale;
                    //mdl.folio = ""; //SE ASIGNA FUERA EN EL CONTROLADOR. DE PEDIDO
                    //mdl.QR;

                    //Domicilio de unidad (datos)
                    mdl.domicilio_unidad = vale[0].direccion;
                    mdl.cp_unidad = vale[0].codpost;
                    mdl.rfc_unidad = vale[0].rfc;
                    mdl.tel_unidad = vale[0].tel;
                    mdl.estado_unidad = vale[0].Entidad;
                    mdl.municipio_unidad = vale[0].Municipio;

                    string clave = "";
                    foreach (valeAlmacen item in vale)
                    {
                        CPMrowModel art = new CPMrowModel();
                        art.presentacion = item.presentacion;
                        art.clave = item.clave;
                        art.descripcion = item.descripcion;
                        art.costo_promedio = item.costo_prom;
                        //art. = item.centro_solicitante;
                        art.solicita = Convert.ToInt32(item.cant_sol);
                        art.surtida = Convert.ToInt32(item.cant_surt);


                        List<valeAlmacen> lotes = (from f in vale where f.clave == item.clave select f).ToList();
                        foreach (valeAlmacen lot in lotes)
                        {
                            lotesInsumo lotin = new lotesInsumo();

                            lotin.lote = lot.lote;
                            lotin.f_caducidad = lot.caducidad;
                            lotin.programa = lot.programa_lote;
                            lotin.costo = lot.costo_lote;
                            lotin.importe = lot.importe_lote;
                            lotin.cantidad_lote = lot.cant_lote;

                            //sumar importe lote. 
                            //sumar iva

                            art.lotes.Add(lotin);
                        }

                        if (clave != item.clave)
                        {
                            mdl.articulos.Add(art);
                        }

                        clave = item.clave;
                        
                    }

                    //RESUMEN
                    QueryString = new StringBuilder();
                    QueryString.Append("exec Get_Vale_Resumen " + folio + "," + anio);
                    List<valeResumen> resumen = dbA.Database.SqlQuery<valeResumen>(QueryString.ToString()).ToList();
                    if (resumen.Count > 0)
                    {
                        mdl.resumen_vale = resumen[0];
                    }

                    //
                    QueryString = new StringBuilder();
                    QueryString.Append("exec Get_Vale_Totales " + folio + "," + anio);
                    List<valeTotales> totalResumen = dbA.Database.SqlQuery<valeTotales>(QueryString.ToString()).ToList();
                    if (totalResumen.Count > 0)
                    {
                        mdl.resumen_suma = totalResumen[0];
                    }

                }
            }
            catch (Exception e)
            {
                mdl = new PedidoModel();
                mdl.exito = false;
            }
            


            

            return mdl;
        }

        //OBTENER LA NEGATIVA DE UN PEDIDO
        public PedidoModel getNegativaAlmacen(int anio, int folio)
        {
      
            PedidoModel mdl = new PedidoModel();
            AlmacenEntity dbA = new AlmacenEntity();

            try
            {
                StringBuilder QueryString = new StringBuilder();
                QueryString.Append("exec Get_Vale_NS " + folio + "," + anio);
                List<valeAlmacen> vale = dbA.Database.SqlQuery<valeAlmacen>(QueryString.ToString()).ToList();


                if (vale.Count() > 0)
                {
                    mdl.descrip_unidad = vale[0].centro_solicitante;
                    //mdl.descrip_tipo_pedido = vale[0].tipo_vale;
                    mdl.descrip_tipo_vale = vale[0].tipo_vale;
                    //mdl.pedido = vale[0].categoria;
                    mdl.fecha = vale[0].fecha_vale;
                    mdl.mes = vale[0].mes.ToUpper();
                    mdl.anio = vale[0].anio;
                    mdl.folio_almacen = vale[0].no_vale;
                    //mdl.folio = ""; //SE ASIGNA FUERA EN EL CONTROLADOR. DE PEDIDO
                    //mdl.QR;

                    //Domicilio de unidad (datos)
                    mdl.domicilio_unidad = vale[0].direccion;
                    mdl.cp_unidad = vale[0].codpost;
                    mdl.rfc_unidad = vale[0].rfc;
                    mdl.tel_unidad = vale[0].tel;
                    mdl.estado_unidad = vale[0].Entidad;
                    mdl.municipio_unidad = vale[0].Municipio;

                    string clave = "";
                    foreach (valeAlmacen item in vale)
                    {
                        CPMrowModel art = new CPMrowModel();
                        art.presentacion = item.presentacion;
                        art.clave = item.clave;
                        art.descripcion = item.descripcion;
                        art.costo_promedio = item.costo_prom;
                        art.costo_insumo = item.costo_insumo;
                        //art. = item.centro_solicitante;
                        art.solicita = Convert.ToInt32(item.cant_sol);
                        art.surtida = Convert.ToInt32(item.cant_surt);


                        List<valeAlmacen> lotes = (from f in vale where f.clave == item.clave select f).ToList();
                        foreach (valeAlmacen lot in lotes)
                        {
                            lotesInsumo lotin = new lotesInsumo();

                            lotin.lote = lot.lote;
                            lotin.f_caducidad = lot.caducidad;
                            lotin.programa = lot.programa_lote;
                            lotin.costo = lot.costo_lote;
                            lotin.importe = lot.importe_lote;
                            lotin.cantidad_lote = lot.cant_lote;

                            //sumar importe lote. 
                            //sumar iva

                            art.lotes.Add(lotin);
                        }

                        if (clave != item.clave)
                        {
                            mdl.articulos.Add(art);
                        }

                        clave = item.clave;

                    }                 

                }
            }
            catch (Exception e)
            {
                mdl = new PedidoModel();
                mdl.exito = false;
            }





            return mdl;
        }

        //GET MESES DISPONIBLES PARA ORDINARIO
        public List<mesesCls> getMesesOrdinario(int id_unidad, int tipo, bool controlado)
        {
            int mes_curso = DateTime.Now.Month;
            int anio = DateTime.Now.Year;
            bool disponible = false;

            List<ma_pedido> pedidos = (from r in dbPedido.ma_pedido where r.anio == anio && r.id_unidad == id_unidad && r.tipo_pedido == tipo && r.controlado == controlado select r).OrderBy(a=> a.mes).ToList();

            List<mesesCls> meses = new List<mesesCls>();

            for (int i = 1; i <= 12; i++)
            {
                mesesCls Mesin = new mesesCls();
                Mesin.anio = DateTime.Now.Year;
                Mesin.MesNumber = i;
                Mesin.MesName = this.getNameMes(i);
                Mesin.actual = false;
                Mesin.controlado = controlado;
                Mesin.tipo = tipo;

                if (i < mes_curso)
                {
                    //No disponible (ya paso el mes)
                    Mesin.estatus = 3; //1 Realizado, 2 Disponible, 3 No Disponible
                    Mesin.estatus_desc = "No Disponible";
                    Mesin.estatus_class = "no-dispo";
                     
                }
                else
                {
                    if (i == mes_curso)
                    {
                        Mesin.actual = true;
                    }
                    if (!disponible)
                    {
                        //si es mayor o igual al mes en curso y no hay ningun otro disponible esta disponoble a reserva de que ya exista en tabla
                        Mesin.estatus = 2; //1 Realizado, 2 Disponible, 3 No Disponible
                        Mesin.estatus_desc = "Disponible";
                        Mesin.estatus_class = "dispo"; 
                        disponible = true;
                    }
                    else {
                        Mesin.estatus = 4; //1 Realizado, 2 Disponible, 3 No Disponible, 4 Pendiente
                        Mesin.estatus_desc = "Pendiente";
                        Mesin.estatus_class = "siguiente-mes";   
                        //disponible = false;
                    }
                    

                }

                //si ya existe,
                List<ma_pedido> mes_dis = (from a in pedidos where a.mes == i select a).ToList();
                if (mes_dis.Count() > 0)
                {
                    Mesin.estatus = 1; //1 Realizado, 2 Disponible, 3 No Disponible
                    Mesin.estatus_desc = "Realizado";
                    Mesin.estatus_class = "realizado"; 
                    disponible = false;
                }


                meses.Add(Mesin);
            }

            return meses;

        }

        public bool getOrdinarioMes(int id_unidad, int tipo, bool controlado)
        {
            bool tieneOrdinario = false;
            int mes_curso = DateTime.Now.Month;
            int anio = DateTime.Now.Year;

            List<ma_pedido> pedidos = (from r in dbPedido.ma_pedido where r.anio == anio && r.id_unidad == id_unidad && r.tipo_pedido == tipo && r.controlado == controlado && r.mes == mes_curso select r).OrderBy(a => a.mes).ToList();
            if (pedidos.Count > 0)
            {
                tieneOrdinario = true;
            }
            else {
                tieneOrdinario = false;
            }

            return tieneOrdinario;
        }

        public string getNameMes(int mes)
        {
            string nombre_mes = "";
            switch (mes)
            {
                case 1:
                    nombre_mes = "ENERO";
                    break;
                case 2:
                    nombre_mes = "FEBRERO";
                    break;
                case 3:
                    nombre_mes = "MARZO";
                    break;
                case 4:
                    nombre_mes = "ABRIL";
                    break;
                case 5:
                    nombre_mes = "MAYO";
                    break;
                case 6:
                    nombre_mes = "JUNIO";
                    break;
                case 7:
                    nombre_mes = "JULIO";
                    break;
                case 8:
                    nombre_mes = "AGOSTO";
                    break;
                case 9:
                    nombre_mes = "SEPTIEMBRE";
                    break;
                case 10:
                    nombre_mes = "OCTUBRE";
                    break;
                case 11:
                    nombre_mes = "NOVIEMBRE";
                    break;
                case 12:
                    nombre_mes = "DICIEMBRE";
                    break;
            }

            return nombre_mes;
        }

        public string getNameMunicipio(int id)
        {
            string nombre_ = "";
            switch (id)
            {                
                case 0:
                    nombre_ = "NO DISPONIBLE";
                    break;  
                case 1:
                    nombre_ = "ARMERIA";
                    break;
                case 2:
                    nombre_ = "COLIMA";
                    break;
                case 3:
                    nombre_ = "COMALA";
                    break;
                case 4:
                    nombre_ = "COQUIMATLAN";
                    break;
                case 5:
                    nombre_ = "CUAUHTEMOC";
                    break;
                case 6:
                    nombre_ = "IXTLAHUACAN";
                    break;
                case 7:
                    nombre_ = "MANZANILLO";
                    break;
                case 8:
                    nombre_ = "MINATITLAN";
                    break;
                case 9:
                    nombre_ = "TECOMAN";
                    break;
                case 10:
                    nombre_ = "VILLA DE ALVAREZ";
                    break;
            }

            return nombre_;
        }

        public DatosUnidadClass getDomicilioUnidad(int unidad)
        {
            //GET DATOS UNIDAD , DOMICILIO
            AlmacenEntity dbA = new AlmacenEntity();
            DatosUnidadClass mdl = new DatosUnidadClass();

            StringBuilder QueryString = new StringBuilder();
            QueryString.Append("select ");
            QueryString.Append("cc.descrip centro_solicitante, isnull(cc.direccion, '') direccion  ");
            QueryString.Append(", isnull(cp.desc_asenta, '') colonia, convert(varchar, isnull(cp.codigo, 0)) codpost  ");
            QueryString.Append(", isnull(cm.[desc], '') Municipio, isnull(e.descripcio, '') Entidad  ");
            QueryString.Append(", cc.tel, cc.rfc  ");
            QueryString.Append("from ca_centros cc  ");
            QueryString.Append("Left Join Entidades E on cc.entidad_id = E.claven  ");
            QueryString.Append("Left join Cat_Municipio CM on cc.id_municipio = CM.id_municipio and cc.entidad_id = CM.entidad_id  ");
            QueryString.Append("Left Join Cat_CodigosPostales CP on cc.pk_cp = CP.pk_cp  ");
            QueryString.Append("where cc.centro =  " + unidad);

            List<DatosUnidadClass> rows_result = dbA.Database.SqlQuery<DatosUnidadClass>(QueryString.ToString()).ToList();

            if (rows_result.Count() > 0)
            {
                mdl.direccion = rows_result[0].direccion;
                mdl.codpost = rows_result[0].codpost;
                mdl.rfc = rows_result[0].rfc;
                mdl.tel = rows_result[0].tel;
                mdl.Entidad = rows_result[0].Entidad;
                mdl.Municipio = rows_result[0].Municipio;
            }
            else
            {
                mdl = new DatosUnidadClass();
            
            }

            return mdl;
        }

       

        public List<ReportInsumos> getReporteChavoya()
        {
            AlmacenEntity dbA = new AlmacenEntity();
            List<ReportInsumos> listado = new List<ReportInsumos>();

            //string[] clavelines = { "010-000-5233-00","010-000-1541-00","010-000-2114-00","010-000-4140-00","010-000-2018-00","010-000-5356-00","010-000-0108-00","010-000-0476-00","010-000-0104-00","010-000-4578-00","010-000-4251-00","040-000-2613-00","010-000-1042-00","010-000-4299-00","010-000-5292-00","010-000-5640-00","010-000-4301-00","010-000-5244-00","010-000-4162-00","010-000-4271-00","010-000-2262-00","010-000-5302-00","010-000-0623-00","010-000-4512-02","010-000-4442-00","010-000-1546-00","010-000-0446-00","040-000-3259-00","010-000-4227-00","010-000-5653-00","010-000-1006-00","010-000-5487-00","010-000-5176-00","040-000-2654-00","040-000-3255-00","010-000-1924-00","010-000-0504-00","010-000-0611-00","010-000-5255-00","010-000-1972-00","010-000-2016-00","010-000-2128-00","010-000-2127-00","010-000-1929-00","010-000-4490-00","010-000-1206-00","010-000-5295-01","010-000-5284-00","010-000-5264-01","010-000-3610-00","010-000-4507-00","010-000-4505-00","010-000-0626-01","010-000-2135-00","010-000-1955-00","010-000-1954-00","010-000-3624-00","010-000-3606-00","010-000-1051-01","010-000-4118-00","010-000-4249-00","010-000-0267-00","010-000-0109-00","010-000-1309-00","010-000-4252-00","010-000-1542-00","010-000-4149-00","010-000-0473-00","010-000-5445-00",            "010-000-3616-00" };
            string[] clavelines = { "010-000-3608-00", "010-000-3609-00", "010-000-3610-00", "010-000-3627-00", "010-000-3616-00", "010-000-3615-00", "010-000-3606-00", "010-000-3607-00" };

            try
            {
                foreach (var item in clavelines)
                {
                    RowRepoInsumos saltoLinea = new RowRepoInsumos();
                    saltoLinea.pk = "---";
                    saltoLinea.mes = 0;
                    saltoLinea.clave = item;
                    saltoLinea.solicitado = "0";
                    saltoLinea.surtido = "0";

                    ReportInsumos rowSalto = new ReportInsumos();
                    rowSalto.rows.Add(saltoLinea);
                    listado.Add(rowSalto);


                    ReportInsumos insumo = new ReportInsumos();
                    string[] dts_cve = item.Split('-');
                    int tipo = Convert.ToInt32(dts_cve[0]);
                    int grupo = Convert.ToInt32(dts_cve[1]);
                    int clave = Convert.ToInt32(dts_cve[2]);
                    int presentacion = Convert.ToInt32(dts_cve[3]);

                    StringBuilder QueryString = new StringBuilder();
                    QueryString.Append("select ");
                    QueryString.Append(" pk_articulos as pk, ");
                    QueryString.Append(" mes, ");
                    QueryString.Append(" clave_t as clave, ");
                    QueryString.Append(" descripcion, ");
                    QueryString.Append("cast (sum(solicitado) as varchar) solicitado, ");
                    QueryString.Append("cast(sum(surtido) as varchar)surtido ");

                    QueryString.Append("from( ");
                    QueryString.Append("select ");
                    QueryString.Append("dp.pk_articulos, ");
                    QueryString.Append("MONTH(mp.fecha_pedido) mes, ");
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


                    QueryString.Append("and mp.fecha_pedido between '20191101' and '20200130' ");
                    QueryString.Append("and mp.centro_sol in (10051) ");
                    QueryString.Append(")f ");
                    QueryString.Append("group by pk_articulos, clave_t, descripcion, mes ");
                    QueryString.Append("order by descripcion ");

                    List<RowRepoInsumos> listadin = dbA.Database.SqlQuery<RowRepoInsumos>(QueryString.ToString()).ToList();
                    if (listadin.Count > 0)
                    {
                        int requerido = 0;
                        int surtido = 0;
                        insumo.rows.AddRange(listadin);

                        listado.Add(insumo);

                        foreach (var prom in listadin)
                        {
                            requerido = requerido + Convert.ToInt32(prom.solicitado);
                            surtido = surtido + Convert.ToInt32(prom.surtido);
                        }

                        RowRepoInsumos promedio_ = new RowRepoInsumos();
                        promedio_.pk = "";
                        promedio_.mes = 0;
                        promedio_.clave = "";
                        promedio_.descripcion = "Promedio: ";
                        double prom_sol = ((double)requerido / (double)12);
                        double prom_sur = ((double)surtido / (double)12);
                        promedio_.solicitado = String.Format("{0:0.##}", prom_sol); //prom_sol.ToString();
                        promedio_.surtido = String.Format("{0:0.##}", prom_sur); //prom_sur.ToString();

                        ReportInsumos rowPromedio = new ReportInsumos();
                        rowPromedio.rows.Add(promedio_);
                        listado.Add(rowPromedio);


                    }
                    else
                    {
                        RowRepoInsumos insumi = new RowRepoInsumos();
                        insumi.mes = 0;
                        insumi.clave = item;
                        insumi.solicitado = "0";
                        insumi.surtido = "0";

                        ma_articulos art = dbA.ma_articulos.SingleOrDefault(a => a.TIPO == tipo && a.GRUPO == grupo && a.CLAVE == clave && a.PRESENTACION == presentacion);

                        if (art != null)
                        {
                            insumi.pk = art.PK_ARTICULOS;
                            insumi.mes = 0;
                            insumi.clave = item;
                            insumi.descripcion = art.DESCRIPCION;
                            insumi.solicitado = "0";
                            insumi.surtido = "0";
                        }

                        insumo.rows.Add(insumi);

                        listado.Add(insumo);
                    }



                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }

            return listado;
        }

        public List<ReportInsumos> getGeneral()
        {
            List<ReportInsumos> lista = new List<ReportInsumos>();
            string[] unidades = { "90000,90001", "52,53", "29,30", "110000,111000", "80000", "100000", "120000", "10051", "180000,180001", "130000,131000" };

            foreach (var item in unidades)
            {
                RowRepoInsumos saltoLinea = new RowRepoInsumos();
                saltoLinea.pk = "***";
                saltoLinea.mes = 0;
                saltoLinea.clave = item;
                saltoLinea.solicitado = "***";
                saltoLinea.surtido = "***";

                ReportInsumos rowSaltoUnidad = new ReportInsumos();
                rowSaltoUnidad.rows.Add(saltoLinea);
                lista.Add(rowSaltoUnidad);

                ReportInsumos mdl = new ReportInsumos();
                mdl = this.getReporteChavoyaGeneral(item);
                lista.Add(mdl);
            }

            return lista;
        }

        public ReportInsumos getReporteChavoyaGeneral(string unidad_)
        {
            AlmacenEntity dbA = new AlmacenEntity();
            ReportInsumos modelo = new ReportInsumos();
            try
            {

                StringBuilder QueryString = new StringBuilder();
                QueryString.Append("select ");
                QueryString.Append(" pk_articulos as pk, ");
                QueryString.Append(" mes, ");
                QueryString.Append(" clave_t as clave, ");
                QueryString.Append(" descripcion, ");
                QueryString.Append("cast (sum(solicitado) as varchar) solicitado, ");
                QueryString.Append("cast(sum(surtido) as varchar)surtido ");

                QueryString.Append("from( ");
                QueryString.Append("select ");
                QueryString.Append("dp.pk_articulos, ");
                QueryString.Append("MONTH(mp.fecha_pedido) mes, ");
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

                //QueryString.Append("and dp.Tipo < 60 ");
                QueryString.Append("and dp.Tipo in (60, 130, 531, 537) ");


                QueryString.Append("and mp.fecha_pedido between '20190101' and '20191130' ");
                QueryString.Append("and mp.centro_sol in (" + unidad_ + ") ");
                QueryString.Append(")f ");
                QueryString.Append("group by pk_articulos, clave_t, descripcion, mes ");
                QueryString.Append("order by descripcion ");

                List<RowRepoInsumos> listadin = dbA.Database.SqlQuery<RowRepoInsumos>(QueryString.ToString()).ToList();

                string pk = "";
                int requerido = 0;
                int surtido = 0;

                foreach (var item in listadin)
                {
                    //SI ES OTRA CLAVE AGREGAR SALTO DE LINEA
                    if (pk != item.pk)
                    {
                        if (pk != "")
                        {
                            //SACAR PROMEDIOS Y AGREGAR RENGLON DE PROMEDIOS
                            RowRepoInsumos promedio_ = new RowRepoInsumos();
                            promedio_.pk = "";
                            promedio_.mes = 0;
                            promedio_.clave = "";
                            promedio_.descripcion = "Promedio: ";
                            double prom_sol = ((double)requerido / (double)12);
                            double prom_sur = ((double)surtido / (double)12);
                            promedio_.solicitado = String.Format("{0:0.##}", prom_sol); //prom_sol.ToString();
                            promedio_.surtido = String.Format("{0:0.##}", prom_sur); //prom_sur.ToString();
                            //ADD ROW
                            modelo.rows.Add(promedio_);
                            //INICIALIZAR CONTADORES SUMA
                            requerido = 0;
                            surtido = 0;

                        }

                        RowRepoInsumos saltoLinea = new RowRepoInsumos();
                        saltoLinea.pk = "---";
                        saltoLinea.mes = 0;
                        saltoLinea.clave = item.clave;
                        saltoLinea.solicitado = "";
                        saltoLinea.surtido = "";
                        modelo.rows.Add(saltoLinea);
                    }

                    pk = item.pk;

                    //IR SUMANDO SOLICITADO Y SURTIDO
                    requerido = requerido + Convert.ToInt32(item.solicitado);
                    surtido = surtido + Convert.ToInt32(item.surtido);

                    //AGREGAR LINEA DE CLAVE
                    modelo.rows.Add(item);

                }

            }
            catch (Exception e)
            {
                string msg = e.Message;
            }

            return modelo;
        }

        public CuadroGeneralModel getDistribucionMaterial()
        {
            string[] clavelines = { "'060.953.0282.12'"  ,
"'060.841.0619.12'"  ,
"'060.621.0038.99'"  ,
"'060.345.1865.11'"  ,
"'060.345.0503.11'"  ,
"'060.203.0298.11'"  ,
"'060.168.8211.11'"  ,
"'060.168.5456.11'"  ,
"'060.168.5431.11'"  ,
"'060.168.5389.12'"  ,
"'060.168.5388.99'"  ,
"'060.168.5387.12'"  ,
"'060.125.2760.13'"  ,
"'060.125.1879.11'"  ,
"'060.621.0482.11'"  ,
"'060.168.9904.11'"  ,
"'060.168.4418.11'"  ,
"'060.168.4277.11'"  ,
"'060.167.3346.07'"  ,
"'060.167.3320.12'"  ,
"'060.167.3312.12'"  ,
"'060.167.3304.12'"  ,
"'060.167.0680.04'"  ,
"'060.167.0482.04'"  ,
"'060.167.0466.05'"  ,
"'060.167.0458.06'"  ,
"'060.166.0251.03'"  ,
"'060.166.0244.03'"  ,
"'060.166.0236.03'"  ,
"'060.166.0228.03'"  ,
"'060.040.9007.07'"  ,
"'060.040.7605.11'"  ,
"'060.040.0543.04'"  ,
"'060.598.0010.11'"  ,
"'060.550.2186.01'"  ,
"'060.550.1279.01'"  ,
"'060.550.0453.11'"  ,
"'060.168.6660.12'"  ,
"'060.168.6645.13'"  ,
"'060.125.2869.00'"  ,
"'060.066.0658.12'"  ,
"'060.842.0394.04'"  ,
"'060.841.0569.12'"  };


            CuadroGeneralModel cuadroMdl = new CuadroGeneralModel();

            foreach (var item in clavelines)
            {
                StringBuilder QueryString = new StringBuilder();
                QueryString.Append(" select ");
                QueryString.Append("c.cve, c.descripcion, c.hru, c.hgt, c.hgm, c.hgi, c.hmi,    c.iec, c.ceh, c.j1, c.j2, c.j3, ISNULL (m.EXISTENCIA,0) existencia_almacen, ");
                QueryString.Append(" cast((case when m.PK_ARTICULOS is null then 0 else 1 end) as bit) existe_ma, ");
                QueryString.Append(" ISNULL(m.PK_ARTICULOS, 0) pk, ");
                QueryString.Append(" ISNULL(m.EXISTENCIA, 0) existencia_almacen ");
                QueryString.Append(" from dbo.cuadro_general_material_curacion c ");
                QueryString.Append(" left join ALMACEN_CENTRAL_NVO.dbo.ma_articulos m on ");
                QueryString.Append(" cast(substring(c.cve, 1, 3) as int) = m.TIPO ");
                QueryString.Append(" and cast(substring(c.cve,5,3) as int)= m.GRUPO ");
                QueryString.Append(" and cast(substring(c.cve,9,4) as int)= m.CLAVE ");
                QueryString.Append(" and cast(substring(c.cve,14,2) as int)= m.PRESENTACION ");
                QueryString.Append(" and m.ACTIVO = 1 ");
                QueryString.Append(" WHERE anio = 2019 and cve = " + item);
                QueryString.Append(" order by c.descripcion ");
                try
                {
                    List<RowsCuadroGeneralModel> listadin = dbPedido.Database.SqlQuery<RowsCuadroGeneralModel>(QueryString.ToString()).ToList();

                    cuadroMdl.rowsCuadro.AddRange(listadin);
                }
                catch (Exception e)
                {

                }
            }

            return cuadroMdl;

        }

        public List<InsumoClassSIAA> getReporteDescripcionesCVE()
        {
            AlmacenEntity dbA = new AlmacenEntity();
            List<InsumoClassSIAA> listado = new List<InsumoClassSIAA>();

            //string[] clavelines = { "010-000-5233-00","010-000-1541-00","010-000-2114-00","010-000-4140-00","010-000-2018-00","010-000-5356-00","010-000-0108-00","010-000-0476-00","010-000-0104-00","010-000-4578-00","010-000-4251-00","040-000-2613-00","010-000-1042-00","010-000-4299-00","010-000-5292-00","010-000-5640-00","010-000-4301-00","010-000-5244-00","010-000-4162-00","010-000-4271-00","010-000-2262-00","010-000-5302-00","010-000-0623-00","010-000-4512-02","010-000-4442-00","010-000-1546-00","010-000-0446-00","040-000-3259-00","010-000-4227-00","010-000-5653-00","010-000-1006-00","010-000-5487-00","010-000-5176-00","040-000-2654-00","040-000-3255-00","010-000-1924-00","010-000-0504-00","010-000-0611-00","010-000-5255-00","010-000-1972-00","010-000-2016-00","010-000-2128-00","010-000-2127-00","010-000-1929-00","010-000-4490-00","010-000-1206-00","010-000-5295-01","010-000-5284-00","010-000-5264-01","010-000-3610-00","010-000-4507-00","010-000-4505-00","010-000-0626-01","010-000-2135-00","010-000-1955-00","010-000-1954-00","010-000-3624-00","010-000-3606-00","010-000-1051-01","010-000-4118-00","010-000-4249-00","010-000-0267-00","010-000-0109-00","010-000-1309-00","010-000-4252-00","010-000-1542-00","010-000-4149-00","010-000-0473-00","010-000-5445-00",            "010-000-3616-00" };
            //string[] clavelines = { "010-000-3608-00", "010-000-3609-00", "010-000-3610-00", "010-000-3627-00", "010-000-3616-00", "010-000-3615-00", "010-000-3606-00", "010-000-3607-00" };

            string[] clavelines = {
                "010.000.0104.00",
"010.000.0106.00",
"010.000.2144.00",
"010.000.2145.00",
"010.000.3623.00",
"010.000.2463.00",
"010.000.0109.00",
"010.000.5187.00",
"010.000.5721.00",
"010.000.4254.00",
"010.000.1937.00",
"010.000.1956.00",
"010.000.5265.00",
"010.000.4251.00",
"010.000.5292.00",
"010.000.4061.00",
"010.000.0247.01",
"040.000.4057.00",
"040.000.0242.00",
"010.000.0612.00",
"010.000.0611.00",
"010.000.0246.00",
"010.000.0254.00",
"010.000.4059.00",
"010.000.0615.00",
"010.000.0614.00",
"010.000.4154.00",
"010.000.0204.00",
"010.000.3629.00",
"010.000.3620.00",
"010.000.3619.00",
"010.000.0524.00",
"010.000.2154.00",
"010.000.4224.00",
"010.000.0621.00",
"010.000.2308.00",
"010.000.0474.00",
"010.000.0473.00",
"010.000.4241.00",
"010.000.2162.00",
"010.000.0439.00",
"010.000.1051.01",
"010.000.1050.01",
"010.000.3610.00",
"010.000.3608.00",
"010.000.3601.00",
"010.000.3601.00",
"010.000.3625.00",
"010.000.0473.00"            

            };
            try
            {
                foreach (var item in clavelines)
                {
                    
                    ReportInsumos insumo = new ReportInsumos();
                    string[] dts_cve = item.Split('.');
                    int tipo = Convert.ToInt32(dts_cve[0]);
                    int grupo = Convert.ToInt32(dts_cve[1]);
                    int clave = Convert.ToInt32(dts_cve[2]);
                    int presentacion = Convert.ToInt32(dts_cve[3]);

                    List<InsumoClassSIAA> listaPrev = new List<InsumoClassSIAA>();

                    StringBuilder QueryString = new StringBuilder();
                    QueryString.Append(" select ma.pk_articulos pk, ");
                    QueryString.Append(" ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.tipo, 0) as varchar(3))))) + cast(isnull(ma.tipo, 0) as varchar(3)))) + '.' + ");
                    QueryString.Append(" ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.grupo, 0) as varchar(3))))) + cast(isnull(ma.grupo, 0) as varchar(3)))) + '.' + ");
                    QueryString.Append(" ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(isnull(ma.clave, 0) as varchar(4))))) + cast(isnull(ma.clave, 0) as varchar(4)))) + '.' + ");
                    QueryString.Append(" ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(isnull(ma.presentacion, 0) as varchar(4))))) + cast(isnull(ma.presentacion, 0) as varchar(4)))) clave_txt,");
                    QueryString.Append(" ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.tipo, 0) as varchar(3))))) + cast(isnull(ma.tipo, 0) as varchar(3)))) + '.' + ");
                    QueryString.Append(" ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.grupo, 0) as varchar(3))))) + cast(isnull(ma.grupo, 0) as varchar(3)))) + '.' + ");
                    QueryString.Append(" ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(isnull(ma.clave, 0) as varchar(4))))) + cast(isnull(ma.clave, 0) as varchar(4))))  clave_ ");
                    QueryString.Append(" , convert(int, ma.tipo) tipo, convert(int, ma.grupo) grupo, convert(int, ma.clave) clave, convert(int, ma.presentacion) presentacion ");
                    QueryString.Append(" , ma.descripcion ");
                    QueryString.Append(" , m.desc_larga descLarga ");
                    QueryString.Append(" , (up.descrip + ' ' + cast(unidades as varchar) + ' ' + um.descrip) presentacion_txt ");
                    QueryString.Append(" from ma_articulos ma  ");
                    QueryString.Append(" left join ma_articulos_desc_larga_tabla_corta m on ma.pk_articulos = m.pk_articulos ");
                    QueryString.Append(" left join ca_unidadesp up on ma.uni_pres = up.unidad ");
                    QueryString.Append(" left join ca_unidadesm um on ma.uni_med = um.unidad ");
                    QueryString.Append(" where ma.tipo = "+tipo+" and grupo = "+grupo+" and clave = "+clave+" and ma.activo = 1 ");

                     listaPrev = dbA.Database.SqlQuery<InsumoClassSIAA>(QueryString.ToString()).ToList();

                    if (listaPrev.Count > 0)
                    {
                        
                        listado.AddRange(listaPrev);


                    }
                    else
                    {
                        
                    }



                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }

            return listado;
        }
    }

    public class ReportInsumos {
        public ReportInsumos()
        {
            rows = new List<RowRepoInsumos>();
        }
        public List<RowRepoInsumos> rows { get; set; }
    }
    public class RowRepoInsumos {
        public string pk { get; set; }
        public int mes { get; set; }
        public string mes_nombre { get; set; }
        public int anio { get; set; }
        public string clave { get; set; }
        public string descripcion { get; set; }
        public string solicitado { get; set; }
        public string surtido { get; set; }
    }
}

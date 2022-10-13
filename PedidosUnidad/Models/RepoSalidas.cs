using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PedidosUnidad.Models.DBConcentradora;
using PedidosUnidad.Models.DBAlmacen;
using PedidosUnidad.Security;
using System.Text;

namespace PedidosUnidad.Models
{
    public class RepoSalidas
    {

        DBCONCENTRADORA dbConcentradora = null;

        public RepoSalidas()
        {
            dbConcentradora = new DBCONCENTRADORA();
        }

        public SalidasClassSIAA getSalidas(int id_centro)
        {
            SalidasClassSIAA mdlSalidas = new SalidasClassSIAA();

            try
            {
                List<vwListadoSalidas> Salidas = new List<vwListadoSalidas>();
                Salidas = (from r in dbConcentradora.vwListadoSalidas where r.id_centro == id_centro select r).ToList();
                mdlSalidas.listadoSalidas = Salidas;
                mdlSalidas.exito = true;
                mdlSalidas.msg = "Éxito";
            }
            catch (Exception e)
            {
                mdlSalidas.listadoSalidas = new List<vwListadoSalidas>();
                mdlSalidas.exito = false;
                mdlSalidas.msg = "Error al obtener el listado de las entradas de la unidad";
            }


                return mdlSalidas;
            }

        public ReturnModelClass saveSalida(ref MaDeSalidasClassSIAA pMaDeSalidasClassSIAA, CurrentUser user)
        {
            ReturnModelClass mdl = new ReturnModelClass();
            //creamos nuestro contexto
            using (var db_c = new DBCONCENTRADORA())
            {
                //creamos el ámbito de la transacción
                using (var dbContextTransaction = db_c.Database.BeginTransaction())
                {
                    try
                    {
                        //DATOS DE CABECERO
                        pMaDeSalidasClassSIAA.maSalidas.PEDIDO = getNewId(user);
                        pMaDeSalidasClassSIAA.maSalidas.ANIO = Convert.ToInt16(DateTime.Now.Year);
                        pMaDeSalidasClassSIAA.maSalidas.id_centro = user.id_unidad;
                        pMaDeSalidasClassSIAA.maSalidas.FECHA_PEDIDO = DateTime.Now;

                        pMaDeSalidasClassSIAA.maSalidas.Afectado = false;
                        pMaDeSalidasClassSIAA.maSalidas.U_ACT = Convert.ToInt16(user.id_user);
                        pMaDeSalidasClassSIAA.maSalidas.F_ACT = DateTime.Now;
                        pMaDeSalidasClassSIAA.maSalidas.U_CREO = Convert.ToInt16(user.id_user);
                        pMaDeSalidasClassSIAA.maSalidas.F_CREO = DateTime.Now;
                        pMaDeSalidasClassSIAA.maSalidas.CANCELADO = false;

                        ma_pedidos mdlMaDeSalidasClassSIAA = new ma_pedidos();
                        mdlMaDeSalidasClassSIAA = pMaDeSalidasClassSIAA.maSalidas;

                        mdlMaDeSalidasClassSIAA.TIPO_INSUMOS = pMaDeSalidasClassSIAA.maSalidas.TIPO_INSUMOS;

                        //GUARDAR CABECERO AGREGADO
                        db_c.ma_pedidos.Add(mdlMaDeSalidasClassSIAA);
                        db_c.SaveChanges();

                        //DATOS DETALLE ENTRADA (PARTIDAS)
                        foreach (var item in pMaDeSalidasClassSIAA.deSalidas)
                        {

                            item.desalidas.PEDIDO = mdlMaDeSalidasClassSIAA.PEDIDO;
                            item.desalidas.ANIO = Convert.ToInt16(DateTime.Now.Year);
                            item.desalidas.U_ACT = Convert.ToInt16(user.id_user);
                            item.desalidas.F_ACT = DateTime.Now;
                            item.desalidas.U_CREO = Convert.ToInt16(user.id_user);
                            item.desalidas.F_CREO = DateTime.Now;
                            item.desalidas.afectado = false;

                            de_pedidos det = new de_pedidos();
                            det = item.desalidas;
                            //GUARDADO DE DETALLE
                            db_c.de_pedidos.Add(det);
                            db_c.SaveChanges();


                            foreach (var cad in item.desalidasCad)
                            {
                                //DATOS DE CADUCIDADES
                                cad.Pedido = mdlMaDeSalidasClassSIAA.PEDIDO;
                                cad.Anio = Convert.ToInt16(DateTime.Now.Year);
                                cad.U_ACT = Convert.ToInt16(user.id_user);
                                cad.F_ACT = DateTime.Now;
                                cad.afectado = false;

                                cad.id_centro = item.desalidas.id_centro;
                                cad.Tipo = item.desalidas.TIPO;
                                cad.Grupo = item.desalidas.GRUPO;
                                cad.Clav_Med = item.desalidas.CLAVE;
                                cad.pk_CADUCIDADES = null;

                                DE_PedidosCAD mdlcad = new DE_PedidosCAD();
                                mdlcad = cad;
                                //GUARDADO DE CADUCIDADES
                                db_c.DE_PedidosCAD.Add(mdlcad);
                                db_c.SaveChanges();
                            }
                        }


                        mdl.exito = true;
                        mdl.msg = "Pedido generado correctamente";
                        mdl.var_1 = pMaDeSalidasClassSIAA.maSalidas.PEDIDO.ToString("000");

                        //Hacemos commit de todos los datos
                        dbContextTransaction.Commit();

                    }
                    catch (Exception e)
                    {
                        //AQUI TENDRIA QUE IR EL ROOL BACK POR ESO SE QUITARON LOS OTROS TRY
                        mdl.exito = false;
                        mdl.msg = "Ocurrio un error: " + e.Message;
                        mdl.var_1 = "000";

                        //hacemos rollback si hay excepción
                        dbContextTransaction.Rollback();

                    }

                }

            }

            return mdl;
        }


        public ReturnModelClass editSalida(ref MaDeSalidasClassSIAA pMaDeSalidasClassSIAA, CurrentUser user)
        {
            ReturnModelClass mdl_return = new ReturnModelClass();

            //creamos nuestro contexto
            using (var db_c = new DBCONCENTRADORA())
            {
                int folio = pMaDeSalidasClassSIAA.maSalidas.PEDIDO;
                int anio = pMaDeSalidasClassSIAA.maSalidas.ANIO;
                int centro = pMaDeSalidasClassSIAA.maSalidas.id_centro;
                //creamos el ámbito de la transacción
                using (var dbContextTransaction = db_c.Database.BeginTransaction())
                {

                    try
                    {

                        //GET CABECERO
                        ma_pedidos salida = db_c.ma_pedidos.SingleOrDefault(a => a.PEDIDO == folio && a.ANIO == anio && a.id_centro == centro);


                        salida.U_ACT = Convert.ToInt16(user.id_user);
                        salida.F_ACT = DateTime.Now;

                        salida.TIPO_PEDIDO = pMaDeSalidasClassSIAA.maSalidas.TIPO_PEDIDO;
                        salida.OBSERVA = pMaDeSalidasClassSIAA.maSalidas.OBSERVA;
                        salida.CENTRO_SOL = pMaDeSalidasClassSIAA.maSalidas.CENTRO_SOL;
                        salida.no_prog = pMaDeSalidasClassSIAA.maSalidas.no_prog;
                        db_c.SaveChanges();

                        //CLEAN DETALLE CAD
                        List<DE_PedidosCAD> detalle_salida_cad = (from r in db_c.DE_PedidosCAD where r.Pedido == folio && r.id_centro == centro && r.Anio == anio select r).ToList();
                        db_c.DE_PedidosCAD.RemoveRange(detalle_salida_cad);
                        db_c.SaveChanges();

                        //CLEAN DETALLE
                        List<de_pedidos> detalle_salida = (from r in db_c.de_pedidos where r.PEDIDO == folio && r.id_centro == centro && r.ANIO == anio select r).ToList();
                        db_c.de_pedidos.RemoveRange(detalle_salida);
                        db_c.SaveChanges();


                        int id_ = 1;
                        foreach (var item in pMaDeSalidasClassSIAA.deSalidas)
                        {
                            de_pedidos det = new de_pedidos();
                            det.PEDIDO = salida.PEDIDO;
                            det.ANIO = salida.ANIO;
                            det.pk_articulos = item.desalidas.pk_articulos;
                            det.TIPO = item.desalidas.TIPO;
                            det.GRUPO = item.desalidas.GRUPO;
                            det.CLAVE = item.desalidas.CLAVE;
                            det.presentacion = item.desalidas.presentacion;
                            det.CANTIDAD = item.desalidas.CANTIDAD;
                            det.COSTO = item.desalidas.COSTO;
                            det.U_ACT = Convert.ToInt16(user.id_user);
                            det.F_ACT = DateTime.Now;
                            det.U_CREO = item.desalidas.U_CREO;
                            det.F_CREO = item.desalidas.F_CREO;
                            det.afectado = false;
                            det.id_centro = salida.id_centro;

                            db_c.de_pedidos.Add(det);
                            db_c.SaveChanges();
                            id_++;

                            foreach (var cad in item.desalidasCad)
                            {
                                //DATOS DE CADUCIDADES
                                cad.Pedido = salida.PEDIDO;
                                cad.Anio = Convert.ToInt16(salida.ANIO);
                                cad.U_ACT = Convert.ToInt16(user.id_user);
                                cad.F_ACT = DateTime.Now;
                                cad.afectado = false;

                                cad.id_centro = item.desalidas.id_centro;
                                cad.Tipo = item.desalidas.TIPO;
                                cad.Grupo = item.desalidas.GRUPO;
                                cad.Clav_Med = item.desalidas.CLAVE;
                                cad.pk_CADUCIDADES = null;

                                DE_PedidosCAD mdlcad = new DE_PedidosCAD();
                                mdlcad = cad;
                                //GUARDADO DE CADUCIDADES
                                db_c.DE_PedidosCAD.Add(mdlcad);
                                db_c.SaveChanges();
                            }
                        }

                        mdl_return.exito = true;
                        mdl_return.msg = "Pedido editado correctamente";
                        mdl_return.var_1 = salida.PEDIDO.ToString("000");
                        //Hacemos commit de todos los datos
                        dbContextTransaction.Commit();


                    }
                    catch (Exception e)
                    {
                        //AQUI TENDRIA QUE IR EL ROOL BACK POR ESO SE QUITARON LOS OTROS TRY
                        mdl_return.exito = false;
                        mdl_return.msg = "Ocurrio un error: " + e.Message;
                        mdl_return.var_1 = "000";
                        //hacemos rollback si hay excepción
                        dbContextTransaction.Rollback();
                    }
                }
            }

            return mdl_return;
        }

        public ReturnModelClass addInsumo(ref List<DeSalidasClassSIAA> mdl, DeSalidasClassSIAA insumo)
        {
            int[] arrayTipo = { 0, 10, 30, 40, 41 };
            ReturnModelClass returnModel = new ReturnModelClass();
            try
            {
                List<DeSalidasClassSIAA> existe = (from r in mdl where r.desalidas.pk_articulos == insumo.desalidas.pk_articulos select r).ToList();
                TA_Parametros mdlParametros = dbConcentradora.TA_Parametros.SingleOrDefault(a => a.id_centro == insumo.desalidas.id_centro);

                if (existe.Count > 0)
                {
                    DeSalidasClassSIAA rowExiste = mdl.SingleOrDefault(a => a.desalidas.pk_articulos == existe[0].desalidas.pk_articulos);
                    insumo.desalidas.CANTIDAD = rowExiste.desalidas.CANTIDAD + insumo.desalidas.CANTIDAD;

                    //if (arrayTipo.Contains(insumo.desalidas.TIPO))
                    //{
                    //    insumo.desalidas.PIVA = 0;
                    //    insumo.desalidas.IVA = 0;
                    //}
                    //else
                    //{

                    //    insumo.desalidas.PIVA = mdlParametros.PIVA;
                    //    insumo.desalidas.IVA = (insumo.desalidas.CANTIDAD * insumo.desalidas.COSTO) * (insumo.desalidas.PIVA / 100);
                    //}

                    mdl.Remove(rowExiste);
                    mdl.Add(insumo);

                    returnModel.exito = true;
                    returnModel.msg = "Clave Agregada Correctamente";
                }
                else
                {
                    mdl.Add(insumo);
                    returnModel.exito = true;
                    returnModel.msg = "Clave Agregada Correctamente";
                }
            }
            catch (Exception e)
            {
                returnModel.exito = false;
                returnModel.msg = "Ocurrio un error al momento de agregar";
            }

            return returnModel;
        }

        public ReturnModelClass deleteInsumo(ref List<DeSalidasClassSIAA> mdl, DeSalidasClassSIAA insumo)
        {
            ReturnModelClass rmodel = new ReturnModelClass();
            try
            {
                List<DeSalidasClassSIAA> existe = (from r in mdl where r.desalidas.pk_articulos== insumo.desalidas.pk_articulos select r).ToList();

                if (existe.Count > 0)
                {
                    DeSalidasClassSIAA rowExiste = mdl.SingleOrDefault(a => a.desalidas.pk_articulos == existe[0].desalidas.pk_articulos);
                    mdl.Remove(rowExiste);
                    rmodel.exito = true;
                    rmodel.msg = "Clave eliminada correctamente";
                }
            }
            catch (Exception e)
            {
                rmodel.exito = false;
                rmodel.msg = "ocurrio un error al momento de eliminar";
            }

            return rmodel;
        }

        public DeSalidasClassSIAA addCaducidad(DeSalidasClassSIAA insumo, DE_PedidosCAD caducidad)
        {
            try
            {
                List<DE_PedidosCAD> existe = (from r in insumo.desalidasCad where r.pk_articulos == caducidad.pk_articulos && r.pk_CADUCIDADES == caducidad.pk_CADUCIDADES select r).ToList();

                if (existe.Count > 0)
                {
                    DE_PedidosCAD rowExiste = insumo.desalidasCad.SingleOrDefault(a => a.pk_articulos == existe[0].pk_articulos && a.pk_CADUCIDADES == existe[0].pk_CADUCIDADES);
                    caducidad.Cantidad = rowExiste.Cantidad + caducidad.Cantidad;
                    insumo.desalidasCad.Remove(rowExiste);
                    insumo.desalidasCad.Add(caducidad);
                }
                else
                {
                    insumo.desalidasCad.Add(caducidad);
                }
            }
            catch (Exception e)
            {

            }

            return insumo;
        }

        public DeSalidasClassSIAA deleteCaducidad(DeSalidasClassSIAA insumo, DE_PedidosCAD caducidad)
        {
            try
            {
                List<DE_PedidosCAD> existe = (from r in insumo.desalidasCad where r.pk_articulos == caducidad.pk_articulos && r.pk_CADUCIDADES == caducidad.pk_CADUCIDADES select r).ToList();

                if (existe.Count > 0)
                {
                    DE_PedidosCAD rowExiste = insumo.desalidasCad.SingleOrDefault(a => a.pk_articulos == existe[0].pk_articulos && a.pk_CADUCIDADES == existe[0].pk_CADUCIDADES);
                    caducidad.Cantidad = rowExiste.Cantidad + caducidad.Cantidad;
                    insumo.desalidasCad.Remove(rowExiste);
                }
            }
            catch (Exception e)
            {

            }

            return insumo;
        }

        public MaDeSalidasClassSIAA getSalida(int folio, int anio, CurrentUser user)
        {
            AlmacenEntity dbA = new AlmacenEntity();
            MaDeSalidasClassSIAA FullEntrada = new MaDeSalidasClassSIAA();
            try
            {

                //GET CABECERO

                FullEntrada.maSalidas = dbConcentradora.ma_pedidos.SingleOrDefault(a => a.PEDIDO == folio && a.id_centro == user.id_unidad && a.ANIO == anio);
                List<de_pedidos> detalle = (from r in dbConcentradora.de_pedidos where r.PEDIDO == folio && r.id_centro == user.id_unidad && r.ANIO == anio select r).ToList();

                foreach (var insumo in detalle)
                {
                    try
                    {
                        DeSalidasClassSIAA de = new DeSalidasClassSIAA();
                        de.desalidas = insumo;
                        de.descLarga = (from r in dbA.ma_articulos_desc_larga_tabla_corta where r.pk_articulos == insumo.pk_articulos select r.desc_larga).SingleOrDefault();
                        de.insumo = getNomInsumo(insumo.pk_articulos);
                        de.pres = getPresentacion(insumo.pk_articulos);
                        List<DE_PedidosCAD> caducidad = (from r in dbConcentradora.DE_PedidosCAD where r.Pedido == folio && r.id_centro == user.id_unidad && r.Anio == anio select r).ToList();

                        foreach (var cad in caducidad)
                        {
                            try
                            {
                                DE_PedidosCAD decad = new DE_PedidosCAD();
                                decad = cad;

                                de.desalidasCad.Add(decad);
                            }
                            catch (Exception e)
                            {

                            }
                        }

                        FullEntrada.deSalidas.Add(de);
                    }
                    catch (Exception e)
                    {

                    }
                }

            }
            catch (Exception e)
            {

            }
            return FullEntrada;
        }

        #region Utilierias

        public SelectList getTipoSalida()
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            List<CA_TIPOSENT> TipoSalida = (dbAlmacen.CA_TIPOSENT).ToList();

            return new SelectList(TipoSalida, "TIPO", "DESCRIP");
        }

        public SelectList getUnidadSolicito()
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            List<CA_CENTROS> Centros = (dbAlmacen.CA_CENTROS).ToList();

            return new SelectList(Centros, "CENTRO", "DESCRIP");
        }


        public int getNewId(CurrentUser user)
        {

            int id = 0;
            try
            {

                string query = "exec dbo.Genera_Folio 'P'," + user.id_user + "," + user.id_unidad;
                List<int> row_ = dbConcentradora.Database.SqlQuery<int>(query.ToString()).ToList();
                id = row_.First();


                //TA_Parametros parametro;
                //parametro = (from r in dbConcentradora.TA_Parametros where r.id_centro == user.id_unidad select r).First();
                //id = parametro.F_PEDIDOS ?? 0;
                //id = id + 1;
            }
            catch (Exception e)
            {
                id = -1;
            }
            return id;

        }

        //LIMPIAR DETALLE DE PEDIDO 
        public bool initDetalleSalida(int id, int id_unidad, int anio)
        {
            bool exito = false;
            try
            {
                List<de_pedidos> detalle_salida = (from r in dbConcentradora.de_pedidos where r.PEDIDO == id && r.id_centro == id_unidad && r.ANIO == anio select r).ToList();
                dbConcentradora.de_pedidos.RemoveRange(detalle_salida);
                dbConcentradora.SaveChanges();
                exito = true;
            }
            catch (Exception e)
            {
                exito = false;
            }
            return exito;
        }

        public string getPresentacion(string pk)
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            ma_articulos rowInsumo = (from r in dbAlmacen.ma_articulos where r.PK_ARTICULOS == pk select r).First();

            CA_UNIDADESP rowuniPres = (from r in dbAlmacen.CA_UNIDADESP where r.Unidad == rowInsumo.UNI_PRES select r).First();
            CA_UNIDADESM rowuniMed = (from r in dbAlmacen.CA_UNIDADESM where r.Unidad == rowInsumo.UNI_MED select r).First();

            return rowuniPres.Descrip + " " + rowInsumo.UNIDADES + " " + rowuniMed.Descrip;
        }

        public string getNomInsumo(string pk)
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            ma_articulos rowInsumo = (from r in dbAlmacen.ma_articulos where r.PK_ARTICULOS == pk select r).First();

            return rowInsumo.DESCRIPCION;
        }

        public List<InsumoClassSIAA> getLstInsumos(int opc)
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();
            StringBuilder QueryString = new StringBuilder();

            QueryString.Append("select ma.pk_articulos pk, ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.tipo, 0) as varchar(3))))) + cast(isnull(ma.tipo, 0) as varchar(3)))) +'-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.grupo, 0) as varchar(3))))) + cast(isnull(ma.grupo, 0) as varchar(3)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(isnull(ma.clave, 0) as varchar(4))))) + cast(isnull(ma.clave, 0) as varchar(4)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(isnull(ma.presentacion, 0) as varchar(4))))) + cast(isnull(ma.presentacion, 0) as varchar(4)))) clave_txt ");
            QueryString.Append(", convert(int, ma.tipo) tipo, convert(int, ma.grupo) grupo, convert(int, ma.clave) clave, convert(int, ma.presentacion) presentacion ");
            QueryString.Append(", ma.descripcion ");
            QueryString.Append(", m.desc_larga descLarga ");
            QueryString.Append(", (up.descrip + ' ' + cast(unidades as varchar) + ' ' + um.descrip) presentacion_txt ");
            QueryString.Append("from ma_articulos ma ");
            QueryString.Append("left join ma_articulos_desc_larga_tabla_corta m on ma.pk_articulos = m.pk_articulos ");
            QueryString.Append("left join ca_unidadesp up on ma.uni_pres = up.unidad ");
            QueryString.Append("left join ca_unidadesm um on ma.uni_med = um.unidad ");

            switch (opc)
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

            QueryString.Append("order by ma.tipo ");

            List<InsumoClassSIAA> lista = new List<InsumoClassSIAA>();

            try
            {
                lista = dbAlmacen.Database.SqlQuery<InsumoClassSIAA>(QueryString.ToString()).ToList();
            }
            catch (Exception e)
            {
            }

            return lista;
        }


        public InsumoClassSIAA getInsumo(string pk)
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();
            StringBuilder QueryString = new StringBuilder();

            QueryString.Append("select ma.pk_articulos pk, ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.tipo, 0) as varchar(3))))) + cast(isnull(ma.tipo, 0) as varchar(3)))) +'-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (3 - LEN(cast(isnull(ma.grupo, 0) as varchar(3))))) + cast(isnull(ma.grupo, 0) as varchar(3)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (4 - LEN(cast(isnull(ma.clave, 0) as varchar(4))))) + cast(isnull(ma.clave, 0) as varchar(4)))) + '-' + ");
            QueryString.Append("ltrim(rtrim(REPLICATE('0', (2 - LEN(cast(isnull(ma.presentacion, 0) as varchar(4))))) + cast(isnull(ma.presentacion, 0) as varchar(4)))) clave_txt ");
            QueryString.Append(", convert(int, ma.tipo) tipo, convert(int, ma.grupo) grupo, convert(int, ma.clave) clave, convert(int, ma.presentacion) presentacion ");
            QueryString.Append(", ma.descripcion ");
            QueryString.Append(", m.desc_larga descLarga ");
            QueryString.Append(", (up.descrip + ' ' + cast(unidades as varchar) + ' ' + um.descrip) presentacion_txt ");
            QueryString.Append("from ma_articulos ma ");
            QueryString.Append("left join ma_articulos_desc_larga_tabla_corta m on ma.pk_articulos = m.pk_articulos ");
            QueryString.Append("left join ca_unidadesp up on ma.uni_pres = up.unidad ");
            QueryString.Append("left join ca_unidadesm um on ma.uni_med = um.unidad ");
            QueryString.Append("where ma.pk_articulos='" + pk + "'");


            InsumoClassSIAA insumo = new InsumoClassSIAA();

            try
            {
                insumo = dbAlmacen.Database.SqlQuery<InsumoClassSIAA>(QueryString.ToString()).First();
            }
            catch (Exception e)
            {
            }

            return insumo;
        }

        public List<vwDispCad> getCadDisp(string pk, int unidad)
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();
            List<vwDispCad> lista = new List<vwDispCad>();

            try
            {
                //lista = (from r in dbConcentradora.vwDispCad where r.pk_articulos == pk select r).ToList();                
                lista = dbConcentradora.Database.SqlQuery<vwDispCad>("exec dbo.get_disponibilidad_lotes "+ unidad+"," + pk).ToList();

            }
            catch (Exception e)
            {
            }

            return lista;

        }

        public ReturnModelClass apartaLote(string Proceso, string pk, string pk_cad, int cantR, int Id_Centro, int Usr)
        {
            ReturnModelClass mdl = new ReturnModelClass();
            try
            {
                string query = "exec dbo.Asigna_Elimina_Caducidades_Manual '" + Proceso + "','" + pk + "','" + pk_cad + "',"+cantR+"," + Id_Centro + "," + Usr;
                List<int> row_ = dbConcentradora.Database.SqlQuery<int>(query.ToString()).ToList();
                int exito = row_.First();

                if (exito == 1)
                {
                    mdl.exito = true;
                    mdl.msg = "Caducidad apartada correctamente.";
                }
                else
                {
                    mdl.exito = false;
                    mdl.msg = "Insuficiente cantidad disponible de la caducidad.";
                }

            }
            catch (Exception e)
            {
                mdl.exito = false;
                mdl.msg = "Ocurrio un error. Favor de intentarlo de nuevo, de otra forma reportar al departamento de sistemas.";
            }
            return mdl;
        }


        public string getTipoInsumo(int tipo)
        {
            string tipo_insumo = "";
            switch (tipo)
            {
                case 1:
                    tipo_insumo = "Medicamento";  //QueryString.Append("where ma.tipo in(10,20,30,40,41) and ma.activo=1 ");
                    break;
                case 2:
                    tipo_insumo = "Material Curación"; //QueryString.Append("where ma.tipo in(60,130,531,537) and ma.activo=1 ");
                    break;
                case 3:
                    tipo_insumo = "Material Laboratorio"; //QueryString.Append("where ma.tipo in(50,80,533,535,553) and ma.activo=1 ");
                    break;
                case 4:
                    tipo_insumo = "Radio Grafico"; //QueryString.Append("where ma.tipo in(70) and ma.activo=1 ");
                    break;
                case 5:
                    tipo_insumo = "Roperia"; //QueryString.Append("where ma.tipo in(120) and ma.activo=1 ");
                    break;
                case 6:
                    tipo_insumo = "Limpieza"; //QueryString.Append("where ma.tipo in(100) and ma.activo=1 ");
                    break;
                case 7:
                    tipo_insumo = "Oficina"; //QueryString.Append("where ma.tipo in(107) and ma.activo=1 ");
                    break;
                case 8:
                    tipo_insumo = "Diversos"; //QueryString.Append("where ma.tipo not in(10,20,30,40,41,50,60,80,70,120,100,107,130,531,537,533,535,553) and ma.activo=1 ");
                    break;
                default:
                    tipo_insumo = "NA"; //QueryString.Append("where ma.tipo in(10,20,30,40,41) and ma.activo=1 ");
                    break;
            }
            return tipo_insumo;
        }
        #endregion


    }
}
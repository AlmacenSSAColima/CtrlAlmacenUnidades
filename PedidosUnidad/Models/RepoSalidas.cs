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

        //public MaDeEntradasClassSIAA saveEntrada(MaDeEntradasClassSIAA pMaDeEntradasClassSIAA, CurrentUser user)
        //{
        //    try
        //    {



        //        pMaDeEntradasClassSIAA.maEntradas.FOLIO = getNewId(user);
        //        pMaDeEntradasClassSIAA.maEntradas.ANIO = Convert.ToInt16(DateTime.Now.Year);
        //        pMaDeEntradasClassSIAA.maEntradas.NO_ENTRADA = null;

        //        string[] claveproveedor = pMaDeEntradasClassSIAA.cvproveedor.Split('|');

        //        pMaDeEntradasClassSIAA.maEntradas.ESTADOP = Convert.ToInt16(claveproveedor[0].ToString());
        //        pMaDeEntradasClassSIAA.maEntradas.GIROP = Convert.ToInt16(claveproveedor[1].ToString());
        //        pMaDeEntradasClassSIAA.maEntradas.CLAVEP = Convert.ToInt16(claveproveedor[2].ToString());

        //        pMaDeEntradasClassSIAA.maEntradas.AFECTADA = false;
        //        pMaDeEntradasClassSIAA.maEntradas.CERRADO = 0;
        //        pMaDeEntradasClassSIAA.maEntradas.U_ACT = Convert.ToInt16(user.id_user);
        //        pMaDeEntradasClassSIAA.maEntradas.F_ACT = DateTime.Now;
        //        pMaDeEntradasClassSIAA.maEntradas.U_CREO = Convert.ToInt16(user.id_user);
        //        pMaDeEntradasClassSIAA.maEntradas.F_CREO = DateTime.Now;
        //        pMaDeEntradasClassSIAA.maEntradas.cancelado = false;

        //        ma_entradas mdlMaDeEntradasClassSIAA = new ma_entradas();
        //        mdlMaDeEntradasClassSIAA = pMaDeEntradasClassSIAA.maEntradas;

        //        dbConcentradora.ma_entradas.Add(mdlMaDeEntradasClassSIAA);
        //        dbConcentradora.SaveChanges();

        //        foreach (var item in pMaDeEntradasClassSIAA.deEntradas)
        //        {
        //            try
        //            {



        //                item.deentradas.FOLIO = mdlMaDeEntradasClassSIAA.FOLIO;
        //                item.deentradas.ANIO = Convert.ToInt16(DateTime.Now.Year);
        //                item.deentradas.U_ACT = Convert.ToInt16(user.id_user);
        //                item.deentradas.F_ACT = DateTime.Now;
        //                item.deentradas.U_CREO = Convert.ToInt16(user.id_user);
        //                item.deentradas.F_CREO = DateTime.Now;
        //                item.deentradas.afectado = false;

        //                de_entradas det = new de_entradas();
        //                det = item.deentradas;
        //                dbConcentradora.de_entradas.Add(det);
        //                dbConcentradora.SaveChanges();


        //                foreach (var cad in item.deEntradasCad)
        //                {
        //                    try
        //                    {


        //                        cad.No_Entrada = mdlMaDeEntradasClassSIAA.FOLIO;
        //                        cad.Anio = Convert.ToInt16(DateTime.Now.Year);
        //                        cad.U_ACT = Convert.ToInt16(user.id_user);
        //                        cad.F_ACT = DateTime.Now;
        //                        cad.afectado = false;

        //                        DE_EntradasCAD mdlcad = new DE_EntradasCAD();
        //                        mdlcad = cad;
        //                        dbConcentradora.DE_EntradasCAD.Add(mdlcad);
        //                        dbConcentradora.SaveChanges();
        //                    }
        //                    catch
        //                    {

        //                    }

        //                }



        //            }
        //            catch
        //            {

        //            }

        //        }
        //    }
        //    catch
        //    {

        //    }

        //    return pMaDeEntradasClassSIAA;
        //}


        //public MaDeEntradasClassSIAA editEntrada(MaDeEntradasClassSIAA mdl, CurrentUser user)
        //{
        //    try
        //    {

        //        //GET CABECERO
        //        ma_entradas entrada = dbConcentradora.ma_entradas.SingleOrDefault(a => a.FOLIO == mdl.maEntradas.FOLIO && a.id_centro == mdl.maEntradas.id_centro);


        //        string[] claveproveedor = mdl.cvproveedor.Split('|');

        //        entrada.ESTADOP = Convert.ToInt16(claveproveedor[0].ToString());
        //        entrada.GIROP = Convert.ToInt16(claveproveedor[1].ToString());
        //        entrada.CLAVEP = Convert.ToInt16(claveproveedor[2].ToString());

        //        entrada.U_ACT = Convert.ToInt16(user.id_user);
        //        entrada.F_ACT = DateTime.Now;

        //        dbConcentradora.SaveChanges();

        //        //CLEAN DETALLE
        //        bool Rexito = this.initDetalleEntrada(mdl.maEntradas.FOLIO, mdl.maEntradas.id_centro, mdl.maEntradas.ANIO);
        //        if (Rexito)
        //        {
        //            int id_ = 1;
        //            foreach (var item in mdl.deEntradas)
        //            {
        //                try
        //                {
        //                    de_entradas det = new de_entradas();
        //                    det.FOLIO = entrada.FOLIO;
        //                    det.ANIO = entrada.ANIO;
        //                    det.PK_ARTICULOS = item.deentradas.PK_ARTICULOS;
        //                    det.TIPO = item.deentradas.TIPO;
        //                    det.GRUPO = item.deentradas.GRUPO;
        //                    det.CLAVE = item.deentradas.CLAVE;
        //                    det.PRESENTACION = item.deentradas.PRESENTACION;
        //                    det.CANTIDAD = item.deentradas.CANTIDAD;
        //                    det.COSTO = item.deentradas.COSTO;
        //                    det.IVA = item.deentradas.IVA;
        //                    det.PIVA = item.deentradas.PIVA;
        //                    det.RAMO = item.deentradas.RAMO;
        //                    det.UP = item.deentradas.UR;
        //                    det.U_ACT = Convert.ToInt16(user.id_user);
        //                    det.F_ACT = DateTime.Now;
        //                    det.U_CREO = item.deentradas.U_CREO;
        //                    det.F_CREO = item.deentradas.F_CREO;
        //                    det.afectado = false;
        //                    det.id_centro = entrada.id_centro;
        //                    dbConcentradora.de_entradas.Add(det);
        //                    dbConcentradora.SaveChanges();
        //                    id_++;
        //                }
        //                catch (Exception e)
        //                {

        //                }
        //            }
        //        }

        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    return mdl;
        //}

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

        //public DeEntradasClassSIAA addCaducidad(DeEntradasClassSIAA insumo, DE_EntradasCAD caducidad)
        //{
        //    try
        //    {
        //        List<DE_EntradasCAD> existe = (from r in insumo.deEntradasCad where r.PK_ARTICULOS == caducidad.PK_ARTICULOS && r.pk_caducidades == caducidad.pk_caducidades select r).ToList();

        //        if (existe.Count > 0)
        //        {
        //            DE_EntradasCAD rowExiste = insumo.deEntradasCad.SingleOrDefault(a => a.PK_ARTICULOS == existe[0].PK_ARTICULOS && a.pk_caducidades == existe[0].pk_caducidades);
        //            caducidad.Cantidad = rowExiste.Cantidad + caducidad.Cantidad;
        //            insumo.deEntradasCad.Remove(rowExiste);
        //            insumo.deEntradasCad.Add(caducidad);
        //        }
        //        else
        //        {
        //            insumo.deEntradasCad.Add(caducidad);
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    return insumo;
        //}

        //public DeEntradasClassSIAA deleteCaducidad(DeEntradasClassSIAA insumo, DE_EntradasCAD caducidad)
        //{
        //    try
        //    {
        //        List<DE_EntradasCAD> existe = (from r in insumo.deEntradasCad where r.PK_ARTICULOS == caducidad.PK_ARTICULOS && r.pk_caducidades == caducidad.pk_caducidades select r).ToList();

        //        if (existe.Count > 0)
        //        {
        //            DE_EntradasCAD rowExiste = insumo.deEntradasCad.SingleOrDefault(a => a.PK_ARTICULOS == existe[0].PK_ARTICULOS && a.pk_caducidades == existe[0].pk_caducidades);
        //            caducidad.Cantidad = rowExiste.Cantidad + caducidad.Cantidad;
        //            insumo.deEntradasCad.Remove(rowExiste);
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    return insumo;
        //}

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
                TA_Parametros parametro;
                parametro = (from r in dbConcentradora.TA_Parametros where r.id_centro == user.id_unidad select r).First();
                id = parametro.F_PEDIDOS ?? 0;
                id = id + 1;
            }
            catch
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



        #endregion


    }
}
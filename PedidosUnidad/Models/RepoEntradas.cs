using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PedidosUnidad.Models.DBConcentradora;
using PedidosUnidad.Models.DBAlmacen;
using PedidosUnidad.Security;
using System.Text;
using System.Data.SqlClient;

namespace PedidosUnidad.Models
{
    public class RepoEntradas
    {
        DBCONCENTRADORA dbConcentradora = null;

        public RepoEntradas()
        {
            dbConcentradora = new DBCONCENTRADORA();
        }

        public EntradasClassSIAA getEntradas(int id_centro)
        {
            EntradasClassSIAA mdlEntradas = new EntradasClassSIAA();

            try
            {
                List<vwListadoEntradas> Entradas = new List<vwListadoEntradas>();
                Entradas = (from r in dbConcentradora.vwListadoEntradas where r.id_centro == id_centro select r).ToList();
                mdlEntradas.listadoEntradas = Entradas;
                mdlEntradas.exito = true;
                mdlEntradas.msg = "Éxito";
            }
            catch (Exception e)
            {
                mdlEntradas.listadoEntradas = new List<vwListadoEntradas>();
                mdlEntradas.exito = false;
                mdlEntradas.msg = "Error al obtener el listado de las entradas de la unidad";
            }


            return mdlEntradas;
        }

        public ReturnModelClass saveEntrada(ref MaDeEntradasClassSIAA pMaDeEntradasClassSIAA, CurrentUser user)
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
                        pMaDeEntradasClassSIAA.maEntradas.FOLIO = getNewId(user);
                        pMaDeEntradasClassSIAA.maEntradas.ANIO = Convert.ToInt16(DateTime.Now.Year);
                        pMaDeEntradasClassSIAA.maEntradas.NO_ENTRADA = pMaDeEntradasClassSIAA.maEntradas.FOLIO;
                        pMaDeEntradasClassSIAA.maEntradas.id_centro = user.id_unidad;
                        pMaDeEntradasClassSIAA.maEntradas.FECHA = DateTime.Now;

                        string[] claveproveedor = pMaDeEntradasClassSIAA.cvproveedor.Split('|');

                        pMaDeEntradasClassSIAA.maEntradas.ESTADOP = Convert.ToInt16(claveproveedor[0].ToString());
                        pMaDeEntradasClassSIAA.maEntradas.GIROP = Convert.ToInt16(claveproveedor[1].ToString());
                        pMaDeEntradasClassSIAA.maEntradas.CLAVEP = Convert.ToInt16(claveproveedor[2].ToString());

                        pMaDeEntradasClassSIAA.maEntradas.AFECTADA = false;
                        pMaDeEntradasClassSIAA.maEntradas.CERRADO = 0;
                        pMaDeEntradasClassSIAA.maEntradas.U_ACT = Convert.ToInt16(user.id_user);
                        pMaDeEntradasClassSIAA.maEntradas.F_ACT = DateTime.Now;
                        pMaDeEntradasClassSIAA.maEntradas.U_CREO = Convert.ToInt16(user.id_user);
                        pMaDeEntradasClassSIAA.maEntradas.F_CREO = DateTime.Now;
                        pMaDeEntradasClassSIAA.maEntradas.cancelado = false;

                        ma_entradas mdlMaDeEntradasClassSIAA = new ma_entradas();
                        mdlMaDeEntradasClassSIAA = pMaDeEntradasClassSIAA.maEntradas;

                        mdlMaDeEntradasClassSIAA.TIPO_INSUMOS = pMaDeEntradasClassSIAA.maEntradas.TIPO_INSUMOS;
                        //GUARDAR CABECERO AGREGADO
                        db_c.ma_entradas.Add(mdlMaDeEntradasClassSIAA);
                        db_c.SaveChanges();

                        //DATOS DETALLE ENTRADA (PARTIDAS)
                        foreach (var item in pMaDeEntradasClassSIAA.deEntradas)
                        {

                            item.deentradas.FOLIO = mdlMaDeEntradasClassSIAA.FOLIO;
                            item.deentradas.ANIO = Convert.ToInt16(DateTime.Now.Year);
                            item.deentradas.U_ACT = Convert.ToInt16(user.id_user);
                            item.deentradas.F_ACT = DateTime.Now;
                            item.deentradas.U_CREO = Convert.ToInt16(user.id_user);
                            item.deentradas.F_CREO = DateTime.Now;
                            item.deentradas.afectado = false;

                            de_entradas det = new de_entradas();
                            det = item.deentradas;
                            //GUARDADO DE DETALLE
                            db_c.de_entradas.Add(det);
                            db_c.SaveChanges();


                            foreach (var cad in item.deEntradasCad)
                            {
                                //DATOS DE CADUCIDADES
                                cad.No_Entrada = mdlMaDeEntradasClassSIAA.FOLIO;
                                cad.Anio = Convert.ToInt16(DateTime.Now.Year);
                                cad.U_ACT = Convert.ToInt16(user.id_user);
                                cad.F_ACT = DateTime.Now;
                                cad.afectado = false;

                                cad.id_centro = item.deentradas.id_centro;
                                cad.Tipo = item.deentradas.TIPO;
                                cad.Grupo = item.deentradas.GRUPO;
                                cad.Clav_Med = item.deentradas.CLAVE;
                                cad.pk_caducidades = null;

                                DE_EntradasCAD mdlcad = new DE_EntradasCAD();
                                mdlcad = cad;
                                //GUARDADO DE CADUCIDADES
                                db_c.DE_EntradasCAD.Add(mdlcad);
                                db_c.SaveChanges();
                            }
                        }


                        mdl.exito = true;
                        mdl.msg = "Entrada generada correctamente";
                        mdl.var_1 = pMaDeEntradasClassSIAA.maEntradas.FOLIO.ToString("000");

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


        public ReturnModelClass editEntrada(ref MaDeEntradasClassSIAA pMaDeEntradasClassSIAA, CurrentUser user)
        {
            ReturnModelClass mdl_return = new ReturnModelClass();

            //creamos nuestro contexto
            using (var db_c = new DBCONCENTRADORA())
            {
                int folio = pMaDeEntradasClassSIAA.maEntradas.FOLIO;
                int anio = pMaDeEntradasClassSIAA.maEntradas.ANIO;
                int centro = pMaDeEntradasClassSIAA.maEntradas.id_centro;
                //creamos el ámbito de la transacción
                using (var dbContextTransaction = db_c.Database.BeginTransaction())
                {

                    try
                    {

                        //GET CABECERO
                        ma_entradas entrada = db_c.ma_entradas.SingleOrDefault(a => a.FOLIO == folio && a.ANIO == anio && a.id_centro == centro);


                        string[] claveproveedor = pMaDeEntradasClassSIAA.cvproveedor.Split('|');
                        entrada.ESTADOP = Convert.ToInt16(claveproveedor[0].ToString());
                        entrada.GIROP = Convert.ToInt16(claveproveedor[1].ToString());
                        entrada.CLAVEP = Convert.ToInt16(claveproveedor[2].ToString());
                        entrada.U_ACT = Convert.ToInt16(user.id_user);
                        entrada.F_ACT = DateTime.Now;

                        entrada.TIPO_ENTRADA = pMaDeEntradasClassSIAA.maEntradas.TIPO_ENTRADA;
                        entrada.FOLIO_PEDIDO = pMaDeEntradasClassSIAA.maEntradas.FOLIO_PEDIDO;
                        entrada.ANIO_PEDIDO = pMaDeEntradasClassSIAA.maEntradas.ANIO_PEDIDO;
                        entrada.TIPO_DOCTO = pMaDeEntradasClassSIAA.maEntradas.TIPO_DOCTO;
                        entrada.FOLIO_DOCTO = pMaDeEntradasClassSIAA.maEntradas.FOLIO_DOCTO;
                        entrada.TIPO_LICITACION = pMaDeEntradasClassSIAA.maEntradas.TIPO_LICITACION;
                        entrada.FOLIO_LICITACION = pMaDeEntradasClassSIAA.maEntradas.FOLIO_LICITACION;
                        entrada.ANIO_LICITACION = pMaDeEntradasClassSIAA.maEntradas.ANIO_LICITACION;
                        entrada.SUBTOTAL = pMaDeEntradasClassSIAA.maEntradas.SUBTOTAL;
                        entrada.IVA = pMaDeEntradasClassSIAA.maEntradas.IVA;
                        entrada.TOTAL = pMaDeEntradasClassSIAA.maEntradas.TOTAL;
                        entrada.OBSERVA = pMaDeEntradasClassSIAA.maEntradas.OBSERVA;
                        entrada.NO_POLIZA_ENT = pMaDeEntradasClassSIAA.maEntradas.NO_POLIZA_ENT;
                        entrada.CENTRO_REQ = pMaDeEntradasClassSIAA.maEntradas.CENTRO_REQ;
                        entrada.ANIO_POLIZA_ENT = pMaDeEntradasClassSIAA.maEntradas.ANIO_POLIZA_ENT;
                        entrada.no_prog = pMaDeEntradasClassSIAA.maEntradas.no_prog;
                        entrada.Id_FuenteFinanciamiento = pMaDeEntradasClassSIAA.maEntradas.Id_FuenteFinanciamiento;
                        db_c.SaveChanges();

                        //CLEAN DETALLE CAD
                        List<DE_EntradasCAD> detalle_entrada_cad = (from r in db_c.DE_EntradasCAD where r.No_Entrada == folio && r.id_centro == centro && r.Anio == anio select r).ToList();
                        db_c.DE_EntradasCAD.RemoveRange(detalle_entrada_cad);
                        db_c.SaveChanges();

                        //CLEAN DETALLE
                        List<de_entradas> detalle_entrada = (from r in db_c.de_entradas where r.FOLIO == folio && r.id_centro == centro && r.ANIO == anio select r).ToList();
                        db_c.de_entradas.RemoveRange(detalle_entrada);
                        db_c.SaveChanges();


                        int id_ = 1;
                        foreach (var item in pMaDeEntradasClassSIAA.deEntradas)
                        {
                            de_entradas det = new de_entradas();
                            det.FOLIO = entrada.FOLIO;
                            det.ANIO = entrada.ANIO;
                            det.PK_ARTICULOS = item.deentradas.PK_ARTICULOS;
                            det.TIPO = item.deentradas.TIPO;
                            det.GRUPO = item.deentradas.GRUPO;
                            det.CLAVE = item.deentradas.CLAVE;
                            det.PRESENTACION = item.deentradas.PRESENTACION;
                            det.CANTIDAD = item.deentradas.CANTIDAD;
                            det.COSTO = item.deentradas.COSTO;
                            det.IVA = item.deentradas.IVA;
                            det.PIVA = item.deentradas.PIVA;
                            det.RAMO = item.deentradas.RAMO;
                            det.UP = item.deentradas.UR;
                            det.U_ACT = Convert.ToInt16(user.id_user);
                            det.F_ACT = DateTime.Now;
                            det.U_CREO = item.deentradas.U_CREO;
                            det.F_CREO = item.deentradas.F_CREO;
                            det.afectado = false;
                            det.id_centro = entrada.id_centro;

                            db_c.de_entradas.Add(det);
                            db_c.SaveChanges();
                            id_++;

                            foreach (var cad in item.deEntradasCad)
                            {
                                //DATOS DE CADUCIDADES
                                cad.No_Entrada = entrada.FOLIO;
                                cad.Anio = Convert.ToInt16(entrada.ANIO);
                                cad.U_ACT = Convert.ToInt16(user.id_user);
                                cad.F_ACT = DateTime.Now;
                                cad.afectado = false;

                                cad.id_centro = item.deentradas.id_centro;
                                cad.Tipo = item.deentradas.TIPO;
                                cad.Grupo = item.deentradas.GRUPO;
                                cad.Clav_Med = item.deentradas.CLAVE;
                                cad.pk_caducidades = null;

                                DE_EntradasCAD mdlcad = new DE_EntradasCAD();
                                mdlcad = cad;
                                //GUARDADO DE CADUCIDADES
                                db_c.DE_EntradasCAD.Add(mdlcad);
                                db_c.SaveChanges();
                            }
                        }

                        mdl_return.exito = true;
                        mdl_return.msg = "Entrada editada correctamente";
                        mdl_return.var_1 = entrada.FOLIO.ToString("000");
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

        public ReturnModelClass addInsumo(ref List<DeEntradasClassSIAA> mdl, DeEntradasClassSIAA insumo, int editar)
        {
            int[] arrayTipo = { 0, 10, 30, 40, 41 };
            ReturnModelClass returnModel = new ReturnModelClass();
            try
            {
                List<DeEntradasClassSIAA> existe = (from r in mdl where r.deentradas.PK_ARTICULOS == insumo.deentradas.PK_ARTICULOS select r).ToList();
                TA_Parametros mdlParametros = dbConcentradora.TA_Parametros.SingleOrDefault(a => a.id_centro == insumo.deentradas.id_centro);

                //NO EDICION
                returnModel.var_1 = "0";

                if (existe.Count > 0)
                {
                    returnModel.var_1 = "1"; //VARIABLE PARA EN EL FRONT ELIMINAR DE PREVIO EL ACTUAL Y PONER LOS NUEVOS DATOS DE RETORNO
                    
                    DeEntradasClassSIAA rowExiste = mdl.SingleOrDefault(a => a.deentradas.PK_ARTICULOS == existe[0].deentradas.PK_ARTICULOS);
                    //SI NO ESTA EDITANDO Y EXISTE YA LA CLAVE AGREGADA SUMAR EL VALOR QE YA ESTABA AL NUEVO
                    if (editar == 0)
                        insumo.deentradas.CANTIDAD = rowExiste.deentradas.CANTIDAD + insumo.deentradas.CANTIDAD;

                    if (arrayTipo.Contains(insumo.deentradas.TIPO))
                    {
                        insumo.deentradas.PIVA = 0;
                        insumo.deentradas.IVA = 0;
                    }
                    else
                    {
                        insumo.deentradas.PIVA = 16;
                        if (mdlParametros != null)
                            insumo.deentradas.PIVA = mdlParametros.PIVA;

                        insumo.deentradas.IVA = (insumo.deentradas.CANTIDAD * insumo.deentradas.COSTO) * (insumo.deentradas.PIVA / 100);
                    }

                    //REMOVER RENGLON
                    mdl.Remove(rowExiste);
                    //AGREGAR NUEVOS VALORES
                    mdl.Add(insumo);

                    returnModel.exito = true;
                    returnModel.msg = "Clave Editada Correctamente";
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

        public ReturnModelClass deleteInsumo(ref List<DeEntradasClassSIAA> mdl, DeEntradasClassSIAA insumo)
        {
            ReturnModelClass rmodel = new ReturnModelClass();
            try
            {
                List<DeEntradasClassSIAA> existe = (from r in mdl where r.deentradas.PK_ARTICULOS == insumo.deentradas.PK_ARTICULOS select r).ToList();

                if (existe.Count > 0)
                {
                    DeEntradasClassSIAA rowExiste = mdl.SingleOrDefault(a => a.deentradas.PK_ARTICULOS == existe[0].deentradas.PK_ARTICULOS);
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

        public DeEntradasClassSIAA addCaducidad(DeEntradasClassSIAA insumo, DE_EntradasCAD caducidad)
        {
            try
            {
                List<DE_EntradasCAD> existe = (from r in insumo.deEntradasCad where r.PK_ARTICULOS == caducidad.PK_ARTICULOS && r.pk_caducidades == caducidad.pk_caducidades select r).ToList();

                if (existe.Count > 0)
                {
                    DE_EntradasCAD rowExiste = insumo.deEntradasCad.SingleOrDefault(a => a.PK_ARTICULOS == existe[0].PK_ARTICULOS && a.pk_caducidades == existe[0].pk_caducidades);
                    caducidad.Cantidad = rowExiste.Cantidad + caducidad.Cantidad;
                    insumo.deEntradasCad.Remove(rowExiste);
                    insumo.deEntradasCad.Add(caducidad);
                }
                else
                {
                    insumo.deEntradasCad.Add(caducidad);
                }
            }
            catch (Exception e)
            {

            }

            return insumo;
        }

        public DeEntradasClassSIAA deleteCaducidad(DeEntradasClassSIAA insumo, DE_EntradasCAD caducidad)
        {
            try
            {
                List<DE_EntradasCAD> existe = (from r in insumo.deEntradasCad where r.PK_ARTICULOS == caducidad.PK_ARTICULOS && r.pk_caducidades == caducidad.pk_caducidades select r).ToList();

                if (existe.Count > 0)
                {
                    DE_EntradasCAD rowExiste = insumo.deEntradasCad.SingleOrDefault(a => a.PK_ARTICULOS == existe[0].PK_ARTICULOS && a.pk_caducidades == existe[0].pk_caducidades);
                    caducidad.Cantidad = rowExiste.Cantidad + caducidad.Cantidad;
                    insumo.deEntradasCad.Remove(rowExiste);
                }
            }
            catch (Exception e)
            {

            }

            return insumo;
        }

        public MaDeEntradasClassSIAA getEntrada(int folio, int anio, CurrentUser user)
        {
            AlmacenEntity dbA = new AlmacenEntity();
            MaDeEntradasClassSIAA FullEntrada = new MaDeEntradasClassSIAA();
            try
            {

                //GET CABECERO

                FullEntrada.maEntradas = dbConcentradora.ma_entradas.SingleOrDefault(a => a.FOLIO == folio && a.id_centro == user.id_unidad && a.ANIO == anio);
                FullEntrada.cvproveedor = (FullEntrada.maEntradas.ESTADOP.ToString() + '|' + FullEntrada.maEntradas.GIROP.ToString() + '|' + FullEntrada.maEntradas.CLAVEP.ToString()).ToString();
                List<de_entradas> detalle = (from r in dbConcentradora.de_entradas where r.FOLIO == folio && r.id_centro == user.id_unidad && r.ANIO == anio select r).ToList();

                foreach (var insumo in detalle)
                {
                    try
                    {
                        DeEntradasClassSIAA de = new DeEntradasClassSIAA();
                        de.deentradas = insumo;
                        de.descLarga = (from r in dbA.ma_articulos_desc_larga_tabla_corta where r.pk_articulos == insumo.PK_ARTICULOS select r.desc_larga).SingleOrDefault();
                        de.insumo = getNomInsumo(insumo.PK_ARTICULOS);
                        de.pres = getPresentacion(insumo.PK_ARTICULOS);
                        List<DE_EntradasCAD> caducidad = (from r in dbConcentradora.DE_EntradasCAD where r.No_Entrada == folio && r.id_centro == user.id_unidad && r.Anio == anio && r.PK_ARTICULOS == insumo.PK_ARTICULOS select r).ToList();

                        foreach (var cad in caducidad)
                        {
                            try
                            {
                                DE_EntradasCAD decad = new DE_EntradasCAD();
                                decad = cad;
                                decad.pk_caducidades = String.Format("{0:dd/MM/yyyy}", cad.Caducidad); 

                                de.deEntradasCad.Add(decad);
                            }
                            catch (Exception e)
                            {

                            }
                        }

                        FullEntrada.deEntradas.Add(de);
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

        public ReturnModelClass afectarEntrada(string Proceso, int Folio,  int Anio, int Id_Centro, int Usr)
        {
            ReturnModelClass mdl = new ReturnModelClass();
            try
            {
                string query = "exec dbo.Afecta_Entradas '" + Proceso+"'," + Folio + "," + Anio + "," + Id_Centro + "," + Usr;
                List<int> row_ = dbConcentradora.Database.SqlQuery<int>(query.ToString()).ToList();
                int exito = row_.First();

                if (exito == 1)
                {
                    mdl.exito = true;
                    mdl.msg = "Entrada " + Folio + "/" + Anio + " afectada correctamente.";
                }
                else
                {
                    mdl.exito = false;
                    mdl.msg = "Ocurrio un error en proceso interno, vuelva a intentarlo.";
                }
                
            }
            catch(Exception e)
            {
                mdl.exito = false;
                mdl.msg = "Ocurrio un error. Favor de intentarlo de nuevo, de otra forma reportar al departamento de sistemas.";
            }
            return mdl;
        }

        #region Utilierias

        public SelectList getTipoEntrada()
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            List<CA_TIPOSENT> TipoEntradas = (dbAlmacen.CA_TIPOSENT).ToList();

            return new SelectList(TipoEntradas, "TIPO", "DESCRIP");
        }

        public SelectList getTipoDocumento()
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            List<CA_TIPOSDOC> TipoDocumentos = (dbAlmacen.CA_TIPOSDOC).ToList();

            return new SelectList(TipoDocumentos, "TIPO", "DESCRIP");
        }

        public SelectList getTipoLicitacion()
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            List<CA_TIPOSLIC> TipoLicitaciones = (dbAlmacen.CA_TIPOSLIC).ToList();

            return new SelectList(TipoLicitaciones, "TIPO", "DESCRIP");
        }

        public SelectList getCentroRequisito()
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            List<CA_CENTROS> Centros = (dbAlmacen.CA_CENTROS).ToList();

            return new SelectList(Centros, "CENTRO", "DESCRIP");
        }

        public SelectList getPrograma()
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            List<CA_PROGRAMAS_SALUD> Programas = (dbAlmacen.CA_PROGRAMAS_SALUD).ToList();

            return new SelectList(Programas, "NO_PROG", "DESCRIP");
        }

        public SelectList getFuente()
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();

            List<Ca_FuenteFinanciamiento> Fuentes = (dbAlmacen.Ca_FuenteFinanciamiento).ToList();

            return new SelectList(Fuentes, "Id_FuenteFinanciamiento", "Descripcion");
        }

        public SelectList getProveedor()
        {
            AlmacenEntity dbAlmacen = new AlmacenEntity();
            List<vwProveedores> Proveedores = (dbAlmacen.vwProveedores).OrderBy(a => a.NOMBRE).ToList();

            return new SelectList(Proveedores, "id_prov", "nombre");
        }

        public int getNewId(CurrentUser user)
        {
            

            int id = 0;
            try
            {

                string query = "exec dbo.Genera_Folio 'E'," + user.id_user + "," + user.id_unidad;
                List<int> row_ = dbConcentradora.Database.SqlQuery<int>(query.ToString()).ToList();
                id = row_.First();


                //TA_Parametros parametro;
                //parametro = (from r in dbConcentradora.TA_Parametros where r.id_centro == user.id_unidad select r).First();
                //id = parametro.F_ENTRADAS ?? 0;
                //id = id + 1;
            }
            catch (Exception e)
            {
                id = -1;
            }
            return id;


        }

        //LIMPIAR DETALLE DE PEDIDO 
        public bool initDetalleEntrada(int id, int id_unidad, int anio)
        {
            bool exito = false;
            try
            {
                List<de_entradas> detalle_entrada = (from r in dbConcentradora.de_entradas where r.FOLIO == id && r.id_centro == id_unidad && r.ANIO == anio select r).ToList();
                dbConcentradora.de_entradas.RemoveRange(detalle_entrada);
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

        public List<InsumoClassSIAA> getLstInsumos(int opc, bool controlado)
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

            QueryString.Append(", ISNULL(ma.id_categoria, 0) id_categoria ");
            QueryString.Append(", ISNULL(cain.descripcion, 'NA')  categoria ");

            QueryString.Append("from ma_articulos ma ");
            QueryString.Append("left join ma_articulos_desc_larga_tabla_corta m on ma.pk_articulos = m.pk_articulos ");
            QueryString.Append("left join ca_unidadesp up on ma.uni_pres = up.unidad ");
            QueryString.Append("left join ca_unidadesm um on ma.uni_med = um.unidad ");
            QueryString.Append("left join ca_categoria_insumo cain on ma.id_categoria = cain.id ");

            switch (opc)
            {
                case 1:
                    if (controlado)
                    {
                        QueryString.Append("where ma.controlado = 1 and ma.activo=1 ");
                    }
                    else
                    {
                        QueryString.Append("where ma.tipo in(10,20,30,40,41) and (ma.controlado is null or ma.controlado = 0) and ma.activo=1 ");
                    }
                    
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

            //QueryString.Append("order by ma.tipo ");
            QueryString.Append("order by ma.descripcion "); 

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



        #endregion

    }
}
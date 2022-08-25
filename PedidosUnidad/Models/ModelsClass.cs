using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PedidosUnidad.Models.DBPedido;

namespace PedidosUnidad.Models
{
    public class ModelsClass
    {
    }

    public class CPMpedidosModel
    {
        public CPMpedidosModel()
        {
            pedidos = new List<PedidoModel>();
        }
        public List<PedidoModel> pedidos { get; set; }

        public int tipo_pedido { get; set; }
        public bool show_msg { get; set; }
        public bool exito { get; set; }
        public string msg { get; set; }
    }

    public class PedidosUnidadModelAlmCentral
    {
        public PedidosUnidadModelAlmCentral()
        {
            pedidos = new List<PedidoModelAlmCentral>();
        }

        public List<PedidoModelAlmCentral> pedidos { get; set; }

        public bool show_msg { get; set; }
        public bool exito { get; set; }
        public string msg { get; set; }
    }

    public class PedidoModelAlmCentral
    {
        public PedidoModelAlmCentral()
        {

        }
        public string pedido { get; set; }
        public string fecha { get; set; }
        public string obs { get; set; }
        public int pedido_ori { get; set; }
        public int anio_ori { get; set; }

    }

    public class PedidoModel
    {
        public PedidoModel()
        {
            articulos = new List<CPMrowModel>();
            resumen_vale = new valeResumen();
            resumen_suma = new valeTotales();
        }

        public bool tieneCPM { get; set; }

        public string folio { get; set; }
        public int id_pedido { get; set; }
        public int id_unidad { get; set; }
        public string descrip_unidad { get; set; }
        public string domicilio_unidad { get; set; }
        public string colonia_unidad { get; set; }
        public string cp_unidad { get; set; }
        public string estado_unidad { get; set; }
        public string municipio_unidad { get; set; }
        public string tel_unidad { get; set; }
        public string rfc_unidad { get; set; }


        public string anio { get; set; }
        public string mes { get; set; }
        public string fecha { get; set; }
        public DateTime fecha_envio { get; set; }
        public int claves_total { get; set; }
        public int claves_pedidas { get; set; }
        public int claves_medicamento { get; set; }
        public int claves_curacion { get; set; }
        public bool confirmado { get; set; }
        public bool editando { get; set; }
        public string folio_almacen { get; set; }
        public int estatus { get; set; }
        public List<CPMrowModel> articulos { get; set; }

        public int dia { get; set; }
        public bool actual { get; set; }

        public bool exito { get; set; }
        public string msg { get; set; }
        public bool existe_pedido { get; set; }

        public string dia_no { get; set; }
        public string dia_des { get; set; }
        public string mes_des { get; set; }
        public string hrs_des { get; set; }
        public int tipo_pedido { get; set; }
        public string pedido { get; set; }
        public string descrip_tipo_pedido { get; set; }
        public string descrip_tipo_vale { get; set; }

        public int mes_ordinario { get; set; }
        public bool controlado { get; set; }

        public int tipo_guardado { get; set; }

        public byte[] QR { get; set; }

        public string encargado { get; set; }
        public string administrador { get; set; }
        public string director { get; set; }

        //DATOS DE RESUMEN DE VALE
        public valeResumen resumen_vale { get; set; }
        public valeTotales resumen_suma { get; set; }

    }

    public class DatosUnidadClass
    {
        public DatosUnidadClass()
        { 
        
        }
        public string direccion { get; set; }
        public string codpost { get; set; }
        public string rfc { get; set; }
        public string tel { get; set; }
        public string Entidad { get; set; }
        public string Municipio { get; set; }
    }

    public class CPMrowModel
    {
        public CPMrowModel()
        {
            cpm = 0;
            existencia = 0;
            solicita = 0;
            lotes = new List<lotesInsumo>();
        }

        public string pk { get; set; }
        public int centro { get; set; }
        public string clave { get; set; }
        public string descripcion { get; set; }
        public string costo_promedio { get; set; }
        public string costo_insumo { get; set; }
        public int cpm { get; set; }
        public int existencia { get; set; }
        public int solicita { get; set; }
        public int surtida { get; set; }
        public string origen { get; set; }
        public int tipo { get; set; }
        public string programa { get; set; }
        public bool desierta { get; set; }
        public bool controlado { get; set; }
        public int maximo { get; set; }
        public int minimo { get; set; }
        public int tipo_mov { get; set; }
        public string des_mov { get; set; }
        public bool consolidada { get; set; }
        public bool dimesa { get; set; }
        public int max_dimesa { get; set; }
        public string presentacion { get; set; }
        public string subtipo_insumo { get; set; }
        //DATOS DE VALE LOTES
        public List<lotesInsumo> lotes { get; set; }

        //PROPIEDADES PARA LA TABLA DE CAPTURA DIARIA
        public int D1 { get; set; }
        public bool A1 { get; set; }
        public int tipo_mov_1 { get; set; }
        public string des_mov_1 { get; set; }
        public int D2 { get; set; }
        public bool A2 { get; set; }
        public int tipo_mov_2 { get; set; }
        public string des_mov_2 { get; set; }
        public int D3 { get; set; }
        public bool A3 { get; set; }
        public int tipo_mov_3 { get; set; }
        public string des_mov_3 { get; set; }
        public int D4 { get; set; }
        public bool A4 { get; set; }
        public int tipo_mov_4 { get; set; }
        public string des_mov_4 { get; set; }
        public int D5 { get; set; }
        public bool A5 { get; set; }
        public int tipo_mov_5 { get; set; }
        public string des_mov_5 { get; set; }
        public int D6 { get; set; }
        public bool A6 { get; set; }
        public int tipo_mov_6 { get; set; }
        public string des_mov_6 { get; set; }
        public int D7 { get; set; }
        public bool A7 { get; set; }
        public int tipo_mov_7 { get; set; }
        public string des_mov_7 { get; set; }
        public int D8 { get; set; }
        public bool A8 { get; set; }
        public int tipo_mov_8 { get; set; }
        public string des_mov_8 { get; set; }
    }
    public class lotesInsumo
    {
        public string lote { get; set; }
        public string f_caducidad { get; set; }
        public string costo { get; set; }
        public string importe { get; set; }
        public string programa { get; set; }

        public string cantidad_lote { get; set; }
    }

    public class ExistePedidoModel
    {
        public ExistePedidoModel()
        {
            pedido = new ma_pedido();
        }

        public bool flag { get; set; }
        public ma_pedido pedido { get; set; }
    }

    public class ExistenciaDiaria
    {
        public ExistenciaDiaria()
        {
            DiasExistencia = new List<PedidoModel>();
            Rows = new List<CPMrowModel>();
            ultimo_dia_captura = "S/C";
        }

        public bool tieneCPM { get; set; }

        public List<PedidoModel> DiasExistencia { get; set; }
        public List<CPMrowModel> Rows { get; set; }
        public int no_claves { get; set; }
        public int claves_medicamento { get; set; }
        public int claves_curacion { get; set; }
        public string semana { get; set; }
        public string f_desde { get; set; }
        public string f_hasta { get; set; }
        public string dia_en_curso { get; set; }
        public string ultimo_dia_captura { get; set; }
        public string d_1 { get; set; }
        public string d_2 { get; set; }
        public string d_3 { get; set; }
        public string d_4 { get; set; }
        public string d_5 { get; set; }
        public string d_6 { get; set; }
        public string d_7 { get; set; }
        public string d_8 { get; set; }
        public int id_ { get; set; }
        public int id_unidad { get; set; }
        public int tipo { get; set; }
        public string fecha_afectar { get; set; }
        public bool editando { get; set; }
        public string tipo_listado { get; set; }
        public string nom_unidad { get; set; }
    }

    public class tblCaptura
    {

    }

    //MODELO PARA CUADRO
    public class CuadroGeneralModel
    {
        public CuadroGeneralModel()
        {
            rowsCuadro = new List<RowsCuadroGeneralModel>();
        }
        public int anio { get; set; }
        public string tipo { get; set; }
        public int total_claves { get; set; }
        public int total_consolidada { get; set; }
        public int total_dimesa { get; set; }
        public int total_desiertas { get; set; }
        public int total_primer_nivel { get; set; }
        public int total_segundo_nivel { get; set; }

        public List<RowsCuadroGeneralModel> rowsCuadro { get; set; }
    }

    public class RowsCuadroGeneralModel
    {
        public RowsCuadroGeneralModel()
        {

        }
        public int anio { get; set; }
        public int id { get; set; }
        public string cve { get; set; }
        public string cve_sist { get; set; }
        public string descripcion { get; set; }
        public bool quitar { get; set; }
        public bool controlado { get; set; }
        public string origen_desc { get; set; }
        public bool origen_desierta { get; set; }
        public bool origen_consolidada { get; set; }
        public bool origen_dimesa { get; set; }
        public bool origen_otro { get; set; }
        public string origen_otro_desc { get; set; }
        public bool programa { get; set; }
        public string programa_desc { get; set; }
        public int hru { get; set; }
        public int hgt { get; set; }
        public int hmi { get; set; }
        public int hgm { get; set; }
        public int hgi { get; set; }
        public int iec { get; set; }
        public int ceh { get; set; }
        public int j1 { get; set; }
        public int j2 { get; set; }
        public int j3 { get; set; }
        public int uneme_colima { get; set; }
        public int uneme_tecoman { get; set; }
        public int uneme_manzanillo { get; set; }
        public int cara_varonil { get; set; }
        public int cara_femenil { get; set; }
        public int capasits_colima { get; set; }
        public int capasits_tecoman { get; set; }
        public int capasits_manzanillo { get; set; }
        public int fam { get; set; }
        public int total_mensual { get; set; }
        public int total_anual { get; set; }
        public int max_consolidada { get; set; }
        public int min_consolidada { get; set; }
        public int max_dimesa { get; set; }
        public int min_dimesa { get; set; }
        public bool nivel_1 { get; set; }
        public bool nivel_2 { get; set; }
        public string tipo_consumo { get; set; }
        public bool prospecta_baja { get; set; }

        public string descrip_almacen { get; set; }
        public string descrip_nacional { get; set; }
        public decimal costo_nacional { get; set; }
        public string proveedor { get; set; }

        public bool existe_ma { get; set; }
        public string pk { get; set; }
        public int existencia_almacen { get; set; }
    }

    public class SalidasClass
    {
        public string unidad { get; set; }
        public int pk_centro { get; set; }
        public string fecha { get; set; }
        public int solicitado { get; set; }
        public int surtido { get; set; }
    }

    public class EntradasClass
    {
        public int folio { get; set; }
        public int anio { get; set; }
        public string fecha { get; set; }
        public int cantidad { get; set; }
        public string tipo_entrada_desc { get; set; }
    }

    public class PermisosClass
    {
        public int id { get; set; }
        public int id_modulo { get; set; }
        public int id_rol { get; set; }
        public string nombre { get; set; }
        public string ruta { get; set; }
        public bool permiso { get; set; }

    }

    public class DocExistenciasClass
    {
        public string pk { get; set; }
        public int existencia { get; set; }

    }

    public class TotalUltimoPedidoClass
    {
        public int solicitado { get; set; }
        public int surtido { get; set; }
        public string mes { get; set; }
    }

    public class ProyeccionPedidoClass
    {
        public string clave { get; set; }
        public string descripcion { get; set; }
        public int cpm { get; set; }
        public int cpm_real { get; set; }
        public int existencia { get; set; }
        public int solicitar { get; set; }
        public string proveedor { get; set; }
        public string licitacion { get; set; }
    }

    public class ReporteExistenciaClass
    {
        public ReporteExistenciaClass()
        {
            rowsInsumos = new List<rowReportExistencia>();
        }
        public string unidad { get; set; }
        public string ultima_captura { get; set; }
        public int tipo_insumo { get; set; }
        public List<rowReportExistencia> rowsInsumos { get; set; }
    }

    public class rowReportExistencia
    {
        public string pk { get; set; }
        public string clave { get; set; }
        public string descripcion { get; set; }
        public string presentacion { get; set; }
        public string cpm { get; set; }
        public string existencias { get; set; }

        public string origen { get; set; }
        public string programa { get; set; }
        public bool desierta { get; set; }
        public bool controlado { get; set; }
        public int maximo { get; set; }
        public int minimo { get; set; }
        public bool consolidada { get; set; }
        public bool dimesa { get; set; }
        public int max_dimesa { get; set; }
    }

    public class ResultClass
    {
        public bool exito { get; set; }
        public string msg { get; set; }

        public int id_ { get; set; }
        public bool editando { get; set; }
    }

    public class ResultDatosClass
    {
        public int tipo { get; set; } // 1 MEDICAMENTO, 2 MATERIAL 
        public int pedido { get; set; }    //  1 ORDINARIO, 2 EXTRAORDINARIO 
        public int mes { get; set; }
        public bool controlado { get; set; }
        public bool flag { get; set; }
    }

    public class CuadroMonitoreoClass
    {
        public CuadroMonitoreoClass()
        {
            rows = new List<RowCuadroMonitoreo>();
        }

        public int tipo { get; set; }
        public int total_claves { get; set; }
        public List<RowCuadroMonitoreo> rows { get; set; }
    }

    public class RowCuadroMonitoreo
    {
        public string pk_articulo { get; set; }
        public string cve { get; set; }
        public string descripcion { get; set; }
        public int exist_almacen { get; set; }
        public bool desierta { get; set; }
        public bool consolidada { get; set; }
        public bool dimesa { get; set; }

        public int cpm_hru { get; set; }
        public int exist_hru { get; set; }
        public DateTime? ult_fec_hru { get; set; }
        public string ult_hr_hru { get; set; }

        public int cpm_hmi { get; set; }
        public int exist_hmi { get; set; }
        public DateTime? ult_fec_hmi { get; set; }
        public string ult_hr_hmi { get; set; }

        public int cpm_hgi { get; set; }
        public int exist_hgi { get; set; }
        public DateTime? ult_fec_hgi { get; set; }
        public string ult_hr_hgi { get; set; }

        public int cpm_hgt { get; set; }
        public int exist_hgt { get; set; }
        public DateTime? ult_fec_hgt { get; set; }
        public string ult_hr_hgt { get; set; }

        public int cpm_hgm { get; set; }
        public int exist_hgm { get; set; }
        public DateTime? ult_fec_hgm { get; set; }
        public string ult_hr_hgm { get; set; }

        public int cpm_j1 { get; set; }
        public int exist_j1 { get; set; }
        public DateTime? ult_fec_j1 { get; set; }
        public string ult_hr_j1 { get; set; }

        public int cpm_j2 { get; set; }
        public int exist_j2 { get; set; }
        public DateTime? ult_fec_j2 { get; set; }
        public string ult_hr_j2 { get; set; }

        public int cpm_j3 { get; set; }
        public int exist_j3 { get; set; }
        public DateTime? ult_fec_j3 { get; set; }
        public string ult_hr_j3 { get; set; }

        public int cpm_ceh { get; set; }
        public int exist_ceh { get; set; }
        public DateTime? ult_fec_ceh { get; set; }
        public string ult_hr_ceh { get; set; }

        public int cpm_iec { get; set; }
        public int exist_iec { get; set; }
        public DateTime? ult_fec_iec { get; set; }
        public string ult_hr_iec { get; set; }

        public bool cuadro { get; set; }

        /**TEMPORAL POR COVID 19 PUNTOS DE CONTROL**/
        public int exist_pfmi_1 { get; set; }
        public DateTime? ult_fec_pfmi_1 { get; set; }
        public string ult_hr_pfmi_1 { get; set; }

        public int exist_pfmi_2 { get; set; }
        public DateTime? ult_fec_pfmi_2 { get; set; }
        public string ult_hr_pfmi_2 { get; set; }

        public int exist_pfmi_3 { get; set; }
        public DateTime? ult_fec_pfmi_3 { get; set; }
        public string ult_hr_pfmi_3 { get; set; }

        public int exist_pfmi_4 { get; set; }
        public DateTime? ult_fec_pfmi_4 { get; set; }
        public string ult_hr_pfmi_4 { get; set; }

        public int exist_pfmi_5 { get; set; }
        public DateTime? ult_fec_pfmi_5 { get; set; }
        public string ult_hr_pfmi_5 { get; set; }

        public int exist_pfmi_6 { get; set; }
        public DateTime? ult_fec_pfmi_6 { get; set; }
        public string ult_hr_pfmi_6 { get; set; }

        public int exist_pfmi_7 { get; set; }
        public DateTime? ult_fec_pfmi_7 { get; set; }
        public string ult_hr_pfmi_7 { get; set; }

        public int exist_pfmi_8 { get; set; }
        public DateTime? ult_fec_pfmi_8 { get; set; }
        public string ult_hr_pfmi_8 { get; set; }

        public int exist_pfmi_9 { get; set; }
        public DateTime? ult_fec_pfmi_9 { get; set; }
        public string ult_hr_pfmi_9 { get; set; }

        public int exist_pfmi_10 { get; set; }
        public DateTime? ult_fec_pfmi_10 { get; set; }
        public string ult_hr_pfmi_10 { get; set; }

        public int exist_pfmi_11 { get; set; }
        public DateTime? ult_fec_pfmi_11 { get; set; }
        public string ult_hr_pfmi_11 { get; set; }

        public int exist_pfmi_12 { get; set; }
        public DateTime? ult_fec_pfmi_12 { get; set; }
        public string ult_hr_pfmi_12 { get; set; }

        public int exist_pfmi_13 { get; set; }
        public DateTime? ult_fec_pfmi_13 { get; set; }
        public string ult_hr_pfmi_13 { get; set; }

        public int exist_pfmi_14 { get; set; }
        public DateTime? ult_fec_pfmi_14 { get; set; }
        public string ult_hr_pfmi_14 { get; set; }

        public int exist_pfmi_15 { get; set; }
        public DateTime? ult_fec_pfmi_15 { get; set; }
        public string ult_hr_pfmi_15 { get; set; }

        public string lote { get; set; }
        public string caducidad { get; set; }
    }

    public class UnidadesMonitoreoConsultaClass
    {
        public UnidadesMonitoreoConsultaClass()
        {
            rows = new List<RowsUnidadesMonitoreo>();

            ult_fec_hru_me = "Sin Captura";
            ult_hr_hru_me = "--";
            ult_fec_hru_ma = "Sin Captura";
            ult_hr_hru_ma = "--";

            ult_fec_hmi_me = "Sin Captura";
            ult_hr_hmi_me = "--";
            ult_fec_hmi_ma = "Sin Captura";
            ult_hr_hmi_ma = "--";

            ult_fec_hgi_me = "Sin Captura";
            ult_hr_hgi_me = "--";
            ult_fec_hgi_ma = "Sin Captura";
            ult_hr_hgi_ma = "--";

            ult_fec_hgt_me = "Sin Captura";
            ult_hr_hgt_me = "--";
            ult_fec_hgt_ma = "Sin Captura";
            ult_hr_hgt_ma = "--";

            ult_fec_hgm_me = "Sin Captura";
            ult_hr_hgm_me = "--";
            ult_fec_hgm_ma = "Sin Captura";
            ult_hr_hgm_ma = "--";

            ult_fec_j1_me = "Sin Captura";
            ult_hr_j1_me = "--";
            ult_fec_j1_ma = "Sin Captura";
            ult_hr_j1_ma = "--";

            ult_fec_j2_me = "Sin Captura";
            ult_hr_j2_me = "--";
            ult_fec_j2_ma = "Sin Captura";
            ult_hr_j2_ma = "--";

            ult_fec_j3_me = "Sin Captura";
            ult_hr_j3_me = "--";
            ult_fec_j3_ma = "Sin Captura";
            ult_hr_j3_ma = "--";

            ult_fec_ceh_me = "Sin Captura";
            ult_hr_ceh_me = "--";
            ult_fec_ceh_ma = "Sin Captura";
            ult_hr_ceh_ma = "--";

            ult_fec_iec_me = "Sin Captura";
            ult_hr_iec_me = "--";
            ult_fec_iec_ma = "Sin Captura";
            ult_hr_iec_ma = "--";
        }
        public string html_view { get; set; }
        public List<RowsUnidadesMonitoreo> rows { get; set; }

        public string ult_fec_hru_me { get; set; }
        public string ult_hr_hru_me { get; set; }
        public string ult_fec_hru_ma { get; set; }
        public string ult_hr_hru_ma { get; set; }

        public string ult_fec_hmi_me { get; set; }
        public string ult_hr_hmi_me { get; set; }
        public string ult_fec_hmi_ma { get; set; }
        public string ult_hr_hmi_ma { get; set; }

        public string ult_fec_hgi_me { get; set; }
        public string ult_hr_hgi_me { get; set; }
        public string ult_fec_hgi_ma { get; set; }
        public string ult_hr_hgi_ma { get; set; }

        public string ult_fec_hgt_me { get; set; }
        public string ult_hr_hgt_me { get; set; }
        public string ult_fec_hgt_ma { get; set; }
        public string ult_hr_hgt_ma { get; set; }

        public string ult_fec_hgm_me { get; set; }
        public string ult_hr_hgm_me { get; set; }
        public string ult_fec_hgm_ma { get; set; }
        public string ult_hr_hgm_ma { get; set; }

        public string ult_fec_j1_me { get; set; }
        public string ult_hr_j1_me { get; set; }
        public string ult_fec_j1_ma { get; set; }
        public string ult_hr_j1_ma { get; set; }

        public string ult_fec_j2_me { get; set; }
        public string ult_hr_j2_me { get; set; }
        public string ult_fec_j2_ma { get; set; }
        public string ult_hr_j2_ma { get; set; }

        public string ult_fec_j3_me { get; set; }
        public string ult_hr_j3_me { get; set; }
        public string ult_fec_j3_ma { get; set; }
        public string ult_hr_j3_ma { get; set; }

        public string ult_fec_ceh_me { get; set; }
        public string ult_hr_ceh_me { get; set; }
        public string ult_fec_ceh_ma { get; set; }
        public string ult_hr_ceh_ma { get; set; }

        public string ult_fec_iec_me { get; set; }
        public string ult_hr_iec_me { get; set; }
        public string ult_fec_iec_ma { get; set; }
        public string ult_hr_iec_ma { get; set; }

    }

    public class RowsUnidadesMonitoreo
    {
        public int id_unidad { get; set; }
        public string unidad { get; set; }
        public string ult_fech_med { get; set; }
        public string ult_hr_med { get; set; }
        public string ult_fech_cur { get; set; }
        public string ult_hr_cur { get; set; }
    }

    public class ReporteSolicitadoVSEntregado
    {
        public ReporteSolicitadoVSEntregado()
        {
        }

        public int? pk_centro { get; set; }
        public string centro { get; set; }
        public string pk_articulos { get; set; }
        public int? anio { get; set; }
        public int? mes { get; set; }
        public string clave_t { get; set; }
        public string descripcion { get; set; }
        public int? solicitado { get; set; }
        public int? surtido { get; set; }

    }

    public class DatosInsumosClass
    {
        public DatosInsumosClass()
        {
            descrip_tipo_insumo = "MEDICAMENTO";
        }
        public string pk { get; set; }
        public string cve { get; set; }
        public string descripcion { get; set; }
        public string presentacion { get; set; }
        public int existencia_almacen { get; set; }
        public int existencia_unidad { get; set; }
        public int tipo_insumo { get; set; }
        public int sub_tipo_insumo { get; set; }
        public string descrip_tipo_insumo { get; set; }
    }

    public class rows_cab
    {
        public rows_cab()
        {

        }

        public int id_row { get; set; }

    }

    //HiQpdf
    public class footer
    {
        public bool hasFooter { get; set; }
        public int height { get; set; }
        public string url { get; set; }
        public bool paginado { get; set; }

        public footer()
        {
            this.hasFooter = false;
            this.height = 100;
            this.url = HttpContext.Current.Request.Url.Authority + "/PDFUtils/Footer";
            this.paginado = true;
        }
    }

    //MESES DISPONIBLES PARA ORDINARIO
    public class mesesCls
    {
        public string MesName { get; set; }
        public int MesNumber { get; set; }
        public int estatus { get; set; }
        public string estatus_desc { get; set; }
        public string estatus_class { get; set; }
        public int anio { get; set; }
        public int tipo { get; set; }
        public bool actual { get; set; }
        public int unidad { get; set; }
        public bool controlado { get; set; }
    }

    //VALE ALMACEN CENTRAL CLASE
    public class valeAlmacen
    {
        public valeAlmacen()
        {

        }

        public string no_vale { get; set; }
        public string tipo_vale { get; set; }
        public string fecha_vale { get; set; }
        public string anio { get; set; }
        public string mes { get; set; }
        public string centro_solicitante { get; set; }
        public string direccion { get; set; }
        public string colonia { get; set; }
        public string codpost { get; set; }
        public string Municipio { get; set; }
        public string Entidad { get; set; }
        public string tel { get; set; }
        public string rfc { get; set; }
        public string programa { get; set; }
        public string cant_sol { get; set; }
        public string cant_surt { get; set; }
        public string clave { get; set; }
        public string descripcion { get; set; }
        public string presentacion { get; set; }
        public string importe { get; set; }
        public string iva { get; set; }
        public string costo_prom { get; set; }
        public string costo_insumo { get; set; }
        
        public string lote { get; set; }
        public string programa_lote { get; set; }
        public string cent_req_lote { get; set; }
        public string caducidad { get; set; }
        public string cant_lote { get; set; }
        public string costo_lote { get; set; }
        public string importe_lote { get; set; }
        public string observa { get; set; }
        public string categoria { get; set; }
    }

    public class valeResumen
    {
        public valeResumen()
        { }

        public string claves { get; set; }
        public string no_surtidas { get; set; }
        public string surtidas { get; set; }
        public string parcialmente { get; set; }
        public string completo { get; set; }
        public string p_surt { get; set; }
        public string p_no_surt { get; set; }
        public string p_par { get; set; }
        public string p_com { get; set; }
    }

    public class valeTotales
    {
        public string importe_v { get; set; }
        public string iva_v { get; set; }
        public string total_v { get; set; }
    }

}
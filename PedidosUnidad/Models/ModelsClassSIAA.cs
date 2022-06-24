using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PedidosUnidad.Models.DBConcentradora;

namespace PedidosUnidad.Models
{
    public class ModelsClassSIAA
    {

    }

    public class EntradasClassSIAA
    {
        public EntradasClassSIAA()
        {
            listadoEntradas = new List<vwListadoEntradas>();
        }

        public List<vwListadoEntradas> listadoEntradas { get; set; }
        public bool exito { get; set; }
        public string msg { get; set; }
    }

    public class SalidasClassSIAA
    {
        public SalidasClassSIAA()
        {
            listadoSalidas = new List<vwListadoSalidas>();
        }

        public List<vwListadoSalidas> listadoSalidas { get; set; }
        public bool exito { get; set; }
        public string msg { get; set; }
    }

    public class MaDeEntradasClassSIAA
    {
        public MaDeEntradasClassSIAA()
        {
            maEntradas = new ma_entradas();
            deEntradas = new List<DeEntradasClassSIAA>();
        }

        public ma_entradas maEntradas { get; set; }
        public string cvproveedor { get; set; }
        public int tipo_insumo { get; set; }
        public List<DeEntradasClassSIAA> deEntradas { get; set; }
        public bool exito { get; set; }
        public string msg { get; set; }
    }

    public class MaDeSalidasClassSIAA
    {
        public MaDeSalidasClassSIAA()
        {
            maSalidas = new ma_pedidos();
            deSalidas = new List<DeSalidasClassSIAA>();
        }

        public ma_pedidos maSalidas { get; set; }
        public List<DeSalidasClassSIAA> deSalidas { get; set; }
        public bool exito { get; set; }
        public string msg { get; set; }
    }

    public class DeEntradasClassSIAA
    {
        public DeEntradasClassSIAA()
        {
            deentradas = new de_entradas();
            deEntradasCad = new List<DE_EntradasCAD>();
        }

        public de_entradas deentradas { get; set; }
        public string insumo { get; set; }
        public string pres { get; set; }
        public List<DE_EntradasCAD> deEntradasCad { get; set; }
        public string descLarga { get; set; }
        public bool exito { get; set; }
        public string msg { get; set; }
    }

    public class DeSalidasClassSIAA
    {
        public DeSalidasClassSIAA()
        {
            desalidas = new de_pedidos();
            desalidasCad = new List<DE_PedidosCAD>();
        }

        public de_pedidos desalidas { get; set; }
        public string insumo { get; set; }
        public string pres { get; set; }
        public List<DE_PedidosCAD> desalidasCad { get; set; }
        public string descLarga { get; set; }
        public bool exito { get; set; }
        public string msg { get; set; }
    }

    public class AddInsumoForm
    {
        public AddInsumoForm()
        {
            clavesAgregadas = new List<DeEntradasClassSIAA>();
            clavesCat = new List<InsumoClassSIAA>();
        }
        public List<DeEntradasClassSIAA> clavesAgregadas { get; set; }
        public List<InsumoClassSIAA> clavesCat { get; set; }

        public string total_sin_iva { get; set; }
        public string iva_ { get; set; }
        public string total_con_iva { get; set; }

        public decimal d_total_sin_iva { get; set; }
        public decimal d_iva_ { get; set; }
        public decimal d_total_con_iva { get; set; }

        public int cantidad_claves { get; set; }
        public string tipo_insumo { get; set; }
        public int tipo_int { get; set; }
    }

    public class InsumoClassSIAA
    {
        public InsumoClassSIAA()
        {
            pk = "";
            clave_txt = "";
            clave_ = "";
            tipo = 0;
            grupo = 0;
            clave = 0;
            presentacion = 0;
            descripcion = "";
            descLarga = "";
            presentacion_txt = "";
        }

        public string pk { get; set; }
        public string clave_txt { get; set; }
        public string clave_ { get; set; }
        public int tipo { get; set; }
        public int grupo { get; set; }
        public int clave { get; set; }
        public int presentacion { get; set; }
        public string descripcion { get; set; }
        public string descLarga { get; set; }
        public string presentacion_txt { get; set; }
        public string lote { get; set; }
        public string Programa { get; set; }
        public string caducidad { get; set; }
        public int  En_proceso { get; set; }
        public string CS_Apartados { get; set; }
        public int Existencia_lote { get; set; }
        public int existencia { get; set; }

        public int id_categoria { get; set; }
        public string categoria { get; set; }

    }

    public class LotesInfoClass
    {
        public string Clave_txt { get; set; }
        public string Lote { get; set; }
        public string Caducidad { get; set; }
        public int Cant_Ent { get; set; }
        public int Cant_Sal { get; set; }        
        public int Existencia { get; set; }
        public string Programa { get; set; }
        public int En_proceso { get; set; }
    }

    public class ReturnModelClass
    {
        public bool exito { get; set; }
        public string msg { get; set; }
        public string var_1 { get; set; }
        public string var_2 { get; set; }
        public string var_3 { get; set; }
    }

}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PedidosUnidad.Models.DBConcentradora
{
    using System;
    using System.Collections.Generic;
    
    public partial class ma_entradas
    {
        public int FOLIO { get; set; }
        public int ANIO { get; set; }
        public Nullable<int> NO_ENTRADA { get; set; }
        public Nullable<int> TIPO_ENTRADA { get; set; }
        public Nullable<int> FOLIO_PEDIDO { get; set; }
        public Nullable<int> ANIO_PEDIDO { get; set; }
        public Nullable<System.DateTime> FECHA { get; set; }
        public Nullable<int> TIPO_DOCTO { get; set; }
        public string FOLIO_DOCTO { get; set; }
        public Nullable<int> TIPO_LICITACION { get; set; }
        public string FOLIO_LICITACION { get; set; }
        public Nullable<int> ANIO_LICITACION { get; set; }
        public Nullable<int> ESTADOP { get; set; }
        public Nullable<int> GIROP { get; set; }
        public Nullable<int> CLAVEP { get; set; }
        public Nullable<decimal> SUBTOTAL { get; set; }
        public Nullable<decimal> IVA { get; set; }
        public Nullable<decimal> TOTAL { get; set; }
        public string OBSERVA { get; set; }
        public bool AFECTADA { get; set; }
        public Nullable<int> CERRADO { get; set; }
        public Nullable<int> U_ACT { get; set; }
        public Nullable<System.DateTime> F_ACT { get; set; }
        public Nullable<int> ESTATUS { get; set; }
        public Nullable<int> NO_POLIZA_ENT { get; set; }
        public Nullable<int> U_CREO { get; set; }
        public Nullable<System.DateTime> F_CREO { get; set; }
        public Nullable<int> CENTRO_REQ { get; set; }
        public Nullable<int> ANIO_POLIZA_ENT { get; set; }
        public Nullable<bool> cancelado { get; set; }
        public Nullable<int> no_prog { get; set; }
        public Nullable<int> Id_FuenteFinanciamiento { get; set; }
        public Nullable<int> carta_compromiso { get; set; }
        public Nullable<System.DateTime> F_AFEC { get; set; }
        public string Motivo { get; set; }
        public Nullable<int> U_CANC { get; set; }
        public Nullable<System.DateTime> F_CANC { get; set; }
        public string H_CANC { get; set; }
        public string FOLIO_DOCTO_2 { get; set; }
        public int id_centro { get; set; }
        public Nullable<int> TIPO_INSUMOS { get; set; }
    }
}
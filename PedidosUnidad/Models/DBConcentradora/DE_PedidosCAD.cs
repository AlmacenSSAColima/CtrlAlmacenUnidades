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
    
    public partial class DE_PedidosCAD
    {
        public int Pedido { get; set; }
        public int Anio { get; set; }
        public Nullable<int> Tipo { get; set; }
        public Nullable<int> Grupo { get; set; }
        public Nullable<int> Clav_Med { get; set; }
        public string Lote { get; set; }
        public Nullable<System.DateTime> Caducidad { get; set; }
        public Nullable<int> Cantidad { get; set; }
        public Nullable<int> U_ACT { get; set; }
        public Nullable<System.DateTime> F_ACT { get; set; }
        public Nullable<bool> afectado { get; set; }
        public Nullable<int> UNI_PRES { get; set; }
        public Nullable<bool> DONACION { get; set; }
        public string pk_CADUCIDADES { get; set; }
        public string pk_articulos { get; set; }
        public Nullable<int> rack { get; set; }
        public Nullable<int> pasillo { get; set; }
        public Nullable<int> nivel { get; set; }
        public int id_centro { get; set; }
    }
}
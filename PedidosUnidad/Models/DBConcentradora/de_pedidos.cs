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
    
    public partial class de_pedidos
    {
        public int PEDIDO { get; set; }
        public int ANIO { get; set; }
        public int TIPO { get; set; }
        public int GRUPO { get; set; }
        public int CLAVE { get; set; }
        public Nullable<int> CANTIDAD { get; set; }
        public Nullable<int> CANT_SURT { get; set; }
        public decimal COSTO { get; set; }
        public Nullable<int> U_ACT { get; set; }
        public Nullable<System.DateTime> F_ACT { get; set; }
        public Nullable<int> U_CREO { get; set; }
        public Nullable<System.DateTime> F_CREO { get; set; }
        public Nullable<bool> afectado { get; set; }
        public string pk_articulos { get; set; }
        public Nullable<int> presentacion { get; set; }
        public int id_centro { get; set; }
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PedidosUnidad.Models.DBPedido
{
    using System;
    using System.Collections.Generic;
    
    public partial class det_inventario_articulo
    {
        public int id_inv { get; set; }
        public int id_unidad { get; set; }
        public int id { get; set; }
        public string pk_articulo { get; set; }
        public string cve { get; set; }
        public Nullable<int> cpm { get; set; }
        public Nullable<int> existencias { get; set; }
        public Nullable<int> entradas { get; set; }
        public string observaciones { get; set; }
        public Nullable<int> tipo { get; set; }
        public Nullable<int> tipo_mov { get; set; }
    }
}

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
    
    public partial class vwDispCad
    {
        public string pk_articulos { get; set; }
        public string pk_CADUCIDADES { get; set; }
        public string LOTE { get; set; }
        public string CADUCIDAD { get; set; }
        public Nullable<int> disponible { get; set; }
        public int id_centro { get; set; }
    }
}

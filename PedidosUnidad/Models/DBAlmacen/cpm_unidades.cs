//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PedidosUnidad.Models.DBAlmacen
{
    using System;
    using System.Collections.Generic;
    
    public partial class cpm_unidades
    {
        public string articulos_pk { get; set; }
        public int centro_pk { get; set; }
        public int cpm { get; set; }
        public int anio { get; set; }
        public Nullable<short> U_Act { get; set; }
        public Nullable<System.DateTime> F_Act { get; set; }
        public Nullable<bool> activo { get; set; }
        public string origen { get; set; }
        public Nullable<bool> consolidada { get; set; }
        public Nullable<bool> dimesa { get; set; }
        public Nullable<bool> desierta { get; set; }
        public Nullable<bool> programa { get; set; }
        public string desc_programa { get; set; }
        public Nullable<bool> otra_fuente { get; set; }
        public string desc_otra_fuente { get; set; }
        public Nullable<bool> controlado { get; set; }
        public Nullable<int> max_consolidada { get; set; }
        public Nullable<int> min_consolidada { get; set; }
        public Nullable<int> max_dimesa { get; set; }
        public Nullable<int> min_dimesa { get; set; }
    }
}

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
    
    public partial class modules_temp
    {
        public int id_modules_temp { get; set; }
        public int idmodules { get; set; }
        public string codemodules { get; set; }
        public string namemodules { get; set; }
        public int parentmenuid { get; set; }
        public int ordernumber { get; set; }
        public string menuurl { get; set; }
        public string menuicon { get; set; }
        public string menuaction { get; set; }
        public string menucontrolador { get; set; }
        public Nullable<int> users_idusers { get; set; }
        public Nullable<bool> create_permission { get; set; }
        public Nullable<bool> read_permission { get; set; }
        public Nullable<bool> update_permission { get; set; }
        public Nullable<bool> delete_permission { get; set; }
        public Nullable<int> hijos { get; set; }
    }
}

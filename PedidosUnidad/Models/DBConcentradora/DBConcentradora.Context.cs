﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PedidosUnidad.Models.DBConcentradora
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBCONCENTRADORA : DbContext
    {
        public DBCONCENTRADORA()
            : base("name=DBCONCENTRADORA")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DE_CADUCIDAD> DE_CADUCIDAD { get; set; }
        public virtual DbSet<de_entradas> de_entradas { get; set; }
        public virtual DbSet<DE_EntradasCAD> DE_EntradasCAD { get; set; }
        public virtual DbSet<de_pedidos> de_pedidos { get; set; }
        public virtual DbSet<DE_PedidosCAD> DE_PedidosCAD { get; set; }
        public virtual DbSet<ma_pedidos> ma_pedidos { get; set; }
        public virtual DbSet<vwListadoSalidas> vwListadoSalidas { get; set; }
        public virtual DbSet<TA_Parametros> TA_Parametros { get; set; }
        public virtual DbSet<ma_entradas> ma_entradas { get; set; }
        public virtual DbSet<vwListadoEntradas> vwListadoEntradas { get; set; }
        public virtual DbSet<vwDispCad> vwDispCad { get; set; }
    }
}

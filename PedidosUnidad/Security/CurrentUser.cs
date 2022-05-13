using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PedidosUnidad.Security
{
    public class CurrentUser
    {
        public CurrentUser()
        {
            id_user = -1;
            name_user = "NoLogin";
            login = false;
        }
        public int id_user { get; set; }
        public int id_unidad { get; set; }
        public string name_user { get; set; }
        public string full_name { get; set; }
        public string nom_unidad { get; set; }
        public int id_rol { get; set; }
        public string rol { get; set; }
        public bool login { get; set; }
        public int tipo { get; set; }
        public string navegador { get; set; }
        public string unidad_select { get; set; }
    }
}
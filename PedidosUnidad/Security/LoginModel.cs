using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PedidosUnidad.Security
{
    public class LoginModel
    {
        public LoginModel()
        {

        }

        public int idusers { get; set; }

        [DisplayName("Usuario")]
        [Required(ErrorMessage = "Usuario requerido")]
        public string loginusers { get; set; }

        [DisplayName("Contraseña")]
        [Required(ErrorMessage = "Contraseña requerida")]
        [DataType(DataType.Password)]
        public string passusers { get; set; }

        public string LoginErrorMessage { get; set; }

        public string navegador { get; set; }
    }
}
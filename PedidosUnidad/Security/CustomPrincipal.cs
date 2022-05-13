using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace PedidosUnidad.Security
{
    public class CustomPrincipal : IPrincipal
    {
        private CurrentUser Account;

        public CustomPrincipal(CurrentUser account)
        {
            this.Account = account;
            this.Identity = new GenericIdentity(account.name_user);
        }

        public IIdentity Identity { get; set; }

        //CHECAR SI TIEN EL ROL
        public bool IsInRole(string role)
        {
            var roles = role.Split(new char[] { ',' });
            return roles.Any(r => this.Account.rol.Contains(r));
        }
    }
}
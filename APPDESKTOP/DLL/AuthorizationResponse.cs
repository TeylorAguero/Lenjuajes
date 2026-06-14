using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL
{
    public class AuthorizationResponse
    {
        public string Token { get; set; }

        public bool Result { get; set; }

        public string Msj { get; set; }
    }
}

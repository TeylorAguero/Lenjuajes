using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Usuario
    {
        public int Id { get; set; }


        public string loginn { get; set; }


        public string passwordd { get; set; }


        public DateTime fechaRegistro { get; set; }


        public bool estado { get; set; }
    }
}

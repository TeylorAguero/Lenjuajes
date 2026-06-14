using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebSecurityCloud.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }


        public string loginn { get; set; }


        public string passwordd { get; set; }


        public DateTime fechaRegistro { get; set; }


        public bool estado { get; set; }
    }
}


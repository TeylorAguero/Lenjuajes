using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiApp.Models
{
    [Table("Facturas")]
    public class Factura
    {
        [Key]
        public int Id { get; set; }

        public string numero { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public int codCliente { get; set; }

        public decimal Subtotal { get; set; }

        public decimal MontoDescuento { get; set; }

        public decimal MontoImpuesto { get; set; }

        public decimal Total { get; set; }

        public string estado { get; set; } = string.Empty;

        public string Usuario { get; set; } = string.Empty;

        public string TipoPago { get; set; } = string.Empty;

        public string Condicion { get; set; } = string.Empty;
    }
}
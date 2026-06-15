using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiApp.Models
{
    [Table("Det_Facturas")]
    public class DetalleFactura
    {
        [Key]
        public int Id { get; set; }

        public string numFacturas { get; set; } = string.Empty;

        public string codInterno { get; set; } = string.Empty;

        public int cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }

        public decimal PorDescuento { get; set; }

        public decimal PorImp { get; set; }
    }
}
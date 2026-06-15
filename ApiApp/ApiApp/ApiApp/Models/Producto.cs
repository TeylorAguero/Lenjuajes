using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiApp.Models
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CodigoInterno { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? CodigoBarras { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descripcion { get; set; } = string.Empty;

        [Column(TypeName = "decimal(12,2)")]
        public decimal PrecioVenta { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Descuento { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Impuesto { get; set; }

        [Required]
        [MaxLength(20)]
        public string UnidadMedia { get; set; } = "Unidad";

        [Column(TypeName = "decimal(12,2)")]
        public decimal PrecioCompra { get; set; }

        [Required]
        [MaxLength(50)]
        public string Usuario { get; set; } = string.Empty;

        public int Existencia { get; set; }
    }
}
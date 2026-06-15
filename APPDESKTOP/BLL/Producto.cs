using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Producto
    {
        public int Id { get; set; }
        public string CodigoInterno { get; set; }
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public string UnidadMedia { get; set; }
        public decimal PrecioCompra { get; set; }
        public string Usuario { get; set; }
        public int Existencia { get; set; }
    }
}

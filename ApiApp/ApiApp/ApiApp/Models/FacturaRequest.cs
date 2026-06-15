namespace ApiApp.Models
{
    public class FacturaRequest
    {
        public Factura Factura { get; set; }

        public List<DetalleFactura> Detalles { get; set; }
    }
}
using ApiApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FacturaController : ControllerBase
    {
        private readonly BigFoodDbContext _context;

        public FacturaController(BigFoodDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            var facturas = await _context.Facturas
                .OrderByDescending(f => f.Fecha)
                .ToListAsync();

            return Ok(facturas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> GetFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);

            if (factura == null)
            {
                return NotFound(new
                {
                    mensaje = "Factura no encontrada"
                });
            }

            return Ok(factura);
        }

        [HttpPost("Guardar")]
        public async Task<IActionResult> GuardarFactura([FromBody] FacturaRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (request == null ||
                    request.Factura == null ||
                    request.Detalles == null ||
                    !request.Detalles.Any())
                {
                    return BadRequest(new
                    {
                        mensaje = "La factura debe contener detalles"
                    });
                }

                request.Factura.Fecha = DateTime.Now;

                _context.Facturas.Add(request.Factura);

                await _context.SaveChangesAsync();

                foreach (var detalle in request.Detalles)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(x =>
                            x.CodigoInterno == detalle.codInterno);

                    if (producto == null)
                    {
                        throw new Exception(
                            $"No existe el producto {detalle.codInterno}");
                    }

                    if (producto.Existencia < detalle.cantidad)
                    {
                        throw new Exception(
                            $"Existencia insuficiente para {producto.Descripcion}");
                    }

                    detalle.numFacturas = request.Factura.numero;

                    _context.Det_Facturas.Add(detalle);

                    producto.Existencia -= detalle.cantidad;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "Factura guardada correctamente",
                    numeroFactura = request.Factura.numero
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [HttpGet("Detalle/{numeroFactura}")]
        public async Task<ActionResult<IEnumerable<DetalleFactura>>> GetDetalleFactura(string numeroFactura)
        {
            var detalles = await _context.Det_Facturas
                .Where(x => x.numFacturas == numeroFactura)
                .ToListAsync();

            return Ok(detalles);
        }
    }
}
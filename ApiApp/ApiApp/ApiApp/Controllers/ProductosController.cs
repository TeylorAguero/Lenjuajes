using ApiApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly BigFoodDbContext _context;

        public ProductosController(BigFoodDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _context.Productos
                .OrderBy(p => p.Descripcion)
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el producto"
                });
            }

            return Ok(producto);
        }

        [HttpGet("codigo/{codigoInterno}")]
        public async Task<ActionResult<Producto>> GetProductoPorCodigo(string codigoInterno)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.CodigoInterno == codigoInterno);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el producto"
                });
            }

            return Ok(producto);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarProductos([FromQuery] string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return BadRequest(new
                {
                    mensaje = "Debe ingresar un texto para buscar"
                });
            }

            var productos = await _context.Productos
                .Where(p =>
                    p.Descripcion.Contains(texto) ||
                    p.CodigoInterno.Contains(texto) ||
                    (p.CodigoBarras != null && p.CodigoBarras.Contains(texto)))
                .OrderBy(p => p.Descripcion)
                .ToListAsync();

            return Ok(productos);
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> CrearProducto([FromBody] Producto producto)
        {
            if (producto == null)
            {
                return BadRequest(new
                {
                    mensaje = "Los datos del producto son obligatorios"
                });
            }

            var existeCodigo = await _context.Productos
                .AnyAsync(p => p.CodigoInterno == producto.CodigoInterno);

            if (existeCodigo)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe un producto con ese código interno"
                });
            }

            if (!string.IsNullOrWhiteSpace(producto.CodigoBarras))
            {
                var existeBarra = await _context.Productos
                    .AnyAsync(p => p.CodigoBarras == producto.CodigoBarras);

                if (existeBarra)
                {
                    return BadRequest(new
                    {
                        mensaje = "Ya existe un producto con ese código de barras"
                    });
                }
            }

            if (producto.PrecioVenta < 0)
            {
                return BadRequest(new
                {
                    mensaje = "El precio de venta no puede ser negativo"
                });
            }

            if (producto.PrecioCompra < 0)
            {
                return BadRequest(new
                {
                    mensaje = "El precio de compra no puede ser negativo"
                });
            }

            if (producto.Descuento < 0 || producto.Descuento > 100)
            {
                return BadRequest(new
                {
                    mensaje = "El descuento debe estar entre 0 y 100"
                });
            }

            if (producto.Impuesto < 0 || producto.Impuesto > 100)
            {
                return BadRequest(new
                {
                    mensaje = "El impuesto debe estar entre 0 y 100"
                });
            }

            if (producto.Existencia < 0)
            {
                return BadRequest(new
                {
                    mensaje = "La existencia no puede ser negativa"
                });
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest(new
                {
                    mensaje = "El ID del producto no coincide"
                });
            }

            var productoActual = await _context.Productos.FindAsync(id);

            if (productoActual == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el producto"
                });
            }

            productoActual.CodigoInterno = producto.CodigoInterno;
            productoActual.CodigoBarras = producto.CodigoBarras;
            productoActual.Descripcion = producto.Descripcion;
            productoActual.PrecioVenta = producto.PrecioVenta;
            productoActual.Descuento = producto.Descuento;
            productoActual.Impuesto = producto.Impuesto;
            productoActual.UnidadMedia = producto.UnidadMedia;
            productoActual.PrecioCompra = producto.PrecioCompra;
            productoActual.Usuario = producto.Usuario;
            productoActual.Existencia = producto.Existencia;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Producto actualizado correctamente"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el producto"
                });
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Producto eliminado correctamente"
            });
        }

        [HttpPut("descontar-existencia/{codigoInterno}")]
        public async Task<IActionResult> DescontarExistencia(string codigoInterno, [FromQuery] int cantidad)
        {
            if (cantidad <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "La cantidad debe ser mayor a cero"
                });
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.CodigoInterno == codigoInterno);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el producto"
                });
            }

            if (producto.Existencia < cantidad)
            {
                return BadRequest(new
                {
                    mensaje = "No hay suficiente existencia"
                });
            }

            producto.Existencia -= cantidad;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Existencia actualizada correctamente",
                producto = producto.Descripcion,
                existenciaActual = producto.Existencia
            });
        }
    }
}
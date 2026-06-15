using BLL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DLL
{
    public class ProductoServices
    {
        private readonly HttpClient client;

        public ProductoServices(string token)
        {
            client = new HttpClient();

            // Si tu API usa otro puerto, cambiá este 7070 por el puerto que sale en Swagger.
            client.BaseAddress = new Uri("https://localhost:7070/");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<Producto>> ObtenerProductos()
        {
            HttpResponseMessage response = await client.GetAsync("api/Productos");

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception("No se pudieron cargar los productos. " + error);
            }

            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Producto>>(json) ?? new List<Producto>();
        }

        public async Task<List<Producto>> BuscarProductos(string texto)
        {
            string textoSeguro = Uri.EscapeDataString(texto);

            HttpResponseMessage response = await client.GetAsync("api/Productos/buscar?texto=" + textoSeguro);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception("No se pudieron buscar los productos. " + error);
            }

            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Producto>>(json) ?? new List<Producto>();
        }

        public async Task GuardarProducto(Producto producto)
        {
            string json = JsonConvert.SerializeObject(producto);

            StringContent content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await client.PostAsync("api/Productos", content);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception("No se pudo guardar el producto. " + error);
            }
        }

        public async Task ModificarProducto(Producto producto)
        {
            string json = JsonConvert.SerializeObject(producto);

            StringContent content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await client.PutAsync("api/Productos/" + producto.Id, content);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception("No se pudo modificar el producto. " + error);
            }
        }

        public async Task EliminarProducto(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync("api/Productos/" + id);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception("No se pudo eliminar el producto. " + error);
            }
        }

        public async Task DescontarExistencia(string codigoInterno, int cantidad)
        {
            string codigoSeguro = Uri.EscapeDataString(codigoInterno);

            HttpResponseMessage response = await client.PutAsync(
                "api/Productos/descontar-existencia/" + codigoSeguro + "?cantidad=" + cantidad,
                null
            );

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception("No se pudo descontar la existencia. " + error);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DLL
{
    public class APIServices
    {
        public HttpClient StartAPISecurity()
        {
            //Objeto para implementar protocolo Http
            HttpClient client = new HttpClient();

            //se indica la direccion web donde esta publicada la API
            client.BaseAddress = new Uri("http://www.ApiSecurityBigF1.somee.com");

            //se retorna la instancia dl cliente http
            return client;
        }
    }
}

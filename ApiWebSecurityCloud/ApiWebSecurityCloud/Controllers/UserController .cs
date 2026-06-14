using ApiWebSecurityCloud.Models;
using ApiWebSecurityCloud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiWebSecurityCloud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthorizationServices _authorizationServices;
        private readonly DbContextSecurity dbContext;

        public UserController(IAuthorizationServices pAuthorizationServices, DbContextSecurity pContext)
        {
            this._authorizationServices = pAuthorizationServices;
            this.dbContext = pContext;
        }

        // EndPoint para autenticar usuario y obtener token
        [HttpPost]
        [Route("Authentication")]
        public AuthorizationResponse Authentication(LoginRequest pRequest)
        {
            return this._authorizationServices.DevolverToken(pRequest);
        }

        // EndPoint para listar todos los usuarios
        [HttpGet]
        [Route("List")]
        [Authorize]
        public List<Usuario> List()
        {
            return this.dbContext.Usuarios.ToList();
        }

        // EndPoint para guardar un nuevo usuario
        [HttpPut]
        [Route("Save")]
        [Authorize]
        public string Save(Usuario temp)
        {
            string msj = "";
            try
            {
                if (temp == null)
                {
                    msj = "No se permiten datos en blanco.";
                }
                else
                {
                    // Verificar si el login ya existe
                    var existe = this.dbContext.Usuarios.FirstOrDefault(x => x.loginn == temp.loginn);
                    if (existe != null)
                    {
                        msj = $"El login {temp.loginn} ya existe, no se puede duplicar.";
                    }
                    else
                    {
                        // Se establece la fecha de registro
                        temp.fechaRegistro = DateTime.Now;
                        // Se agrega el usuario
                        this.dbContext.Usuarios.Add(temp);
                        // Se aplican los cambios
                        this.dbContext.SaveChanges();
                        msj = $"Usuario {temp.loginn} almacenado correctamente.";
                    }
                }
            }
            catch (Exception ex)
            {
                msj = $"Error al guardar, {ex.InnerException.Message}...";
            }
            return msj;
        }

        // EndPoint para modificar los datos del usuario
        [HttpPost]
        [Route("Update")]
        [Authorize]
        public string Update(Usuario temp)
        {
            string msj = "";
            try
            {
                if (temp == null)
                {
                    msj = "No se permite datos vacios";
                }
                else
                {
                    var usuarioActual = this.dbContext.Usuarios.FirstOrDefault(x => x.Id == temp.Id);

                    if (usuarioActual != null)
                    {
                        // Verificar si el nuevo login ya existe en otro usuario
                        var existeLogin = this.dbContext.Usuarios.FirstOrDefault(x => x.loginn == temp.loginn && x.Id != temp.Id);
                        if (existeLogin != null)
                        {
                            msj = $"El login {temp.loginn} ya está siendo usado por otro usuario";
                        }
                        else
                        {
                            // Se actualizan los datos
                            usuarioActual.loginn = temp.loginn;
                            usuarioActual.passwordd = temp.passwordd;
                            usuarioActual.estado = temp.estado;
                            // No se modifica fechaRegistro porque es la fecha de creación

                            this.dbContext.Usuarios.Update(usuarioActual);
                            this.dbContext.SaveChanges();
                            msj = $"Usuario {temp.loginn} datos actualizados correctamente";
                        }
                    }
                    else
                    {
                        msj = $"No existe usuario con el id {temp.Id}";
                    }
                }
            }
            catch (Exception ex)
            {
                msj = $"Error al modificar, {ex.InnerException.Message}";
            }
            return msj;
        }

        //Eliminado Logico
        [HttpPut]
        [Route("Delete/{id}")]
        [Authorize]
        public string Deactivate(int id)
        {
            string msj = "";
            try
            {
                var temp = this.dbContext.Usuarios.FirstOrDefault(x => x.Id == id);

                if (temp != null)
                {
                    temp.estado = false;
                    this.dbContext.Usuarios.Update(temp);
                    this.dbContext.SaveChanges();
                    msj = $"Usuario {temp.loginn} ha sido desactivado";
                }
                else
                {
                    msj = $"No existe ningún usuario con el id {id}";
                }
            }
            catch (Exception ex)
            {
                msj = $"Error al desactivar, {ex.InnerException.Message}...";
            }
            return msj;
        }

        // EndPoint para consultar un usuario por su ID
        [HttpGet]
        [Route("Search/{id}")]
        [Authorize]
        public Usuario Search(int id)
        {
            Usuario temp = new Usuario() { Id = id, loginn = "No existe..." };
            try
            {
                var aux = this.dbContext.Usuarios.FirstOrDefault(x => x.Id == id);
                if (aux != null)
                {
                    temp = aux;
                }
            }
            catch (Exception ex)
            {
                temp.loginn = $"Error, {ex.InnerException.Message}";
            }
            return temp;
        }
    }
}

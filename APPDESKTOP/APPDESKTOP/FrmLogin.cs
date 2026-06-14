using BLL;
using DLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DLL;
using BLL;
using Newtonsoft.Json;

namespace APPDESKTOP
{
    public partial class FrmLogin : Form
    {

        //Variable estática para almacenar el usuario actual
        public static string UsuarioActual = "";
        public static string token = "";

        private bool authenticated = false;

        //variable client
        private HttpClient client = null;

        //Variable para referenciar api services
        private readonly APIServices services = null;
        public FrmLogin()
        {
            InitializeComponent();

            //se instancia el services
            this.services = new APIServices();

            //se inicia el servicio
            this.client = this.services.StartAPISecurity();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                await this.AuthenticationUser(this.txtUsuario.Text.Trim(),
                    this.txtContrasena.Text.Trim());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task AuthenticationUser(string pUsuario, string pContrasena)
        {
            try
            {   //preguntando si el email esta en blanco
                if (pUsuario.Trim().Equals(""))
                {
                    throw new Exception("No se permite el usuario en blanco....");
                }
                //preguntando si el password está en blanco
                if (pContrasena.Trim().Equals(""))
                {
                    throw new Exception("No se permite el password en blanco....");
                }

                //Una variable para almacenar el token
                AuthorizationResponse authorization = null;

                //variable para almacenar las credenciales para enviar al endPont
                Usuario temp = new Usuario();

                //se almacenan los valores para las credenciales 
                temp.loginn = pUsuario;
                temp.passwordd = pContrasena;

                this.client.DefaultRequestHeaders.Authorization = null;

                //Se convierte el objeto en formato json
                string datajson = JsonConvert.SerializeObject(temp);


                StringContent contenido = new StringContent(

                    datajson, Encoding.UTF8, "application/json"
                    );

                HttpResponseMessage response = await
                        this.client.PostAsync("/User/Authentication", contenido);

                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();

                        authorization = JsonConvert.DeserializeObject<AuthorizationResponse>(result);
                    }

                }

                //se valida si la autorizacion tiene datos 
                if (authorization != null)
                {//Se valida si el resultado es ok
                    if (authorization.Result)
                    {
                        //se insica que la autenticacion es correcta
                        this.authenticated = true;

                        // Guardar el nombre del usuario que inició sesión
                        FrmLogin.UsuarioActual = pUsuario;

                        //Se guarda el token
                        FrmLogin.token = authorization.Token;

                        //se cierra el formulario
                        this.Hide();

                        FrmMain frmMain = new FrmMain();
                        frmMain.ShowDialog();
                        frmMain.Dispose();
                    }
                    else  //La authentication faild 
                    {
                        this.authenticated = false;
                        MessageBox.Show("Usuario o password incorrecto, " +
                            "por favor verificar las credenciales", "Autenticación", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                }
                else  //La authentication faild 
                {
                    this.authenticated = false;
                    MessageBox.Show("Usuario o password incorrecto, " +
                        "por favor verificar las credenciales", "Autenticación", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void lblRegistrarse_Click(object sender, EventArgs e)
        {
            //Oculta el formulario Login
            this.Hide();

            //Muestra el formulario Register
            FrmRegistrarseLogin frmRegistrarseLogin = new FrmRegistrarseLogin();
            frmRegistrarseLogin.ShowDialog();

            //se libera la memoria
            frmRegistrarseLogin.Dispose();
        }
    }
}

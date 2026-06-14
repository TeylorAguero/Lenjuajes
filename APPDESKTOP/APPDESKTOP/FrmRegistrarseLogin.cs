using BLL;
using DLL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APPDESKTOP
{
    public partial class FrmRegistrarseLogin : Form
    {
        private HttpClient client = null;
        private readonly APIServices services = null;
        private string token = "";

        // Variables para modo edición
        private bool esModoEdicion = false;
        private int usuarioIdEditar = 0;

        public FrmRegistrarseLogin()
        {
            InitializeComponent();
            this.services = new APIServices();
            this.client = this.services.StartAPISecurity();
            this.grbTitulo.Text = "Registrar Usuario"; // Título del GroupBox

            // Configurar cbEstado
            cbEstado.Items.Clear();
            cbEstado.Items.Add("Activo");
            cbEstado.Items.Add("Inactivo");
            cbEstado.SelectedIndex = 0;

            _ = ObtenerTokenPorDefecto();
        }

        /// <summary>
        /// Constructor para modo edición
        /// </summary>
        public FrmRegistrarseLogin(int id, string login, string password, bool estado)
        {
            InitializeComponent();
            this.services = new APIServices();
            this.client = this.services.StartAPISecurity();

            // Configurar modo edición
            this.esModoEdicion = true;
            this.usuarioIdEditar = id;
            this.grbTitulo.Text = "Editar Usuario"; // Cambiar título del GroupBox
            this.btnAccptar.Text = "Actualizar"; // Cambiar texto del botón

            // Configurar cbEstado
            cbEstado.Items.Clear();
            cbEstado.Items.Add("Activo");
            cbEstado.Items.Add("Inactivo");

            // Cargar datos del usuario
            txtUsuario.Text = login;
            txtContrasena.Text = password;
            cbEstado.SelectedIndex = estado ? 0 : 1;

            _ = ObtenerTokenPorDefecto();
        }

        private async Task ObtenerTokenPorDefecto()
        {
            try
            {
                var loginRequest = new { loginn = "admin", passwordd = "admin123" };
                string datajson = JsonConvert.SerializeObject(loginRequest);
                StringContent contenido = new StringContent(datajson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await this.client.PostAsync("/User/Authentication", contenido);

                if (response != null && response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonConvert.DeserializeObject<AuthorizationResponse>(result);

                    if (authResponse != null && authResponse.Result)
                    {
                        this.token = authResponse.Token;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener token: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<bool> VerificarSiUsuarioExiste(string login)
        {
            try
            {
                this.client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.token);

                HttpResponseMessage response = await this.client.GetAsync("/User/List");

                if (response != null && response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(result);

                    // Si es modo edición, excluir el usuario actual de la validación
                    if (esModoEdicion)
                    {
                        return usuarios.Any(x => x.loginn == login && x.Id != this.usuarioIdEditar);
                    }

                    return usuarios.Any(x => x.loginn == login);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnAccptar_Click(object sender, EventArgs e)
        {
            if (esModoEdicion)
            {
                await ActualizarUsuario();
            }
            else
            {
                await RegistrarUsuario();
            }
        }

        private async Task RegistrarUsuario()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsuario.Text))
                {
                    MessageBox.Show("No se permite el usuario en blanco...", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtContrasena.Text))
                {
                    MessageBox.Show("No se permite la contraseña en blanco...", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (txtContrasena.Text.Length < 4)
                {
                    MessageBox.Show("La contraseña debe tener al menos 4 caracteres...", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool usuarioExiste = await VerificarSiUsuarioExiste(txtUsuario.Text.Trim());

                if (usuarioExiste)
                {
                    MessageBox.Show($"El usuario '{txtUsuario.Text.Trim()}' ya existe en el sistema.",
                        "Usuario Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsuario.Focus();
                    txtUsuario.SelectAll();
                    return;
                }

                Usuario nuevoUsuario = new Usuario();
                nuevoUsuario.loginn = txtUsuario.Text.Trim();
                nuevoUsuario.passwordd = txtContrasena.Text.Trim();
                nuevoUsuario.estado = (cbEstado.SelectedItem.ToString() == "Activo");

                this.client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.token);

                string datajson = JsonConvert.SerializeObject(nuevoUsuario);
                StringContent contenido = new StringContent(datajson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await this.client.PutAsync("/User/Save", contenido);

                if (response != null && response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    MessageBox.Show($"Usuario {nuevoUsuario.loginn} registrado exitosamente.\n{result}",
                        "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtUsuario.Clear();
                    txtContrasena.Clear();
                    cbEstado.SelectedIndex = 0;
                    txtUsuario.Focus();

                    this.Hide();

                    FrmLogin frmLogin = new FrmLogin();
                    frmLogin.ShowDialog();
                    frmLogin.Dispose();
                }
                else if (response != null)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al registrar: {error}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("No se recibió respuesta del servidor", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ActualizarUsuario()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsuario.Text))
                {
                    MessageBox.Show("No se permite el usuario en blanco...", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtContrasena.Text))
                {
                    MessageBox.Show("No se permite la contraseña en blanco...", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Verificar si el usuario ya existe (excluyendo el actual)
                bool usuarioExiste = await VerificarSiUsuarioExiste(txtUsuario.Text.Trim());

                if (usuarioExiste)
                {
                    MessageBox.Show($"El usuario '{txtUsuario.Text.Trim()}' ya existe en el sistema.",
                        "Usuario Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsuario.Focus();
                    txtUsuario.SelectAll();
                    return;
                }

                // Crear objeto con los datos actualizados
                Usuario usuarioActualizado = new Usuario();
                usuarioActualizado.Id = this.usuarioIdEditar;
                usuarioActualizado.loginn = txtUsuario.Text.Trim();
                usuarioActualizado.passwordd = txtContrasena.Text.Trim();
                usuarioActualizado.estado = (cbEstado.SelectedItem.ToString() == "Activo");

                this.client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.token);

                string datajson = JsonConvert.SerializeObject(usuarioActualizado);
                StringContent contenido = new StringContent(datajson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await this.client.PostAsync("/User/Update", contenido);

                if (response != null && response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    MessageBox.Show($"Usuario {usuarioActualizado.loginn} actualizado exitosamente.\n{result}",
                        "Actualización Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                }
                else if (response != null)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al actualizar: {error}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("No se recibió respuesta del servidor", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
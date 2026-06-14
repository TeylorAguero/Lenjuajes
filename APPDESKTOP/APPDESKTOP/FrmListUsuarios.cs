using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;
using DLL;
using BLL;
using Newtonsoft.Json;

namespace APPDESKTOP
{
    public partial class FrmListUsuarios : Form
    {
        // Variable para manejar el servicio API
        private readonly APIServices services = null;

        // Variable para implementar el protocolo HttpClient
        private readonly HttpClient client = null;

        // Timer para búsqueda automática
        private Timer searchTimer = null;

        /// <summary>
        /// Constructor por omisión del form
        /// </summary>
        public FrmListUsuarios()
        {
            InitializeComponent();

            // Se instancia el servicio API
            this.services = new APIServices();

            // Se inicia el consumo de API Security
            this.client = this.services.StartAPISecurity();

            // Configurar el timer para búsqueda automática
            ConfigurarTimer();

            // Configurar columnas
            ConfigurarColumnas();

            // Cargar la lista de usuarios al iniciar
            _ = CargarListaUsuarios();

            // Configurar evento del txtID
            this.txtID.TextChanged += TxtID_TextChanged;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Configurar el timer para búsqueda automática
        /// </summary>
        private void ConfigurarTimer()
        {
            searchTimer = new Timer();
            searchTimer.Interval = 500; // 500 milisegundos de espera
            searchTimer.Tick += SearchTimer_Tick;
        }

        /// <summary>
        /// Evento del timer - Ejecuta la búsqueda después de que el usuario deja de escribir
        /// </summary>
        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            searchTimer.Stop();
            this.BuscarUsuarioPorId();
        }

        /// <summary>
        /// Evento cuando el texto del txtID cambia
        /// </summary>
        private void TxtID_TextChanged(object sender, EventArgs e)
        {
            // Reiniciar el timer cada vez que se escribe
            searchTimer.Stop();
            searchTimer.Start();
        }

        /// <summary>
        /// Configurar las columnas del DataGridView
        /// </summary>
        private void ConfigurarColumnas()
        {
            // Limpiar columnas existentes
            dgvUsuarios.Columns.Clear();

            // Agregar columnas al DataGridView
            dgvUsuarios.Columns.Add("Id", "ID");
            dgvUsuarios.Columns.Add("loginn", "Usuario");
            dgvUsuarios.Columns.Add("passwordd", "Contraseña");
            dgvUsuarios.Columns.Add("estado", "Estado");
            dgvUsuarios.Columns.Add("fechaRegistro", "Fecha Registro");

            // Configurar el ancho de las columnas
            dgvUsuarios.Columns["Id"].Width = 50;
            dgvUsuarios.Columns["loginn"].Width = 120;
            dgvUsuarios.Columns["passwordd"].Width = 100;
            dgvUsuarios.Columns["estado"].Width = 80;
            dgvUsuarios.Columns["fechaRegistro"].Width = 130;

            // Configurar propiedades del DataGridView
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.RowHeadersVisible = false;
        }

        /// <summary>
        /// Cargar la lista de usuarios desde la API
        /// </summary>
        private async System.Threading.Tasks.Task CargarListaUsuarios()
        {
            try
            {
                // Agregar el token al header
                this.client.DefaultRequestHeaders.Authorization = this.AuthorizationToken();

                // Realizar petición GET para obtener todos los usuarios
                HttpResponseMessage response = await this.client.GetAsync("/User/List");

                if (response != null && response.IsSuccessStatusCode)
                {
                    // Leer la respuesta
                    string result = await response.Content.ReadAsStringAsync();

                    // Deserializar la lista de usuarios
                    List<Usuario> usuarios = JsonConvert.DeserializeObject<List<Usuario>>(result);

                    // Limpiar las filas existentes
                    dgvUsuarios.Rows.Clear();

                    // Agregar cada usuario al DataGridView
                    foreach (Usuario user in usuarios)
                    {
                        // Convertir el estado booleano a texto
                        string estadoTexto = user.estado ? "Activo" : "Inactivo";

                        // Agregar fila al DataGridView
                        dgvUsuarios.Rows.Add(
                            user.Id,
                            user.loginn,
                            user.passwordd,
                            estadoTexto,
                            user.fechaRegistro.ToString("dd/MM/yyyy HH:mm")
                        );
                    }
                }
                else if (response != null)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al cargar usuarios: {error}", "Error",
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
                MessageBox.Show($"Error al cargar lista de usuarios: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Buscar usuario por ID
        /// </summary>
        private async void BuscarUsuarioPorId()
        {
            try
            {
                // Validar si el campo está vacío
                if (string.IsNullOrWhiteSpace(txtID.Text))
                {
                    // Si está vacío, cargar todos los usuarios
                    await CargarListaUsuarios();
                    return;
                }

                // Validar que sea un número válido
                if (!int.TryParse(txtID.Text.Trim(), out int id))
                {
                    return; // No es número, no hacer nada
                }

                // Agregar el token al header
                this.client.DefaultRequestHeaders.Authorization = this.AuthorizationToken();

                // Realizar petición GET para buscar usuario por ID
                HttpResponseMessage response = await this.client.GetAsync($"/User/Search/{id}");

                if (response != null && response.IsSuccessStatusCode)
                {
                    // Leer la respuesta
                    string result = await response.Content.ReadAsStringAsync();

                    // Deserializar el usuario
                    Usuario user = JsonConvert.DeserializeObject<Usuario>(result);

                    // Verificar si el usuario existe
                    if (user != null && user.loginn != "No existe...")
                    {
                        // Limpiar las filas existentes
                        dgvUsuarios.Rows.Clear();

                        // Convertir el estado booleano a texto
                        string estadoTexto = user.estado ? "Activo" : "Inactivo";

                        // Agregar el usuario encontrado al DataGridView
                        dgvUsuarios.Rows.Add(
                            user.Id,
                            user.loginn,
                            user.passwordd,
                            estadoTexto,
                            user.fechaRegistro.ToString("dd/MM/yyyy HH:mm")
                        );
                    }
                    else
                    {
                        // Si no existe, recargar todos
                        await CargarListaUsuarios();
                    }
                }
                else
                {
                    await CargarListaUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar usuario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Método encargado de obtener el token
        /// </summary>
        private AuthenticationHeaderValue AuthorizationToken()
        {
            try
            {
                // Se toma el token de autenticación
                string token = FrmLogin.token;

                // Variable para crear el token de autenticación
                AuthenticationHeaderValue authentication = null;

                if (token != null) // Se valida si hay token
                {
                    // Se instancia el token de autenticación
                    authentication = new AuthenticationHeaderValue("Bearer", token);
                }

                // Se retorna la autenticación
                return authentication;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evento del menú contextual para eliminar (desactivar) usuario
        /// </summary>
        private async void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar que haya una fila seleccionada
                if (dgvUsuarios.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar un usuario para eliminar.", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener el ID y nombre del usuario seleccionado
                int usuarioId = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["Id"].Value);
                string usuarioLogin = dgvUsuarios.SelectedRows[0].Cells["loginn"].Value.ToString();
                string estadoActual = dgvUsuarios.SelectedRows[0].Cells["estado"].Value.ToString();

                // Verificar si el usuario ya está inactivo
                if (estadoActual == "Inactivo")
                {
                    MessageBox.Show($"El usuario '{usuarioLogin}' ya está inactivo.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Confirmar eliminación lógica
                DialogResult confirm = MessageBox.Show($"¿Está seguro de desactivar al usuario '{usuarioLogin}'?",
                    "Confirmar Desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;

                    this.client.DefaultRequestHeaders.Authorization = this.AuthorizationToken();

                    HttpResponseMessage response = await this.client.PutAsync($"/User/Delete/{usuarioId}", null);

                    if (response != null && response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Usuario '{usuarioLogin}' ha sido desactivado exitosamente.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Limpiar el campo de búsqueda
                        txtID.Clear();

                        // Recargar la lista de usuarios
                        await CargarListaUsuarios();
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al desactivar usuario: {error}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar usuario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Evento del menú contextual para editar usuario
        /// </summary>
        private void editarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Validación que tenga seleccionada una fila
                if (this.dgvUsuarios.SelectedRows.Count <= 0)
                {
                    MessageBox.Show("Seleccione la fila del usuario a modificar...", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener datos del usuario seleccionado
                int usuarioId = Convert.ToInt32(this.dgvUsuarios.SelectedRows[0].Cells["Id"].Value);
                string usuarioLogin = this.dgvUsuarios.SelectedRows[0].Cells["loginn"].Value.ToString();
                string usuarioPassword = this.dgvUsuarios.SelectedRows[0].Cells["passwordd"].Value.ToString();
                string usuarioEstado = this.dgvUsuarios.SelectedRows[0].Cells["estado"].Value.ToString();

                // Convertir estado texto a booleano
                bool estadoBool = (usuarioEstado == "Activo");

                // Abrir formulario de edición con los datos del usuario
                FrmRegistrarseLogin frmEditar = new FrmRegistrarseLogin(usuarioId, usuarioLogin, usuarioPassword, estadoBool);
                frmEditar.ShowDialog();
                frmEditar.Dispose();

                // Recargar la lista después de editar
                _ = CargarListaUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar usuario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento del DataGridView MouseClick para mostrar menú contextual
        /// </summary>
        private void dgvUsuarios_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    var hitTest = dgvUsuarios.HitTest(e.X, e.Y);

                    if (hitTest.RowIndex >= 0)
                    {
                        dgvUsuarios.ClearSelection();
                        dgvUsuarios.Rows[hitTest.RowIndex].Selected = true;
                        contextMenuStrip1.Show(dgvUsuarios, e.Location);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvUsuarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // No se requiere acción
        }
    }
}
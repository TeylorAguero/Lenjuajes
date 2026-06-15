using BLL;
using DLL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APPDESKTOP
{
    public partial class FrmProductos : Form
    {
        private ProductoServices productoServices;
        private string token;

        public FrmProductos()
        {
            InitializeComponent();
        }

        public FrmProductos(string tokenRecibido)
        {
            InitializeComponent();

            token = tokenRecibido;
            productoServices = new ProductoServices(token);
        }

        private async void FrmProductos_Load(object sender, EventArgs e)
        {
            ConfigurarTabla();

            if (productoServices == null)
            {
                MessageBox.Show("No se recibió el token para cargar productos.",
                    "Token no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await CargarProductos();
        }

        private void ConfigurarTabla()
        {
            dgvProductos.ReadOnly = true;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
        }

        private async Task CargarProductos()
        {
            try
            {
                dgvProductos.DataSource = null;

                List<Producto> productos = await productoServices.ObtenerProductos();

                dgvProductos.DataSource = productos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private async void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (productoServices == null)
                {
                    return;
                }

                string texto = txtBuscar.Text.Trim();

                if (string.IsNullOrWhiteSpace(texto))
                {
                    await CargarProductos();
                    return;
                }

                List<Producto> productos = await productoServices.BuscarProductos(texto);

                dgvProductos.DataSource = null;
                dgvProductos.DataSource = productos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar productos: " + ex.Message);
            }
        }

        private async void btnCrear_Click(object sender, EventArgs e)
        {
            FrmCrearProducto frm = new FrmCrearProducto(token);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                await CargarProductos();
            }

            frm.Dispose();
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (productoServices == null)
                {
                    MessageBox.Show("No se recibió el token para eliminar productos.",
                        "Token no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvProductos.CurrentRow == null)
                {
                    MessageBox.Show("Debe seleccionar un producto para eliminar.",
                        "Eliminar producto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Producto productoSeleccionado = dgvProductos.CurrentRow.DataBoundItem as Producto;

                if (productoSeleccionado == null)
                {
                    MessageBox.Show("No se pudo obtener el producto seleccionado.",
                        "Eliminar producto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    "¿Seguro que desea eliminar el producto: " + productoSeleccionado.Descripcion + "?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (respuesta == DialogResult.No)
                {
                    return;
                }

                await productoServices.EliminarProducto(productoSeleccionado.Id);

                MessageBox.Show("Producto eliminado correctamente.",
                    "Eliminar producto", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar producto: " + ex.Message);
            }
        }
    }
}
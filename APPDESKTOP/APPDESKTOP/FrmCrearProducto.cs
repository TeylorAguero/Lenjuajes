using BLL;
using DLL;
using System;
using System.Windows.Forms;

namespace APPDESKTOP
{
    public partial class FrmCrearProducto : Form
    {
        private ProductoServices productoServices;

        public FrmCrearProducto()
        {
            InitializeComponent();
        }

        public FrmCrearProducto(string token) : this()
        {
            productoServices = new ProductoServices(token);
        }

        private void FrmCrearProducto_Load(object sender, EventArgs e)
        {
            txtDescuento.Text = "0";
            txtImpuesto.Text = "13";
            txtUnidad.Text = "Unidad";
            txtUsuario.Text = "admin";
            txtCodigoInterno.Focus();
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (productoServices == null)
                {
                    MessageBox.Show("No se recibió el token para consumir la API.");
                    return;
                }

                if (!ValidarCampos())
                {
                    return;
                }

                Producto producto = new Producto
                {
                    Id = 0,
                    CodigoInterno = txtCodigoInterno.Text.Trim(),
                    CodigoBarras = txtCodigoBarras.Text.Trim(),
                    Descripcion = txtDescripcion.Text.Trim(),
                    PrecioVenta = Convert.ToDecimal(txtPrecioVenta.Text.Trim()),
                    Descuento = Convert.ToDecimal(txtDescuento.Text.Trim()),
                    Impuesto = Convert.ToDecimal(txtImpuesto.Text.Trim()),
                    UnidadMedia = txtUnidad.Text.Trim(),
                    PrecioCompra = Convert.ToDecimal(txtPrecioCompra.Text.Trim()),
                    Usuario = txtUsuario.Text.Trim(),
                    Existencia = Convert.ToInt32(txtExistencia.Text.Trim())
                };

                await productoServices.GuardarProducto(producto);

                MessageBox.Show("Producto guardado correctamente.");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el producto: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtCodigoInterno.Text))
            {
                MessageBox.Show("Debe ingresar el código interno.");
                txtCodigoInterno.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Debe ingresar la descripción del producto.");
                txtDescripcion.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioVenta.Text))
            {
                MessageBox.Show("Debe ingresar el precio de venta.");
                txtPrecioVenta.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrecioVenta.Text.Trim(), out decimal precioVenta))
            {
                MessageBox.Show("El precio de venta debe ser un número válido.");
                txtPrecioVenta.Focus();
                return false;
            }

            if (precioVenta < 0)
            {
                MessageBox.Show("El precio de venta no puede ser negativo.");
                txtPrecioVenta.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescuento.Text))
            {
                txtDescuento.Text = "0";
            }

            if (!decimal.TryParse(txtDescuento.Text.Trim(), out decimal descuento))
            {
                MessageBox.Show("El descuento debe ser un número válido.");
                txtDescuento.Focus();
                return false;
            }

            if (descuento < 0 || descuento > 100)
            {
                MessageBox.Show("El descuento debe estar entre 0 y 100.");
                txtDescuento.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtImpuesto.Text))
            {
                txtImpuesto.Text = "13";
            }

            if (!decimal.TryParse(txtImpuesto.Text.Trim(), out decimal impuesto))
            {
                MessageBox.Show("El impuesto debe ser un número válido.");
                txtImpuesto.Focus();
                return false;
            }

            if (impuesto < 0 || impuesto > 100)
            {
                MessageBox.Show("El impuesto debe estar entre 0 y 100.");
                txtImpuesto.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUnidad.Text))
            {
                MessageBox.Show("Debe ingresar la unidad del producto.");
                txtUnidad.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioCompra.Text))
            {
                MessageBox.Show("Debe ingresar el precio de compra.");
                txtPrecioCompra.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrecioCompra.Text.Trim(), out decimal precioCompra))
            {
                MessageBox.Show("El precio de compra debe ser un número válido.");
                txtPrecioCompra.Focus();
                return false;
            }

            if (precioCompra < 0)
            {
                MessageBox.Show("El precio de compra no puede ser negativo.");
                txtPrecioCompra.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("Debe ingresar el usuario.");
                txtUsuario.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtExistencia.Text))
            {
                MessageBox.Show("Debe ingresar la existencia.");
                txtExistencia.Focus();
                return false;
            }

            if (!int.TryParse(txtExistencia.Text.Trim(), out int existencia))
            {
                MessageBox.Show("La existencia debe ser un número entero.");
                txtExistencia.Focus();
                return false;
            }

            if (existencia < 0)
            {
                MessageBox.Show("La existencia no puede ser negativa.");
                txtExistencia.Focus();
                return false;
            }

            return true;
        }

        private void txtCodigoInterno_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCodigoBarras_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPrecioVenta_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDescuento_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtImpuesto_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPrecioCompra_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtExistencia_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUnidad_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DLL;
using BLL;
using Newtonsoft.Json;
namespace APPDESKTOP
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void archivoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Validar que solo el usuario "admin" pueda ver la lista de usuarios
            if (FrmLogin.UsuarioActual == "admin")
            {
                // Mostrar el formulario de lista de usuarios
                FrmListUsuarios frmLista = new FrmListUsuarios();
                frmLista.ShowDialog();
                frmLista.Dispose();
            }
            else
            {
                // Mostrar mensaje de acceso denegado
                MessageBox.Show("Acceso denegado. Solo el Administrador puede ver la lista de usuarios.",
                    "Permiso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

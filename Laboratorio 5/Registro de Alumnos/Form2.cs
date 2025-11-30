using Registro_de_Alumnos.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registro_de_Alumnos
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            CargarPDFs();
        }

        //Formulario de abrir los reportes por medio de sumatra externo

        //Boton Abrir
        private void btnAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                if (lsbListaReportes.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione un PDF de la lista.");
                    return;
                }

                string archivoSeleccionado = lsbListaReportes.SelectedItem.ToString();

                string carpeta = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "MisReportes"
                );

                string rutaPDF = Path.Combine(carpeta, archivoSeleccionado);

                // Llamamos a la clase VerPDFSumatra
                VerPDFSumatra.AbrirConSumatra(rutaPDF);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el archivo:\n" + ex.Message);
            }
        }


        //Metodo de cargar los PDF desde la carpeta
        private void CargarPDFs()
        {
            try
            {
                lsbListaReportes.Items.Clear();

                string carpeta = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "MisReportes"
                );

                if (!Directory.Exists(carpeta))
                {
                    MessageBox.Show("La carpeta 'MisReportes' no existe. Aún no hay reportes generados.");
                    return;
                }

                string[] archivos = Directory.GetFiles(carpeta, "*.pdf");

                if (archivos.Length == 0)
                {
                    MessageBox.Show("No hay archivos PDF en la carpeta MisReportes.");
                    return;
                }

                foreach (var archivo in archivos)
                {
                    lsbListaReportes.Items.Add(Path.GetFileName(archivo));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando archivos PDF:\n" + ex.Message);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

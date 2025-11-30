using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Registro_de_Alumnos.Clases
{
    public class VerPDFSumatra
    {
        // Busca SumatraPDF en rutas comunes de mi computadora
        public static string BuscarSumatra()
        {
            try
            {
                string[] posiblesRutas =
                {
                @"C:\Program Files\SumatraPDF\SumatraPDF.exe",
                @"C:\Users\FLIA PEREZ\AppData\Local\SumatraPDF\SumatraPDF.exe",
                @"C:\SumatraPDF\SumatraPDF.exe",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SumatraPDF.exe"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SumatraPDF.exe"),
            };

                foreach (var ruta in posiblesRutas)
                {
                    if (File.Exists(ruta))
                        return ruta;
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error buscando SumatraPDF:\n" + ex.Message);
                return null;
            }
        }

        // Abre un PDF en SumatraPDF
        public static void AbrirConSumatra(string rutaPDF)
        {
            try
            {
                // Validar PDF
                if (string.IsNullOrWhiteSpace(rutaPDF) || !File.Exists(rutaPDF))
                {
                    MessageBox.Show("El archivo PDF no existe o la ruta es inválida.");
                    return;
                }

                // Buscar Sumatra
                string sumatra = BuscarSumatra();

                if (string.IsNullOrEmpty(sumatra))
                {
                    MessageBox.Show("SumatraPDF no fue encontrado.\n" +
                                    "Colócalo junto al .exe o instálalo.");
                    return;
                }

                // Iniciar SumatraPDF sin bloquear tu programa
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = sumatra,
                    Arguments = $"\"{rutaPDF}\"",
                    UseShellExecute = false
                };

                Process.Start(psi);
            }
            catch (Win32Exception)
            {
                MessageBox.Show("No se pudo iniciar SumatraPDF. Verifique permisos o la instalación.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al abrir el PDF:\n" + ex.Message);
            }
        }
    }
}

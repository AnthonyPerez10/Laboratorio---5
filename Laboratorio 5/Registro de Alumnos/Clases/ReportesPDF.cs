using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registro_de_Alumnos.Clases
{
    public class ReportesPDF
    {
        private static string Ruta(string nombre)
        {
            //Se extrae la carpeta de resultados de espcial 
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), nombre + ".pdf");
        }

        //Reporte General de alumnos
        public static void ReporteGeneral(List<Alumno> lista)
        {
            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(Ruta("Reporte_General"), FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph("REPORTE GENERAL DE ALUMNOS\nFecha: " +
                DateTime.Now.ToShortDateString() + "\n\n"));

            PdfPTable tabla = new PdfPTable(6);
            tabla.AddCell("ID");
            tabla.AddCell("Nombre");
            tabla.AddCell("Apellido");
            tabla.AddCell("Carrera");
            tabla.AddCell("Jornada");
            tabla.AddCell("Fecha Registro");

            foreach (var a in lista)
            {
                tabla.AddCell(a.Id.ToString());
                tabla.AddCell(a.Nombre);
                tabla.AddCell(a.Apellido);
                tabla.AddCell(a.Carrera);
                tabla.AddCell(a.Jornada);
                tabla.AddCell(a.FechaRegistro.ToShortDateString());
            }

            doc.Add(tabla);
            doc.Close();
        }

       //Reporte por carreras 
        public static void ReportePorCarrera(List<Alumno> lista, string carrera)
        {
            var filtrado = lista.Where(x => x.Carrera == carrera).ToList();

            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(Ruta("Reporte_Carrera_" + carrera), FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph($"REPORTE POR CARRERA\nCarrera: {carrera}\nFecha: {DateTime.Now.ToShortDateString()}\n\n"));

            PdfPTable tabla = new PdfPTable(4);
            tabla.AddCell("ID");
            tabla.AddCell("Nombre");
            tabla.AddCell("Apellido");
            tabla.AddCell("Semestre");

            foreach (var a in filtrado)
            {
                tabla.AddCell(a.Id.ToString());
                tabla.AddCell(a.Nombre);
                tabla.AddCell(a.Apellido);
                tabla.AddCell(a.Semestre);
            }

            doc.Add(tabla);
            doc.Close();
        }

        // Reportes por jornada 
        public static void ReportePorJornada(List<Alumno> lista, string jornada)
        {
            var filtrado = lista.Where(x => x.Jornada == jornada).ToList();

            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(Ruta("Reporte_Jornada_" + jornada), FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph($"REPORTE POR JORNADA\nJornada: {jornada}\nFecha: {DateTime.Now.ToShortDateString()}\n\n"));

            PdfPTable tabla = new PdfPTable(4);
            tabla.AddCell("ID");
            tabla.AddCell("Nombre");
            tabla.AddCell("Apellido");
            tabla.AddCell("Carrera");

            foreach (var a in filtrado)
            {
                tabla.AddCell(a.Id.ToString());
                tabla.AddCell(a.Nombre);
                tabla.AddCell(a.Apellido);
                tabla.AddCell(a.Carrera);
            }

            doc.Add(tabla);
            doc.Close();
        }

        // Reportes por rango de fecha 
        public static void ReporteRango(List<Alumno> lista, DateTime desde, DateTime hasta)
        {
            var filtrado = lista
                .Where(x => x.FechaRegistro >= desde && x.FechaRegistro <= hasta)
                .ToList();

            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(Ruta("Reporte_Rango"), FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph($"REPORTE EN RANGO DE FECHAS\nDesde: {desde.ToShortDateString()} - Hasta: {hasta.ToShortDateString()}\n\n"));

            PdfPTable tabla = new PdfPTable(4);
            tabla.AddCell("ID");
            tabla.AddCell("Nombre");
            tabla.AddCell("Apellido");
            tabla.AddCell("Fecha Registro");

            foreach (var a in filtrado)
            {
                tabla.AddCell(a.Id.ToString());
                tabla.AddCell(a.Nombre);
                tabla.AddCell(a.Apellido);
                tabla.AddCell(a.FechaRegistro.ToShortDateString());
            }

            doc.Add(tabla);
            doc.Close();
        }

        // Reportes por perfil de alumno
        public static void ReportePerfil(Alumno a)
        {
            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(Ruta("Perfil_" + a.Nombre), FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph("PERFIL DEL ALUMNO\n\n"));

            doc.Add(new Paragraph($"ID: {a.Id}"));
            doc.Add(new Paragraph($"Nombre: {a.Nombre} {a.Apellido}"));
            doc.Add(new Paragraph($"Cedula: {a.Cedula}"));
            doc.Add(new Paragraph($"Carrera: {a.Carrera}"));
            doc.Add(new Paragraph($"Semestre: {a.Semestre}"));
            doc.Add(new Paragraph($"Jornada: {a.Jornada}"));
            doc.Add(new Paragraph($"Usuario: {a.Usuario}"));
            doc.Add(new Paragraph($"Recibe Notificaciones: {(a.RecibirNotificaciones ? "Sí" : "No")}"));
            doc.Add(new Paragraph($"Fecha Registro: {a.FechaRegistro}"));

            doc.Close();
        }
    }
}

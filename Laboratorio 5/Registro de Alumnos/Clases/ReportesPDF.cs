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

    // Evento para agregar pie de página con número de página
    public class PiePagina : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfPTable footer = new PdfPTable(1);
            footer.TotalWidth = document.PageSize.Width - 40;

            footer.AddCell(new PdfPCell(new Phrase("Página " + writer.PageNumber))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });

            footer.WriteSelectedRows(0, -1, 20, 30, writer.DirectContent);
        }
    }

    //Clase que genera los reportes 
    public class ReportesPDF
    {
        private static string Ruta(string nombre)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), nombre + ".pdf");
        }

        // Reporte general de alumnos
        public static void ReporteGeneral(List<Alumno> lista)
        {
            lista = lista.OrderBy(x => x.FechaRegistro).ToList();

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Ruta("Reporte_General"), FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            doc.Add(new Paragraph("REPORTE GENERAL DE ALUMNOS"));
            doc.Add(new Paragraph("Fecha de emisión: " + DateTime.Now.ToShortDateString() + "\n\n"));

            PdfPTable tabla = new PdfPTable(8);
            tabla.AddCell("ID");
            tabla.AddCell("Nombre");
            tabla.AddCell("Apellido");
            tabla.AddCell("Carrera");
            tabla.AddCell("Semestre");
            tabla.AddCell("Jornada");
            tabla.AddCell("Usuario");
            tabla.AddCell("Fecha Registro");

            foreach (var a in lista)
            {
                tabla.AddCell(a.Id.ToString());
                tabla.AddCell(a.Nombre);
                tabla.AddCell(a.Apellido);
                tabla.AddCell(a.Carrera);
                tabla.AddCell(a.Semestre);
                tabla.AddCell(a.Jornada);
                tabla.AddCell(a.Usuario);
                tabla.AddCell(a.FechaRegistro.ToShortDateString());
            }

            doc.Add(tabla);
            doc.Close();
        }

        // Reporte por carrera
        public static void ReportePorCarrera(List<Alumno> lista, string carrera)
        {
            var filtrado = lista
                .Where(x => x.Carrera == carrera)
                .OrderBy(x => x.Nombre)
                .ToList();

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Ruta("Reporte_Carrera_" + carrera), FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            doc.Add(new Paragraph("REPORTE POR CARRERA"));
            doc.Add(new Paragraph("Carrera: " + carrera));
            doc.Add(new Paragraph("Fecha de emisión: " + DateTime.Now.ToShortDateString() + "\n\n"));

            PdfPTable tabla = new PdfPTable(5);
            tabla.AddCell("ID");
            tabla.AddCell("Nombre");
            tabla.AddCell("Apellido");
            tabla.AddCell("Semestre");
            tabla.AddCell("Fecha Registro");

            foreach (var a in filtrado)
            {
                tabla.AddCell(a.Id.ToString());
                tabla.AddCell(a.Nombre);
                tabla.AddCell(a.Apellido);
                tabla.AddCell(a.Semestre);
                tabla.AddCell(a.FechaRegistro.ToShortDateString());
            }

            doc.Add(tabla);
            doc.Close();
        }

        // Reporte por jornada
        public static void ReportePorJornada(List<Alumno> lista, string jornada)
        {
            var filtrado = lista
                .Where(x => x.Jornada == jornada)
                .OrderBy(x => x.Apellido)
                .ToList();

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Ruta("Reporte_Jornada_" + jornada), FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            doc.Add(new Paragraph("REPORTE POR JORNADA"));
            doc.Add(new Paragraph("Jornada: " + jornada));
            doc.Add(new Paragraph("Fecha de emisión: " + DateTime.Now.ToShortDateString() + "\n\n"));

            PdfPTable tabla = new PdfPTable(5);
            tabla.AddCell("ID");
            tabla.AddCell("Nombre");
            tabla.AddCell("Apellido");
            tabla.AddCell("Carrera");
            tabla.AddCell("Fecha Registro");

            foreach (var a in filtrado)
            {
                tabla.AddCell(a.Id.ToString());
                tabla.AddCell(a.Nombre);
                tabla.AddCell(a.Apellido);
                tabla.AddCell(a.Carrera);
                tabla.AddCell(a.FechaRegistro.ToShortDateString());
            }

            doc.Add(tabla);
            doc.Close();
        }

        // Reporte en rango de fechas
        public static void ReporteRango(List<Alumno> lista, DateTime desde, DateTime hasta)
        {
            var filtrado = lista
                .Where(x => x.FechaRegistro >= desde && x.FechaRegistro <= hasta)
                .OrderBy(x => x.FechaRegistro)
                .ToList();

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Ruta("Reporte_Rango"), FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            doc.Add(new Paragraph("REPORTE EN RANGO DE FECHAS"));
            doc.Add(new Paragraph($"Desde: {desde.ToShortDateString()} - Hasta: {hasta.ToShortDateString()}"));
            doc.Add(new Paragraph("Fecha de emisión: " + DateTime.Now.ToShortDateString() + "\n\n"));

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

        // Reporte de perfil de alumno
        public static void ReportePerfil(Alumno a)
        {
            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Ruta("Perfil_" + a.Nombre), FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            doc.Add(new Paragraph("PERFIL DEL ALUMNO"));
            doc.Add(new Paragraph("Fecha de emisión: " + DateTime.Now.ToShortDateString() + "\n\n"));

            doc.Add(new Paragraph($"ID: {a.Id}"));
            doc.Add(new Paragraph($"Nombre: {a.Nombre} {a.Apellido}"));
            doc.Add(new Paragraph($"Cédula: {a.Cedula}"));
            doc.Add(new Paragraph($"Carrera: {a.Carrera}"));
            doc.Add(new Paragraph($"Semestre: {a.Semestre}"));
            doc.Add(new Paragraph($"Jornada: {a.Jornada}"));
            doc.Add(new Paragraph($"Usuario: {a.Usuario}"));
            doc.Add(new Paragraph($"Recibe Notificaciones: {(a.RecibirNotificaciones ? "Sí" : "No")}"));
            doc.Add(new Paragraph($"Fecha Registro: {a.FechaRegistro.ToShortDateString()}"));

            doc.Close();
        }
    }
}

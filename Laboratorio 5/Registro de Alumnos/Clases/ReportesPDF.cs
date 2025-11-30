using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Registro_de_Alumnos.Clases
{
    // Pie de página con número de página
    public class PiePagina : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            PdfPTable footer = new PdfPTable(1) { TotalWidth = doc.PageSize.Width - 40 };
            PdfPCell celda = new PdfPCell(new Phrase("Página " + writer.PageNumber));
            celda.Border = Rectangle.NO_BORDER;
            celda.HorizontalAlignment = Element.ALIGN_RIGHT;
            footer.AddCell(celda);
            footer.WriteSelectedRows(0, -1, 20, 30, writer.DirectContent);
        }
    }

    public class ReportesPDF
    {
        // Carpeta donde se guardan los PDF
        private static string Ruta(string nombre)
        {
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MisReportes");
            Directory.CreateDirectory(carpeta);
            return Path.Combine(carpeta, nombre + ".pdf");
        }

        // Crear tabla con encabezado estilizado
        private static PdfPTable CrearTablaCabecera(string[] headers)
        {
            PdfPTable tabla = new PdfPTable(headers.Length) { WidthPercentage = 100 };
            foreach (var h in headers)
            {
                PdfPCell celda = new PdfPCell(new Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE)))
                {
                    BackgroundColor = BaseColor.DARK_GRAY,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                tabla.AddCell(celda);
            }
            return tabla;
        }

        // Agregar filas con estilo alternado (blanco y gris claro)
        private static void AgregarFila(PdfPTable tabla, int indiceFila, params string[] valores)
        {
            BaseColor colorFondo = (indiceFila % 2 == 0) ? BaseColor.WHITE : new BaseColor(230, 230, 230); // alternar filas
            foreach (var val in valores)
            {
                PdfPCell celda = new PdfPCell(new Phrase(val, FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5,
                    BackgroundColor = colorFondo
                };
                tabla.AddCell(celda);
            }
        }

        // Agregar título grande azul
        private static void AgregarTitulo(Document doc, string texto)
        {
            Paragraph titulo = new Paragraph(texto, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLUE));
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 15;
            doc.Add(titulo);
        }

        // Agregar fecha
        private static void AgregarFecha(Document doc)
        {
            Paragraph fecha = new Paragraph("Fecha de emisión: " + DateTime.Now.ToShortDateString(),
                FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10, BaseColor.DARK_GRAY));
            fecha.Alignment = Element.ALIGN_RIGHT;
            fecha.SpacingAfter = 10;
            doc.Add(fecha);
        }

        // ------------------- REPORTES -------------------

        // Reporte General 
        public static string ReporteGeneral(List<Alumno> lista)
        {
            lista = lista.OrderBy(x => x.FechaRegistro).ToList();
            string ruta = Ruta("Reporte_General");

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(ruta, FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            AgregarTitulo(doc, "REPORTE GENERAL DE ALUMNOS");
            AgregarFecha(doc);

            string[] headers = { "ID", "Nombre", "Apellido", "Carrera", "Semestre", "Jornada", "Usuario", "Fecha Registro" };
            PdfPTable tabla = CrearTablaCabecera(headers);

            int i = 0;
            foreach (var a in lista)
            {
                AgregarFila(tabla, i,
                    a.Id.ToString(),
                    a.Nombre,
                    a.Apellido,
                    a.Carrera,
                    a.Semestre,
                    a.Jornada,
                    a.Usuario,
                    a.FechaRegistro.ToShortDateString()
                );
                i++;
            }

            doc.Add(tabla);
            doc.Close();
            return ruta;
        }

        //Reporte por carrera
        public static string ReportePorCarrera(List<Alumno> lista, string carrera)
        {
            var filtrado = lista.Where(x => x.Carrera == carrera).OrderBy(x => x.Nombre).ToList();
            string ruta = Ruta("Reporte_Carrera_" + carrera);

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(ruta, FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            AgregarTitulo(doc, "REPORTE POR CARRERA");
            AgregarFecha(doc);
            doc.Add(new Paragraph("Carrera: " + carrera + "\n\n"));

            string[] headers = { "ID", "Nombre", "Apellido", "Semestre", "Fecha Registro" };
            PdfPTable tabla = CrearTablaCabecera(headers);

            int i = 0;
            foreach (var a in filtrado)
            {
                AgregarFila(tabla, i,
                    a.Id.ToString(),
                    a.Nombre,
                    a.Apellido,
                    a.Semestre,
                    a.FechaRegistro.ToShortDateString()
                );
                i++;
            }

            doc.Add(tabla);
            doc.Close();
            return ruta;
        }

        //Reporte Por Jornada 
        public static string ReportePorJornada(List<Alumno> lista, string jornada)
        {
            var filtrado = lista.Where(x => x.Jornada == jornada).OrderBy(x => x.Apellido).ToList();
            string ruta = Ruta("Reporte_Jornada_" + jornada);

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(ruta, FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            AgregarTitulo(doc, "REPORTE POR JORNADA");
            AgregarFecha(doc);
            doc.Add(new Paragraph("Jornada: " + jornada + "\n\n"));

            string[] headers = { "ID", "Nombre", "Apellido", "Carrera", "Fecha Registro" };
            PdfPTable tabla = CrearTablaCabecera(headers);

            int i = 0;
            foreach (var a in filtrado)
            {
                AgregarFila(tabla, i,
                    a.Id.ToString(),
                    a.Nombre,
                    a.Apellido,
                    a.Carrera,
                    a.FechaRegistro.ToShortDateString()
                );
                i++;
            }

            doc.Add(tabla);
            doc.Close();
            return ruta;
        }

        //Reporte por rango de fecha
        public static string ReporteRango(List<Alumno> lista, DateTime desde, DateTime hasta)
        {
            var filtrado = lista.Where(x => x.FechaRegistro >= desde && x.FechaRegistro <= hasta)
                                .OrderBy(x => x.FechaRegistro).ToList();
            string ruta = Ruta("Reporte_Rango");

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(ruta, FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            AgregarTitulo(doc, "REPORTE EN RANGO DE FECHAS");
            AgregarFecha(doc);
            doc.Add(new Paragraph($"Desde: {desde.ToShortDateString()} - Hasta: {hasta.ToShortDateString()}\n\n"));

            string[] headers = { "ID", "Nombre", "Apellido", "Fecha Registro" };
            PdfPTable tabla = CrearTablaCabecera(headers);

            int i = 0;
            foreach (var a in filtrado)
            {
                AgregarFila(tabla, i,
                    a.Id.ToString(),
                    a.Nombre,
                    a.Apellido,
                    a.FechaRegistro.ToShortDateString()
                );
                i++;
            }

            doc.Add(tabla);
            doc.Close();
            return ruta;
        }

        //Reporte por perfil de alumno
        public static string ReportePerfil(Alumno a)
        {
            string ruta = Ruta("Perfil_" + a.Nombre);

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(ruta, FileMode.Create));
            writer.PageEvent = new PiePagina();
            doc.Open();

            AgregarTitulo(doc, "PERFIL DEL ALUMNO");
            AgregarFecha(doc);

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
            return ruta;
        }
    }
}

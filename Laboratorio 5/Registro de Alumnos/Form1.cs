using Microsoft.VisualBasic;
using Registro_de_Alumnos.Clases;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;   



namespace Registro_de_Alumnos
{
    public partial class Form1 : Form
    {

        //ID: 1 y 11
        private CRUD repo = new CRUD();
        private List<Alumno> alumnosActuales = new List<Alumno>();
        private int idSeleccionado = 0;

        public Form1()
        {
            InitializeComponent();

            this.KeyPreview = true;
            // Configuración de contraseña
            txtContraseña.PasswordChar = '*';
            txtConfirmar.PasswordChar = '*';
            txtContraseña.MaxLength = 12;
            txtConfirmar.MaxLength = 12;

            // Usuario solo lectura
            txtUsuario.ReadOnly = true;

            // Eventos
            txtIngresarNombre.TextChanged += GenerarUsuario;
            txtIngresarCedula.TextChanged += GenerarUsuario;
            txtIngresarCedula.KeyPress += SoloNumeros;
            txtIngresarNombre.KeyPress += SoloLetras;
            txtInsertarApellido.KeyPress += SoloLetras;
        }

        //Formulario principal
        private void Form1_Load(object sender, EventArgs e)
        {
            //Mensaje de ingreso
            string codigoIngreso = Interaction.InputBox(
                "Ingrese el codigo de profesor: ",
                "Acceso rapido",
                ""
                );

            //Verificar y declarar un codigo predefinido
            if (codigoIngreso != "12345678")
            {
                Interaction.MsgBox("Codigo incorrecto. El programa se cerrara.",
                    MsgBoxStyle.Critical, "Acceso denegado");
                this.Close();
                return;
            }

            this.KeyPreview = true;
            this.KeyDown += AtajosDelTeclado;

            //Llenado del comboBox con datos de la carreras
            string[] carreras = {
                "Seleccione una carrera",
                "Ingeniería en Sistemas",
                "Ingeniería Industrial",
                "Administración",
                "Contabilidad"
            };

            cmbCarrera.Items.AddRange(carreras);
            cmbCarrera.SelectedIndex = 0;
            cmbCarrera.DropDownStyle = ComboBoxStyle.DropDownList;

            lstListaAlumnos.ScrollAlwaysVisible = true; // Scroll vertical siempre visible
            lstListaAlumnos.HorizontalScrollbar = true;  // Scroll horizontal si el texto es largo
            lstListaAlumnos.MultiColumn = false;         // Para que sea una sola columna vertical
            CargarAlumnosEnListBox();


            //Activacion del desplazamiento con la flechas en textBox
            this.KeyPreview = true;
        }

        // Boton nuevo: Solo limpia las credenciales para agregar un nuevo alumno
        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCredenciales();
        }

        //Boton Guardar datos de nuevo estudiante
        private void BtnGuardar_Click(object sender, EventArgs e)
        {

            //Validando el Nombre
            if (string.IsNullOrWhiteSpace(txtIngresarNombre.Text))
            {
                Interaction.MsgBox("El campo Nombre no puede estar vacío.",
                    MsgBoxStyle.Exclamation, "Validación");
                txtIngresarNombre.Focus();
                return;
            }

            //Validar apellido
            if (string.IsNullOrWhiteSpace(txtInsertarApellido.Text))
            {
                Interaction.MsgBox("El campo Apellido no puede estar vacío.",
                    MsgBoxStyle.Exclamation, "Validación");
                txtIngresarNombre.Focus();
                return;
            }

            //Validar cedula
            if (string.IsNullOrWhiteSpace(txtIngresarCedula.Text))
            {
                Interaction.MsgBox("El campo Cédula no puede estar vacío.",
                    MsgBoxStyle.Exclamation, "Validación");
                txtIngresarCedula.Focus();
                return;
            }

            //Validando la selecion en combo de carrera 
            if (cmbCarrera.SelectedIndex == 0)
            {
                Interaction.MsgBox("Debe seleccionar una carrera.",
                    MsgBoxStyle.Exclamation, "Validación");
                return;
            }

            // Validacion de Jornadas
            string jornadaSeleccionada = "";
            if (rdoJornada1.Checked) jornadaSeleccionada = "Matutina";
            else if (rdoJornada2.Checked) jornadaSeleccionada = "Vespertina";
            else
            {
                Interaction.MsgBox("Debe seleccionar una jornada.",
                    MsgBoxStyle.Exclamation, "Validación");
                return;
            }

            
            // Validación de semestre
            string semestreSeleccionado = "";
            if (rdoSemestre1.Checked)
                semestreSeleccionado = "Primer Semestre";
            else if (rdoSemestre2.Checked)
                semestreSeleccionado = "Segundo Semestre";
            else
            {
                Interaction.MsgBox("Debe seleccionar un semestre.",
                    MsgBoxStyle.Exclamation, "Validación");
                return;
            }

            // --- Validar si la cédula ya existe en la BD ---
            if (repo.CedulaExiste(txtIngresarCedula.Text.Trim()))
            {
                Interaction.MsgBox(
                    "Esta cédula ya está registrada. Ingrese una diferente.",
                    MsgBoxStyle.Exclamation,
                    "Duplicado"
                );
                txtIngresarCedula.Focus();
                return;
            }

            //Validar credenciales
            if (!ValidarCredenciales())
                return;                                  

            // Crear objeto Alumno con los datos del formulario
            Alumno a = new Alumno
            {
                Nombre = txtIngresarNombre.Text.Trim(),
                Apellido = txtInsertarApellido.Text.Trim(),
                Cedula = txtIngresarCedula.Text.Trim(),
                Carrera = cmbCarrera.Text,
                Semestre = semestreSeleccionado,
                Jornada = jornadaSeleccionada,
                Usuario = txtUsuario.Text.Trim(),
                Contrasena = txtContraseña.Text,
                RecibirNotificaciones = chkNotificaciones.Checked
            };

            int nuevoId;

            try
            {
                nuevoId = repo.Insertar(a);
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(
                    "Error al guardar en la base de datos:\n" + ex.Message,
                    MsgBoxStyle.Critical,
                    "Error");
                return;
            }

            Interaction.MsgBox($"Alumno registrado con ID: {nuevoId}.",
                MsgBoxStyle.Information, "Éxito");


            // Recargar lista desde la BD
            CargarAlumnosEnListBox();

            LimpiarCredenciales(); //Limpiamos despues de guardar
        }

        //Metodo para cargar los alumnos en ListBox desde la base de datos
        private void CargarAlumnosEnListBox()
        {
            lstListaAlumnos.Items.Clear();
            alumnosActuales = repo.ObtenerTodos();

            foreach (var a in alumnosActuales)
            {
                lstListaAlumnos.Items.Add ($"{a.Id} - {a.Nombre} {a.Apellido} - {a.Cedula} - {a.Carrera} - {a.Jornada}");

            }
        }

        //Meetodo para listar la lista de alumnos predefinida de la base de datos
        private void lstListaAlumnos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstListaAlumnos.SelectedIndex == -1) return;

            var a = alumnosActuales[lstListaAlumnos.SelectedIndex];

            idSeleccionado = a.Id;

            txtIngresarNombre.Text = a.Nombre;
            txtInsertarApellido.Text = a.Apellido;
            txtIngresarCedula.Text = a.Cedula;
            cmbCarrera.Text = a.Carrera;

            rdoJornada1.Checked = a.Jornada == "Matutina";
            rdoJornada2.Checked = a.Jornada == "Vespertina";

            rdoSemestre1.Checked = a.Semestre == "Primer Semestre";
            rdoSemestre2.Checked = a.Semestre == "Segundo Semestre";

            txtUsuario.Text = a.Usuario;
            txtContraseña.Text = a.Contrasena;
            txtConfirmar.Text = a.Contrasena;

            chkNotificaciones.Checked = a.RecibirNotificaciones;
        }


        private void LimpiarCredenciales() //Metodo que limpia los campos texto
        {
            // Datos del alumno
            txtIngresarNombre.Clear();
            txtInsertarApellido.Clear();
            txtIngresarCedula.Clear();
            cmbCarrera.SelectedIndex = 0;

            rdoJornada1.Checked = false;
            rdoJornada2.Checked = false;

            rdoSemestre1.Checked = false;
            rdoSemestre2.Checked = false;
            
            //Credenciales
            txtUsuario.Clear();
            txtContraseña.Clear();
            txtConfirmar.Clear();
            chkTerminos.Checked = false;
            chkNotificaciones.Checked = false;

            txtUsuario.Focus();
        }

        //Validar credenciales de login 
        private bool ValidarCredenciales()
        {
            // Usuario vacío
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                Interaction.MsgBox("El campo Usuario no puede estar vacío.",
                    MsgBoxStyle.Exclamation,
                    "Validación");

                txtUsuario.Focus();
                return false;
            }

            // Contraseña vacía
            if (string.IsNullOrWhiteSpace(txtContraseña.Text))
            {
                Interaction.MsgBox("La contraseña no puede estar vacía.",
                    MsgBoxStyle.Exclamation,
                    "Validación");
                txtContraseña.Focus();
                return false;
            }

            // Contraseña con un maximo de 8 cacteres
            if (txtContraseña.Text.Length < 8)
            {
                Interaction.MsgBox("La contraseña debe tener al menos 8 caracteres.",
                    MsgBoxStyle.Exclamation,
                    "Validación");

                txtContraseña.Focus();
                txtContraseña.SelectAll();
                return false;
            }

            // Confirmación vacía
            if (string.IsNullOrWhiteSpace(txtConfirmar.Text))
            {
                Interaction.MsgBox("Debe confirmar la contraseña.",
                    MsgBoxStyle.Exclamation,
                    "Validación");
                txtConfirmar.Focus();
                return false;
            }

            // Contraseñas diferentes
            if (txtContraseña.Text != txtConfirmar.Text)
            {
                Interaction.MsgBox("Las contraseñas no coinciden.",
                    MsgBoxStyle.Critical,
                    "Error");
                txtConfirmar.Focus();
                txtConfirmar.SelectAll();
                return false;
            }

            // Términos no aceptados
            if (!chkTerminos.Checked)
            {
                Interaction.MsgBox("Debe aceptar los términos antes de continuar.",
                    MsgBoxStyle.Exclamation,
                    "Validación");
                chkTerminos.Focus();
                return false;
            }

            // Si se dio todo
            return true;
        }

        // Generar usuario automáticamente: primera letra del nombre + cédula
        private void GenerarUsuario(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtIngresarNombre.Text) &&
                !string.IsNullOrWhiteSpace(txtIngresarCedula.Text))
            {
                string cedulaSinGuiones = txtIngresarCedula.Text.Replace("-", "");
                txtUsuario.Text = txtIngresarNombre.Text.Substring(0, 1).ToLower() + cedulaSinGuiones;
            }
        }

        // Solo permitir números y guiones en cédula
        private void SoloNumeros(object sender, KeyPressEventArgs e)
        {
            // Permitir teclas de control 
            if (char.IsControl(e.KeyChar))
                return;

            // Permitir dígitos
            if (char.IsDigit(e.KeyChar))
                return;

            // Permitir guion
            if (e.KeyChar == '-')
                return;

            // Todo lo demás se bloquea
            e.Handled = true;
        }


        // Solo permitir letras y espacios en nombre y apellido
        private void SoloLetras(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsLetter(e.KeyChar) &&
                !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        // Validating para términos
        private void chkTerminos_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!chkTerminos.Checked)
            {
                Interaction.MsgBox("Debe aceptar los términos antes de continuar.", MsgBoxStyle.Exclamation, "Validación");
                e.Cancel = true;
            }
        }

        // Validating para Nombre
        private void txtIngresarNombre_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIngresarNombre.Text))
            {
                Interaction.MsgBox("El campo Nombre es obligatorio.",
                    MsgBoxStyle.Exclamation, "Validación");
                e.Cancel = true;
            }
        }

        // Validating para Cedula
        private void txtIngresarCedula_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string valor = txtIngresarCedula.Text.Trim();

            // Patrón: 1-111-111 o 1-111-1111
            string patron = @"^\d-\d{3}-\d{3,4}$";

            if (!Regex.IsMatch(valor, patron))
            {
                Interaction.MsgBox(
                    "Formato de cédula inválido.\nUse 1-111-111 o 1-111-1111.",
                    MsgBoxStyle.Exclamation,
                    "Validación de cédula");

                e.Cancel = true;
            }
        }

        //Atajos de teclado
        private void AtajosDelTeclado(object sender, KeyEventArgs e)
        {
            // Ctrl + S = Guardar
            if (e.Control && e.KeyCode == Keys.S)
            {
                BtnGuardar.PerformClick();
                e.SuppressKeyPress = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.D)  // Eliminar
            {
                BtnEliminar.PerformClick();
                e.SuppressKeyPress = true;
                return;
            }

            // Editar
            if (e.Control && e.KeyCode == Keys.E)
            {
                BtnEditar.PerformClick();
                e.SuppressKeyPress = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.R)  // Reportes
            {
                mnuReportes.PerformClick();
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Escape)  // Nuevo
            {
                BtnNuevo.PerformClick();
                e.SuppressKeyPress = true;
                return;
            }

            //Buscar
            if (e.Control && e.KeyCode == Keys.B)
            {
                BtnBuscarCedula.PerformClick();
                e.SuppressKeyPress = true;
                return;
            }

            //Desplazamiento con las flechas usando el teclado 
            // --- Navegación con Enter ---
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(ActiveControl, true, true, true, true);
                e.SuppressKeyPress = true;  // Evita que suene el “ding”
                return;
            }

            // --- Navegación con flecha abajo---
            if (e.KeyCode == Keys.Down)
            {
                this.SelectNextControl(ActiveControl, true, true, true, true);
                e.SuppressKeyPress = true;
                return;
            }

            // --- Navegación con flecha arriba ---
            if (e.KeyCode == Keys.Up)
            {
                this.SelectNextControl(ActiveControl, false, true, true, true);
                e.SuppressKeyPress = true;
                return;
            }

        }

        //Menu De la parte superior

        // Archivo -> Nuevo
        private void mnuNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCredenciales();
        }

        // Archivo -> Guardar
        private void mnuGuardar_Click(object sender, EventArgs e)
        {
            BtnGuardar.PerformClick();
        }

        // Archivo -> Salir
        private void mnuSalir_Click(object sender, EventArgs e)
        {
            var respuesta = Interaction.MsgBox(
                "¿Desea salir de la aplicación?",
                MsgBoxStyle.YesNo | MsgBoxStyle.Question,
                "Salir");

            if (respuesta == MsgBoxResult.Yes)
            {
                this.Close();
            }
        }

        // Ayuda -> Acerca de
        private void mnuAcercaDe_Click(object sender, EventArgs e)
        {
            Interaction.MsgBox(
                "Sistema de Registro de Alumnos\nVersión 1.0\nAutores: Brandom Arcia, Anthony Perez",
                MsgBoxStyle.Information,
                "Acerca de");
        }

        //Opcion de editar un alumno
        private void BtnEditar_Click(object sender, EventArgs e)
        {
            // Verifica que se haya seleccionado un alumno en la lista
            if (idSeleccionado == 0)
            {
                Interaction.MsgBox("Debe seleccionar un alumno de la lista.",
                    MsgBoxStyle.Exclamation, "Editar");
                return; // Si no hay selección, termina el método
            }

            // Valida las credenciales ingresadas antes de editar
            if (!ValidarCredenciales())
                return;

            // Determina la jornada seleccionada según los RadioButtons
            string jornadaSeleccionada = rdoJornada1.Checked ? "Matutina" :
                                         rdoJornada2.Checked ? "Vespertina" : "";

            // Determina el semestre seleccionado según los RadioButtons
            string semestreSeleccionado = rdoSemestre1.Checked ? "Primer Semestre" :
                                          rdoSemestre2.Checked ? "Segundo Semestre" : "";

            // Crea un objeto Alumno con los datos ingresados en el formulario
            Alumno a = new Alumno
            {
                Id = idSeleccionado,
                Nombre = txtIngresarNombre.Text.Trim(),
                Apellido = txtInsertarApellido.Text.Trim(),
                Cedula = txtIngresarCedula.Text.Trim(),
                Carrera = cmbCarrera.Text,
                Semestre = semestreSeleccionado,
                Jornada = jornadaSeleccionada,
                Usuario = txtUsuario.Text.Trim(),
                Contrasena = txtContraseña.Text,
                RecibirNotificaciones = chkNotificaciones.Checked
            };

            // Llama al repositorio para actualizar los datos del alumno en la base de datos
            repo.Actualizar(a);

            // Mensaje de confirmación al usuario
            Interaction.MsgBox("Datos del alumno actualizados correctamente.",
                MsgBoxStyle.Information, "Editar");

            // Recarga la lista de alumnos en el ListBox
            CargarAlumnosEnListBox();

            // Limpia los campos de credenciales
            LimpiarCredenciales();

            // Resetea la variable de selección
            idSeleccionado = 0;
        }

        // Boton de eliminar alumno 
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                Interaction.MsgBox("Debe seleccionar un alumno de la lista.",
                    MsgBoxStyle.Exclamation, "Eliminar");
                return;
            }

            var resp = Interaction.MsgBox(
                "¿Está seguro de eliminar este alumno?",
                MsgBoxStyle.YesNo | MsgBoxStyle.Question,
                "Confirmar eliminación");

            if (resp != MsgBoxResult.Yes) return;

            repo.Eliminar(idSeleccionado);

            Interaction.MsgBox("Alumno eliminado correctamente.",
                MsgBoxStyle.Information, "Eliminar");

            CargarAlumnosEnListBox();
            LimpiarCredenciales();
            idSeleccionado = 0;
        }

        //Boton de busqueda por medio de la cedula 
        private void BtnBuscarCedula_Click(object sender, EventArgs e)
        {
            // Valida que se haya ingresado una cédula
            if (string.IsNullOrWhiteSpace(txtBuscarCedula.Text))
            {
                Interaction.MsgBox("Ingrese una cédula para buscar.",
                    MsgBoxStyle.Exclamation, "Buscar");
                return;
            }

            // Busca el alumno en el repositorio por cédula
            var alumno = repo.BuscarPorCedula(txtBuscarCedula.Text.Trim());

            // Si no existe el alumno, muestra mensaje
            if (alumno == null)
            {
                Interaction.MsgBox("No existe un alumno con esa cédula.",
                    MsgBoxStyle.Information, "Buscar");
                return;
            }

            // MessageBox con la información encontrada
            Interaction.MsgBox(
                $"Alumno encontrado:\n\n" +
                $"Nombre: {alumno.Nombre} {alumno.Apellido}\n" +
                $"Cédula: {alumno.Cedula}\n" +
                $"Carrera: {alumno.Carrera}" +
                $"Usuario: {alumno.Usuario}" +
                $"Jornada: {alumno.Jornada}",
                MsgBoxStyle.Information,
                "Resultado de Búsqueda");

            // Limpiar el campo de búsqueda
            txtBuscarCedula.Clear();
            txtBuscarCedula.Focus();
        }

        // Archivo -> Reportes
        private void mnuReportes_Click(object sender, EventArgs e)
        {
            CRUD repo = new CRUD();
            var lista = repo.ObtenerTodos();

            ReportesPDF.ReporteGeneral(lista);

            MessageBox.Show("Reporte general generado en el Escritorio.");
        }

        //Reportes -> Reportes por carrera
        private void reportePorCarreraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string carrera = "";

            // Desplegar opciones en un InputBox simple
            carrera = Microsoft.VisualBasic.Interaction.InputBox(
                "Seleccione la carrera:\n\n" +
                "1. Ingeniería en Sistemas\n" +
                "2. Ingeniería Industrial\n" +
                "3. Administración\n" +
                "4. Contabilidad",
                "Reporte por carrera",
                "1"
            );

            // Convertir número a texto
            switch (carrera)
            {
                case "1": carrera = "Ingeniería en Sistemas"; break;
                case "2": carrera = "Ingeniería Industrial"; break;
                case "3": carrera = "Administración"; break;
                case "4": carrera = "Contabilidad"; break;
                default:
                    MessageBox.Show("Opción no válida.");
                    return;
            }

            CRUD repo = new CRUD();
            var lista = repo.ObtenerTodos();

            ReportesPDF.ReportePorCarrera(lista, carrera);

            MessageBox.Show("Reporte por carrera generado en el Escritorio.");
        }

        // Reportes -> reportes por jornada
        private void reportePorJornadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string jornada = Microsoft.VisualBasic.Interaction.InputBox(
                "Seleccione jornada:\n\n" +
                "1. Matutina\n" +
                "2. Vespertina",
                "Reporte por jornada",
                "1"
            );

            if (jornada == "1") jornada = "Matutina";
            else if (jornada == "2") jornada = "Vespertina";
            else
            {
                MessageBox.Show("Opción no válida.");
                return;
            }

            CRUD repo = new CRUD();
            var lista = repo.ObtenerTodos();

            ReportesPDF.ReportePorJornada(lista, jornada);

            MessageBox.Show("Reporte por jornada generado en el Escritorio.");
        }

        //Reportes -> reportes por rango de fechas 
        private void reporteEnRangoDeFechasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CRUD repo = new CRUD();
            var lista = repo.ObtenerTodos();   // Trae todos los alumnos

            // Definir rango manualmente 
            DateTime desde = DateTime.Today.AddMonths(-1);
            DateTime hasta = DateTime.Today.AddDays(1).AddSeconds(-1); 

            // Filtrar por fecha
            var filtrado = lista.Where(a =>
                a.FechaRegistro >= desde &&
                a.FechaRegistro <= hasta
            ).ToList();

            if (filtrado.Count == 0)
            {
                MessageBox.Show("No hay registros dentro del rango de fechas.");
                return;
            }

            // Generar PDF SOLO con los filtrados
            ReportesPDF.ReporteRango(filtrado, desde, hasta);

            MessageBox.Show("Reporte generado en el escritorio.");
        }

        //Reportes -> Perfil individual
        private void perfilIndividualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CRUD repo = new CRUD();
            var lista = repo.ObtenerTodos();

            string idStr = Microsoft.VisualBasic.Interaction.InputBox(
                "Ingrese ID del alumno:",
                "Perfil del alumno",
                "1");

            if (int.TryParse(idStr, out int id))
                {
                    var alumno = lista.FirstOrDefault(a => a.Id == id);

                    if (alumno != null)
                    {
                        ReportesPDF.ReportePerfil(alumno);
                        MessageBox.Show("Perfil generado.");
                    }
                    else
                    {
                        MessageBox.Show("Alumno no encontrado.");
                }
             }
        }
    }
}

using Microsoft.VisualBasic;
using Registro_de_Alumnos.Clases;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;



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

        }
        //Boton Guardar datos de nuevo estudiante

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCredenciales();
        }

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

        private void CargarAlumnosEnListBox()
        {
            lstListaAlumnos.Items.Clear();
            alumnosActuales = repo.ObtenerTodos();

            foreach (var a in alumnosActuales)
            {
                lstListaAlumnos.Items.Add ($"{a.Id} - {a.Nombre} {a.Apellido} - {a.Carrera} - {a.Jornada}");

            }
        }

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

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                Interaction.MsgBox("Debe seleccionar un alumno de la lista.",
                    MsgBoxStyle.Exclamation, "Editar");
                return;
            }

            if (!ValidarCredenciales())
                return;

            // Puedes reutilizar tus otras validaciones de nombre, carrera, etc.

            string jornadaSeleccionada = rdoJornada1.Checked ? "Matutina" :
                                         rdoJornada2.Checked ? "Vespertina" : "";

            string semestreSeleccionado = rdoSemestre1.Checked ? "Primer Semestre" :
                                          rdoSemestre2.Checked ? "Segundo Semestre" : "";

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

            repo.Actualizar(a);

            Interaction.MsgBox("Datos del alumno actualizados correctamente.",
                MsgBoxStyle.Information, "Editar");

            CargarAlumnosEnListBox();
            LimpiarCredenciales();
            idSeleccionado = 0;
        }

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

        private void BtnBuscarCedula_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscarCedula.Text))
            {
                Interaction.MsgBox("Ingrese una cédula para buscar.",
                    MsgBoxStyle.Exclamation, "Buscar");
                return;
            }

            var alumno = repo.BuscarPorCedula(txtBuscarCedula.Text.Trim());

            if (alumno == null)
            {
                Interaction.MsgBox("No existe un alumno con esa cédula.",
                    MsgBoxStyle.Information, "Buscar");
                return;
            }

            // Rellenar campos igual que seleccionar en el ListBox
            idSeleccionado = alumno.Id;
            txtIngresarNombre.Text = alumno.Nombre;
            txtInsertarApellido.Text = alumno.Apellido;
            txtIngresarCedula.Text = alumno.Cedula;
            cmbCarrera.Text = alumno.Carrera;
            rdoJornada1.Checked = alumno.Jornada == "Matutina";
            rdoJornada2.Checked = alumno.Jornada == "Vespertina";
            rdoSemestre1.Checked = alumno.Semestre == "Primer Semestre";
            rdoSemestre2.Checked = alumno.Semestre == "Segundo Semestre";
            txtUsuario.Text = alumno.Usuario;
            txtContraseña.Text = alumno.Contrasena;
            txtConfirmar.Text = alumno.Contrasena;
            chkNotificaciones.Checked = alumno.RecibirNotificaciones;
        }

        private void mnuReportes_Click(object sender, EventArgs e)
        {
          /*  using (var frm = new FrmReportes())
            {
                frm.ShowDialog();
            }*/
        }
    }
}

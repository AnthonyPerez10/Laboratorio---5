using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registro_de_Alumnos.Clases
{
    //Metodos para extraer y setear los datos del alumno
    public class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }
        public string Carrera { get; set; }
        public string Semestre { get; set; }
        public string Jornada { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public bool RecibirNotificaciones { get; set; }
        public DateTime FechaRegistro { get; set; }

    }
}

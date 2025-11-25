using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registro_de_Alumnos.Clases
{
    public static class ConexionBD
    {
        //Enlace a a la base de datos. 
        private static string cadenaConexion =
    "Server=DESKTOP-6Q8S0IM;Database=DBAlumnos;Trusted_Connection=True;"; //Conexion a la base de datos loca

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}

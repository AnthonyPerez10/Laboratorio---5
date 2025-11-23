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
        private static string cadenaConexion =
    "Server=DESKTOP-F6PIVOK;Database=DBAlumnos;Trusted_Connection=True;";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}

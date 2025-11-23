using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registro_de_Alumnos.Clases
{
    public class CRUD
    {
        public List<Alumno> ObtenerTodos()
        {
            var lista = new List<Alumno>();

            using (SqlConnection conn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Alumnos ORDER BY FechaRegistro", conn))
            {
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Alumno
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Apellido = dr["Apellido"].ToString(),
                            Cedula = dr["Cedula"].ToString(),
                            Carrera = dr["Carrera"].ToString(),
                            Semestre = dr["Semestre"].ToString(),
                            Jornada = dr["Jornada"].ToString(),
                            Usuario = dr["Usuario"].ToString(),
                            Contrasena = dr["Contrasena"].ToString(),
                            RecibirNotificaciones = (bool)dr["RecibirNotificaciones"],
                            FechaRegistro = (DateTime)dr["FechaRegistro"]
                        });
                    }
                }
            }

            return lista;
        }

        public int Insertar(Alumno a)
        {
            using (SqlConnection conn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Alumnos
                (Nombre,Apellido, Cedula, Carrera, Semestre, Jornada, Usuario, Contrasena, RecibirNotificaciones)
                VALUES (@Nombre, @Apellido, @Cedula, @Carrera, @Semestre, @Jornada, @Usuario, @Contrasena, @RecibirNotificaciones);
                SELECT SCOPE_IDENTITY();", conn))
            {
                cmd.Parameters.AddWithValue("@Nombre", a.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", a.Apellido);
                cmd.Parameters.AddWithValue("@Cedula", a.Cedula);
                cmd.Parameters.AddWithValue("@Carrera", a.Carrera);
                cmd.Parameters.AddWithValue("@Semestre", a.Semestre);
                cmd.Parameters.AddWithValue("@Jornada", a.Jornada);
                cmd.Parameters.AddWithValue("@Usuario", a.Usuario);
                cmd.Parameters.AddWithValue("@Contrasena", a.Contrasena);
                cmd.Parameters.AddWithValue("@RecibirNotificaciones", a.RecibirNotificaciones);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public void Actualizar(Alumno a)
        {
            using (SqlConnection conn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Alumnos SET
                    Nombre = @Nombre,
                    Apellido = @Apellido,
                    Cedula = @Cedula,
                    Carrera = @Carrera,
                    Semestre = @Semestre,
                    Jornada = @Jornada,
                    Usuario = @Usuario,
                    Contrasena = @Contrasena,
                    RecibirNotificaciones = @RecibirNotificaciones
                WHERE Id = @Id;", conn))
            {
                cmd.Parameters.AddWithValue("@Id", a.Id);
                cmd.Parameters.AddWithValue("@Nombre", a.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", a.Apellido);
                cmd.Parameters.AddWithValue("@Cedula", a.Cedula);
                cmd.Parameters.AddWithValue("@Carrera", a.Carrera);
                cmd.Parameters.AddWithValue("@Semestre", a.Semestre);
                cmd.Parameters.AddWithValue("@Jornada", a.Jornada);
                cmd.Parameters.AddWithValue("@Usuario", a.Usuario);
                cmd.Parameters.AddWithValue("@Contrasena", a.Contrasena);
                cmd.Parameters.AddWithValue("@RecibirNotificaciones", a.RecibirNotificaciones);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Alumnos WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Alumno BuscarPorCedula(string cedula)
        {
            using (SqlConnection conn = ConexionBD.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Alumnos WHERE Cedula = @Cedula", conn))
            {
                cmd.Parameters.AddWithValue("@Cedula", cedula);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return null;

                    return new Alumno
                    {
                        Id = (int)dr["Id"],
                        Nombre = dr["Nombre"].ToString(),
                        Apellido = dr["Apellido"].ToString(),
                        Cedula = dr["Cedula"].ToString(),
                        Carrera = dr["Carrera"].ToString(),
                        Semestre = dr["Semestre"].ToString(),
                        Jornada = dr["Jornada"].ToString(),
                        Usuario = dr["Usuario"].ToString(),
                        Contrasena = dr["Contrasena"].ToString(),
                        RecibirNotificaciones = (bool)dr["RecibirNotificaciones"],
                        FechaRegistro = (DateTime)dr["FechaRegistro"]
                    };
                }
            }
        }
    }
}

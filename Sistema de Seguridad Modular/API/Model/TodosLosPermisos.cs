using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
namespace APISeguridad.Model
{
    public class TodosLosPermisos
    {
        public int idUsuario { get; set; }
        public int idPantalla { get; set; }
        public int idSistema { get; set; }
        public string nombrePantalla { get; set; }
        public int permisoInsertar { get; set; }
        public int permisoModificar { get; set; }
        public int permisoBorrar { get; set; }
        public int permisoConsultar { get; set; }
        public string fuente { get; set; } // 'DIRECTO' o 'ROL'
        public string nombreRol { get; set; } // puede ser null si es DIRECTO
    }

    public class PermisoRepository
    {
        private readonly string _connectionString;

        public PermisoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<TodosLosPermisos> ObtenerPermisosUsuario(int idUsuario)
        {
            var permisos = new List<TodosLosPermisos>();

            using (var conn = new OracleConnection(_connectionString))
            using (var cmd = new OracleCommand("p_permisosUsuario", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_idusuario", OracleDbType.Int32).Value = idUsuario;
                cmd.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permisos.Add(new TodosLosPermisos
                        {
                            idUsuario = reader.GetInt32(reader.GetOrdinal("IDUSUARIO")),
                            idPantalla = reader.GetInt32(reader.GetOrdinal("IDPANTALLA")),
                            idSistema = reader.GetInt32(reader.GetOrdinal("IDSISTEMA")),
                            nombrePantalla = reader.GetString(reader.GetOrdinal("NOMBRE_PANTALLA")),
                            permisoInsertar = reader.GetInt32(reader.GetOrdinal("PERMISOINSERTAR")),
                            permisoModificar = reader.GetInt32(reader.GetOrdinal("PERMISOMODIFICAR")),
                            permisoBorrar = reader.GetInt32(reader.GetOrdinal("PERMISOBORRAR")),
                            permisoConsultar = reader.GetInt32(reader.GetOrdinal("PERMISOCONSULTAR"))
                        });
                    }
                }
            }

            return permisos;
        }
    }


}
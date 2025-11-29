using System.Globalization;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.IO;


using EUsuario;
using MVC.Interfaces;
namespace MVC.Repositorios;

public class UsuarioRepository : IUserRepository
{
    private readonly string CadenaConexion;

    public UsuarioRepository(){
        CadenaConexion = "Data Source=db/Tienda.db";
    }
    // Lógica para conectar con la DB y buscar por user/pass.
    public Usuarios GetUser(string usuario, string contrasena)
    {
        Usuarios user = null;
        //Consulta SQL que busca por Usuario Y Contrasena
        const string sql = @"
                        SELECT Id, Nombre, User, Pass, Rol
                        FROM Usuarios
                        WHERE User = @Usuario AND Pass = @Contrasena";
        using var conexion = new SqliteConnection(CadenaConexion);
        conexion.Open();
        using var comando = new SqliteCommand(sql, conexion);

        // Se usan parámetros para prevenir inyección SQL
        comando.Parameters.AddWithValue("@Usuario", usuario);
        comando.Parameters.AddWithValue("@Contrasena", contrasena);
        using var reader = comando.ExecuteReader();
        if (reader.Read())
        {
            // Si el lector encuentra una fila, el usuario existe y las credenciales son correctas
            user = new Usuarios
            {
                IdUser = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                User = reader.GetString(2),
                Pass = reader.GetString(3),
                Rol = reader.GetString(4)
            };
        }
        return user;
    }
}
using System.Globalization;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.IO;

using EPresupuestosDetalles;
using EPresupuestos;
using EProductos;
public class PresupuestosRepository
{
    private readonly string ConexionString;

    public PresupuestosRepository()
    {
        ConexionString = "Data Source=db/Tienda.db";
    }

    public void Create(Presupuestos presupuesto)
    {
        string query = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion)";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var command = new SqliteCommand(query, Conexion);
        command.Parameters.Add(new SqliteParameter("@NombreDestinatario", presupuesto.NombreDestinatario));
        command.Parameters.Add(new SqliteParameter("@FechaCreacion", presupuesto.FechaCreada));
        command.ExecuteNonQuery();
        Conexion.Close();
    }

    public List<Presupuestos> GetAll()
    {
        var ListaPresupuestos = new List<Presupuestos>();
        string query = "SELECT * FROM Presupuestos";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var command = new SqliteCommand(query, Conexion);
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var presupuesto = new Presupuestos()
                {
                    IdPresupuesto = Convert.ToInt32(reader["idPresupuesto"]),
                    NombreDestinatario = reader["NombreDestinatario"].ToString(),
                    FechaCreada = DateOnly.FromDateTime(Convert.ToDateTime(reader["FechaCreacion"]))
                };
                ListaPresupuestos.Add(presupuesto);
            }
        }

        return ListaPresupuestos;
    }

    public Presupuestos Detalle(int Id)
    {
        string query = @"SELECT 
                            p.idPresupuesto,
                            p.NombreDestinatario,
                            p.FechaCreacion,
                            pr.idProducto,
                            pr.Descripcion,
                            pr.Precio,
                            d.Cantidad
                            FROM Presupuestos p
                            INNER JOIN PresupuestosDetalle d ON p.idPresupuesto = d.idPresupuesto
                            INNER JOIN Productos pr ON d.idProducto = pr.idProducto
                            WHERE p.idPresupuesto = @Id";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.Add(new SqliteParameter("@Id", Id));
        using SqliteDataReader reader = comman.ExecuteReader();
        Presupuestos detallePresupuesto = null;
        while (reader.Read())
        {
            if (detallePresupuesto == null)
            {
                detallePresupuesto = new Presupuestos()
                {
                    IdPresupuesto = Convert.ToInt32(reader["idPresupuesto"]),
                    NombreDestinatario = reader["NombreDestinatario"].ToString(),
                    FechaCreada = DateOnly.FromDateTime(Convert.ToDateTime(reader["FechaCreacion"])),
                    Detalle = new List<PresupuestosDetalle>()
                };

                var producto = new Productos()
                {
                    IdProducto = Convert.ToInt32(reader["idProducto"]),
                    Descripcion = reader["Descripcion"].ToString(),
                    Precio = Convert.ToInt32(reader["Precio"])
                };
                var detalle = new PresupuestosDetalle()
                {
                    productos = producto,
                    Cantidad = Convert.ToInt32(reader["Cantidad"])
                };
                detallePresupuesto.Detalle.Add(detalle);
            }
        }
        return detallePresupuesto;
    }

    public void AddProductoAPresupuestos(int IdProducto, int IdPresupuesto, int cant)
    {
        string query = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@IdPresupuesto, @IdProducto, @cant)";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.Add(new SqliteParameter("@IdPresupuesto", IdPresupuesto));
        comman.Parameters.Add(new SqliteParameter("@IdProducto",IdProducto));
        comman.Parameters.Add(new SqliteParameter("@cant",cant));
        comman.ExecuteNonQuery();
        Conexion.Close();
    }

    public bool Delete(int Id)
    {
        string query = @"DELETE FROM Presupuesto WHER idPresupuesto = @Id";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.Add(new SqliteParameter("@Id", Id));
        int filasAfectadas = comman.ExecuteNonQuery();
        Conexion.Close();
        return filasAfectadas > 0;
    }

    public Presupuestos ObtenerPresupuesto(int Id)
    {
        var presupuesto = new Presupuestos();
        string query = @"SELECT NomreDestinatario, FechaCreacion
                        FROM Presupuestos
                        WHERE idPresupuesto = @Id";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
            comman.Parameters.AddWithValue("@Id", Id);
        using(var reader = comman.ExecuteReader())
        {
            presupuesto.NombreDestinatario = reader["NombreDestinatario"].ToString();
            presupuesto.FechaCreada = DateOnly.FromDateTime(Convert.ToDateTime(reader["FechaCreacion"]));
        }
        if(presupuesto == null) return null;
        return presupuesto;
    }

    public bool Modificar(Presupuestos presupuesto)
    {
        string query = @"UPDATE Presupuesto SET NombreDestinatario = @nombre, FechaCreacion = @fecha 
                        WHERE idPresupuesto = @id";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.AddWithValue("@nombre", presupuesto.NombreDestinatario);
        comman.Parameters.AddWithValue("@fecha", presupuesto.FechaCreada);
        comman.Parameters.AddWithValue("@id", presupuesto.IdPresupuesto);
        int filasAfectadas = comman.ExecuteNonQuery();
        Conexion.Close();
        return filasAfectadas > 0;
    }


}
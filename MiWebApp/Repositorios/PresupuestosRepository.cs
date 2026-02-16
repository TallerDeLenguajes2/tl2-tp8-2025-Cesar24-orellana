using System.Globalization;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.IO;

using MVC.Interfaces;
using EPresupuestosDetalles;
using EPresupuestos;
using EProductos;
public class PresupuestosRepository : IPresupuestosRepository
{
    private readonly string ConexionString;

    public PresupuestosRepository(string conexionString)
    {
        ConexionString = conexionString;
    }

    public void Create(Presupuestos presupuesto)
    {
        string query = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion)";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var command = new SqliteCommand(query, Conexion);
        command.Parameters.Add(new SqliteParameter("@NombreDestinatario", presupuesto.NombreDestinatario));
        command.Parameters.Add(new SqliteParameter("@FechaCreacion", presupuesto.FechaCreada));
        int filasAfectadas = command.ExecuteNonQuery();
        if (filasAfectadas == 0) throw new Exception("No se pudo crear un Presupuesto");
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
        if (ListaPresupuestos.Count == 0) throw new Exception("Lista presupuestos vacia");
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
        if(detallePresupuesto == null) throw new Exception($"No se encontro un presupuesto de ID: {Id}");
        return detallePresupuesto;
    }

    public void AddDetalle(int IdPresupuesto, int IdProducto, int cant)
    {
        string query = @"INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad)
                VALUES (@IdPresupuesto, @IdProducto, @cant)";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var command = new SqliteCommand(query, Conexion);
        command.Parameters.AddWithValue("@IdPresupuesto", IdPresupuesto);
        command.Parameters.AddWithValue("@IdProducto", IdProducto);
        command.Parameters.AddWithValue("@cant", cant);

        int filasAfectadas = command.ExecuteNonQuery();
        /* string query = @"INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) 
                        VALUES (@IdPresupuesto, @IdProducto, @cant)";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.Add(new SqliteParameter("@IdPresupuesto", IdPresupuesto));
        comman.Parameters.Add(new SqliteParameter("@IdProducto",IdProducto));
        comman.Parameters.Add(new SqliteParameter("@cant",cant));
        comman.ExecuteNonQuery(); */
        if (filasAfectadas == 0) throw new Exception("No se pudo crear un nuevo presupuesto");
    }

    public bool Delete(int Id)
    {
        string query = @"DELETE FROM Presupuestos WHERE idPresupuesto = @Id";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        using var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.AddWithValue("@Id", Id);
        int filasAfectadas = comman.ExecuteNonQuery();
        if (filasAfectadas == 0) throw new Exception($"No se pudo eliminar el presupuesto de ID: {Id}");
        return filasAfectadas > 0;
    }

    public Presupuestos ObtenerPresupuesto(int Id)
    {
        var presupuesto = new Presupuestos();
        string query = @"SELECT NombreDestinatario, FechaCreacion 
                        FROM Presupuestos
                        WHERE idPresupuesto = @Id";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.AddWithValue("@Id", Id);
        using (var reader = comman.ExecuteReader())
        {
            if (reader.Read())   // <<-- Siempre llamar a Read()
            {
                return new Presupuestos
                {
                    IdPresupuesto = Id,
                    NombreDestinatario = reader["NombreDestinatario"].ToString(),
                    FechaCreada = DateOnly.FromDateTime(Convert.ToDateTime(reader["FechaCreacion"]))
                };
            }
        }
        throw new Exception($"El prespuesto de ID: {Id} no fue encontrado");
    }

    public bool Modificar(Presupuestos presupuesto)
    {
        string query = @"UPDATE Presupuestos SET NombreDestinatario = @nombre, FechaCreacion = @fecha 
                        WHERE idPresupuesto = @id";
        using var Conexion = new SqliteConnection(ConexionString);
        Conexion.Open();

        using var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.AddWithValue("@nombre", presupuesto.NombreDestinatario);
        comman.Parameters.AddWithValue("@fecha", presupuesto.FechaCreada.ToString("yyyy-MM-dd"));
        comman.Parameters.AddWithValue("@id", presupuesto.IdPresupuesto);
        int filasAfectadas = comman.ExecuteNonQuery();
        if(filasAfectadas == 0) throw new Exception($"No se pudo modificar el presupuesto de ID: {presupuesto.IdPresupuesto}");        return filasAfectadas > 0;
    }


}
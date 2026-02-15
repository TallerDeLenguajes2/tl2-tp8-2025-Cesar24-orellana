using System.Globalization;
using System.Linq;
using Microsoft.Data.Sqlite;

using EProductos;
using MVC.Interfaces;
public class ProductoRepository : IProductoRepository
{
    private readonly string cadenaConexion;

    public ProductoRepository()
    {
        cadenaConexion = "Data Source=db/Tienda.db";
    }

    public void Add(Productos producto)
    {
        string query = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
        using var Conexion = new SqliteConnection(cadenaConexion);

        Conexion.Open();
        var command = new SqliteCommand(query, Conexion);
        command.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
        command.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));
        int filasAfectadas = command.ExecuteNonQuery();
        if(filasAfectadas == 0) throw new Exception("No fue posible agregar el producto");
    }
    public bool Update(Productos producto)
    {
        using var Conexion = new SqliteConnection(cadenaConexion);
        string query = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @IdProducto";
        Conexion.Open();
        using var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.Add(new SqliteParameter("@IdProducto", producto.IdProducto));
        comman.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
        comman.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));
        //comman.Parameters.AddWithValue("@IdProducto", IdProducto);
        // comman.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
        // comman.Parameters.AddWithValue("@Precio", producto.Precio);
        int filasAfectadas = comman.ExecuteNonQuery();
        if(filasAfectadas == 0) throw new Exception($"Error al actualizar el producto de ID: {producto.IdProducto}");
        return filasAfectadas > 0;
    }
    public List<Productos> GetAll()
    {
        string query = "SELECT * FROM productos";
        var productos = new List<Productos>();
        using var Conexion = new SqliteConnection(cadenaConexion);
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        using (SqliteDataReader reader = comman.ExecuteReader())
        {
            while (reader.Read())
            {
                var producto = new Productos(
                    Convert.ToInt32(reader["idProducto"]),
                    reader["Descripcion"].ToString(),
                    Convert.ToInt32(reader["Precio"])
                );
                productos.Add(producto);
            }
        }
        if(productos.Count == 0) throw new Exception("Lista producto vacia"); 
        return productos;
    }
    public Productos GetById(int IdProducto)
    {

        
        using var Conexion = new SqliteConnection(cadenaConexion);
        string query = "SELECT Descripcion, Precio FROM Productos WHERE idProducto = @IdProducto";
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.AddWithValue("@IdProducto", IdProducto);
        using SqliteDataReader reader = comman.ExecuteReader();

        if (reader.Read())
        {
            return new Productos
            {
                IdProducto = IdProducto,
                Descripcion = reader["Descripcion"].ToString(),
                Precio = Convert.ToInt32(reader["Precio"])
            };
            //Conexion.Close();
        }
        throw new Exception("Producto Inexistenete.");
        //return null;
    }
    public bool Delete(int IdProducto)
    {
        using var Conexion = new SqliteConnection(cadenaConexion);
        Conexion.Open();
        using var transaccion = Conexion.BeginTransaction();
        try
        {
            var commanDetalles = new SqliteCommand(
                        "DELETE FROM PresupuestosDetalle WHERE idProducto = @IdProducto",
                        Conexion, transaccion
            );
            commanDetalles.Parameters.AddWithValue("@IdProducto", IdProducto);
            commanDetalles.ExecuteNonQuery();

            var comman = new SqliteCommand(
                        "DELETE FROM Productos WHERE idProducto = @IdProducto",
                        Conexion, transaccion
            );
            comman.Parameters.AddWithValue("@IdProducto", IdProducto);
            int filasAfectadas = comman.ExecuteNonQuery();
            transaccion.Commit();
            if(filasAfectadas == 0) throw new Exception($"No se pudo eliminar el producto de ID: {IdProducto}");
            return filasAfectadas > 0;
        }
        catch (Exception ex)
        {
            transaccion.Rollback();
            //return false;
            throw new Exception($"No se pudo eliminar el producto con ID {IdProducto}.", ex);
        }
    }
}
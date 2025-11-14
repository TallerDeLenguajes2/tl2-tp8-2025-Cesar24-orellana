using System.Globalization;
using System.Linq;
using Microsoft.Data.Sqlite;
public class ProductoRepository
{
    private readonly string cadenaConexion;

    public ProductoRepository(){
        cadenaConexion = "Data Source=db/Tienda.db";
    }

    public void CreaProducto(Productos producto)
    {
        string query = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
        using var Conexion = new SqliteConnection(cadenaConexion);
        
            Conexion.Open();
            var command = new SqliteCommand(query, Conexion);
            command.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
            command.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));
            command.ExecuteNonQuery();
            Conexion.Close();
        
    }
    public bool ModificarProducto(int IdProducto, Productos producto)
    {
        using var Conexion = new SqliteConnection(cadenaConexion);
        string query = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @IdProducto";
        Conexion.Open();
        using var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.Add(new SqliteParameter("@IdProducto", IdProducto));
        comman.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
        comman.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));
        //comman.Parameters.AddWithValue("@IdProducto", IdProducto);
        // comman.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
        // comman.Parameters.AddWithValue("@Precio", producto.Precio);
        int filasAfectadas = comman.ExecuteNonQuery();
        Conexion.Close();
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
        Conexion.Close();
        return productos;
    }
    public Productos DetallesProducto(int IdProducto)
    {
        var producto = new Productos();
        using var Conexion = new SqliteConnection(cadenaConexion);
        string query = "SELECT Descripcion, Precio FROM Productos WHERE idProducto = @IdProducto";
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        comman.Parameters.AddWithValue("@IdProducto", IdProducto);
        using SqliteDataReader reader = comman.ExecuteReader();

        if (reader.Read())
        {
            producto.IdProducto = IdProducto;
            producto.Descripcion = reader["Descripcion"].ToString();
            producto.Precio = Convert.ToInt32(reader["Precio"]);
            Conexion.Close();
            return producto;
        }
        return null;
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
            Conexion.Close();
            return filasAfectadas > 0;
        }
        catch (Exception ex)
        {
            transaccion.Rollback();
            return false;
        }
    }
}
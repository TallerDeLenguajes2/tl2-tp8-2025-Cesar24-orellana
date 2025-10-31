using System.Globalization;
using System.Linq;
using Microsoft.Data.Sqlite;
public class ProductoRepository{
    private string cadenaConexion = "db/tienda.db";


    
    public void CreaProducto(Productos producto){
        string query = $"INSERT INTO Productos (Descripcion, Precio) VALUE ({producto.Descripcion}, {producto.Precio})";
        using(var Conexion = new SqliteConnection(cadenaConexion))
        {
            Conexion.Open();
            var command = new SqliteCommand(query, Conexion);
            command.ExecuteNonQuery();
            Conexion.Close();
        }
    }
    public void ModificarProducto(int IdProducto, Productos producto){
        using var Conexion = new SqliteConnection(cadenaConexion);
        string query = $"UPDATE Productos SET Descripcion = {producto.Descripcion}, Precio = {producto.Precio} WHERE = idProducto = {IdProducto}";
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        comman.ExecuteNonQuery();
        Conexion.Close();
    }
    public List<Productos> GetAll()
    {
        string query = "SELECT * FROM productos";
        var productos = new List<Productos>();
        using var Conexion = new SqliteConnection(cadenaConexion);
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        using(SqliteDataReader reader = comman.ExecuteReader()){
            while(reader.Read()){
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
    public Productos DetallesProducto(int IdProducto){
        var producto = new Productos();
        using var Conexion = new SqliteConnection(cadenaConexion);
        string query = $"SELECT Descripcion, Precio FROM Productos WHERE idProducto = {IdProducto}";
        Conexion.Open();
        var comman = new SqliteCommand(query, Conexion);
        using (SqliteDataReader reader = comman.ExecuteReader())
        {
            while (reader.Read())
            {
                producto.IdProducto = Convert.ToInt32(reader["idProducto"]);
                producto.Descripcion = reader["Descripcion"].ToString();
                producto.Precio = Convert.ToInt32(reader["Precio"]);
            }
        }
        Conexion.Close();
        return producto;
    }
    public void EliminarProducto(int IdProducto){
        using var Conexion = new SqliteConnection(cadenaConexion);
        SqliteCommand comman = Conexion.CreateCommand();
        comman.CommandText = $"DELETE FROM Productos WHERE idProducto = {IdProducto}";
        Conexion.Open();
        comman.ExecuteNonQuery();
        Conexion.Close();
    }
}
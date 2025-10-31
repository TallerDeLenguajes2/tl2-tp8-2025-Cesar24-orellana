
public class Productos{
    public int IdProducto{get; set;}
    public string? Descripcion{get;  set;}
    public int Precio{get;  set;}
    public Productos(int idProducto, string? descripcion = null, int precio = 0)
    {
        this.IdProducto = idProducto;
        this.Descripcion = descripcion;
        this.Precio = precio;
    }
    public Productos(){}
}
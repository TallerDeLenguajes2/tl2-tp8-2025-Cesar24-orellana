
public class Presupuestos
{
    public int IdPresupuesto{get;private set;}
    public string? NombreDestinatario{get;private set;}
    public DateTime FechaCreada{get;private set;}
    public List<PresupuestosDetalle> Detalle{get;private set;}

    public int MontoPresupuesto(){
        int precio = 0;
        foreach (var item in Detalle)
        {
                precio += item.productos.Precio;
        }
        return precio;
    }

    public float MontoPresupuestoConIva(){
        return (MontoPresupuesto()*1.21f);
    }

    public int CantProductos(){
        return Detalle.Count;
    }

}
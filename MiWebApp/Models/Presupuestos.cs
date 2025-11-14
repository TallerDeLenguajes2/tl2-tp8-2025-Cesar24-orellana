using EPresupuestosDetalles;
namespace EPresupuestos;
public class Presupuestos
{
    public int IdPresupuesto{get; set;}
    public string? NombreDestinatario{get; set;}
    public DateOnly FechaCreada{get; set;}
    public List<PresupuestosDetalle> Detalle{get; set;}

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
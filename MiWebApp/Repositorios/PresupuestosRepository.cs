using System.Globalization;
using System.Linq;
using Microsoft.Data.Sqlite;

public class PresupuestosRepository
{
    private string ConexionString = "db/tienda.db";

    public void CrearPresupuesto(Presupuestos presupuestos)
    {

    }

    public List<Presupuestos> GetAll()
    {
        var ListPresupuestos = new List<Presupuestos>();

        return ListPresupuestos;
    }

    public Presupuestos DetallesPrespuestos(int idPresupuesto)
    {
        var presupuesto = new Presupuestos();

        return presupuesto;
    }

    public void AddProducto(int id)
    {

    }

    public void Delete(int id)
    {
            
    }
}
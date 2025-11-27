using EPresupuestos;
namespace EInterface
{
    interface IPresupuestosRepository
    {
        public void Create(Presupuestos presupuesto);
        public List<Presupuestos> GetAll();
        public Presupuestos Detalle(int Id);
        public void AddDetalle(int IdPresupuesto, int IdProducto, int cant);
        public bool Delete(int Id);
        public Presupuestos ObtenerPresupuesto(int Id);
        public bool Modificar(Presupuestos preupuesto);
    }
}
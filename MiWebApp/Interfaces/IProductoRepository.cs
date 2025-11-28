using EProductos;
namespace MVC.Interfaces
{
    interface IProductoRepository
    {
        public void Add(Productos producto);
        public bool Update(Productos producto);
        public List<Productos> GetAll();
        public Productos GetById(int IdProducto);
        public bool Delete(int IdProducto);
    }
}
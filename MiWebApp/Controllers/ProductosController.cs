using Microsoft.AspNetCore.Mvc;
IEnumerable ClaseMVC.Models.Productos
{
public class ProductosController : Controller
{
    private readonly
    private readonly ProductoRepository productoRepository;
    public ProductosController()
    {
        productoRepository = new ProductoRepository();
    }
    //A partir de aquí van todos los Action Methods (Get, Post,etc.)

    [HttpGet]
    public IActionResult Index()
    {
        List<Productos> productos = productoRepository.GetAll();
        return View(productos);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var producto = new Productos();
        return View(producto);
    }

    [HttpPost]
    public IActionResult Create(Productos producto)
    {
        productoRepository.Create(producto);
        return View(Index);
    }
    [HttpGet]
    public IActionResult Edit(int Id)
    {
        var producto = productoRepository.Detalle(Id);
        
        return View(producto);
    }

}
}
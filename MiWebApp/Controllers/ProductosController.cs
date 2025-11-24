using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
// - - -
using SistemaVentas.Web.Repositorios;
using SistemaVentas.Web.ViewModels; // ❗ Nuevo using

using MiWebApp.Models;
using SistemaVentas.Web.ViewModels;
//IEnumerable MiWebApp.Models.Productos

using EProductos;
namespace MiWebApp.Controllers;
public class ProductosController : Controller
{
    //private readonly
    private readonly ILogger<ProductosController> _logger;
    private readonly ProductoRepository _producRepo;
    public ProductosController(ILogger<ProductosController> _logger)
    {
        _producRepo = new ProductoRepository();
        _logger = logger;
    }
    //A partir de aquí van todos los Action Methods (Get, Post,etc.)

    [HttpGet]
    public IActionResult Index()
    {
        List<Productos> productos = _producRepo.GetAll();
        return View(productos);
    }

    [HttpGet]  // Recibe los datos
    public IActionResult Create()
    {
        var producto = new Productos();
        return View(producto);
    }

    [HttpPost]  // Ejecuta los datos
    public IActionResult CreateOk(ProductoViewModel productoMVC)
    {
        if (!ModelState.IsValid) return View(productoMVC);
        var NuevoProducto = new Productos
        {
            Descripcion = productoMVC.Descripcion,
            Precio = productoMVC.Precio
        };

        _producRepo.Add(NuevoProducto);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public IActionResult Edit(int Id)
    {
        var producto = _producRepo.GetById(Id);

        return View(producto);
    }

    [HttpPost]
    public IActionResult EditOk(int Id, ProductoViewModedel productoMVC)
    {
        if(Id != productoMVC.IdProducto) return NotFound();

        if(!ModelState.IsValid) return View(productoMVC);
        var nuevoPorducto = new Productos
        {
            IdProducto = productoMVC.IdProducto,
            Descripcion = productoMVC.Descripcion,
            Precio = productoMVC.Precio
        };
        _producRepo.ModificarProducto(nuevoPorducto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]  // Recobe los datos
    public IActionResult Delete(int Id)
    {
        var producto = _producRepo.GetById(Id);
        if(producto == null) return RedirectToAction("Index");
        return View(producto);
    }

    [HttpPost]  // Ejecuta los datos
    public IActionResult Delete(Productos producto)
    {
        _producRepo.Delete(producto.IdProducto);
        return RedirectToAction("Index");
    }

}

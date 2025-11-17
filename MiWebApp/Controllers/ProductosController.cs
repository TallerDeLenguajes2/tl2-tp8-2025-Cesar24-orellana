using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiWebApp.Models;
//IEnumerable MiWebApp.Models.Productos

using EProductos;
namespace MiWebApp.Controllers;
public class ProductosController : Controller
{
    //private readonly
    private readonly ProductoRepository _producRepo;
    public ProductosController()
    {
        _producRepo = new ProductoRepository();
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
    public IActionResult CreateOk(Productos producto)
    {
        _producRepo.Add(producto);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Edit(int Id)
    {
        var producto = _producRepo.GetById(Id);

        return View(producto);
    }

    [HttpPost]
    public IActionResult Edit(Productos producto){
        _producRepo.ModificarProducto(producto.IdProducto, producto);
        return RedirectToAction("Index");
    }

    [HttpGet]  // Recobe los datos
    public IActionResult Delete(int Id){
        var producto = _producRepo.GetById(Id);
        if(producto == null) return NotFound();
        return View(producto);
    }

    [HttpPost]  // Ejecuta los datos
    public IActionResult Delete(Productos producto){
        _producRepo.Delete(producto.IdProducto);
        return RedirectToAction("Index");
    }

}

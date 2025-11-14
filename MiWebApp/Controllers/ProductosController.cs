using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiWebApp.Models;
//IEnumerable MiWebApp.Models.Productos

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

    [HttpGet]
    public IActionResult Create()
    {
        var producto = new Productos();
        return View(producto);
    }

    [HttpPost]
    public IActionResult Create(Productos producto)
    {
        _producRepo.CreaProducto(producto);
        return View(Index);
    }
    [HttpGet]
    public IActionResult Edit(int Id)
    {
        var producto = _producRepo.DetallesProducto(Id);

        return View(producto);
    }

}

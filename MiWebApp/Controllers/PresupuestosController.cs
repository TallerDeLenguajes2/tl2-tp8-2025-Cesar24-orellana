using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiWebApp.Models;
// - - -
using SistemaVentas.Web.ViewModels; //Necesario para poder llegar a los ViewModels
using Microsoft.AspNetCore.Mvc.Rendering; // Necesario para SelectList

using EProductos;
using EPresupuestos;
namespace MiWebApp.Controllers;

public class PresupuestosController : Controller
{
    private readonly ILogger<PresupuestosController> _logger;
    private readonly PresupuestosRepository _PresuRepo;
    private readonly ProductoRepository _ProducRepo;
    public PresupuestosController(ILogger<PresupuestosController> logger)
    {
        _logger = logger;
        _PresuRepo = new PresupuestosRepository();
        _ProducRepo = new ProductoRepository();
    }

    [HttpGet]
    public IActionResult Index()
    {
        var presupuestos = _PresuRepo.GetAll();
        return View(presupuestos);
    }

    [HttpGet]
    public IActionResult Details(int Id)
    {
        var detalle = _PresuRepo.Detalle(Id);
        if (detalle == null) return RedirectToAction("Index");
        return View(detalle);
    }

    [HttpGet]
    public IActionResult Create(int Id)
    {
        var presupuesto = new Presupuestos();
        return View(presupuesto);
    }

    [HttpPost]
    public IActionResult CreateOk(Presupuestos presupuesto)
    {
        _PresuRepo.Create(presupuesto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int Id)
    {
        var presupuesto = _PresuRepo.ObtenerPresupuesto(Id);
        return View(presupuesto);
    }

    [HttpPost]
    public IActionResult EditOk(Presupuestos presupuesto)
    {
        _PresuRepo.Modificar(presupuesto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int Id)
    {
        var presupuesto = _PresuRepo.ObtenerPresupuesto(Id);
        return View(presupuesto);
    }

    [HttpPost]
    public IActionResult DeleteOk(Presupuestos presupuesto)
    {
        _PresuRepo.Delete(presupuesto.IdPresupuesto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult AgregarProducto(int Id)
    {
        // 1. Obtener los productos para el SelectList
        List<Productos> productos = _ProducRepo.GetAll();

        // 2. Crear el ViewModel
        AgregarProductoViewModel model = new AgregarProductoViewModel
        {
            IdPresupuesto = Id, // Pasamos el ID del presupuesto actual
                                // 3. Crear el SelectList
            ListaProductos = new SelectList(productos, "IdProducto", "Descripcion")
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult AgregarProducto(AgregarProductoViewModel model)
    {
        // 1. Chequeo de Seguridad para la Cantidad
        if (!ModelState.IsValid)
        {
            // LÓGICA CRÍTICA DE RECARGA: Si falla la validación,
            // debemos recargar el SelectList porque se pierde en el POST.
            var productos = _ProducRepo.GetAll();
            model.ListaProductos = new SelectList(productos, "IdProducto", "Descripcion");

            // Devolvemos el modelo con los errores y el dropdown recargado
            return View(model);
        }

        // 2. Si es VÁLIDO: Llamamos al repositorio para guardar la relación
        _PresuRepo.AddDetalle(model.IdPresupuesto, model.IdProducto, model.Cantidad);

        // 3. Redirigimos al detalle del presupuesto
        return RedirectToAction(nameof(Details), new { id = model.IdPresupuesto });
    }

    public IActionResult AccesoDenegado(){}

}
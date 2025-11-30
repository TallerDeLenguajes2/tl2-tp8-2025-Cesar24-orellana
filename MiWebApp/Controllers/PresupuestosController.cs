using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiWebApp.Models;
// - - -
using SistemaVentas.Web.ViewModels; //Necesario para poder llegar a los ViewModels
using Microsoft.AspNetCore.Mvc.Rendering; // Necesario para SelectList

using EProductos;
using EPresupuestos;
using MVC.Interfaces;
namespace MiWebApp.Controllers;

public class PresupuestosController : Controller
{
    private readonly ILogger<PresupuestosController> _logger;
    private readonly IPresupuestosRepository _PresuRepo;
    private readonly IProductoRepository _ProducRepo;
    private readonly IAuthenticationService _authService;
    public PresupuestosController(ILogger<PresupuestosController> logger, IPresupuestosRepository presuRepo, IProductoRepository ProducRepo, IAuthenticationService authService)
    {
        _logger = logger;
        _PresuRepo = presuRepo;
        _ProducRepo = ProducRepo;
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
        if (_authService.HasAccessLevel("Administrador") || _authService.HasAccessLevel("Cliente"))
        {
            var presupuestos = _PresuRepo.GetAll();
            return View(presupuestos);
        }
        else
        {
            return RedirectToAction("Index", "Login");
        }

    }

    [HttpGet]
    public IActionResult Details(int Id)
    {
        // Comprobación de si está logueado
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");

        if (_authService.HasAccessLevel("Administrador") || _authService.HasAccessLevel("Cliente"))
        {
            var detalle = _PresuRepo.Detalle(Id);
            if (detalle == null) return RedirectToAction("Index");
            return View(detalle);
        }
        else
        {
            return RedirectToAction("Index", "Login");
        }
    }

    [HttpGet]
    public IActionResult Create(int Id)
    {
        // Comprobación de si está logueado
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
        var check = CheckAdminPermissions();
        if (check != null) return check;

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
        // Comprobación de si está logueado
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
        var check = CheckAdminPermissions();
        if (check != null) return check;

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
        // Comprobación de si está logueado
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
        var check = CheckAdminPermissions();
        if (check != null) return check;

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
        // Comprobación de si está logueado
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");
        var check = CheckAdminPermissions();
        if (check != null) return check;

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


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult CheckAdminPermissions()
    {
        // 1 No Logueado? -> Vuelve al login
        if (_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");

        // 2 No es Admin -> Da Error
        // Llamamos a AccesoDenegado (Vista correspondiente de Producto)
        if (!_authService.HasAccessLevel("Administrador")) return RedirectToAction(nameof(AccesoDenegado));

        // Logueo con Admin (Rango necesario)
        return null;
    }

    public IActionResult AccesoDenegado()
    {
        // Logueado pero con rango insuficiente
        return View();
    }

}
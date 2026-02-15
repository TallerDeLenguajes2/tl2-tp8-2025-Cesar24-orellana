using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
// - - -
using SistemaVentas.Web.ViewModels; // ❗ Nuevo using

using MVC.Interfaces;
using MVC.Services;
using MiWebApp.Models;
//IEnumerable MiWebApp.Models.Productos

using EProductos;
namespace MiWebApp.Controllers;

public class ProductosController : Controller
{
    //private readonly

    private readonly ILogger<ProductosController> _logger;
    private readonly IProductoRepository _producRepo;
    private readonly IAuthenticationService _authService;

    public ProductosController(ILogger<ProductosController> logger, IProductoRepository ProduRepo, IAuthenticationService authService)
    {
        _logger = logger;
        _producRepo = ProduRepo;
        _authService = authService;
    }
    //A partir de aquí van todos los Action Methods (Get, Post,etc.)

    [HttpGet]
    public IActionResult Index()
    {
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        List<Productos> productos = _producRepo.GetAll();
        return View(productos);
    }

    [HttpGet]  // Recibe los datos
    public IActionResult Create()
    {
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        var productoVM = new ProductoViewModel();
        return View(productoVM);
    }

    [HttpPost]  // Ejecuta los datos
    public IActionResult Create(ProductoViewModel productoMVC)
    {
        try
        {
            if (!ModelState.IsValid) return View(productoMVC);
            var NuevoProducto = new Productos
            {
                Descripcion = productoMVC.Descripcion,
                Precio = Convert.ToInt32(productoMVC.Precio)
            };

            _producRepo.Add(NuevoProducto);
            return RedirectToAction(nameof(Index));

        }
        catch (Exception ex)
        {
            /* var mensaje = "Error Mensaje: " + ex.Message;
            if(ex.InnerException != null) mensaje = mensaje + " Inner exeption: " + ex.InnerException.Message;
            mensaje = mensaje + "Stack trace: " + ex.StackTrace;
            _logger.LogError(mensaje); */
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }
    [HttpGet]
    public IActionResult Edit(int Id)
    {
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        var producto = _producRepo.GetById(Id);
        if (producto == null) return RedirectToAction(nameof(Index));

        var productoVM = new ProductoViewModel
        {
            IdProducto = producto.IdProducto,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio
        };
        return View(productoVM);
    }

    [HttpPost]
    public IActionResult Edit(int Id, ProductoViewModel productoMVC)
    {
        try
        {
            if (Id != productoMVC.IdProducto) return NotFound();

            if (!ModelState.IsValid) return View(productoMVC);
            var productoEdit = new Productos
            {
                IdProducto = productoMVC.IdProducto,
                Descripcion = productoMVC.Descripcion,
                Precio = Convert.ToInt32(productoMVC.Precio)
            };
            _producRepo.Update(productoEdit);
            return RedirectToAction(nameof(Index));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    [HttpGet]  // Recobe los datos
    public IActionResult Delete(int Id)
    {
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        var producto = _producRepo.GetById(Id);
        if (producto == null) return RedirectToAction("Index");
        return View(producto);
    }

    [HttpPost]  // Ejecuta los datos
    public IActionResult Delete(Productos producto)
    {
        try
        {
        _producRepo.Delete(producto.IdProducto);
        return RedirectToAction("Index");
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BadRequest();
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult CheckAdminPermissions()
    {
        // 1 No Logueado? -> Vuelve al login
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Login");

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

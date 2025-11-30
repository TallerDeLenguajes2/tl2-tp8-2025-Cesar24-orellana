// implementación del LoginController.cs
using Microsoft.AspNetCore.Mvc;
using MVC.Interfaces;
using SistemaVentas.Web.ViewModels; //Necesario para poder llegar a los ViewModels
public class LoginController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    public LoginController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpGet] //Muestra la vista de login
    public IActionResult Index()
    {
        // ... (Crear LoginViewModel)
        return View(new LoginViewModel());
    }

    //[HttpPost] //Procesa el login
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
        {
            model.ErrorMessage = "Debe ingresar usuario y contraseña.";
            return View("Index", model);
        }
        if (_authenticationService.Login(model.Username, model.Password))
        {
            return RedirectToAction("Index", "Home");
        }
        model.ErrorMessage = "Credenciales inválidas.";
        return View("Index", model);
    }

    [HttpGet] //Cierra sesión
    public IActionResult Logout()
    {
        _authenticationService.Logout();
        return RedirectToAction("Index");
    }
}
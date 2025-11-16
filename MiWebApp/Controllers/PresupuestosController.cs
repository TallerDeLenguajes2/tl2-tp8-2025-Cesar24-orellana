using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiWebApp.Models;

using EPresupuestos;
namespace MiWebApp.Controllers;

public class PresupuestosController : Controller
{
    private readonly PresupuestosRepository _PresuRepo;
    public PresupuestosController()
    {
        _PresuRepo = new PresupuestosRepository();
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
        return View(detalle);
    }

    // [HttpPost]
    // public IActionResult Details(Presupuestos detalle)
    // {
    //     _
    // }
}
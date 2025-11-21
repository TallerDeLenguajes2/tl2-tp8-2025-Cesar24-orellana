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

}
using System.ComponentModel.DataAnnotations;
using System;
namespace SistemaVentas.Web.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Nombre o Email del Destinatario")]
    [Required(ErrorMessage = "El nombre o email es obligatorio.")]
    // Opcional: Se puede añadir la validación de formato de email si se opta por guardar el mail.
    // [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    public string? Username { get; set; }

    //      -----   Modificar Password    -------
    
    [Display(Name = "Nombre o Email del Destinatario")]
    [Required(ErrorMessage = "El nombre o email es obligatorio.")]
    // Opcional: Se puede añadir la validación de formato de email si se opta por guardar el mail.
    // [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    public string? Password { get; set; }
    public string? ErrorMessage { get; internal set; }
}
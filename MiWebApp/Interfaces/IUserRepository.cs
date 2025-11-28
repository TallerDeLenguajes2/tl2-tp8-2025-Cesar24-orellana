using EUsuario;
namespace MVC.Interfaces;

public interface IUserRepository
{
    // Retorna el objeto Usuario si las credenciales son válidas, sino null.
    Usuarios GetUser(string username, string password);
}
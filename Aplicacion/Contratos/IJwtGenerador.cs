using Dominio;

// Las interfaces se conocen como contratos 
namespace Aplicacion.Contratos
{
    public interface IJwtGenerador
    {
        string CrearToken(Usuario usuario);
    }
}

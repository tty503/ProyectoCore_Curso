using Dominio;
using Microsoft.AspNetCore.Identity;

namespace Persistencia
{
    public class DataPrueba
    {
        public async static Task InsertarData(CursosOnlineContext context, UserManager<Usuario> userManager)
        {
            if (!userManager.Users.Any()) 
            {
                var usuario = new Usuario { NombreCompleto = "Thomas Anderson", UserName = "Neo", Email = "neo@matrix.dev"};
                // Creamos el user / password
                await userManager.CreateAsync(usuario, "404$NotFound");
            }
            //Console.WriteLine("Any() returned: {0}", userManager.Users.Any());

        }
    }
}

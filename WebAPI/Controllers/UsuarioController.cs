using Aplicacion.Seguridad;
using Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
        // Permite que este controller no requiera autenticacion
    [AllowAnonymous]
    public class UsuarioController : MyControllerBase
    {
        [HttpPost("login")]
        //public async Task<ActionResult<Usuario>> login(Login.Ejecuta parametros)
        public async Task<ActionResult<UsuarioData>> login(Login.Ejecuta parametros)
        {
            return await mediator.Send(parametros);
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioData>> registrar(Registrar.Ejecuta parametros)
        {
            return await mediator.Send(parametros);
        }

        [HttpGet]
        public async Task<ActionResult<UsuarioData>> devolverUsuario()
        {
            return await mediator.Send(new UsuarioActual.Ejecutar());
        }
    }
}

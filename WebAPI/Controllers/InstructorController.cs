using Aplicacion.Instructores;
using Microsoft.AspNetCore.Mvc;
using Persistencia.DapperConexion.Instructor;

namespace WebAPI.Controllers
{
    public class InstructorController : MyControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstructorModel>>> ObtenerInstructores()
        {
            return await mediator.Send(new Consulta.Lista());
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dominio;
using Aplicacion.CursoApp;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class CursosController : MyControllerBase
    {
        // Inyeccion manual, cuando usabamos ControllerBase
        //private readonly IMediator mediator;
        //public CursosController(IMediator _mediator)
        //{
        //    this.mediator = _mediator;
        //}
        [HttpGet("")]
        //[Authorize]
        public async Task<ActionResult<List<CursoDto>>> GetCurso()
        {
            return await mediator.Send(new Consulta.ListaCursos());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDto>> GetOneCurso(Guid id)
        {
            return await mediator.Send(new ConsultaId.CursoUnico{ Id = id});
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            return await mediator.Send(data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Editar(Guid id, Editar.Ejecuta data)
        {
            data.CursoId = id;
            return await mediator.Send(data);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            return await mediator.Send(new Eliminar.Ejecuta {CursoId = id});
        }
    }
}
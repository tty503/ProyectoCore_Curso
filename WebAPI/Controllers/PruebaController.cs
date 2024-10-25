using Dominio;
using Microsoft.AspNetCore.Mvc;
using Persistencia;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("prueba")]
    public class PruebaController
    {
        private readonly CursosOnlineContext context;
        public PruebaController(CursosOnlineContext _context)
        {
            this.context = _context;
        }

        [HttpGet("prueba")]
        public IEnumerable<string> Get() 
        {
            string[] nombres = new[] { "Christian", "David", "Juan", "Pablo", "Leonardo"};
            return nombres;
        }

        [HttpGet("curso")]
        public IEnumerable<Curso> GetCurso()
        {
            return context.Curso.ToList();
        }
    }
}

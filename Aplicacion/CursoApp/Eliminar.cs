using MediatR;
using Dominio;
using Persistencia;
using Aplicacion.HandlerError;
using System.Net;
namespace Aplicacion.CursoApp
{
    public class Eliminar
    {
        public class Ejecuta : IRequest
        {
            public Guid CursoId { get; set; }
        }
        public class Handler : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext context;
            public Handler(CursosOnlineContext _context)
            {
                this.context = _context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // Primero debe conseguirse las referencias, como los instructores en este caso y eliminarlos. 
                var instructoresBD = context.CursoInstructor.Where(x => x.CursoId == request.CursoId).ToList();
                foreach(var instructor in instructoresBD)
                {
                    context.CursoInstructor.Remove(instructor);
                }

                // Eliminar comentarios 
                var comentariosDB = context.Comentario.Where(x => x.CursoId == request.CursoId);
                foreach(var cmt in comentariosDB)
                {
                    context.Comentario.Remove(cmt);
                }

                // Eliminar precio 
                var precioDB = context.Precio.Where(x => x.CursoId == request.CursoId).FirstOrDefault();
                if(precioDB != null)
                {
                    context.Precio.Remove(precioDB);
                }


                var curso = await context.Curso.FindAsync(request.CursoId);
                if (curso == null)
                {
                    //throw new Exception("No se pudo eliminar el curso");
                    throw new HandlerException(HttpStatusCode.NotFound, new { curso = "No se encontro el curso" }); 
                }
                context.Remove(curso);
                var valor = await context.SaveChangesAsync();
                if (valor > 0)
                {
                    return Unit.Value;
                }
                throw new Exception("No se pudieron guardar los cambios");
            }
        }
    }
}
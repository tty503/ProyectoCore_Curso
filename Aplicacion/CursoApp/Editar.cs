using MediatR;
using Persistencia;
using Dominio;

namespace Aplicacion.CursoApp
{
    public class Editar
    {
        public class Ejecuta : IRequest
        {
            public Guid    CursoId { get; set; }
            public string? Titulo  { get; set; }
            public string? Descripcion { get; set; }
            public DateTime? FechaPublicacion { get; set; }

            public List<Guid> ListaInstructor { get; set; } 
            public decimal? Precio { get; set; }
            public decimal? Promocion { get; set; }
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
                // Actualizar curso 
                var curso = await context.Curso.FindAsync(request.CursoId);
                if(curso == null)
                {
                    throw new Exception($"El curso {request.CursoId} no existe");
                }
                curso.Titulo = request.Titulo ?? curso.Titulo;
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;

                // Actualizar el precio del curso 
                var precioEntidad = context.Precio.Where(x => x.CursoId == curso.CursoId).FirstOrDefault();
                if (precioEntidad != null)
                {
                    precioEntidad.Promocion = request.Promocion ?? precioEntidad.Promocion;
                    precioEntidad.PrecioActual = request.Precio ?? precioEntidad.PrecioActual;
                }
                else
                {
                    precioEntidad = new Precio
                    {
                        PrecioId = Guid.NewGuid(),
                        CursoId  = curso.CursoId,
                        PrecioActual = request.Precio ?? 0,
                        Promocion = request.Promocion ?? 0
                    };
                    await context.Precio.AddAsync(precioEntidad);
                }

                // Actualizar instructores 
                if (request.ListaInstructor != null) 
                {
                    if (request.ListaInstructor.Count > 0) 
                    {
                        // Eliminar los instructores actuales del curso en la base de datos 
                        var instructorBD = context.CursoInstructor.Where(x => x.CursoId == request.CursoId).ToList();
                        foreach(var instructorEliminar in instructorBD)
                        {
                            context.CursoInstructor.Remove(instructorEliminar);
                        }

                        // Agregar instructores (provienen de la consulta)
                        foreach (var ids in request.ListaInstructor)
                        {
                            var nuevoInstructor = new CursoInstructor
                            {
                                CursoId = request.CursoId,
                                InstructorId = ids,
                            };
                            context.CursoInstructor.Add(nuevoInstructor);
                        }
                    }
                }

                var valor = await context.SaveChangesAsync();
                if(valor>0)
                    return Unit.Value;

                throw new Exception("No se guardaron los cambios");
            }
        }
    }
}

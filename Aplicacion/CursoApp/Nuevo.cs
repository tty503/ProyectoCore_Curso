using MediatR;
using Persistencia;
using Dominio;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Aplicacion.CursoApp
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            [Required(ErrorMessage = "Es necesario agregar un titulo")]
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime? FechaPublicacion { get; set; }
            public List<Guid> ListaInstructor {  get; set; }
            public decimal Precio { get; set; }
            public decimal Promocion {  get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion() 
            {
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotNull();
            }
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
                // Logica para agregar curso
                Guid _CursoId = Guid.NewGuid();
                var curso = new Curso
                {
                    CursoId = _CursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion ?? null,
                };
                context.Curso.Add(curso);

                // Logica para agregar instructores 
                if (request.ListaInstructor != null) 
                {
                    foreach(var id in request.ListaInstructor)
                    {
                        var cursoInstructor = new CursoInstructor
                        {
                            CursoId = _CursoId,
                            InstructorId = id,
                        };
                        context.CursoInstructor.Add(cursoInstructor);
                    }
                }

                // Logica para agregar Precio 
                var precioEntidad = new Precio
                {
                    CursoId = _CursoId,
                    PrecioId = Guid.NewGuid(),
                    PrecioActual = request.Precio,
                    Promocion = request.Promocion
                };

                context.Add(precioEntidad);

                    // Devuelve el estado de la transaccion, 0 no se realizo la operacion, 1 o valor superios la transaccion es correcta. 
                var valor = await context.SaveChangesAsync();
                if(valor > 0)
                {
                    return Unit.Value;
                }
                throw new Exception("No se pudo insertar el curso");
            }
        }
    }
}

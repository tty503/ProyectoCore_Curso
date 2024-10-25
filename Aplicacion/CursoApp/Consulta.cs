using Dominio;
using MediatR;
using Persistencia;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
namespace Aplicacion.CursoApp
{
    public class Consulta
    {
        public class ListaCursos : IRequest<List<CursoDto>> { }
        public class Handler : IRequestHandler<ListaCursos, List<CursoDto>>
        {
            private readonly CursosOnlineContext context;
            private readonly IMapper _mapper;
            public Handler(CursosOnlineContext _context, IMapper mapper)
            {
                context = _context;
                _mapper = mapper;
            }
            public async Task<List<CursoDto>> Handle(ListaCursos request, CancellationToken cancellationToken)
            {
                var cursos = await context.Curso
                                          .Include(x => x.ComentarioLista)
                                          .Include(x => x.PrecioPromocion)
                                          .Include(x => x.InstructorLink)
                                          .ThenInclude(x => x.Instructor)
                                          .ToListAsync();

                var cursosDto = _mapper.Map<List<Curso>, List<CursoDto>>(cursos);
                return cursosDto;
            }
        }
    }
}



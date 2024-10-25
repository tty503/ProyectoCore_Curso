using MediatR;
using Dominio;
using Persistencia;
using Aplicacion.HandlerError;
using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.CursoApp
{
    public  class ConsultaId
    {
        public class CursoUnico : IRequest<CursoDto> 
        { 
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<CursoUnico, CursoDto>
        {
            private readonly CursosOnlineContext context;
            private readonly IMapper _mapper;
            public Handler(CursosOnlineContext _context, IMapper mapper)
            {
                this.context = _context;
                _mapper = mapper;
            }

            async Task<CursoDto> IRequestHandler<CursoUnico, CursoDto>.Handle(CursoUnico request, CancellationToken cancellationToken)
            {
                var curso = await context.Curso.Include(x => x.InstructorLink)
                                                .ThenInclude(y => y.Instructor)
                                                .FirstOrDefaultAsync(x => x.CursoId == request.Id);
                if(curso == null)
                {
                    throw new HandlerException(HttpStatusCode.NotFound, "El curso no existe");               
                }
                var cursoDto = _mapper.Map<Curso, CursoDto>(curso);
                return cursoDto;
            }
        }
    }
}

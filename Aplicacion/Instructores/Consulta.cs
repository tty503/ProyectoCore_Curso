using MediatR;
using Persistencia.DapperConexion.Instructor;
using System.Net.WebSockets;

namespace Aplicacion.Instructores
{
    public class Consulta
    {
        public class Lista : IRequest<List<InstructorModel>> { }
        public class Handler : IRequestHandler<Lista, List<InstructorModel>>
        {
            private readonly IInstructor _instructorRepository;
            public Handler(IInstructor instructorRepository)
            {
                _instructorRepository = instructorRepository;
            }

            public async Task<List<InstructorModel>> Handle(Lista request, CancellationToken cancellationToken)
            {
                var result = await _instructorRepository.ObtenerLista();
                return result.ToList();
            }
        }
    }
}

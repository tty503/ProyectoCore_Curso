﻿// En los DTO colocamoss la estructura de los datos que queremos devolver
using Dominio;

namespace Aplicacion.CursoApp
{
    public class CursoDto
    {
        public Guid   CursoId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public ICollection<InstructorDto> Instructores { get; set; }
        public PrecioDto Precio { get; set; }
        public ICollection<ComentarioDto> Comentarios { get; set; }
    }
}

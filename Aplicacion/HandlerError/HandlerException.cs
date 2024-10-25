using System;
using System.Net;

namespace Aplicacion.HandlerError
{
    public class HandlerException : Exception
    {
        public HttpStatusCode _codigo { get; }
        public object _errores { get; }
        public HandlerException(HttpStatusCode codigo, object errores = null) 
        { 
            _codigo = codigo;
            _errores = errores;
        }
    }
}

using Aplicacion.HandlerError;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using Aplicacion.HandlerError;
using Newtonsoft.Json;
using System.Net;
namespace WebAPI.Middleware
{
    public class HandlerErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HandlerErrorMiddleware> _logger;
        
        public HandlerErrorMiddleware(RequestDelegate next, ILogger<HandlerErrorMiddleware> logger) 
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandlerExceptionAsync(context, ex, _logger);
            }
        }
        private async Task HandlerExceptionAsync(HttpContext context, Exception ex, ILogger<HandlerErrorMiddleware> logger)
        {
            object errores = null;
            switch(ex)
            {
                case HandlerException me :
                    logger.LogError(ex, "Handler Error");
                    errores = me._errores;
                    context.Response.StatusCode = (int)me._codigo;
                    break;
                case Exception e:
                    logger.LogError(ex, "Error de servidor");
                    errores = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break; 
            }
            context.Response.ContentType = "application/json";
            if(errores != null)
            {
                var resultado = JsonConvert.SerializeObject(new { errores });
                await context.Response.WriteAsync(resultado);
            }
        }
    }
}

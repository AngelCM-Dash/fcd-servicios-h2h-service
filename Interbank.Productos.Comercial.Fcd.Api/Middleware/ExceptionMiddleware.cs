using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;
using Interbank.Productos.Comercial.Fcd.Application.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Interbank.Productos.Comercial.Fcd.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException e)
            {
                _logger.LogError(e, "Ocurrieron uno o mas errores de validacion (404). Detalles: {ErrorDetails}", e.ToString());

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var errorResponse = new
                {
                    Status = 23,
                    Detail = e.Message,
                    Errors = e.Errors
                };

                string json = JsonSerializer.Serialize(errorResponse);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "Ocurrió un error, no se puedo obtener información (400). Detalles: {ErrorDetails}", e.ToString());

                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                ProblemDetails problem = new()
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Type = null,
                    Title = null,
                    Detail = e.Message
                };

                string json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
            catch (CustomException e)
            {
                _logger.LogError(e, "Ocurrió un error interno del servidor (500). Detalles: {ErrorDetails}", e.ToString());

                context.Response.StatusCode = e.StatusCode == 0 ? (int)HttpStatusCode.InternalServerError : e.StatusCode;

                CustomExceptionResponse problem = new CustomExceptionResponse
                {
                    Status = e.CodigoError,
                    Detail = e.Message,
                };

                string json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
            catch (ThrowException e)
            {
                _logger.LogError(e, "Ocurrió un error interno del servidor (500). Detalles: {ErrorDetails}", e.Message.ToString());

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                PlanillaResponse problem = new PlanillaResponse
                {
                    CodigoRespuesta = e.StatusCode,
                    MensajeRespuesta = e.Message,
                    NumeroPlanilla = ""
                };

                string json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ocurrió un error interno del servidor (500). Detalles: {ErrorDetails}", e.ToString());

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ProblemDetails problem = new()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = null,
                    Title = null,
                    Detail = e.Message
                };

                string json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
        }
    }
}

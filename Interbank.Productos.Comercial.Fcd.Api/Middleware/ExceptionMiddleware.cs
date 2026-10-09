using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Application.Features.Planilla.Commands.CargaMasiva;
using Interbank.Productos.Comercial.Fcd.Application.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;
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
                using (CreateDynatraceErrorScope(e, (int)HttpStatusCode.BadRequest, "Validation"))
                {
                    _logger.LogError(e, "Ocurrieron uno o mas errores de validacion (404). Detalles: {ErrorDetails}", e.ToString());
                }

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
                using (CreateDynatraceErrorScope(e, (int)HttpStatusCode.NotFound, "NotFound"))
                {
                    _logger.LogError(e, "Ocurrió un error, no se puedo obtener información (400). Detalles: {ErrorDetails}", e.ToString());
                }

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
                var statusCode = e.StatusCode == 0 ? (int)HttpStatusCode.InternalServerError : e.StatusCode;
                using (CreateDynatraceErrorScope(e, statusCode, "Custom"))
                {
                    _logger.LogError(e, "Ocurrió un error interno del servidor (500). Detalles: {ErrorDetails}", e.ToString());
                }

                context.Response.StatusCode = statusCode;

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
                using (CreateDynatraceErrorScope(e, (int)HttpStatusCode.InternalServerError, "Throw"))
                {
                    _logger.LogError(e, "Ocurrió un error interno del servidor (500). Detalles: {ErrorDetails}", e.Message.ToString());
                }

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
                using (CreateDynatraceErrorScope(e, (int)HttpStatusCode.InternalServerError, "Unhandled"))
                {
                    _logger.LogError(e, "Ocurrió un error interno del servidor (500). Detalles: {ErrorDetails}", e.ToString());
                }

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

        private static IDisposable CreateDynatraceErrorScope(Exception exception, int statusCode, string stage)
        {
            return LogContext.Push(
                new Serilog.Core.Enrichers.PropertyEnricher("EnviarDynatrace", true),
                new Serilog.Core.Enrichers.PropertyEnricher("HttpResponseStatusCode", statusCode),
                new Serilog.Core.Enrichers.PropertyEnricher("ErrorType", exception.GetType().Name),
                new Serilog.Core.Enrichers.PropertyEnricher("ErrorTitle", exception.Message),
                new Serilog.Core.Enrichers.PropertyEnricher("ErrorStage", stage),
                new Serilog.Core.Enrichers.PropertyEnricher("ErrorInner", exception.InnerException?.Message ?? string.Empty),
                new Serilog.Core.Enrichers.PropertyEnricher("WioExceptionsCount", 1));
        }
    }
}

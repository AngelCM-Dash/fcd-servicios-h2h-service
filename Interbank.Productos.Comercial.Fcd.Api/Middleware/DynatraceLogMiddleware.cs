using Serilog.Context;

namespace Interbank.Productos.Comercial.Fcd.Api.Middleware
{
    public class DynatraceLogMiddleware
    {
        private static readonly HashSet<string> DynatraceRoutes = new(StringComparer.OrdinalIgnoreCase)
        {
            "/api/v1/Planillas/CargaMasivaPlanillas",
            "/api/v1/Planillas/ProcesoCargaMasiva",
            "/api/v1/Desembolso/desembolsarPlanilla",
            "/api/v1/Desembolso/Encolamiento",
            "/api/v1/Desembolso/ProcesoAbono",
            "/api/v1/Desembolso/ProcesarTramas"
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<DynatraceLogMiddleware> _logger;

        public DynatraceLogMiddleware(RequestDelegate next, ILogger<DynatraceLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var shouldSendToDynatrace = DynatraceRoutes.Contains(path);
            var transactionId = context.TraceIdentifier;

            using (LogContext.PushProperty("EnviarDynatrace", shouldSendToDynatrace))
            using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
            using (LogContext.PushProperty("HttpRoute", path))
            using (LogContext.PushProperty("Accion", context.GetEndpoint()?.DisplayName ?? string.Empty))
            using (LogContext.PushProperty("TransactionId", transactionId))
            using (LogContext.PushProperty("ArchivoNombre", string.Empty))
            using (LogContext.PushProperty("EmpresaCodigo", string.Empty))
            using (LogContext.PushProperty("ProveedorCodigo", string.Empty))
            using (LogContext.PushProperty("PlanillaNumero", string.Empty))
            using (LogContext.PushProperty("OperacionCodigo", string.Empty))
            using (LogContext.PushProperty("PlanillaNumeroSeq", string.Empty))
            using (LogContext.PushProperty("ProductoCodigo", string.Empty))
            using (LogContext.PushProperty("Canal", string.Empty))
            using (LogContext.PushProperty("WioExceptionsCount", 0))
            using (LogContext.PushProperty("HttpResponseStatusCode", 0))
            using (LogContext.PushProperty("ErrorType", string.Empty))
            using (LogContext.PushProperty("ErrorTitle", string.Empty))
            using (LogContext.PushProperty("ErrorStage", string.Empty))
            using (LogContext.PushProperty("ErrorInner", string.Empty))
            using (LogContext.PushProperty("EstacionId", string.Empty))
            {
                try
                {
                    await _next(context);
                }
                finally
                {
                    if (shouldSendToDynatrace)
                    {
                        using (LogContext.PushProperty("HttpResponseStatusCode", context.Response.StatusCode))
                        {
                            _logger.LogInformation("Solicitud procesada para Dynatrace. Ruta: {HttpRoute}, StatusCode: {HttpResponseStatusCode}", path, context.Response.StatusCode);
                        }
                    }
                }
            }
        }
    }
}

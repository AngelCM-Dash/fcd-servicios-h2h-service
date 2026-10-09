using Interbank.Productos.Comercial.Fcd.Application.Contracts.Persistence;
using Interbank.Productos.Comercial.Fcd.Application.Exceptions;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.RechazoDocumentos
{
    public class RejectionDocumentsCommandHandler : IRequestHandler<RejectionDocumentsCommand, BaseResponse>
    {
        private readonly IDocumentosRepository _documentosRepository;
        private readonly ILogger<RejectionDocumentsCommandHandler> _logger;

        public RejectionDocumentsCommandHandler(IDocumentosRepository documentosRepository, ILogger<RejectionDocumentsCommandHandler> logger)
        {
            _documentosRepository = documentosRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> Handle(RejectionDocumentsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando proceso de rechazo de documentos. FlagTipoRechazoDesembolso: {FlagTipoRechazoDesembolso}, NumeroPlanilla: {NumeroPlanilla}", request.FlagTipoRechazoDesembolso, request.NumeroPlanilla);

            try
            {
                if (request.FlagTipoRechazoDesembolso == 1)
                {
                    _logger.LogInformation("Rechazo de documentos diferidos para Planilla: {NumeroPlanilla}, Linea: {NumeroLinea}", request.NumeroPlanilla, request.NumeroLinea);
                    await _documentosRepository.RechazarDocumentosDiferidos(request.NumeroPlanilla ?? "", request.NumeroLinea ?? "", request.Observacion ?? "", request.FechaAdelanto ?? "");
                }
                else
                {
                    _logger.LogInformation("Rechazo de documentos distribuidos para Planilla: {NumeroPlanilla}", request.NumeroPlanilla);
                    await _documentosRepository.RechazarDocumentosDistribuido(request.NumeroPlanilla ?? "", request.Observacion ?? "");
                }

                var response = new BaseResponse
                {
                    CodigoRespuesta = 32,
                    MensajeRespuesta = "Respuesta correcta"
                };

                _logger.LogInformation("Proceso de rechazo de documentos finalizado correctamente para Planilla: {NumeroPlanilla}", request.NumeroPlanilla);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al rechazar documentos para Planilla: {NumeroPlanilla}", request.NumeroPlanilla);
                throw new ThrowException("36", $"Error al procesar la planilla {request.NumeroPlanilla}: {ex.Message}");
            }

        }
    }
}

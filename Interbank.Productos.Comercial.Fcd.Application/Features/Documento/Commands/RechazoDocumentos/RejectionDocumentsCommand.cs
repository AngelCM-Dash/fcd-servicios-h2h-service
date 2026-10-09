using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;
using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Documento.Commands.RechazoDocumentos
{
    public class RejectionDocumentsCommand : IRequest<BaseResponse>
    {
        public int? FlagTipoRechazoDesembolso { get; set; }
        public string? NumeroPlanilla { get; set; }
        public string? NumeroLinea { get; set; }
        public string? Observacion { get; set; }
        public string? FechaAdelanto { get; set; }

        public RejectionDocumentsCommand(int? flagTipoRechazoDesembolso, string? numeroPlanilla, string? numeroLinea, string? observacion, string? fechaAdelanto)
        {
            FlagTipoRechazoDesembolso = flagTipoRechazoDesembolso;
            NumeroPlanilla = numeroPlanilla;
            NumeroLinea = numeroLinea;
            Observacion = observacion;
            FechaAdelanto = fechaAdelanto;
        }
    }
}

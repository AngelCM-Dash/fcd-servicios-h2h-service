using MediatR;

namespace Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Query.ConsultaNroOperacionDesembolso
{
    public class GetNroOperacionMovimientoQuery : IRequest<List<GetNroOperacionMovimientoQueryVM>>
    {
        public string? NumeroPlanilla { get; set; }
        public string? NumeroSecuencia { get; set; }
    }
}

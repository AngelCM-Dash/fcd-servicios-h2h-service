using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices
{
    public interface ILineaService
    {
        Task<LineaOperacionResponse> ConsultaLineas(LineaOperacionRequest lineaOperacionRequest);
        Task<List<DetalleReservaLineaResponse>> ObtenerDetalleReservaLinea(string numeroPlanilla);
        Task<BaseResponse> LiberacionReserva(LiberacionReservaRequest liberacionReservaRequest);
    }
}

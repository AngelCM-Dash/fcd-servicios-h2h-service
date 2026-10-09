using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;
using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Response;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.ExternalServices
{
    public interface ICommonService
    {
        Task<TokenAuthorizationResponse> GenerarTokenAuthorization(int tipoScope, string Canal);
        Task<BaseResponse> NotificacionAssi(NotificacionAssiRequest notificacionAssiRequest);
        Task<BaseResponse> NotificacionBackAssi(NotificacionAssiRequest notificacionAssiRequest);
    }
}

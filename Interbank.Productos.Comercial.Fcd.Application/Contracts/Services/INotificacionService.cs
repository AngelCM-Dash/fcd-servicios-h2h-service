using Interbank.Productos.Comercial.Fcd.Domain.Entities.Services.Request;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface INotificacionService
    {
        Task<int> NotificacionAssi(NotificacionAssiRequest notificacionAssiRequest, decimal idDetalle);
    }
}

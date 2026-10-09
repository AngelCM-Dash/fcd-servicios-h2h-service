using Interbank.Productos.Comercial.Fcd.Application.Features.Desembolso.Commands.Encolamiento;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface IEncolamientoDesembolsoService
    {
        Task EnqueueAsync(EncolarDesembolsoCommand desembolso);
    }
}
